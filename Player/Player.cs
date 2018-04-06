using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Direction
{
    left, right, down, up
}

public class Player : LivingObject
{

    // 스탯 공격력, 방어력, 사거리
    public Stat Power;
    public Stat Defanse;
    public Stat Range;

    [SerializeField] Inventory inventory;
    [SerializeField] EquipmentPanel equipmentPanel;
    [SerializeField] StatsPanel statsPanel;
    [SerializeField] ActiveItemSlot activeItemSlot;

    /// <summary>
    /// 스탯 값 갱신
    /// </summary>
    public void SetStats ()
    {
        power = (int)Power.Value;
        defense = (int)Defanse.Value;
        attackRange = (int)Range.Value;
    }

    bool isMove, isAttack, isHit;
    public Text playerHpText;
    public Image uiHpBar;
    public Image uiSieldBar;
    public bool isMagicTargeting;
    PlayerMagic playerMagic;
    int startHealth;
    Color originColor;
    Vector3 startPostion;

    public override int Health
    {
        get
        {
            return health;
        }

        set
        {
            health = value;

            if (health > 0)
            {
                healthBar.fillAmount = Mathf.Clamp01 ((float)health / maxHealth);
                uiHpBar.fillAmount = healthBar.fillAmount;

                if (shield <= 0)
                {
                    playerHpText.text = $"{health} / {maxHealth}";
                }

            }
            else
            {
                healthBar.fillAmount = 0;
                uiHpBar.fillAmount = 0;
            }
        }
    }

    public override int Shield
    {
        get
        {
            return shield;
        }

        set
        {
            shield = value;

            if (shield > 0)
            {
                uiSieldBar.fillAmount = 1;
                shieldBar.fillAmount = 1;
                playerHpText.text = $"Shield {shield}";
            }
            else if (shield <= 0)
            {
                shieldBar.fillAmount = 0;
                uiSieldBar.fillAmount = 0;
                playerHpText.text = $"{health} / {maxHealth}";
            }
        }
    }

    private void Awake ()
    {
        if (statsPanel == null)
        {
            statsPanel = FindObjectOfType<StatsPanel> ();
        }

        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory> ();
        }

        if (equipmentPanel == null)
        {
            equipmentPanel = FindObjectOfType<EquipmentPanel> ();
        }

        if (activeItemSlot == null)
        {
            activeItemSlot = FindObjectOfType<ActiveItemSlot> ();
        }

        statsPanel.SetStats (Power , Defanse , Range);
        statsPanel.UpdateStatValues ();
        statsPanel.UpdateStatNames ();

        SetStats ();

        inventory.OnItemClickEvent += EquipFromInventory;
        equipmentPanel.OnItemClickEnvet += UnequipFromEquipPanel;
    }

    void PlayerNewGame ()
    {
        StopAllCoroutines ();
        Debug.Log ("Player New Game()");
        maxHealth = startHealth;
        Health = startHealth;
        Shield = 0;
        isDead = false;
        isHit = false;
        playerMagic.MagicPoint = playerMagic.maxMagicPoint;
        material.color = originColor;
        transform.position = startPostion;
        Debug.Log ($"transform.position: {transform.position}");
        activeItemSlot.Item = null;
        inventory.InventroyReset ();
        ResetEquippableItem ();
        if (inventory.isInventoryOpen)
        {
            inventory.InventoryOpen ();
        }

        
    }

    protected override void Start ()
    {
        base.Start ();
        startHealth = maxHealth;
        playerMagic = GetComponent<PlayerMagic> ();
        startPostion = tileMapManager.CoordinatesToPostion (tileMapManager.currentMap.MapCenter);
        transform.position = startPostion;
        GameManager.Instance.ActionOnTurn += StartTurn;
        GameManager.Instance.ActionNewGame += PlayerNewGame;
        originColor = material.color;

    }

    // WASD, 화살표키를 입력받아서 이동, 스페이스바로 마법모드에 들어갔다가 빠져나옴
    void Update ()
    {
        if (isMove  || isAttack || isDead)
        {
            return;
        }

        if (Input.GetKeyDown (KeyCode.RightArrow) || Input.GetKeyDown (KeyCode.D))    // right
        {
            InputDirection (Direction.right);
        }
        else if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetKeyDown (KeyCode.A)) // left
        {
            InputDirection (Direction.left);
        }
        else if (Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.W))    // up
        {
            InputDirection (Direction.up);
        }
        else if (Input.GetKeyDown (KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.S))   // down
        {
            InputDirection (Direction.down);
        }

        if ((Input.GetKeyDown (KeyCode.Space)) && !isMove)
        {
            playerMagic.MagicInput ();
        }
    }


    public void MobileTouch (int direction)
    {
        if (isMove || isAttack || isDead)
        {
            return;
        }

        InputDirection ((Direction)direction);
    }


    void InputDirection (Direction direction)
    {
        // 마법 타게팅 중일때.
        if (isMagicTargeting)
        {
            playerMagic.MagicTageting (direction);
        }
        // 일반
        else
        {
            AttemptMove (PlayerDirection (direction) , direction);
        }
    }

    public Coordinates PlayerDirection (Direction direction)
    {
        return CoordinatesDirection (tileMapManager.playerCoordinates , direction);
    }

    public Coordinates CoordinatesDirection (Coordinates coordinates , Direction direction)
    {
        switch (direction)
        {
            case Direction.left:
                coordinates = coordinates.Left ();
                break;
            case Direction.right:
                coordinates = coordinates.Right ();
                break;
            case Direction.down:
                coordinates = coordinates.Down ();
                break;
            case Direction.up:
                coordinates = coordinates.Up ();
                break;
            default:
                break;
        }
        return coordinates;
    }

    #region 아이템 관련

    /// <summary>
    /// 인벤토리 아이템을 클릭하면 호출됨. (휴지통)isItemRemve가 트루일 경우 장착하지 않고 제거함
    /// </summary>
    /// <param name="item"></param>
    private void EquipFromInventory (Item item)
    {
        
        if (inventory.isItemRemve)
        {
            inventory.RemoveItem (item);

            return;
        }

        if (item is EquippableItem)
        {
            Equip ((EquippableItem)item);
        }
        else if (item is Portion || item is Scroll)
        {
            SetActiveItem (item);
        }
    }
    /// <summary>
    /// 장비 아이템 장착 해제
    /// </summary>
    /// <param name="item"></param>
    private void UnequipFromEquipPanel (Item item)
    {
        if (item is EquippableItem)
        {
            Unequip ((EquippableItem)item);
        }
    }

    /// <summary>
    /// 아이템 장착
    /// </summary>
    /// <param name="item">장착할 아이템</param>
    public void Equip (EquippableItem item)
    {
        if (inventory.RemoveItem (item))
        {
            EquippableItem previosuItem; // 이전에 장착했던 아이템을 받을 변수
            if (equipmentPanel.AddItem (item , out previosuItem))
            {
                // 장착 했던 아이템이 있을 경우 인벤토리에 추가함
                if (previosuItem != null)
                {
                    inventory.AddItem (previosuItem);
                    previosuItem.Unequip (this);    // 장착 해체 후 스텟 반영
                    statsPanel.UpdateStatValues ();
                }
                item.Equip (this);  // 장비한 아이템 스텟 반영
                statsPanel.UpdateStatValues ();  // 스텟 창 값 갱신
                SetStats ();
            }
            // 장착 실패 -> 인벤토리로 돌려 보냄
            else
            {
                inventory.AddItem (item);
            }
        }
    }

    void SetActiveItem (Item item)
    {
        if (inventory.RemoveItem (item))
        {
            if (activeItemSlot.Item != null)
            {
                inventory.AddItem (activeItemSlot.Item);
            }
            activeItemSlot.Item = item;
        }
    }

    /// <summary>
    /// 액티브 아이템 사용
    /// </summary>
    /// <param name="item"></param>
    /// <returns>아이템 사용하면 null 반환 </returns>
    public Item ActiveItem (Item item)
    {
        Item retunItem = item;
        if (item is Portion)
        {
            Portion portion = item as Portion;
    
            if (portion.HpPoint != 0 || portion.HpPercent != 0)
            {
                Health = (int)Mathf.Lerp (health , maxHealth , health + portion.HpPoint + maxHealth * (float)portion.HpPercent);
                retunItem = null;
            }

            if (portion.MpPoint != 0 || portion.MpPercent != 0)
            {
                playerMagic.MagicPoint = (int)Mathf.Lerp (playerMagic.magicPoint , playerMagic.maxMagicPoint ,
                    playerMagic.magicPoint + portion.HpPoint + playerMagic.maxMagicPoint * (float)portion.MpPercent);
                retunItem = null;
            }
        }
        else if (item is Scroll)
        {
            retunItem = null;
        }

        return retunItem;
    }

    /// <summary>
    /// 아이템 장착 해체
    /// </summary>
    /// <param name="item">장착 해체 할 아이템</param>
    public void Unequip (EquippableItem item)
    {
        // 인벤토리가 풀이 아닐때 && 장비 해체 성공
        if (!inventory.IsFull () && equipmentPanel.RemoveItem (item))
        {
            item.Unequip (this);    // 장착 해체 후 스텟 반영
            statsPanel.UpdateStatValues ();
            inventory.AddItem (item);   // 인벤토리에 장착 해체한 아이템 추가
            SetStats ();
        }
    }

    // 장착 아이템 리셋
    public void ResetEquippableItem ()
    {
        List<EquippableItem> UnequipItem = equipmentPanel.EquipmentSlotsReset ();

        if (UnequipItem != null)
        {
            for (int i = 0 ; i < UnequipItem.Count ; i++)
            {
                UnequipItem[i].Unequip (this);
            }
        }
        statsPanel.UpdateStatValues ();
        SetStats ();
    }

    #endregion


    // 이동, 공격 확인.
    void AttemptMove (Coordinates coordinates , Direction direction)
    {
        if (tileMapManager.TileCheck (coordinates)) // 이동하려는 좌표가 이동 가능한 타일인지 체크
        {
            Coordinates enemeyCoord = null;
            if (AttackCheck (coordinates , direction , out enemeyCoord))
            {
                // 에너미를 공격
                Attack (enemeyCoord);
            }
            else if (!tileMapManager.currentObject.ContainsKey (coordinates))
            {
                // 플레이어 이동
                StartCoroutine (SmoothMovement (tileMapManager.CoordinatesToPostion (coordinates)));   
                tileMapManager.playerCoordinates = coordinates;
            }
            else if (tileMapManager.currentItems.ContainsKey(coordinates))   
            {
                // 아이템 획득
                GetItem (coordinates);
            }
        }
    }

    void GetItem (Coordinates coordinates)
    {
        tileMapManager.currentItems[coordinates].GetItemPlayer();
    }

    bool AttackCheck (Coordinates coordinates, Direction direction, out Coordinates isEnemeyCoord)
    {
        for (int i = 0 ; i < attackRange ; i++)
        {
            if (tileMapManager.currentEnemys.ContainsKey(coordinates))
            {
                isEnemeyCoord = coordinates;
                return true;
            }
            else
            {
                coordinates = CoordinatesDirection (coordinates , direction);
            }
        }

        isEnemeyCoord = null;
        return false;
    }

    // 파워값 입력 안하면 기본 공격력으로
    protected void Attack (Coordinates attackCoordinates)
    {
        if (tileMapManager.currentEnemys.ContainsKey (attackCoordinates))
        {

            tileMapManager.currentEnemys[attackCoordinates].TakeDamage (power);
            // 몬스터가 사라진 뒤에도 딕셔너리를 참조하려고 하기 때문에 오류 발생.
            //AttackAnimation (tileMapManager.currentEnemys[attackCoordinates]);
        }
        GameManager.Instance.NextTurn ();
    }

    // 데미지 받음
    public override void TakeDamage (int damge)
    {
        if (shield > 0)
        {
            if (damge > shield)
            {
                Shield = 0;
            }
            else
            {
                Shield -= damge;
            }
        }
        else
        {
            if (damge > defense)
            {
                damge -= defense;
            }
            else
            {
                damge = 1;
            }

            Health -= damge;

            if (!isHit)
            {
                StartCoroutine (TakeHit ());
            }
           
            if (health <= 0 && !isDead)
            {
                Die ();
                playerHpText.text = "Die";
                return;
            }
        }
    }

    public override void Die ()
    {
        isDead = true;
        GameManager.Instance.GameOver ();

    }

    // 부드럽게 end 까지 이동.
    protected override IEnumerator SmoothMovement (Vector3 end)
    {
        isMove = true;

        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > float.Epsilon)
        {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards (transform.position , end , inverseMoveTime * Time.deltaTime);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            transform.position = newPostion;

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }
        transform.position = end;
        isMove = false;
        GameManager.Instance.NextTurn ();
    }


    public void StartTurn ()
    {
        // 플레이어의 모든 코루틴 중단
        StopAllCoroutines ();
        isHit = false;
        material.color = originColor;
        transform.position = tileMapManager.CoordinatesToPostion (tileMapManager.playerCoordinates);
    }

    // 어택 애니메이션
    protected IEnumerator AttackAnimation (LivingObject livingObject)
    {
        isAttack = true;
        Vector3 originglPosition = transform.position;
        Vector3 dirToTarget = (livingObject.transform.position - transform.position).normalized;  // 타겟으로의 벡터
        Vector3 attackPosition = livingObject.transform.position - dirToTarget * 0.5f; // 에너미가 타겟 표면 조금 안까지 닿게 함.

        float attackSpeed = 3;  //공격 속도, 높을수록 빨라짐
        float percent = 0;  // 0~1까지의 값을 가짐

        Color originColor = material.color;
        material.color = Color.red;

        while (percent <= 1)
        {
            percent += Time.deltaTime * attackSpeed;
            // 공격 후에 돌아가야 하기 때문에 대칭함수를 보간값으로 사용. 
            // y = 4(-(x^2) + x) 0에서 출발 0.5에서 1, 1에 0으로 돌아옴. - 구글에 검색하면 곡선 이미지를 보여줌.
            // 보간 - 알려진 점들의 위치를 참조하여, 집합의 일정 범위의 점들(선)을 새롭게 그리는 방법.
            // Lerp 메소드는 두 벡터 사이에 비례값 (0~1 사이) 으로 내분점 지점을 반환함.
            // 0이면 처음 위치 1이면 두번쨰 위치 0.5면 두 위치의 중간값
            float interpolation = (-Mathf.Pow (percent , 2) + percent) * 4;  // 보간
            transform.position = Vector3.Lerp (originglPosition , attackPosition , interpolation);

            yield return null;  // Update 메소드가 완전히 수행된 이후 매 프레임 마다 실행.
        }
        material.color = originColor;
        isAttack = false;
        GameManager.Instance.NextTurn ();
    }

    IEnumerator TakeHit ()
    {
        float percent = 0;
        float flashSpeed = 3;
        isHit = true;

        while (percent <= 1)
        {
            percent += Time.deltaTime * flashSpeed;

            material.color = new Color (Mathf.Lerp (Color.red.r , originColor.r , percent) , Mathf.Lerp (Color.red.g , originColor.g , percent) , Mathf.Lerp (Color.red.b , originColor.b , percent));

            yield return null;
        }

        isHit = false;
        material.color = originColor;
    }
}

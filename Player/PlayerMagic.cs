using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMagic : MonoBehaviour
{
    MagicBook magicBook;
    [SerializeField] CurrentMagicSlot currentMagicSlot;
    TileMapManager tileMapManager;
    [SerializeField] Magic currentMagic;
    Player player;
    int magicCastingCount;
    public int maxMagicPoint;
    public int magicPoint = 10;
    public int turnUpMagicPoint = 1;
    public Image uiMpBar;
    public Text playerMpText;
    public Transform magicMarker;
    public Coordinates magicMarkerCoordinates;
    public LineRenderer magicLineEffte;
    public ParticleSystem magicParticle;
    Direction magicDirection;

    public int MagicPoint
    {
        get { return magicPoint; }
        set
        {
           
            magicPoint = value;

            if (magicPoint > 0)
            {
                uiMpBar.fillAmount = Mathf.Clamp01 ((float)magicPoint / maxMagicPoint);
            }
            else
            {
                uiMpBar.fillAmount = 0;
            }

            playerMpText.text = magicPoint + " / " + maxMagicPoint;

        }
    }

    public Action<Magic> currentMagicClick;

    private void Start ()
    {
        if (magicBook == null)
        {
            magicBook = FindObjectOfType<MagicBook> ();
        }

        if (currentMagicSlot == null)
        {
            currentMagicSlot = FindObjectOfType<CurrentMagicSlot> ();
        }

        tileMapManager = GameManager.Instance.tileMapManager;
        player = GetComponent<Player> ();
        magicBook.OnMagicClickEvent += MagicChange;
        currentMagicSlot.OnClickEvent += MagicInput;
        magicBook.MagicActionSet ();
        magicMarker.gameObject.SetActive (false);

        if (currentMagic != null)
        {
            currentMagicSlot.Magic = currentMagic;
        }

        magicPoint = maxMagicPoint;

        GameManager.Instance.ActionOnTurn += TurnRegenMagicPoint;
    }


    public void MagicTageting (Direction direction)
    {
        Coordinates coordinates;
        if (currentMagic.type == Magic.Type.shoot || !currentMagic.targeting)
        {
            if (tileMapManager.TileCheck (coordinates = player.PlayerDirection (direction)))
            {
                magicMarkerCoordinates = coordinates;
                magicMarker.position = tileMapManager.CoordinatesToPostion (magicMarkerCoordinates) + Vector3.up * 0.2f;
                magicDirection = direction;
            }
        }
        else
        {
            if (tileMapManager.TileCheck (coordinates = player.CoordinatesDirection (magicMarkerCoordinates , direction)) && tileMapManager.RangeCheck (tileMapManager.playerCoordinates , coordinates , currentMagic.range))
            {
                magicMarkerCoordinates = coordinates;
                magicMarker.position = tileMapManager.CoordinatesToPostion (magicMarkerCoordinates) + Vector3.up * 0.2f;
            }
        }
    }

    void MagicChange (Magic magic)
    {
        MagicCastingCancel ();
        currentMagic = magic;
        currentMagicSlot.Magic = magic;
    }

    public void MagicInput ()
    {
        MagicCasting ();
    }

    // 캐스팅 카운터를 증가 시키고, 카운터를 만족 했을때 마법 발동
    public void MagicCasting ()
    {
        if (currentMagic != null)
        {
            if (currentMagic.castingCount <= 0 || currentMagic.castingCount <= magicCastingCount)
            {
                MagicCastingCompleteSwitch ();
            }
            else
            {
                magicCastingCount++;
                GameManager.Instance.NextTurn ();
            }
        }
    }

    // 마법 캐스팅 취소, 초기화
    void MagicCastingCancel ()
    {
        magicCastingCount = 0;

        if (player.isMagicTargeting)
        {
            magicMarker.gameObject.SetActive (false);
            player.isMagicTargeting = false;
        }
    }

    // 캐스팅 완료후 마법 타입에 따라서 바로 동작할지, 타게팅을 할지 넘어감.
    // 타케팅 완료후 호출할시 각각에 맞는 동작을 실행.
    public void MagicCastingCompleteSwitch ()
    {
        if (MagicPoint < currentMagic.magicPointCost)
        {
            GameManager.Instance.NextTurn ();
            return;
        }

        if (currentMagic.range == 0)
        {
            MagicActivationTypeSelf ();
            MagicFeedback ();
            return;
        }

        // 랜덤 발동은 타게팅이 필요 없음.
        if (currentMagic.type == Magic.Type.random)
        {
            MagicActivationTypeRandom ();
            MagicFeedback ();
            GameManager.Instance.NextTurn ();
        }
        else
        {
            // 타게팅 안했을때
            if (!player.isMagicTargeting)
            {
                MagicMakerSetUp ();
                player.isMagicTargeting = true;
            }
            else
            {
                if (currentMagic.type == Magic.Type.impact)
                {
                    MagicActivationTypeImpact ();
                    MagicFeedback ();
                    GameManager.Instance.NextTurn ();
                }
                // 마법 타입에 따라 다른 마법 발동
                else if (magicMarkerCoordinates != tileMapManager.playerCoordinates)
                {
                    switch (currentMagic.type)
                    {
                        case Magic.Type.shoot:
                            MagicActivationTypeShoot ();
                            break;
                        case Magic.Type.move:
                            MagicActivationTypeMove ();
                            break;
                        case Magic.Type.chain:
                            if (tileMapManager.currentEnemys.ContainsKey (magicMarkerCoordinates))
                            {
                                MagicActivationTypeChain (magicMarkerCoordinates);
                            }
                            break;
                        default:
                            break;
                    }
                    MagicFeedback ();
                    GameManager.Instance.NextTurn ();
                }
            }
        }
    }

    // 마법 사용후 공통으로 적용 될 부분.
    void MagicFeedback ()
    {
        if (currentMagic.magicPointCost > 0)
        {
            MagicPoint = Mathf.Clamp ((MagicPoint - currentMagic.magicPointCost) , 0 , maxMagicPoint);
        }

        if (currentMagic.shieldPoint > 0)
        {
            player.Shield += currentMagic.shieldPoint;
        }

        magicCastingCount = 0;
        magicMarker.gameObject.SetActive (false);
        player.isMagicTargeting = false;
    }

    // 마커 세팅
    void MagicMakerSetUp ()
    {
        player.isMagicTargeting = true;
        magicMarkerCoordinates = tileMapManager.playerCoordinates;
        magicMarker.position = tileMapManager.CoordinatesToPostion (magicMarkerCoordinates) + Vector3.up * 0.2f;
        magicMarker.gameObject.SetActive (true);
    }

    // 자신에게 사용하는 마법
    void MagicActivationTypeSelf ()
    {
        player.TakeDamage (currentMagic.damage);
    }

    // Shoot 마법
    void MagicActivationTypeShoot ()
    {
        if (magicMarkerCoordinates != tileMapManager.playerCoordinates)
        {
            for (int i = 0 ; i < currentMagic.range ; i++)
            {
                if (tileMapManager.currentEnemys.ContainsKey (magicMarkerCoordinates))
                {
                    MagicAttack (magicMarkerCoordinates , currentMagic.damage);
                    break;
                }
                switch (magicDirection)
                {
                    case Direction.left:
                        if (tileMapManager.TileCheck (magicMarkerCoordinates.Left ()))
                        {
                            magicMarkerCoordinates = magicMarkerCoordinates.Left ();
                        }
                        else
                        {
                            return;
                        }
                        break;
                    case Direction.right:
                        if (tileMapManager.TileCheck (magicMarkerCoordinates.Right ()))
                        {
                            magicMarkerCoordinates = magicMarkerCoordinates.Right ();
                        }
                        else
                        {
                            return;
                        }
                        break;
                    case Direction.down:
                        if (tileMapManager.TileCheck (magicMarkerCoordinates.Down ()))
                        {
                            magicMarkerCoordinates = magicMarkerCoordinates.Down ();
                        }
                        else
                        {
                            return;
                        }
                        break;
                    case Direction.up:
                        if (tileMapManager.TileCheck (magicMarkerCoordinates.Up ()))
                        {
                            magicMarkerCoordinates = magicMarkerCoordinates.Up ();
                        }
                        else
                        {
                            return;
                        }
                        break;
                    default:
                        break;
                }
            }

        }
    }

    // 랜덤 발동
    void MagicActivationTypeRandom ()
    {
        int magicRange = currentMagic.range;

        for (int i = 0 ; i < currentMagic.randomCount ; i++)
        {
            // 플레이어 좌표를 기준으로 magicRange 범위 중 랜덤 좌표를 구
            Coordinates randomCoordinates = new Coordinates (UnityEngine.Random.Range (tileMapManager.playerCoordinates.x - magicRange , tileMapManager.playerCoordinates.x + magicRange) ,
                UnityEngine.Random.Range (tileMapManager.playerCoordinates.y - magicRange , tileMapManager.playerCoordinates.y + magicRange));

            if (randomCoordinates == tileMapManager.playerCoordinates)
            {
                player.TakeDamage (currentMagic.damage);
            }
            else if (tileMapManager.TileCheck (randomCoordinates))
            {

                if (tileMapManager.currentEnemys.ContainsKey (randomCoordinates))
                {
                    MagicAttack (randomCoordinates , currentMagic.damage);
                }
            }
            else
            {
                // 좌표가 실제로 맵상에 존재하지 않는 좌표일경우 다시 반복함
                i--;
            }
        }
    }

    // 체인마법, 주변 범위에 적이 있으면 연쇄함.
    // 재귀함수 사용
    // 코루틴으로 변경, 이펙트 추가 예정
    void MagicActivationTypeChain (Coordinates coordinates , int chainCount = 0)
    {
        if (tileMapManager.currentEnemys.ContainsKey (coordinates))
        {
            tileMapManager.currentEnemys[coordinates].TakeDamage (currentMagic.damage);
        }

        if (chainCount < currentMagic.chainCount)
        {
            foreach (var item in tileMapManager.currentEnemys.Keys)
            {
                if (coordinates != item && tileMapManager.RangeCheck (coordinates , item , currentMagic.chainRange))
                {
                    MagicActivationTypeChain (item , chainCount + 1);
                    break;
                }
            }

        }
    }

    // 범위 데미지 마법
    void MagicActivationTypeImpact ()
    {
        if (tileMapManager.currentEnemys.ContainsKey (magicMarkerCoordinates))
        {
            tileMapManager.currentEnemys[magicMarkerCoordinates].TakeDamage (currentMagic.damage);
        }

        foreach (var item in tileMapManager.currentEnemys.Keys)
        {
            if (tileMapManager.RangeCheck (magicMarkerCoordinates , item , currentMagic.impactRange + currentMagic.splashRange))
            {
                if (tileMapManager.RangeCheck (magicMarkerCoordinates , item , currentMagic.impactRange))
                {
                    MagicAttack (item,currentMagic.damage);
                }
                else
                {
                    MagicAttack (item, Mathf.RoundToInt (currentMagic.damage * currentMagic.splashDamage));
                }
            }
        }
    }

    // 이동 마법
    void MagicActivationTypeMove ()
    {
        // 이동하려는 위치에 에너미가 있으면 에너미랑 위치를 변경하게 함.
        if (tileMapManager.currentEnemys.ContainsKey (magicMarkerCoordinates))
        {
            tileMapManager.currentEnemys[magicMarkerCoordinates].transform.position = tileMapManager.CoordinatesToPostion (tileMapManager.playerCoordinates);
            tileMapManager.currentEnemys[magicMarkerCoordinates].ResetCoordinatesData (tileMapManager.playerCoordinates);
        }
        else if (tileMapManager.currentItems.ContainsKey (magicMarkerCoordinates))
        {
            tileMapManager.currentItems[magicMarkerCoordinates].GetItemPlayer ();
        }

        tileMapManager.playerCoordinates = magicMarkerCoordinates;

        transform.position = tileMapManager.CoordinatesToPostion (magicMarkerCoordinates);
    }

    void MagicAttack (Coordinates coordinates , int damgae)
    {
        if (tileMapManager.currentEnemys.ContainsKey (coordinates))
        {
            Vector3 taget = tileMapManager.CoordinatesToPostion (coordinates);

            /*
            LineRenderer line = Instantiate (magicLineEffte , Vector3.Lerp(transform.position, taget, 0.5f) + new Vector3(0, 0.5f), Quaternion.identity);
            line.SetPositions (new Vector3[] { transform.position , taget });
            Destroy (line , 0.5f);
            */

            Destroy (Instantiate (magicParticle , taget + Vector3.up, Quaternion.identity).gameObject , 0.3f);


            tileMapManager.currentEnemys[coordinates].TakeDamage (damgae);
        }
    }

    public void StartGame ()
    {
        MagicPoint = maxMagicPoint;
        magicBook.MagicActionSet ();
        magicMarker.gameObject.SetActive (false);
    }

    void TurnRegenMagicPoint ()
    {
        MagicPoint += Mathf.Clamp (turnUpMagicPoint , 0 , maxMagicPoint- MagicPoint);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 추격, 도주 부분에 대한 개선이 필요함

public class Enemy : LivingObject
{
    public Coordinates coordinates;
    Player player;
    public bool firstAttack = true;

    // 도주
    public bool escape;
    [Range (0 , 1)]
    public float escapeHp;  // 퍼센트 이하일때 도주

    // 턴
    public int startTurnCounter = 2; // 시작 턴
    public int turnCounter;
    public int seeRange = 3;

    protected override void Start ()
    {
        base.Start ();
        GameManager.Instance.ActionOnTurn += StartTurn;
        player = GameObject.FindWithTag ("Player").GetComponent<Player> ();
    }

    // EnemyData 설정, 인덱스값을 받아옴
    public void GeneratorSetUp (EnemyData enemyData, Coordinates coordinates)
    {
        power = enemyData.dameg;
        maxHealth = enemyData.healt;
        health = enemyData.healt;
        attackRange = enemyData.attackRange;
        startTurnCounter = enemyData.turnCounter;
        firstAttack = enemyData.firstAttack;
        escape = enemyData.escape;
        escapeHp = enemyData.escapeHp;
        seeRange = enemyData.seeRange;
        this.coordinates = coordinates;
        healthBar.fillAmount = health / maxHealth;
        shieldBar.fillAmount = 0;
    }

    // 턴 시작, 델리게이트로 불러옴.
    public void StartTurn ()
    {
        if (firstAttack && turnCounter-- <= 1)
        {
            turnCounter = startTurnCounter;
            GetMoveCoordinates ();
        }
    }

    // 플레이어와 자신의 거리를 계산해서, 판단
    // 이동방향, 로직 개선 필요함.
    // 이동방향이 다른 에너미와 겹치는지 판정할 필요성 있음.
    protected void GetMoveCoordinates ()
    {
        // 플레이어와 에너미 사이의 거리를 구합니다.
        int betweenX = coordinates.x - tileMapManager.playerCoordinates.x;
        int betweenY = coordinates.y - tileMapManager.playerCoordinates.y;

        // 플레이어와의 거리를 가져와서, 인식 범위 밖이면 빠져나감.
        if (!((Mathf.Abs (betweenX) <= seeRange && Mathf.Abs (betweenY) <= seeRange) || seeRange == 0))
        {
            return;
        }

        // escape(도주)가 트루 일때 escapeHp 로 지정한 퍼센트 이하 일때 도주함 
        if (escape && (escapeHp >= (health / maxHealth)))
        {
            Escape (betweenX , betweenY);
        }
        else
        {
            Chase (betweenX , betweenY);
        }
    }

    // 추적
    void Chase (int betweenX , int betweenY)
    {
        if (betweenX == 0) // x축 (가로로 같음)
        {
            if (Mathf.Abs (betweenY) <= attackRange) // 공격 사정거리 안임.
            {
                AttemptMove (tileMapManager.playerCoordinates);
            }
            else if (betweenY > 0)
            {
                AttemptMove (coordinates.Down ());
            }
            else if (betweenY < 0)
            {
                AttemptMove (coordinates.Up ());
            }
        }
        else if (betweenY == 0)    // y축 (세로로 같음)
        {
            if (Mathf.Abs (betweenX) <= attackRange)   // 
            {
                AttemptMove (tileMapManager.playerCoordinates);
            }
            else if (betweenX > 0)
            {
                AttemptMove (coordinates.Left ());
            }
            else if (betweenX < 0)
            {
                AttemptMove (coordinates.Right ());
            }
        }
        else if (Mathf.Abs (betweenY) < Mathf.Abs (betweenX)) // x가 y보다 크다 - 세로가 더 가깝다.
        {
            if (betweenY > 0)
            {
                AttemptMove (coordinates.Down ());
            }
            else if (betweenY < 0)
            {
                AttemptMove (coordinates.Up ());
            }
        }
        else if (Mathf.Abs (betweenY) >= Mathf.Abs (betweenX))    // y가 x보다 크거나 같다 - 가로로 가깝거나 같다.
        {
            if (betweenX > 0)
            {
                AttemptMove (coordinates.Left ());
            }
            else if (betweenX < 0)
            {
                AttemptMove (coordinates.Right ());
            }
        }
    }

    // 도주
    void Escape (int betweenX , int betweenY)
    {
        if (betweenX == 0) // x축 (가로로 같음)
        {
            if (Mathf.Abs (betweenY) <= attackRange) // 공격 사정거리 안임.
            {
                AttemptMove (tileMapManager.playerCoordinates);
            }
            else if (betweenY > 0)
            {
                AttemptMove (coordinates.Up ());
            }
            else if (betweenY < 0)
            {
                AttemptMove (coordinates.Down ());
            }
        }
        else if (betweenY == 0)    // y축 (세로로 같음)
        {
            if (Mathf.Abs (betweenX) <= attackRange)   // 
            {
                AttemptMove (tileMapManager.playerCoordinates);
            }
            else if (betweenX > 0)
            {
                AttemptMove (coordinates.Right ());
            }
            else if (betweenX < 0)
            {
                AttemptMove (coordinates.Left ());
            }
        }
        else if (Mathf.Abs (betweenY) < Mathf.Abs (betweenX)) // x가 y보다 크다 - 세로가 더 가깝다.
        {
            if (betweenY > 0)
            {
                AttemptMove (coordinates.Up ());
            }
            else if (betweenY < 0)
            {
                AttemptMove (coordinates.Down ());
            }
        }
        else if (Mathf.Abs (betweenY) >= Mathf.Abs (betweenX))    // y가 x보다 크거나 같다 - 가로로 가깝거나 같다.
        {
            if (betweenX > 0)
            {
                AttemptMove (coordinates.Right ());
            }
            else if (betweenX < 0)
            {
                AttemptMove (coordinates.Left ());
            }
        }
    }

    // 공격 범위 안에 플레이어가 있으면 공격, 없으면 이동 가능하면 이동
    void AttemptMove (Coordinates moveCoordinates)
    {
        if (tileMapManager.TileCheck (moveCoordinates))    // 이동하려는 타일 입력 확인. - 인풋, 에너미 AI의 코드에 따라 제거 가능.
        {
            if (tileMapManager.PlayerCheck (moveCoordinates))  // 플레이어 인지 체크
            {
                // 공격 실행
                Attack (player);
                StartCoroutine (AttackAnimation ());
            }
            else if (!tileMapManager.currentObject.ContainsKey (moveCoordinates))
            {
                ResetCoordinatesData (moveCoordinates);
                coordinates = moveCoordinates;
                StartCoroutine (SmoothMovement (tileMapManager.CoordinatesToPostion (moveCoordinates)));   // 이동하는 코루틴 실행
            }
            else
            {
                
            }
        }
    }

    public void ResetCoordinatesData (Coordinates moveCoordinates)
    {
        tileMapManager.currentEnemys.Remove (coordinates);
        tileMapManager.currentEnemys.Add (moveCoordinates , this as Enemy);
        tileMapManager.currentObject.Remove (coordinates);
        tileMapManager.currentObject.Add (moveCoordinates , TileMapManager.ObjectType.enemy);
        coordinates = moveCoordinates;
    }

    // 데미지 받은 경우
    public override void TakeDamage (int damge)
    {
        if (shield > 0)
        {
            Shield -= damge;
        }
        else
        {
            Health -= damge;

            if (health <= 0 && !isDead)
            {
                Die ();
            }
            else
            {
                if (!firstAttack)
                {
                    firstAttack = true;
                }
            }
        }
    }

    // 죽었을때 실행.
    public override void Die ()
    {
        GameManager.Instance.ActionOnTurn -= StartTurn;
        tileMapManager.currentEnemys.Remove (coordinates);
        tileMapManager.currentObject.Remove (coordinates);
        isDead = true;
        GameObject.Destroy (this.gameObject);
    }

    // 에너미 공격 - 임시 사용 , 공격 했는지 확인 용임.
    // 에너미가 플레이어를 찌르는 애니메이션
    // 공격할때 색상이 변함
    IEnumerator AttackAnimation ()
    {
        Vector3 originglPosition = transform.position;
        Vector3 dirToTarget = (player.transform.position - transform.position).normalized;  // 타겟으로의 벡터
        Vector3 attackPosition = player.transform.position - dirToTarget * 0.5f; // 에너미가 타겟 표면 조금 안까지 닿게 함.

        float attackSpeed = 3;  //공격 속도, 높을수록 빨라짐
        float percent = 0;  // 0~1까지의 값을 가짐

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
        material.color = Color.white;
    }

    // 지정한 좌표로 스무스 하게 이동 합니다.
    protected override IEnumerator SmoothMovement (Vector3 end)
    {
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
    }

}

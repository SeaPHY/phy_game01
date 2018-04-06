using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Player, Enemy의 부모
/// </summary>
public class LivingObject : MonoBehaviour
{
    public int maxHealth;
    public int health;
    public int power;
    public int defense;
    public int move;
    public int shield;
    public int attackRange = 1;

    public float moveTime = 0.1f;
    protected float inverseMoveTime;
    protected bool isDead;
    protected TileMapManager tileMapManager;
    protected Material material;
    public Image healthBar;
    public Image shieldBar;

    public virtual int Health
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
            }
            else
            {
                healthBar.fillAmount = 0;
            }
        }
    }

    public virtual int Shield
    {
        get
        {
            return shield;
        }

        set
        {
            value = shield;

            if (shield > 0)
            {
                shieldBar.fillAmount = 1;
            }
            else
            {
                shieldBar.fillAmount = 0;
            }
        }
    }
    

    protected virtual void Start ()
    {
        inverseMoveTime = 1f / moveTime;
        tileMapManager = GameManager.Instance.tileMapManager;
        material = GetComponentInChildren<MeshRenderer> ().material;
        Health = maxHealth;
        isDead = false;
        Shield = 0;
    }

    // 공격 시도
    protected virtual void Attack<T> (T hitObject) where T : LivingObject
    {
        hitObject.TakeDamage (power);
    }

    // 데미지 받은 경우
    public virtual void TakeDamage (int damge)
    {
        if (Shield > 0)
        {
            Shield -= damge;

        }
        else
        {
            Health -= damge;

            if (Health <= 0 && !isDead)
            {
                Die ();
            }
        }
    }

    // 사망
    public virtual void Die ()
    {
        isDead = true;
        this.gameObject.SetActive (false);
    }

    // 이동 코루틴.
    protected virtual IEnumerator SmoothMovement (Vector3 end)
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

        //nowCoordinates = TileMapManager.Instance.GetCoordFromPosition (transform.position);
    }

}

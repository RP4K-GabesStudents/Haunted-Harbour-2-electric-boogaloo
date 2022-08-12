using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BulletTypes;


[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))] //forces gameobject to have these components


public class Character : MonoBehaviour
{
    //SerializedField means access in unity
    [SerializeField] protected int health; //can be accessed only by parent and child
    public int maxHealth;

    [SerializeField] protected int jumpForce;
    [SerializeField] public float moveSpeed;
    [SerializeField] protected float velocityLimit;


    [Header("TEMPORARY")] //keeps organized
    [SerializeField] protected bool isShooting;
    [SerializeField] protected bool isShootingCircle;
    [SerializeField] public Vector2 movementVector; //this

    [SerializeField] private AudioClip shootSound;

    public float distance; //for the enemy distance


    [Header("JUMPING")]
    [SerializeField] private int numJumps; //what
    private int jumpNum = 0;
    private float jumpTime;
    [SerializeField] private float endJumpTime;


    [Header("SHOOTING")]
    [SerializeField] private float shootDelay;

    [SerializeField] protected bool targetMouse;
    [SerializeField] public Transform targetObject;

    [SerializeField] GameObject bullet;
    [SerializeField] float bulletSpeed;
    private EBulletType bulletType;

    [SerializeField] float spread;
    [SerializeField] int bulletsToShoot;

    private Camera c;
    private float shootTimer = 0;

    private static readonly int Direction = Animator.StringToHash("Direction");
    private float dir; //last direction of the player
    private static readonly int Movement = Animator.StringToHash("MovementX");
    private static readonly int IsAlive = Animator.StringToHash("IsAlive");
    private static readonly int IsShooting = Animator.StringToHash("IsShooting");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");


    protected Rigidbody2D myRB; //Storage
    protected Animator animator;


    //Awake is called when a character is created
    protected virtual void Awake()
    {
        myRB = GetComponent<Rigidbody2D>(); //When character awake set RB
        animator = GetComponent<Animator>();
        c = Camera.main;

        maxHealth = health; //set max health to be starting health
    }


    // Update is called once per frame
    void FixedUpdate() //generally use fixed for physics based movement bc it runs on time
    {
        Move();
        HandleAnimations();
        Shoot();
        CircleShoot();
    }


    protected virtual void HandleAnimations()
    {
        if (movementVector.x != 0)
        {
            dir = movementVector.x;
        }

        animator.SetFloat(Direction, dir);
        animator.SetFloat(Movement, Mathf.Abs(movementVector.x));
        animator.SetBool(IsAlive, health > 0);
        animator.SetBool(IsShooting, isShooting);

        //TODO: fix this 
        animator.SetBool(IsGrounded, (Mathf.Abs(myRB.velocity.y) - 0.01f <= 0)); //tolerance 
    }


    //can move -- differently
    protected virtual void Move()
    {
        //Delta -- change in, sub between 2 pts
        myRB.AddForce(moveSpeed * Time.deltaTime * new Vector2(movementVector.x, 0), ForceMode2D.Impulse); //l/r movement

        if (dir * myRB.velocity.x > velocityLimit)
        {
            myRB.velocity = new Vector2(dir * velocityLimit, myRB.velocity.y);
        }

        jumpTime += Time.deltaTime;

        if (myRB.velocity.y == 0) //if we are grounded
        {
            jumpNum = 0; //reset jump counter
        }

        if (movementVector.y > 0 && jumpTime > endJumpTime) //if player asks to jump
        {
            jumpTime = 0; //reset timer
            if (jumpNum++ < numJumps) //if player has jumps allowed
            {
                Jump();
            }
        }



    }


    protected void Jump()
    {
        myRB.velocity = new Vector2(myRB.velocity.x, 0);
        myRB.AddForce(jumpForce * Time.deltaTime * new Vector2(0, movementVector.y), ForceMode2D.Impulse);
    }


    public virtual void TakeDamage(int damage, Vector3 force)
    {
        health -= damage;
        //Made force + because was moving wrong way
        myRB.AddForce(force * damage, ForceMode2D.Impulse); //impulse so that it doesnt accelerate (instant force)

        if (health <= 0)
        {
            Die();
        }
    }


    protected void Shoot()
    {
        shootTimer += Time.deltaTime;
        if (isShooting && shootTimer > shootDelay && health > 0) //if player is trying to shoot, check timer
        {
            shootTimer = 0; //reset timer


            //select direction
            Vector2 directionShoot;

            if (targetObject)
            {
                Vector2 line = (targetObject.position - transform.position);
                if (line.magnitude > distance)
                {
                    return;
                }
                directionShoot = line.normalized;
            }
            else if (targetMouse)
            {
                directionShoot = ((Vector2)c.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;
            }
            else
            {
                directionShoot = new Vector2(dir, 0); //if there's no target, shoot in the previously registered direction
            }


            for (int i = 0; i < bulletsToShoot; i++)
            {
                Vector2 newVec = directionShoot;
                if (i > 1)
                {
                    float angle = Mathf.Atan2(directionShoot.y, directionShoot.x) * Mathf.Rad2Deg;
                    angle += Random.Range(-spread, spread);

                    newVec.x = Mathf.Cos(angle * Mathf.Deg2Rad);
                    newVec.y = Mathf.Sin(angle * Mathf.Deg2Rad);
                }
                startShoot(newVec);   //shoot
            }
        }
    }

    protected void CircleShoot()
    {
        shootTimer += Time.deltaTime;

        if (isShootingCircle && shootTimer > shootDelay) //if player is trying to shoot, check timer
        {
            shootTimer = 0; //reset timer


            //select direction
            Vector2 directionShoot;
            directionShoot = new Vector2(dir, 0);

            for (int i = 0; i <= 36; i++)
            {
                Vector2 newVec = directionShoot;
                if (i > 1)
                {

                    float angle = Mathf.Atan2(directionShoot.y, directionShoot.x) * Mathf.Rad2Deg;
                    angle += 10 * i;

                    newVec.x = Mathf.Cos(angle * Mathf.Deg2Rad);
                    newVec.y = Mathf.Sin(angle * Mathf.Deg2Rad);
                }
                startShoot(newVec);   //shoot
            }
        }
    }

    protected virtual void startShoot(Vector2 direction)
    {
        GameObject go = Instantiate(bullet, transform.position, Quaternion.identity); //create instance of a bullet, at char position, with no rotation

        go.GetComponentInChildren<Projectiles>().Init(gameObject.layer, 3); //hard coded for now, projectile lifetime of 5 seconds
        go.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

        GameManager.Instance.AudioManager.PlayOneShot(shootSound);
    }

    protected virtual void Die()
    {
        moveSpeed = 0; // Set movespeed to 0 when dead.
        jumpForce = 0; // Set jump to 0 when dead
        Destroy(gameObject, 3.0f);
    }

    public IEnumerator SetTempSpeed(float effectDuration, float speed)
    {
        float temp = moveSpeed;
        moveSpeed = speed;
        yield return new WaitForSeconds(effectDuration);
        moveSpeed = temp;
    }

    public IEnumerator SetOnFire(float effectDuration, int damage)
    {
        float curTime = 0;
        float intervalTime = 0;
        while (curTime < effectDuration)
        {
            intervalTime += Time.deltaTime;
            curTime += Time.deltaTime;

            if (intervalTime >= 0.1f)
            {
                TakeDamage(damage, Vector3.zero);
                intervalTime = 0;
            }
            yield return null;
        }
        yield return null;
    }
}


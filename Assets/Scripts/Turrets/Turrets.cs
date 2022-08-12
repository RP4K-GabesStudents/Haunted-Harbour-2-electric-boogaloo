using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public abstract class Turrets : MonoBehaviour
{
    //Turret Behavior
    [SerializeField] public int health;
    public bool isCloaked; //this doesn't determine if something is cloaked or not, it just tells the shoot function whether it is so we can not shoot from a cloaked turret
    public bool isOff;
    [SerializeField] protected bool isShooting;

    public float distance;

    //Shooting Behavior
    [SerializeField] GameObject bullet;
    [SerializeField] public GameObject[] bulletsList;

    [SerializeField] protected float shootTimer;
    [SerializeField] protected float burstShootDelay;
    [SerializeField] protected float shootDelay;
    [SerializeField] float bulletSpeed;
    [SerializeField] protected Transform targetObject;
    protected int shotgunBulletAllowance = 6;
    protected int shotgunSpread = 10;
    protected delegate void TurretAttackTypeDel(Turrets turret, Vector2 direction);
    protected Coroutine currentRoutine;

    //Audio

    [SerializeField] protected AudioClip cloakSound;
    [SerializeField] protected AudioClip uncloakSound;
    [SerializeField] protected AudioClip destructionSound;
    [SerializeField] protected AudioClip fireSound;

    [SerializeField] public Vector2 movementVector; //this

    private static readonly int Direction = Animator.StringToHash("Direction");
    private float dir; //last direction of the player
    private static readonly int Movement = Animator.StringToHash("MovementX");
    private static readonly int IsAlive = Animator.StringToHash("IsAlive");
    private static readonly int IsShooting = Animator.StringToHash("IsShooting");

    protected Animator animator;

    protected TurretAttackTypeDel selectedAttackType;

    protected virtual void Awake()
    {
        isOff = false;

        ChooseAttackType(0);
        ChooseBulletType(0);
        animator = GetComponent<Animator>();

    }

    // Start is called before the first frame update
    void Start()
    {
        targetObject = GameManager.Instance.Player.transform;
    }

    // fixed update, because we used time in shooting???
    protected virtual void FixedUpdate()
    {
        Shoot();
        HandleAnimations();

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

    }

    public virtual void Shoot()
    {
        isShooting = false;
        if (isCloaked) return;

        shootTimer += Time.deltaTime; //increment timer
        Vector2 line = (targetObject.position - transform.position); //get the distance from turret to player

        if (line.magnitude < distance && shootTimer > shootDelay) //if turret is within range and timer expired
        {
            //select direction and shoot there using the selected attack type
            Vector2 directionShoot = line.normalized;
            selectedAttackType?.Invoke(this, directionShoot);
            isShooting = true;

            //GameManager.Instance.AudioManager.PlayOneShot(fireSound); 
            //I originally put this here so every time it fired we'd get one noise but separate noises for burst might be better
            //So now SFX are handled in the respective attack types

            shootTimer = 0; //reset timer  
        }

    }

    //WHAT THIS DOES IS MAKE A BULLET
    protected virtual void startShoot(Vector2 direction)
    {
        GameObject go = Instantiate(bullet, transform.position, Quaternion.identity); //create instance of a bullet, at char position, with no rotation

        go.GetComponentInChildren<Projectiles>().Init(gameObject.layer, 3); //hard coded for now, projectile lifetime of 5 seconds
        go.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
    }


    protected void Basic(Turrets t, Vector2 direction)
    {
        startShoot(direction);
        GameManager.Instance.AudioManager.PlayOneShot(fireSound);
    }

    protected static IEnumerator Timer(Turrets t, Vector2 dir, float time)
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(time);
            t.startShoot(dir);
            GameManager.Instance.AudioManager.PlayOneShot(t.fireSound);
        }

        t.currentRoutine = null;
    }

    protected static void Burst(Turrets turret, Vector2 direction)
    {
        if (turret.currentRoutine == null)
            turret.currentRoutine = turret.StartCoroutine(Timer(turret, direction, turret.burstShootDelay));
    }

    protected static void Shotgun(Turrets turret, Vector2 direction)
    {
        for (int i = 0; i < turret.shotgunBulletAllowance; i++)
        {
            Vector2 newVec = direction;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (i <= 2)
            {
                angle += turret.shotgunSpread * i;
            }
            else if (i > 2)
            {
                angle += turret.shotgunSpread * 2; //because otherwise the middle spread is 20 and it looks off
                angle -= turret.shotgunSpread * i;
            }

            newVec.x = Mathf.Cos(angle * Mathf.Deg2Rad);
            newVec.y = Mathf.Sin(angle * Mathf.Deg2Rad);
            turret.startShoot(newVec);   //shoot
        }
        GameManager.Instance.AudioManager.PlayOneShot(turret.fireSound);
    }


    public enum TAttackType
    {
        Basic,
        Burst,
        Shotgun
    }


    protected void ChooseAttackType(int i)
    {
        TAttackType attackType = (TAttackType)i;

        switch (attackType)
        {
            case TAttackType.Basic:
                selectedAttackType = Basic;
                isShooting = true;
                break;
            case TAttackType.Burst:
                isShooting = true;
                selectedAttackType = Burst;
                break;
            case TAttackType.Shotgun:
                isShooting = true;
                selectedAttackType = Shotgun;
                break;
            default:
                break;
        }
    }


    //tried to pass it a parameter so that we can use it during random selection and whatnot
    //probably a better way to do this but IT WORKS
    protected void ChooseBulletType(int i)
    {
        EBulletType eBulletType = (EBulletType)i;
        bullet = bulletsList[(int)eBulletType];
    }


    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
        GameManager.Instance.AudioManager.PlayOneShot(destructionSound);
    }
}



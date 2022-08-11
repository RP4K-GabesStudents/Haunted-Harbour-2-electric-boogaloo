using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public abstract class Turrets : MonoBehaviour
{
    //Turret Behavior
    [SerializeField] public int health;
    public bool isCloaked; //this doesn't determine if something is cloaked or not, it just tells the shoot function whether it is so we can not shoot from a cloaked turret
    public float distance;
    SpriteRenderer sr;

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

    //I hate Static Utilities I hate them so much
    [SerializeField] private TAttackType attackType;

    protected TurretAttackTypeDel selectedAttackType;

    protected virtual void Awake()
    {
        ChooseAttackType();
        ChooseBulletType();
    }

    // Start is called before the first frame update
    void Start()
    {
        targetObject = GameManager.Instance.Player.transform;
    }

    // fixed update, because we used time in shooting???
    void FixedUpdate()
    {
        Shoot();
    }

    public void Shoot()
    {
        if (!isCloaked)
        {
            //is this function even necessary
            shootTimer += Time.deltaTime; //increment timer
            Vector2 line = (targetObject.position - transform.position); //get the distance from turret to player

            if (line.magnitude < distance && shootTimer > shootDelay) //if turret is within range and timer expired
            {
                //select direction and shoot there using the selected attack type
                Vector2 directionShoot = line.normalized;
                selectedAttackType?.Invoke(this, directionShoot);

                shootTimer = 0; //reset timer  
            }

        }
    }

    //WHAT THIS DOES IS MAKE A BULLET
    protected virtual void startShoot(Vector2 direction)
    {
        GameObject go = Instantiate(bullet, transform.position, Quaternion.identity); //create instance of a bullet, at char position, with no rotation

        go.GetComponentInChildren<Projectiles>().Init(gameObject.layer, 5); //hard coded for now, projectile lifetime of 5 seconds
        go.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
    }


    protected void Basic(Turrets t, Vector2 direction)
    {
        startShoot(direction);
    }

    protected static IEnumerator Timer(Turrets t, Vector2 dir, float time)
    {
        for (int i = 0; i < 3; i++)
        {
            t.startShoot(dir);
        }

        yield return new WaitForSeconds(time);
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

            if (i > 2)
            {
                angle += turret.shotgunSpread; //because otherwise the middle spread is 20 and it looks off
                angle -= turret.shotgunSpread * i;
            }

            newVec.x = Mathf.Cos(angle * Mathf.Deg2Rad);
            newVec.y = Mathf.Sin(angle * Mathf.Deg2Rad);
            turret.startShoot(newVec);   //shoot
        }
    }


    public enum TAttackType
    {
        Basic,
        Burst,
        Shotgun
    }

    private void ChooseAttackType()
    {
        switch (attackType)
        {
            case TAttackType.Burst:
                selectedAttackType = Burst;
                break;
            case TAttackType.Shotgun:
                selectedAttackType = Shotgun;
                break;
            case TAttackType.Basic:
                selectedAttackType = Basic;
                break;
            default:
                break;
        }
    }

    private void ChooseBulletType()
    {
        bullet = bulletsList[(int)EBulletType.Purple];
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
        GameManager.Instance.AudioManager.PlayOneShot(destructionSound);
    }


    protected void Cloak()
    {
        isCloaked = true;

        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(sr.ColorLerp(new Color(255, 255, 255, 0), 5));
        GameManager.Instance.AudioManager.PlayOneShot(cloakSound);
    }


    protected void Uncloak()
    {
        isCloaked = false;

        GameManager.Instance.AudioManager.PlayOneShot(uncloakSound);
    }

}



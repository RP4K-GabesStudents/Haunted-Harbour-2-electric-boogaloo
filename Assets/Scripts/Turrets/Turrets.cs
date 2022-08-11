using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Turrets : MonoBehaviour
{
    //Turret Behavior
    [SerializeField] public int health;
    public bool isCloaked;
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
        print("calling");

        //this never gets called. determine why
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
            if (i > 1)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                angle += Random.Range(-turret.shotgunSpread, turret.shotgunSpread);

                newVec.x = Mathf.Cos(angle * Mathf.Deg2Rad);
                newVec.y = Mathf.Sin(angle * Mathf.Deg2Rad);
            }
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
        print("ChooseAttackType is called");

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
        bullet = bulletsList[(int)EBulletType.Red];
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
        //play sound
    }


    protected void Cloak()
    {
        //play a sound
    }


    protected void Uncloak()
    {
        //play sound
    }

}



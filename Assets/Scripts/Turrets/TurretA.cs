using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretA : Turrets
{

    private float turretShootTimer;
    [SerializeField] private float turretMoveSpeed;

    private Rigidbody2D myRB;


    protected override void Awake()
    {
        base.Awake();
        
        int attack = UnityEngine.Random.Range(0, 3);
        int bullet = UnityEngine.Random.Range(0, 4);

        ChooseAttackType(attack);
        ChooseBulletType(bullet);

        myRB = GetComponent<Rigidbody2D>();

    }
    
    protected override void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.position.y > transform.position.y)
        {
            if (col.gameObject.TryGetComponent(out Player ply))
            {
                ply.StartCoroutine(ply.SetTempSpeed(5, ply.moveSpeed * 100));

            }

        }

        base.OnCollisionEnter2D(col);
    }

    private void Update()
    {
        turnOff();
        Shoot();
    }

    private void turnOff()
    {
        turretShootTimer += Time.deltaTime;

        if (turretShootTimer > 7) // if the turret has been shooting for 7 seconds
        {
            isOff = true;
            turretShootTimer = 0;
        }
        
        if (turretShootTimer > 3 && isOff) // if the turret has been off for 3 seconds
        {
            isOff = false;
            turretShootTimer = 0;
        }

    }

    public override void Shoot()
    {
        int attack = UnityEngine.Random.Range(0, 3);
        int bullet = UnityEngine.Random.Range(0, 4);

        ChooseAttackType(attack);
        ChooseBulletType(bullet); 

        base.Shoot();
        
        myRB.AddForce(turretMoveSpeed * Time.deltaTime * new Vector2(dir, 0), ForceMode2D.Impulse);
    }
 


}

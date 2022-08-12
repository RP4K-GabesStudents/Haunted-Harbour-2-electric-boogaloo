using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretA : Turrets
{

    private float turretShootTimer;

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

    private void turnOff()
    {


    }

    public override void Shoot()
    {
        //ChooseAttackType();
        //ChooseBulletType(); 
        //somehow pass it random enums here. i have no idea how to do this and may need to change the original function

        base.Shoot();
    }
 


}

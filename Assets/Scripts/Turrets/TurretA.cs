using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretA : Turrets
{

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
 
}

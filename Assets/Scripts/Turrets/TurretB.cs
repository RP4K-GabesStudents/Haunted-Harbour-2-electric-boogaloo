using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretB : Turrets
{
    SpriteRenderer sr;
    
    protected override void Awake()
       
    {
        base.Awake();

    sr = GetComponent<SpriteRenderer>();


        StartCoroutine(sr.ColorLerp(new Color(255, 255, 255, 0), 5));
    }

    
}

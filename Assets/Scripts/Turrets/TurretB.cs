using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretB : Turrets
{

    protected override void Awake()

    {
        base.Awake();
        //Cloak(); //this turret autocloaks
    }

    protected override void FixedUpdate()
    {

    }

    private void manageStealth()
    {
        //if within distance uncloak if outside cloak
    }

}

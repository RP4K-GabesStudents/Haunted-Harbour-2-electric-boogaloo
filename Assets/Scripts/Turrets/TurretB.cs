using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretB : Turrets
{

    [SerializeField] private AudioClip cloak;
    private bool isCloaked;

    protected override void Awake()

    {
        base.Awake();
        //Cloak(); //this turret autocloaks

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private void manageStealth()
    {
        //if within distance uncloak if outside cloak
        GameManager.Instance.AudioManager.PlayOneShot(cloak);

    }

}

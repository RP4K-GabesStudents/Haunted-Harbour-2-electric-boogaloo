using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretB : Turrets
{

    [SerializeField] private AudioClip cloak;
    private bool isCloaked;
    SpriteRenderer sr;

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

    public override void Shoot()
    {

    }

    protected void Cloak()
    {
        isCloaked = true;

        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(sr.ColorLerp(new Color(255, 255, 255, 0), 2));
        GameManager.Instance.AudioManager.PlayOneShot(cloakSound);
    }


    protected void Uncloak()
    {
        isCloaked = false;
        StartCoroutine(sr.ColorLerp(new Color(255, 255, 255, 255), 2));
        GameManager.Instance.AudioManager.PlayOneShot(uncloakSound);
    }
}

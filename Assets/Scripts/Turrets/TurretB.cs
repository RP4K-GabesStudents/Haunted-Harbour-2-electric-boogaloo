using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretB : Turrets
{
    //Audio
    [SerializeField] protected AudioClip cloakSound;
    [SerializeField] protected AudioClip uncloakSound;

    private bool isCloaked;
    SpriteRenderer sr;

    //jumping
    public Rigidbody2D myRB;
    private int jumpForce;

    protected override void Awake()

    {
        base.Awake();
        myRB = GetComponent<Rigidbody2D>();

        ChooseAttackType(2);
        ChooseBulletType(2);
        Cloak(); //this turret autocloaks
    }

    protected override void FixedUpdate()
    {
        HandleAnimations();
        ManageStealth();
        Shoot();
    }

    private void ManageStealth()
    {
        Vector2 line = (targetObject.position - transform.position);
        if (line.magnitude < distance)
        {
            Uncloak();
        }
        else
        {
            Cloak();
        }
    }

    public override void Shoot()
    {
        if (isCloaked) { isShooting = false; return; }
        DoSick360();
        base.Shoot();
    }

    public void DoSick360()
    {
        myRB.velocity = new Vector2(myRB.velocity.x, 0);
        myRB.AddForce(jumpForce * Time.deltaTime * new Vector2(0, movementVector.y), ForceMode2D.Impulse);
    }

    protected void Cloak()
    {
        isCloaked = true;
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(sr.ColorLerp(new Color(255, 255, 255, 0), 0));
        GameManager.Instance.AudioManager.PlayOneShot(cloakSound);
    }


    protected void Uncloak()
    {
        isCloaked = false;
        StartCoroutine(sr.ColorLerp(new Color(255, 255, 255, 255), 0));
        GameManager.Instance.AudioManager.PlayOneShot(uncloakSound);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    //on hit bullets invoke a delegate and destroy themselves
    //what delegate the bullet invokes depends on the prefab (in Unity)
    //if a bullet hits a character of opposite layer (?) then it will do damage

    //purple bullets -- Boring
    //fire bullets -- Fire Damage
    //red bullets -- Explode using CircleCast
    //green -- Freezes enemy forever

    //a delegate is a variable function
    private delegate void OnHit(Character hitObject);
    private OnHit hitInstance;
    [SerializeField] int damage;

    [SerializeField] private GameObject particle;
    [SerializeField] private float effectDuration;

    private Animator animator;
    private bool isExploding;

    [SerializeField] private EBulletType bulletType;

    private static readonly int BType = Animator.StringToHash("Type");
    private static readonly int IsExploding = Animator.StringToHash("IsExploding");

    [Header("EXPLOSION STUFF")]
    [SerializeField] private float radius;
    [SerializeField] private int explosionDamage;
    [SerializeField] private float explosionForce;


    private void Explode(Character other)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, radius, Vector2.right);

        //iterate through every hit
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.TryGetComponent(out Character c))
            {
                c.TakeDamage(explosionDamage, (hits[i].point - (Vector2)transform.position).normalized);
            }
        }
    }

    private void Freeze(Character other)
    {
        GameObject go = Instantiate(particle, other.transform);
        other.StartCoroutine(other.SetTempSpeed(effectDuration, 0));
        Destroy(go, effectDuration);
    }

    private void FireDamage(Character other)
    {
        GameObject go = Instantiate(particle, other.transform);
        other.StartCoroutine(other.SetOnFire(effectDuration, 5));
        Destroy(go, effectDuration);
    }

    //set layer
    public void Init(LayerMask gameObjectLayer, float lifeTime)
    {
        gameObject.layer = gameObjectLayer;
        Destroy(gameObject, lifeTime); //kill bullet after a bit
        animator = GetComponent<Animator>();

        switch (bulletType)
        {
            case EBulletType.Red:
                hitInstance = Explode;
                break;
            case EBulletType.Green:
                hitInstance = Freeze;
                break;
            case EBulletType.Fire:
                hitInstance = FireDamage;
                break;
        }


    }



    // Update is called once per frame
    void LateUpdate()
    {
        if (!animator) Destroy(gameObject);
        animator.SetBool(IsExploding, isExploding);
        animator.SetFloat(BType, (int)bulletType);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent(out Character character))
        {
            character.TakeDamage(damage, Vector3.zero);
            hitInstance?.Invoke(character);
        }
        Destroy(gameObject);
    }
}

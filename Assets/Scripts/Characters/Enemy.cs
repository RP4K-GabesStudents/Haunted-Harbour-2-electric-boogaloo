using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : Character
{

    //can do damage when they touch
    public float size { get; private set; } //public get - get anywhere, private set - can only be set in this class
    public float speed => moveSpeed;
    public float angle = 0;

    [SerializeField] private int contactDamage = 1;

    [SerializeField] private EAttackType attackType;
    [SerializeField] private EMovementType movementType;

    private AttackTypeDel selectedAttackType;
    private MovementTypeDel selectedMovementType; //variable pointing to function

    public float timer;
    public Coroutine currentRoutine;

    protected override void Awake()
    {
        base.Awake();

        ChooseMoveType();
        ChooseAttackType();

        size = transform.localScale.x / 2; //exception room
    }


    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        selectedAttackType?.Invoke(this);
        selectedMovementType?.Invoke(this); 
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent(out Player ply)) //Important for C#. Try get component, gives component only if it exists
        {
            Vector3 directionVector = (transform.position - ply.transform.position).normalized; //3 floats
            ply.TakeDamage(contactDamage, directionVector);
        }
    }


    private void ChooseAttackType()
    {
        switch (attackType)
        {
            case EAttackType.Constant:
                //selectedAttackType = AttackTypes.Constant;
                isShooting = true;
                break;
            case EAttackType.Targeting:
                selectedAttackType = AttackTypes.Targeting;
                break;
            default: //like else
                print("No attack type selected");
                break;
        }
    }


    private void ChooseMoveType()
    {
        myRB.simulated = true;
        switch (movementType)
        {
            case EMovementType.Dumb:
                selectedMovementType = MovementTypes.Dumb;
                break;
            case EMovementType.Circle:
                selectedMovementType = MovementTypes.Circle;
                break;
            case EMovementType.TargetGhost:
                selectedMovementType = MovementTypes.TargetGhost;
                myRB.simulated = false;
                break;
            case EMovementType.TargetSmart:
                selectedMovementType = MovementTypes.TargetSmart;
                break;
            default: //like else
                print("No move type selected");
                break;
        }
    }
}

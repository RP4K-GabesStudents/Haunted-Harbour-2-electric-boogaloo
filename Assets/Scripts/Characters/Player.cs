using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character
{
    public int coinCount;
    private PlayerControls _controls;

    protected override void Awake()
    {
        base.Awake(); 

        _controls = new PlayerControls();
        _controls.Enable();

        _controls.controls.Movement.performed += MovementInput; //subscribe to movement input function when event called 
        _controls.controls.Movement.canceled += MovementInput;

        //when pressed toggle true when released toggle false
        _controls.controls.Shoot.started += _ => isShooting = !isShooting; //_ = null, no callback context needed
        _controls.controls.Shoot.canceled += _ => isShooting = !isShooting;

        _controls.controls.CircleShoot.started += _ => isShootingCircle = !isShootingCircle; //_ = null, no callback context needed
        _controls.controls.CircleShoot.canceled += _ => isShootingCircle = !isShootingCircle;

        //_controls.controls.PauseGame.started += _ => GameManager.Instance.PauseGame

        GameManager.Instance.UpdatePlayerHealth(health);
    }

    private void MovementInput(InputAction.CallbackContext ctx)
    {
        movementVector = ctx.ReadValue<Vector2>();
    }

    // Start is called before the first frame update
    void Start()
    {
        health = 5;
        coinCount = 0;
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    protected override void Move()
    {
        base.Move();
    }

    public override void TakeDamage(int damage, Vector3 force)
    {
        base.TakeDamage(damage, force);
        GameManager.Instance.UpdatePlayerHealth(health);
    }
}

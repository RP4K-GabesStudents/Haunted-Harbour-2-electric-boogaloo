using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticUtilities
{
    public static IEnumerator ColorLerp(this SpriteRenderer sr, Color to, float duration)    {        Color start = sr.color;        float curTime = 0;        while (curTime <= duration)        {            curTime += Time.deltaTime;            sr.color = Color.Lerp(start, to, curTime / duration);            yield return null;        }        yield return null;    }


}

public static class BulletTypes
{

}

public static class AttackTypes
{

    public static void Targeting(Enemy enemy)    {
        //SOHCAHTOA. figure this out later
    }


}

public static class MovementTypes
{
    private static IEnumerator Timer(Enemy thisEnemy, float duration)
    {
        thisEnemy.movementVector = new Vector2(UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1, 0);
        yield return new WaitForSeconds(duration);
        thisEnemy.currentRoutine = null;
    }
    public static void Dumb(Enemy enemy)    {
        if (enemy.currentRoutine == null)
            enemy.currentRoutine = enemy.StartCoroutine(Timer(enemy, enemy.timer));
    }

    public static void TargetGhost(Enemy enemy)    {
        Vector2 origin = enemy.transform.position;
        Vector3 directionVector = (enemy.targetObject.position - (Vector3)origin).normalized;

        RaycastHit2D AtPlayer = Physics2D.Raycast(origin, directionVector, enemy.distance, GameManager.Instance.PlayerLayer);

        if (AtPlayer) //If we can see the player //Hitting the player
        {
            Debug.DrawRay(origin, directionVector * AtPlayer.distance, Color.green); //Draw debug lines to visualize

            enemy.transform.position += (directionVector + new Vector3(0.0f, Mathf.Sin(Time.time * 2) * 3, 0.0f)) * (enemy.speed * Time.deltaTime) / 5;

            //can see, move towards player
        }

        //target ghost is sine wave
        //https://answers.unity.com/questions/781748/using-mathfsin-to-move-an-object.html
        //go through stuff, set collider to trigger, disable rigidbody simulation
    }

    public static void TargetSmart(Enemy enemy)    {
        //layermasks and raycast
        //enemy.target (later)

        //Origin, Direction, Distance, LayerMask
        Vector2 origin = enemy.transform.position;
        Vector2 directionVector = ((Vector2)enemy.targetObject.position - origin).normalized;
        Vector2 movement = Vector2.zero;

        //raycasts - temp variable, move to game manager
        RaycastHit2D left = Physics2D.Raycast(origin, Vector2.left, enemy.size, GameManager.Instance.WorldLayer);
        RaycastHit2D right = Physics2D.Raycast(origin, Vector2.right, enemy.size, GameManager.Instance.WorldLayer);

        RaycastHit2D AtPlayer = Physics2D.Raycast(origin, directionVector, enemy.distance, GameManager.Instance.PlayerLayer);

        if (AtPlayer) //If we can see the player //Hitting the player
        {
            Debug.DrawRay(origin, directionVector * AtPlayer.distance, Color.green); //Draw debug lines to visualize

            movement.x = Mathf.RoundToInt(directionVector.x); //lr mvt

            //can see, move towards player
        }

        if (left || right)
        {
            movement.y = 1; //replication of player jump

            if (left)
            {
                movement.x = -1;
            }
            else
            {
                movement.x = 1;
            }
        }
        enemy.movementVector = movement;
    }

    

    public static void Circle(Enemy enemy)    {

        //consider the elements of a circle - radius, angles, sohcahtoa

        enemy.angle += (1 * Time.deltaTime);

        //this is ok
        enemy.transform.position = new Vector3(enemy.targetObject.position.x + (enemy.distance * Mathf.Cos(enemy.angle)), enemy.targetObject.position.y + (enemy.distance * Mathf.Sin(enemy.angle)), 0);
    
    }

}

public enum EBulletType
{
    Purple,
    Green,
    Red,
    Fire
}


public enum EAttackType
{
    None,
    Constant,
    Targeting
}


public enum EMovementType
{
    None,
    Dumb,
    TargetGhost,
    TargetSmart,
    Circle
}

public delegate void AttackTypeDel(Enemy enemy);
public delegate void MovementTypeDel(Enemy enemy);

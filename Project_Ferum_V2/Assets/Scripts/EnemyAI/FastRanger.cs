using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastRanger : AbstractEnemy
{
    //Move constants
    private const int SHOOTER_MOVE = 0;
    private const int DASH_MOVE = 1;
    private const int MELEE_DEFENSE = 2;

    //Variables for movement
    private bool ccw = false;                   //Variable indicating if going in counter clockwise rotation
    private Vector2 distVector;                 //Vector from enemy to target
    private const float MIN_DISTANCE = 4.5f;    //The optimal min distance
    private const float MAX_DISTANCE = 5.5f;    //The optimal max distance for being with

    private const float MOVE_REDUCTION = 0.75f;

    //Does circular movement around the player
    public override void movement() {
        if (isAgitated()) {
            //Calculate 2 vectors
            Vector3 tgtPos = getTgtPos();
            Vector2 distVector = new Vector2(transform.position.x - tgtPos.x, transform.position.y - tgtPos.y);
            Vector2 circVector = calculateCircVector(distVector);
            float distance = distVector.magnitude;

            //If distance between enemy and player too far, reverse direction. or if equal, have no movement on that axis
            if (distance > MAX_DISTANCE)
                distVector *= -1;
            else if (distance >= MIN_DISTANCE && distance <= MAX_DISTANCE)
                distVector = Vector2.zero;

            //Calculate vector
            distVector.Normalize();
            Vector2 movement = distVector + circVector;
            movement.Normalize();

            //Update animations
            transform.Translate(movement * getMoveSpeed() * MOVE_REDUCTION);
        }
    }

    //Calculates circular vector for movement
    private Vector2 calculateCircVector(Vector2 distVector) {
        //Decide which value to negate by looking at the angle of the vector
        distVector *= -1;

        //Create vector from new information
        float circX = distVector.y;
        float circY = distVector.x;

        circY *= -1;
        Vector2 circVector = new Vector2(circX, circY);
        circVector *= (ccw) ? -1 : 1;

        circVector.Normalize();

        return circVector;
    }

    //Collision method, everytime hits a platform, reverse direction
    void OnCollisionEnter2D(Collision2D collision) {
        Collider2D collider = collision.collider;

        if (collider.tag == GeneralConstants.WALL_TAG || collider.tag == GeneralConstants.PLAYER_TAG)
            ccw = (ccw) ? false : true;
    }

    //Collision method, if stay on a wall for too long, change direction
    private const float WALL_STAY = 0.5f;
    private float stayTimer;

    void OnCollision2DStay(Collision2D collision) {
        Collider2D wall = collision.collider;

        if(GetComponent<Collider>().tag == GeneralConstants.WALL_TAG) {
            stayTimer += Time.deltaTime;

            if(stayTimer >= WALL_STAY) {
                ccw = (ccw) ? false : true;
                stayTimer = 0f;
            }
        }
    }


    //Method for attacking
    public override void attack() {
        executeMove(SHOOTER_MOVE);
    }

    //Method for reacting to the player
    public override bool reactToPlayer() {
        executeMove(DASH_MOVE);
        return true;
    }

    //Method for reacting to the enemy
    public override bool reactToAttack(Transform attk) {
        int attackNum = Random.Range(0, 3);
        bool attackUsed = false;

        if (attackNum != 0) {
            executeMove(attackNum, attk);
            attackUsed = true;
        }

        return attackUsed;
    }
}

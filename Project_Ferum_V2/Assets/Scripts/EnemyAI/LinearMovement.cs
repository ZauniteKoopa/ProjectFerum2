using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Move horizontally left and right  (VECTOR MAY CHANGE)*/
public class LinearMovement : AbstractEnemy
{ 
    //Movement variables
    private const float MAX_DISTANCE = 10f;
    private float curDist = 0f;
    private bool moveRight = true;

    //Boolean flag that keeps track of the last direction to ensure you only change once per frame
    //  Used in the case where both movement() pivot same time as collision pivot
    //  Update (movement()) is always checked before Collision2D
    private bool lastMoveRight = true;


    public override void movement() {
        /* Set lastMoveRight to current moveRight */
        lastMoveRight = moveRight;

        /* Get movement speed */
        float moveSpeed = getMoveSpeed();

        /* Move unit */
        Vector3 moveVect = Vector3.right * moveSpeed;
        transform.position += (moveRight) ? moveVect : moveVect * -1;
        curDist += moveSpeed;

        /* Change direction */
        if (curDist >= MAX_DISTANCE) {
            moveRight = !moveRight;
            curDist = 0f;
        }

    }

    public override void attack() {

    }

    /* When colliding wall, reverse directions immediately */
    void OnCollisionEnter2D(Collision2D collision) {
        string colliderTag = collision.collider.tag;

        /* Valid collision = Player && wall */
        bool hit = colliderTag == GeneralConstants.WALL_TAG || colliderTag == GeneralConstants.PLAYER_TAG;

        /* Checks if valid collision and that moveRight was only updated once */
        if (hit && lastMoveRight == moveRight) {
            moveRight = !moveRight;
            curDist = 0f;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /* Directional variables that describe in what of the 8 directions player is facing */
    private int hDir = 1;
    private int vDir = 0;

    /* Reference variable to entity associated with this controller (TO BE DELETED)*/
    private EntityStatus status;

    // Start is called before the first frame update
    void Awake()
    {
        status = GetComponent<EntityStatus>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /* If cannot move, you don't move or attack */
        if(status.canMove()) {
            movement();
            attack();
        }
    }

    /* Movement helper method for FixedUpdate(): Allows player to move
        Post: hDir and vDir should not equal 0 at the same time */
    void movement() {
        /* Store previous dir values */
        int prevHDir = hDir;
        int prevVDir = vDir;

        /* Vertical movement */
        if (Input.GetKey(ControlMap.MOVE_UP)) {
            vDir = 1;
        } else if (Input.GetKey(ControlMap.MOVE_DOWN)) {
            vDir = -1;
        } else {
            vDir = 0;
        }

        /* Horizontal movement */
        if (Input.GetKey(ControlMap.MOVE_RIGHT)) {
            hDir = 1;
        } else if (Input.GetKey(ControlMap.MOVE_LEFT)) {
            hDir = -1;
        } else {
            hDir = 0;
        }

        /* Actually move the transform */
        Vector3 moveDir = new Vector3(hDir, vDir, 0);
        moveDir.Normalize();
        float curSpeed = status.getMovementSpeed();
        transform.position += moveDir * curSpeed;

        /* If player didn't move, go back to previous dir values */
        if(hDir == 0 && hDir == vDir) {
            hDir = prevHDir;
            vDir = prevVDir;
        }

        Debug.Assert(hDir != 0 || vDir != 0);
    }

    /* Helper method for attacking: allows player to attack */
    void attack() {
        if (Input.GetKeyDown(ControlMap.ABILITY_1) && status.canUseMove(0)) {
            StartCoroutine(status.executeMovePlayer(0, hDir, vDir));
        } else if (Input.GetKeyDown(ControlMap.ABILITY_2) && status.canUseMove(1)) {
            StartCoroutine(status.executeMovePlayer(1, hDir, vDir));
        } else if (Input.GetKeyDown(ControlMap.ABILITY_3) && status.canUseMove(2)) {
            StartCoroutine(status.executeMovePlayer(2, hDir, vDir));
        }
    }

}

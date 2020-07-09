using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /* Directional variables that describe in what of the 8 directions player is facing */
    private int hDir = 1;
    private int vDir = 0;

    /* Move array */
    private IMove[] moves = new IMove[3];

    /* Player speed */
    [SerializeField]
    private float speed = 0.125f;
    private const float ATTACK_MOVE_REDUCTION = 0.45f;

    /* Flags for attacking and moving relationships */
    private bool movingDisabled = false;
    private bool attacking = false;

    /* Reference variable to entity associated with this controller (TO BE DELETED)*/
    private EntityStatus status;

    // Start is called before the first frame update
    void Awake()
    {
        status = GetComponent<EntityStatus>();
        moves[0] = new BasicMeleeAttack(30, 175f, status);              // POUND
        moves[1] = new BulletSeed(status, ControlMap.ABILITY_2);        // BULLET SEED

        //Hyper Voice
        Transform hyperVoice = Resources.Load<Transform>("MoveHitboxes/HyperVoiceHitbox");
        moves[2] = new CircleAoE(10f, true, 100, 300f, false, hyperVoice, status);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /* If cannot move, you don't move or attack */
        if(!movingDisabled) {
            movement();

            /* If already attacking, don't start another attack */
            if (!attacking)
                attack();
        }

        /* Regenerating move CD and resources */
        for(int i = 0; i < moves.Length; i++) {
            if (moves[i] != null) {
                moves[i].regen();
            }
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
        float curSpeed = (attacking) ? speed * ATTACK_MOVE_REDUCTION : speed;
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
        if (Input.GetKeyDown(ControlMap.ABILITY_1) && isMoveValid(0)) {
            StartCoroutine(executeMove(0));
        } else if (Input.GetKeyDown(ControlMap.ABILITY_2) && isMoveValid(1)) {
            StartCoroutine(executeMove(1));
        } else if (Input.GetKeyDown(ControlMap.ABILITY_3) && isMoveValid(2)) {
            StartCoroutine(executeMove(2));
        }
    }

    /* IEnumerator used to execute moves
        Pre: moveID >= 0 && moveID < 3 and move is valid (not null and canRun())*/
    IEnumerator executeMove(int moveID) {
        if(moveID < 0 || moveID >= 3)
            throw new System.Exception("Error: Invalid move ID");

        /* Set attacking to true */
        attacking = true;

        /* Get necessary data from move */
        IMove curMove = moves[moveID];
        movingDisabled = curMove.isMovementDisabled();

        yield return curMove.executeMovePlayer(hDir, vDir);

        /* Set flag variables back to false */
        movingDisabled = false;
        attacking = false; 
    }

    /* Helper method to check if move is valid
        A move is valid IFF the move slot is not null AND it can be run at this moment*/
    bool isMoveValid(int moveID) {
        if(moveID < 0 || moveID >= 3)
            return false;
        
        return moves[moveID] != null && moves[moveID].canRun();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /* Directional variables that describe in what of the 8 directions player is facing */
    private int hDir = 1;
    private int vDir = 0;

    /* Reference variable to entity associated with this controller (TO BE DELETED)*/
    private const int MAX_NUM_FIGHTERS = 3;
    [SerializeField]
    private EntityStatus[] fighters = new EntityStatus[MAX_NUM_FIGHTERS];
    private int mainIndex = 0;
    private int numLiving = 0;

    // Start is called before the first frame update
    void Awake()
    {
        /* Count how many ,living fighters at the start of the game */
        for(int i = 0; i < fighters.Length; i++) {
            if (i >= MAX_NUM_FIGHTERS)
                throw new System.Exception("ERROR: Too many slots in fighters on this player controller");
            
            if (fighters[i] != null)
                numLiving++;
        }

        if(numLiving == 0)
            throw new System.Exception("ERROR: Player controller has no fighters initially");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /* If cannot move, you don't move or attack */
        if(fighters[mainIndex].canMove()) {
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

        /* Make speed calculations */
        Vector3 moveDir = new Vector3(hDir, vDir, 0);
        moveDir.Normalize();
        float curSpeed = fighters[mainIndex].getMovementSpeed();

        /* Move the fighter first and then move the overall controller transform to fighter's position */
        fighters[mainIndex].transform.position += moveDir * curSpeed;
        transform.position += fighters[mainIndex].transform.localPosition;

        /* Reset fighter's local position to avoid continous movement */
        fighters[mainIndex].transform.localPosition = Vector3.zero;


        /* If player didn't move, go back to previous dir values */
        if(hDir == 0 && hDir == vDir) {
            hDir = prevHDir;
            vDir = prevVDir;
        }

        Debug.Assert(hDir != 0 || vDir != 0);
    }

    /* Helper method for attacking: allows player to attack */
    void attack() {
        /* Doing your main selected fighter's 3 abilities */
        if (Input.GetKeyDown(ControlMap.ABILITY_1) && fighters[mainIndex].canUseMove(0)) {
            StartCoroutine(fighters[mainIndex].executeMovePlayer(0, hDir, vDir));
        } else if (Input.GetKeyDown(ControlMap.ABILITY_2) && fighters[mainIndex].canUseMove(1)) {
            StartCoroutine(fighters[mainIndex].executeMovePlayer(1, hDir, vDir));
        } else if (Input.GetKeyDown(ControlMap.ABILITY_3) && fighters[mainIndex].canUseMove(2)) {
            StartCoroutine(fighters[mainIndex].executeMovePlayer(2, hDir, vDir));
        }

        /* Swapping characters */
        if(numLiving > 1) {
            if(Input.GetKeyDown(ControlMap.SWAP_LEFT)) {
                swapFighter(false);
            }else if(Input.GetKeyDown(ControlMap.SWAP_RIGHT)) {
                swapFighter(true);
            }
        }
    }

    /* Swap characters method: swapRight == true is adding, else subtracting
        Pre: numiving > 1 
        Post: previous fighter will deactivate and new fighter will activate */
    void swapFighter(bool swapRight) {
        Debug.Assert(numLiving > 1);

        /* Get swap number depending on boolean flag */
        int swapNumber = (swapRight) ? 1 : -1;

        /* Find the new index representing the fighter you want to swap to */
        int newIndex = mainIndex;

        do
        {
            /* Move to the next fighter */
            newIndex += swapNumber + MAX_NUM_FIGHTERS;
            newIndex %= MAX_NUM_FIGHTERS;

        } while (fighters[newIndex] == null);
        
        /* Assert that you got an entirely different fighter*/
        Debug.Assert(newIndex != mainIndex);

        /* Disable old fighter and enable new fighter */
        fighters[mainIndex].gameObject.SetActive(false);
        fighters[newIndex].gameObject.SetActive(true);

        /* Update main Index */
        mainIndex = newIndex;
    }

}

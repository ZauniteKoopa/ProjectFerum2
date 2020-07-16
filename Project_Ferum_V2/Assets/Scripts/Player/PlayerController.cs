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

    /* Assist move constants */
    private const float ASSIST_MOVE_SLOW = 0.1f;
    private const float ASSIST_MOVE_LINGER_TIME = 5f;
    private bool assistMove = false;

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

            if (!assistMove)
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
        if (!assistMove) {
            transform.position += fighters[mainIndex].transform.localPosition;

            /* Reset fighter's local position to avoid continous movement */
            fighters[mainIndex].transform.localPosition = Vector3.zero;
        }

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

        /* Swapping characters and assist moves */
        if(numLiving > 1) {

            /* Swapping */
            if(Input.GetKeyDown(ControlMap.SWAP_LEFT)) {
                swapFighter(false);
            }else if(Input.GetKeyDown(ControlMap.SWAP_RIGHT)) {
                swapFighter(true);
            }

            /* Assist move */
            if(Input.GetKeyDown(ControlMap.ASSIST_MOVE_LEFT)) {
                StartCoroutine(executeAssistMove(false));
            }else if(Input.GetKeyDown(ControlMap.ASSIST_MOVE_RIGHT)) {
                StartCoroutine(executeAssistMove(true));
            }
        }
    }

    /* Swap characters method: swapRight == true is adding, else subtracting
        Pre: numiving > 1 
        Post: previous fighter will deactivate and new fighter will activate */
    void swapFighter(bool swapRight) {
        Debug.Assert(numLiving > 1);
        int newIndex = getNextFighter(swapRight);

        /* Disable old fighter and enable new fighter */
        fighters[mainIndex].gameObject.SetActive(false);
        fighters[newIndex].gameObject.SetActive(true);

        /* Update main Index */
        mainIndex = newIndex;
    }

    /* Execute an assist move of a partner 
        Pre: numLiving > 1
        Post: an assist move will be executed */
    IEnumerator executeAssistMove(bool swapRight) {
        Debug.Assert(numLiving > 1);

        /* Get fighter to do assist move */
        assistMove = true;
        int assistIndex = getNextFighter(swapRight);
        EntityStatus assistFighter = fighters[assistIndex];

        /* Change main index and assist index, detach assistFighter, and get rid of loop */
        int prevMainIndex = mainIndex;
        mainIndex = assistIndex;

        assistFighter.gameObject.SetActive(true);
        assistFighter.transform.parent = null;
        //assistFighter.transform.position = transform.position;
        numLiving--;

        bool linger = false;
        int abilityUsed = -1;       //If this number is still -1, no ability used

        Time.timeScale = ASSIST_MOVE_SLOW;


        /* Wait until player releases c/v button or takes damage / uses an ability*/
        while(holdingAssistKey() && abilityUsed == -1) {
            /* Doing your main selected fighter's 3 abilities */
            if (Input.GetKeyDown(ControlMap.ABILITY_1) && fighters[mainIndex].canUseMove(0)) {
                abilityUsed = 0;
            } else if (Input.GetKeyDown(ControlMap.ABILITY_2) && fighters[mainIndex].canUseMove(1)) {
                abilityUsed = 1;
            } else if (Input.GetKeyDown(ControlMap.ABILITY_3) && fighters[mainIndex].canUseMove(2)) {
                abilityUsed = 2;
            }

            /* If ability was actually used, set linger to true */
            if (abilityUsed != -1)
                linger = true;

            yield return new WaitForSecondsRealtime(0.01f);
        }

        /* Shift control back to main fighter and revert timescale to normal. Disable assist */
        mainIndex = prevMainIndex;
        Time.timeScale = 1.0f;
        assistMove = false;

        /* In the case where you do linger */
        if (linger) {
            /* Disable fighter in fighters array */
            fighters[assistIndex] = null;

            /* execute assist move if ability done */
            if (abilityUsed != -1)
                yield return assistFighter.executeAssistMove(abilityUsed, hDir, vDir);

            /* Have assist linger before going back */
            yield return new WaitForSeconds(ASSIST_MOVE_LINGER_TIME);
            fighters[assistIndex] = assistFighter;
        }

        /* Disable assist fighter */
        assistFighter.transform.parent = transform;
        assistFighter.transform.localPosition = Vector3.zero;
        assistFighter.gameObject.SetActive(false);
        numLiving++;
    }

    /* Private method for finding the next fighter to swap to or do an assist move
        Pre: numLiving > 1
        Post: number returned represent the index of the next fighter*/
    private int getNextFighter(bool swapRight) {
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

        /* Check that you actually got a different fighter */
        Debug.Assert(newIndex != mainIndex);
        return newIndex;
    }

    // --------------------
    // Assist move helper methods
    // --------------------

    private bool holdingAssistKey() {
        return Input.GetKey(ControlMap.ASSIST_MOVE_LEFT) || Input.GetKey(ControlMap.ASSIST_MOVE_RIGHT);
    }

}

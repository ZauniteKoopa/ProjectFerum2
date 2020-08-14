using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    /* Directional variables that describe in what of the 8 directions player is facing */
    private int hDir = 1;
    private int vDir = 0;

    /* Variable to help with swapping teammates */
    private int initialNumFighters;

    /* Reference variable to entity associated with this controller (TO BE DELETED)*/
    private const int MAX_NUM_FIGHTERS = 3;
    [SerializeField]
    private EntityStatus[] fighters = new EntityStatus[MAX_NUM_FIGHTERS];
    private int mainIndex = 0;
    private int numLiving = 0;
    private const int SEC_MOVE_INDEX = 2;

    /* Move Cancellation variables */
    private bool primaryToSec = false;
    private bool secToPrimary = false;

    /* Assist move constants */
    private const float ASSIST_MOVE_SLOW = 0.15f;
    private const float ASSIST_MOVE_LINGER_TIME = 1f;
    private const float MAX_ASSIST_SEQ_DURATION = 1.5f;
    private const float ENEMY_LINGER_REDUCTION = 0.3f;
    private bool assistMoveSeq = false;                 //Flag for moving during assistMoveSequence
    private int assistIndex = -1;                       //Index of assist fighter before doing assistMoves
    private EntityStatus curAssist = null;              //The current assist fighter
    private bool assistDeath;                           //Flag to check if the assist fighter dies
    [SerializeField]
    private ChannelUI assistTimerUI = null;

    /* Player mouse scrolling */
    private const float SCROLL_DELAY_DURATION = 0.085f;
    private bool scrolled = false;  //Allow for better scroll control when changing abilities

    /* Flags */
    private bool dying = false;     //If the main party member is in the middle of dying

    /* UI Variables */
    [SerializeField]
    private UIResources[] UIStats = new UIResources[3];
    [SerializeField]
    private AbilitySelector selector = null;

    // Start is called before the first frame update
    void Start()
    {
        bool hitNull = false;

        /* Count how many ,living fighters at the start of the game: THERE SHOULD BE NO NULL IN-BETWEEN FIGHTERS INITIALLY */
        for(int i = 0; i < fighters.Length; i++) {
            if (i >= MAX_NUM_FIGHTERS)
                throw new System.Exception("ERROR: Too many slots in fighters on this player controller");
            
            if (fighters[i] != null) {
                //Checks for errors
                if (hitNull)
                    throw new System.Exception("ERROR: null in between fighters in fighters array");
                
                numLiving++;
                fighters[i].initializeEntity();
                UIStats[i].enableUI();
            } else {
                hitNull = true;
            }
        }

        if(numLiving == 0)
            throw new System.Exception("ERROR: Player controller has no fighters initially");
        
        initialNumFighters = numLiving;
        rotateFighterUI();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /* If cannot move, you don't move or attack */
        if(!dying && fighters[mainIndex].canMove()) {
            movement();

            if (!assistMoveSeq) {
                
                attack();

                /* teamwork moves */
                if(numLiving > 1) {
                    teamwork();
                }
            }
                
        }

        /* You can still select ability if cannot move */
        if (!assistMoveSeq)
            selectAbility();
        
        /* Allows resetting prototype */
        if (Input.GetKey(KeyCode.Escape))
            SceneManager.LoadScene(0);

        updateResources();
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

        /* Move the fighter first and then move the overall controller transform to fighter's position (if controlling mian fighter)*/
        fighters[mainIndex].transform.position += moveDir * curSpeed;

        /* If player didn't move, go back to previous dir values */
        if(hDir == 0 && hDir == vDir) {
            hDir = prevHDir;
            vDir = prevVDir;
        }

        Debug.Assert(hDir != 0 || vDir != 0);
    }

    /* Helper method for attacking: allows player to attack */
    void attack() {

        /* Execute ability */
        int selectedAbility = selector.getMoveIndex();

        if ((Input.GetMouseButtonDown(0) && fighters[mainIndex].canUseMove(selectedAbility)) || secToPrimary) {
            secToPrimary = false;
            StartCoroutine(fighters[mainIndex].executeMovePlayer(selectedAbility));
        }

        if ((Input.GetMouseButtonDown(1) && fighters[mainIndex].canUseMove(SEC_MOVE_INDEX)) || primaryToSec) {
            primaryToSec = false;
            StartCoroutine(fighters[mainIndex].executeMovePlayer(SEC_MOVE_INDEX));
        }
    }

    /* Helper method for teamwork methods (swapping mainFighters / assist moves */
    void teamwork() {
        /* Swapping */
        if(Input.GetKeyDown(ControlMap.SWAP_LEFT)) {
            swapFighter(false);
        }else if(Input.GetKeyDown(ControlMap.SWAP_RIGHT)) {
            swapFighter(true);
        }

        /* Assist move */
        if(curAssist == null) {
            if(Input.GetKeyDown(ControlMap.ASSIST_MOVE_LEFT)) {
                StartCoroutine(executeAssistMove(false));
            }else if(Input.GetKeyDown(ControlMap.ASSIST_MOVE_RIGHT)) {
                StartCoroutine(executeAssistMove(true));
            }
        }
    }

    void selectAbility() {
        /* Changing abilities via buttons */
        if (Input.GetKey(ControlMap.SELECT_ABILITY_1) && fighters[mainIndex].canSelectMove(0)) {
            selector.changeSelectAbility(0, true);
        } else if (Input.GetKey(ControlMap.SELECT_ABILITY_2) && fighters[mainIndex].canSelectMove(1)) {
            selector.changeSelectAbility(1, true);
        } 

        if (Input.GetKeyDown(ControlMap.CHANGE_SELECT) && !scrolled) {
            StartCoroutine(scrollDelay(true));
        }

        /* Changing abilities via mouse scroll */
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0 && !scrolled) {
            StartCoroutine(scrollDelay(true));
        } else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0 && !scrolled) {
            StartCoroutine(scrollDelay(false));
        }
    }

    void updateResources() {
        /* Update ability UI regarding player */
        for(int i = 0; i < UIStats.Length; i++) {
            //Get a mapping from fighter to UI resources
            int assignedUI = (i - mainIndex + initialNumFighters) % initialNumFighters;

            if (fighters[i] != null)
                fighters[i].playerRegen(UIStats[assignedUI].abilities);
        }

        /* Update ability UI for assistFighter */
        if (curAssist != null) {
            int assignedUI = (assistIndex - mainIndex + 3) % 3;
            curAssist.playerRegen(UIStats[assignedUI].abilities);
        }

        /* Camera always focused on for mainFighter */
        if (!assistMoveSeq && !dying) {
            transform.position += fighters[mainIndex].transform.localPosition;
            fighters[mainIndex].transform.localPosition = Vector3.zero;
        }
    }


    //Private helper method with scrolling through secondary moves without doubling on scrolling
    //  Pre: an item was scrolled through
    private IEnumerator scrollDelay(bool isRight) {
        scrolled = true;

        if (isRight) {
            selector.shiftRight();
        } else {
            selector.shiftLeft();
        }

        yield return new WaitForSecondsRealtime(SCROLL_DELAY_DURATION);
        scrolled = false;
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
        rotateFighterUI();
        StartCoroutine(fighters[mainIndex].runSwapAnimation(selector));
    }

    /* Move cancelling 
        Pre: newMove must be either 0 (left click) or 1 (right click)
        Post: Returns if the flag was actually set to true */
    public bool cancelMove(int newInput) {
        if (newInput == 0) {                                    //Case from switching from secondary to main
            int selectedAbility = selector.getMoveIndex();
            secToPrimary = fighters[mainIndex].canCancelMove(selectedAbility);
            return secToPrimary;
        } else if (newInput == 1) {                             //Case from switching from main to secondary
            primaryToSec = fighters[mainIndex].canCancelMove(SEC_MOVE_INDEX);
            return primaryToSec;
        } else {
            return false;
        }
    }

    /* Execute an assist move of a partner 
        Pre: numLiving > 1
        Post: an assist move will be executed */
    IEnumerator executeAssistMove(bool swapRight) {
        Debug.Assert(numLiving > 1);

        /* Set up boolean flags for this controller */
        assistMoveSeq = true;  assistDeath = false;
        
        /* Get fighter to do assist move */
        assistIndex = getNextFighter(swapRight);
        curAssist = fighters[assistIndex];
        curAssist.resetAssistStatus();  fighters[mainIndex].resetAssistStatus();

        /* Change main index and assist index, detach curAssist, and get rid of loop */
        int prevMainIndex = mainIndex;
        mainIndex = assistIndex;

        curAssist.gameObject.SetActive(true);
        curAssist.transform.parent = null;
        numLiving--;
        rotateFighterUI();

        assistTimerUI.setActive(true);
        assistTimerUI.setChannel(1, 1);

        /* Set up loop */
        int abilityUsed = -1;       //If this number is still -1, no ability used
        int prevLiving = numLiving;
        float assistSeqTimer = 0f;
        Time.timeScale = ASSIST_MOVE_SLOW;

        /* Wait until player releases c/v button or takes damage or uses an ability or runs out of time */
        while(holdingAssistKey() && abilityUsed == -1 && enemyNotHitAssist(prevLiving, prevMainIndex) && assistSeqTimer < MAX_ASSIST_SEQ_DURATION) {
            /* Select Abilities doing quick select */
            selectAbility();

            /* Execute primary ability */
            int selectedAbility = selector.getMoveIndex();
            if (Input.GetMouseButtonDown(0) && fighters[mainIndex].canUseMove(selectedAbility)) {
                abilityUsed = selectedAbility;
            }

            /* execute secondary ability */
            if (Input.GetMouseButtonDown(1) && fighters[mainIndex].canUseMove(SEC_MOVE_INDEX)) {
                abilityUsed = SEC_MOVE_INDEX;
            }
            
            yield return new WaitForSecondsRealtime(0.01f);

            /* Update timer and set up UI */
            assistSeqTimer += 0.01f;
            assistTimerUI.setChannel(MAX_ASSIST_SEQ_DURATION - assistSeqTimer, MAX_ASSIST_SEQ_DURATION);
        }

        //If an ability is used or curAssist was damaged, have the assist linger
        bool linger = abilityUsed != -1 || !enemyNotHitAssist(prevLiving, prevMainIndex) || assistSeqTimer >= MAX_ASSIST_SEQ_DURATION;

        /* Shift control back to main fighter and revert timescale to normal. Disable assist */
        mainIndex = prevMainIndex;
        Time.timeScale = 1.0f; assistMoveSeq = false;
        rotateFighterUI();

        /* In the case where you do linger */
        if (linger && !assistDeath) {
            /* Disable fighter in fighters array */
            fighters[assistIndex] = null;
            assistTimerUI.setChannel(1, 1);

            /* execute assist move if ability done */
            if (abilityUsed != -1)
                yield return curAssist.executeAssistMove(abilityUsed);

            /* Have assist linger before going back */
            yield return lingering(); 
        }

        /* Check for assistDeath to decide what to do with assistFighter after sequence */
        assistTimerUI.setActive(false);
        if (assistDeath)
            destroyAssistFighter(assistIndex);
        else
            disableAssistFighter(assistIndex);
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


    /* Helper method meant to "rotate" fighter UI so that they align with appropriate fighter */
    private void rotateFighterUI() {
        selector.setToFighter(fighters[mainIndex].getNumMoves());

        for(int i = 0; i < initialNumFighters; i++) {
            //Get a mapping from fighter to UI resources
            int assignedUI = (i - mainIndex + initialNumFighters) % initialNumFighters;

            if (fighters[i] != null) {
                fighters[i].setUpUI(UIStats[assignedUI]);

                //Change character icon
                if (assignedUI != 0) {
                    Color fighterColor = fighters[i].GetComponent<SpriteRenderer>().color;
                    UIStats[assignedUI].changeIcon(fighterColor);
                }

            } else {
                //Check if this is an assist fighter case
                if (curAssist != null && assistIndex == i) {
                    curAssist.setUpUI(UIStats[assignedUI]);

                    //Change character icon
                    if (assignedUI != 0) {
                        Color fighterColor = curAssist.GetComponent<SpriteRenderer>().color;
                        UIStats[assignedUI].changeIcon(fighterColor);
                    }

                } else {
                    UIStats[assignedUI].setDead();
                }
                
            }
        }
    }


    // --------------------
    // Assist move helper methods
    // --------------------

    //IEnumerator to execute lingering
    private IEnumerator lingering() {
        float lingerTime = 0f;
        curAssist.resetAssistStatus();

        while(lingerTime <= ASSIST_MOVE_LINGER_TIME && !assistDeath) {
            yield return new WaitForSeconds(0.01f);
            lingerTime += 0.01f;

            //If enemy hits assist, extend linger by ENEMY_LINGER_REDUCTION seconds (2 sec is max)
            if (curAssist.isAssistCancelled() || curAssist.armorBroke()) {
                float newLinger = lingerTime - ENEMY_LINGER_REDUCTION;
                lingerTime = (newLinger >= 0f) ? newLinger : 0f;
                curAssist.resetAssistStatus(); 
            }

            assistTimerUI.setChannel(ASSIST_MOVE_LINGER_TIME - lingerTime, ASSIST_MOVE_LINGER_TIME);
        }
    }

    //Gets rid of assistFighter's corpse if an assistFighter dies during assistMove
    //  Pre: assistFighter is considered dead
    private void destroyAssistFighter(int assistIndex) {
        Debug.Assert(assistDeath);
        Debug.Assert(assistIndex >= 0 && assistIndex < 3);

        //Set UI resources
        int assignedUI = (assistIndex - mainIndex + initialNumFighters) % initialNumFighters;
        UIStats[assignedUI].setDead();

        //Clear assistFighter out
        curAssist.transform.parent = transform;
        curAssist.transform.localPosition = Vector3.zero;
        curAssist.gameObject.SetActive(false);
        curAssist = null;

        if(numLiving <= 0) {
            dying = true;
            Debug.Log("gameOver");
        }
    }

    //Method meant to disable assist fighter after an assist move
    //  Pre: assistFighter must still be considered living
    private void disableAssistFighter(int assistIndex) {
        Debug.Assert(!assistDeath);
        Debug.Assert(assistIndex >= 0 && assistIndex < 3);

        curAssist.transform.parent = transform;
        curAssist.transform.localPosition = Vector3.zero;
        curAssist.gameObject.SetActive(false);
        fighters[assistIndex] = curAssist;
        curAssist = null;
        
        numLiving++;
    }

    //Method to check if player is holding assist key
    private bool holdingAssistKey() {
        return Input.GetKey(ControlMap.ASSIST_MOVE_LEFT) || Input.GetKey(ControlMap.ASSIST_MOVE_RIGHT);
    }

    //Method to check if an enemy has interrupted an assist move 
    private bool enemyNotHitAssist(int prevLiving, int prevMain) {
        //Checks for the death case 
        if(numLiving < prevLiving)
            return false;

        return !curAssist.isAssistCancelled() && !fighters[prevMain].isAssistCancelled();
    }



    //Death Method: Played when one of the 3 members dies
    //  Pre: either the mainFighter or the assistFighter dies with numLiving >= 0
    //  Post: If no living fighters available, game over
    //        If fighter is available, swap to available fighter
    //        If assistFighter is available, wait until assistFighter finished with assistMove and then swap
    public IEnumerator OnDeath(EntityStatus corpse) {

        if(corpse == fighters[mainIndex] && numLiving > 1) {        //Case where the corpse is the main fighter NOT ASSIST FIGHTER
            numLiving--;

            dying = true;
            fighters[mainIndex] = null;

            //Consider cases concerning numLiving
            if (numLiving > 0) {
                //Members are still inactive in party: swap immediately
                yield return OnDeathSwap();
                dying = false;
            } else {
                //In this case, check for the assist fighter status
                //Busy wait on curAssist (bad solution)
                while(curAssist != null)
                    yield return 0;
                    
                //Check if the previous assistFighter is still alive
                if(numLiving > 0) {
                    yield return OnDeathSwap();
                    dying = false;
                }else{
                    Debug.Log("gameOver!");
                }
            }

        }else if(corpse == curAssist) {                             //Case where the corpse is the assist fighter
            assistDeath = true;
        }else if (corpse == fighters[mainIndex]) {                  //Case during assist move sequence where main fighter dies
            numLiving--;

            dying = true;
            fighters[mainIndex].gameObject.SetActive(false);
            fighters[mainIndex] = null;

            while (curAssist != null) {
                yield return new WaitForFixedUpdate();
            }

            if (numLiving > 0) {
                yield return OnDeathSwap();
                dying = false;
            }else {
                Debug.Log("gameOver!");
            }
        }
    }

    //Private IEnumerator that does the OnDeath swapping
    //  Pre: numLiving >= 1
    //  Post: Player swaps to next living fighter

    private const float DEATH_SILENCE = 1f;

    private IEnumerator OnDeathSwap() {
        Debug.Assert(numLiving > 0);

        yield return new WaitForSeconds(DEATH_SILENCE);

        //Get new fighter active
        int newIndex = getNextFighter(true);
        fighters[newIndex].gameObject.SetActive(true);
        mainIndex = newIndex;
        rotateFighterUI();

        yield return fighters[mainIndex].runSwapAnimation(selector);
    }

}

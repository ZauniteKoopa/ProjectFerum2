using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractEnemy : MonoBehaviour
{
    //Reference to enemy's own status
    private EntityStatus status = null;
    
    //Variable that looks at enemy's target
    [SerializeField]
    private Transform tgt = null;

    //Methods to determine behavior
    [SerializeField]
    private float attackTimeDelay = 0;
    private float attackTimer;
    private bool canAttack;

    //Flag to indicate death
    private bool dead = false;

    // Start is called before the first frame update
    void Awake()
    {
        status = GetComponent<EntityStatus>();
        status.initializeEntity();
        attackTimer = 0f;
        canAttack = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        status.regenBars();

        if (status.canMove() && !dead) {
            movement();

            /* Attack timer */
            if (tgt != null) {
                if (canAttack)
                {
                    attack();
                    canAttack = false;
                }
                else
                {
                    /* Allow timer to run */
                    attackTimer += Time.deltaTime;

                    if (attackTimer >= attackTimeDelay)
                    {
                        canAttack = true;
                        attackTimer = 0f;
                    }
                }
            }
        }
    }

    //Method to activate enemy
    public void setTgt(Transform tgt) {
        this.tgt = tgt;
        attackTimer = Random.Range(0, attackTimeDelay);
    }

    //Method meant to react to player's presence or player attack presence when nearby
    public void react(Collider2D tgt) {
        if (status.canMove() && !dead) {
            bool moveUsed = false;

            if (tgt.tag == GeneralConstants.PLAYER_ATTK_TAG) {
                moveUsed = reactToAttack(tgt.transform);
            } else if (tgt.tag == GeneralConstants.PLAYER_TAG){
                moveUsed = reactToPlayer();
            }

            //If move used, reduce attack timer by half
            if (moveUsed) {
                attackTimer *= 0.5f;
            }
        }
    }

    //Method to kill enemy AI
    public void kill() {
        dead = true;
    }

    //Method to get this entity's movement speed
    protected float getMoveSpeed() {
        return status.getMovementSpeed();
    }

    // Method used to execute a move 
    protected void executeMove(int moveID) {
        StartCoroutine(status.executeMoveEnemy(moveID, tgt));
    }

    //Overriden method of execute move that includes a seperate tgt from this tgt (usually attacks)
    protected void executeMove(int moveID, Transform other) {
        StartCoroutine(status.executeMoveEnemy(moveID, tgt));
    }

    //Method to check if this enemy is agitated: has a tgt set
    protected bool isAgitated() {
        return tgt != null;
    }

    //Method to get tgt's position
    protected Vector3 getTgtPos() {
        return tgt.position;
    }


    // ------------------------------------------
    // Abstract methods that must be implemented in all abstractEnemy scripts
    // ------------------------------------------

    //Method that executes enemy's movement patterns
    public abstract void movement();

    //Method that executes enemy's attack patterns
    public abstract void attack();

    //Method to react to player when player aproaches closely: returns whether or not a move was used
    public abstract bool reactToPlayer();

    //Method to react to player attacks when they're nearby: returns whether or not a move was used
    public abstract bool reactToAttack(Transform attk);
}

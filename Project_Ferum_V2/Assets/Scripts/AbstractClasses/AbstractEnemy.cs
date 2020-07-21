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

        if (status.canMove()) {
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

    //Method to get this entity's movement speed
    protected float getMoveSpeed() {
        return status.getMovementSpeed();
    }

    // Method used to execute a move 
    protected void executeMove(int moveID) {
        StartCoroutine(status.executeMoveEnemy(moveID, tgt));
    }


    // ------------------------------------------
    // Abstract methods that must be implemented in all abstractEnemy scripts
    // ------------------------------------------

    //Method that executes enemy's movement patterns
    public abstract void movement();

    //Method that executes enemy's attack patterns
    public abstract void attack();
}

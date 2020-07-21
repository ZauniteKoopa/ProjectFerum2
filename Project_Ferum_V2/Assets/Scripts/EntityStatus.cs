﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityStatus : MonoBehaviour
{
    /* Level */
    [SerializeField]
    private int lvl = 0;
    [Space(20)]

    /* Stats */
    [SerializeField]
    private int attk = 0;
    [SerializeField]
    private int def = 0;
    [SerializeField]
    private int sAttk = 0;
    [SerializeField]
    private int sDef = 0;
    [SerializeField]
    private int speed = 0;
    [Space(20)]

    /* Health */
    [SerializeField]
    private float maxHealth = 0f;
    private float curHealth = 0f;
    [Space(20)]

    /* Armor */
    private float maxArmor = 0f;
    private float curArmor = 0f;

    /* Moves of this entity */
    [SerializeField]
    private string[] moveNames = null;
    private IMove[] moves = new IMove[3];
    [Space(20)]

    /* Stat effects - to be added */


    /* Health regen variables */
    private float hTimer = 0f;
    private const float HP_REGEN_PERCENT = 0.02f;
    private const float HP_REGEN_RATE = 3f;

    /* Armor regen variables */
    private float aTimer = 0f;
    private bool underPressure;
    private const float PRESSURE_DURATION = 5f;

    /* Flags */
    private bool invincibility = false;     //Entity ignores attacks
    private bool movingDisabled = false;    //Entity cannot move
    private bool attacking = false;         //Entity is in the middle of an attack
    private bool shieldStunned = false;     //Entity is shield stunned
    private bool assistCancelled = false;   //Entity took damage, cancelling an assistMove sequence
    
    /* UI Elements */
    [Header("User Interface:")]
    [SerializeField]
    private Image healthBar = null;
    [SerializeField]
    private Image armorBar = null;
    [Space(20)]

    /* gameObject Controller to send message to upon death IF NECESSARY */
    [SerializeField]
    private GameObject controller = null;

    //  ---------------------
    //  Accessor methods 
    //  ---------------------

    /* Accessor method to level */
    public int getLevel() {
        return lvl;
    }

    /* Accessor methods to stats */
    public int getStat(int statID) {
        switch(statID)
        {
            case (int)GeneralConstants.statIDs.ATTACK:
                return attk;
            case (int)GeneralConstants.statIDs.DEFENSE:
                return def;
            case (int)GeneralConstants.statIDs.SP_ATTACK:
                return sAttk;
            case (int)GeneralConstants.statIDs.SP_DEFENSE:
                return sDef;
            case (int)GeneralConstants.statIDs.SPEED:
                return speed;
            default:
                throw new System.Exception("Error: Invalid stat ID");
        }
    }

    /* Accessor method to health */
    public float getHealth() {
        return curHealth;
    }

    /* Method that checks if the player can move */
    public bool canMove() {
        return !movingDisabled;
    }

    /* Method that checks if entity is shield stunned */ 
    public bool armorBroke() {
        return shieldStunned;
    }

    //Consts for movement speed calculations
    private const float MIN_BASE_MOVE = 0.07f;          //Minimum movement speed a pokemon can go
    private const float MAX_BASE_MOVE = 0.2f;           //Maximum movement speed a pokemon can go
    private const float BASE_SPEED_CAP = 150f;          //Capped speed
    private const float ATTACK_MOVE_REDUCTION = 0.35f;  //Movement speed reduction if entity is attacking

    /* Method to calculate movement speed of this entity */
    public float getMovementSpeed() {
        float curSpeed = (float)speed;
        float movement;

        if (curSpeed >= BASE_SPEED_CAP)
            movement = MAX_BASE_MOVE;
        else 
            movement = MIN_BASE_MOVE + (MAX_BASE_MOVE - MIN_BASE_MOVE) * (curSpeed / BASE_SPEED_CAP);

        return (attacking) ? movement * ATTACK_MOVE_REDUCTION : movement;
    }

    /* Method used to reset assist status */
    public void resetAssistStatus() {
        assistCancelled = false;
    }

    /* Method to check if assist is cancelled */
    public bool isAssistCancelled() {
        return assistCancelled;
    }

    //  ---------------------
    //  Actual methods
    //  ---------------------

    // Initilizes entity status: called in playerController or abstractEnemy Awake function
    public void initializeEntity()
    {
        curHealth = maxHealth;
        maxArmor = Mathf.Max(def, sDef);
        curArmor = maxArmor;

        /* Sets moves for this entity by looking at its moveNames */
        for(int i = 0; i < moveNames.Length; i++) {
            moves[i] = nameToMove(moveNames[i]);
        }
    }

    // Regenerates health and armor bars
    public void regenBars() {
        /* Regenerating health */
        if(curHealth < maxHealth) {
            //Update timer 
            hTimer += Time.deltaTime;

            if (hTimer >= HP_REGEN_RATE) {
                /* Get some health back */
                curHealth += (maxHealth * HP_REGEN_PERCENT);
                if (curHealth > maxHealth)
                    curHealth = maxHealth;
                
                /* Update health bar */
                healthBar.fillAmount = curHealth / maxHealth;

                /* reset timer */
                hTimer = 0f;
            }
        }

        /* Recovering armor */
        if(underPressure && !shieldStunned) {
            //Update timer 
            aTimer += Time.deltaTime;

            /* If pressure duration has passed, recover all lost armor*/
            if(aTimer >= PRESSURE_DURATION) {
                curArmor = maxArmor;
                armorBar.fillAmount = 1f;
            }
        }
    }

    /* Method used to apply damage to this entity
        Pre: this entity has been hit by a hostile hitbox
        Post: this entity will now receive damage. If the player actually took damage and is still alive, return true, else return false */
    public bool applyDamage(int damage) {
        /* Only allow invincibility if target is a player */
        if (!invincibility) {
            /* Only allow invincibility if the unit is a player */
            if(transform.tag == GeneralConstants.PLAYER_TAG)
                StartCoroutine(invincibilityFrames());

            /* Do damage */
            curHealth -= damage;
            curArmor -= (damage * 3) / 2;
            assistCancelled = true;

            if (curHealth <= 0) {               //Case where this entity dies from this move
                if(tag == GeneralConstants.ENEMY_TAG) {
                    gameObject.SetActive(false);
                } else if (tag == GeneralConstants.PLAYER_TAG){
                    controller.SendMessage("OnDeath", this);
                }
                
                return false;
            }else{                              //Case where entity still lives 
                //Update player UI Bars
                healthBar.fillAmount = (float)curHealth / (float)maxHealth;
                armorBar.fillAmount = (float)curArmor / (float)maxArmor;

                /* Reset regen timers */
                aTimer = 0f;
                hTimer = 0f;

                /* Puts underPressure flag to true */
                if(!shieldStunned)
                    underPressure = true;

                //Checks if armor is shattered
                if(curArmor <= 0 && !shieldStunned)
                    StartCoroutine(shieldStun());
                
                return true;
            }
        }

        return false;
    }

    /* Allow for small invincibility period upon getting hit */
    private const float INVINCIBILITY_DURATION = 0.1f;

    IEnumerator invincibilityFrames() {
        invincibility = true;
        yield return new WaitForSeconds(INVINCIBILITY_DURATION);
        invincibility = false;
    }

    /* Method for armor shattering */
    private const float ARMOR_SHATTER_DURATION_ENEMY = 2.5f;
    private const float ARMOR_SHATTER_DURATION_PLAYER = 1.75f;

    IEnumerator shieldStun() {
        shieldStunned = true;
        movingDisabled = true;
        underPressure = false;
        Debug.Log("Armor shattered!");

        /* Calculate armor shatter duration */
        float curShatterDuration = (transform.tag == GeneralConstants.PLAYER_TAG) ? 
                                    ARMOR_SHATTER_DURATION_PLAYER
                                    : ARMOR_SHATTER_DURATION_ENEMY;

        yield return new WaitForSeconds(curShatterDuration);

        Debug.Log("Armor Restored");
        curArmor = maxArmor;
        armorBar.fillAmount = 1f;

        shieldStunned = false;
        movingDisabled = false;
    }


    /* Method used to execute a certain move as player
        Pre: moveID >= 0 && moveID < 3 and move is valid (not null and canRun()).
                hDir and vDir cannot be 0 at the same time*/
    public IEnumerator executeMovePlayer(int moveID, int hDir, int vDir) {
        if((moveID < 0 || moveID >= 3) && (hDir != 0 || vDir != 0))
            throw new System.Exception("Error: Invalid move ID");

        /* Set attacking to true */
        attacking = true;

        /* Get necessary data from move */
        IMove curMove = moves[moveID];
        movingDisabled = shieldStunned || curMove.isMovementDisabled();

        yield return curMove.executeMovePlayer(hDir, vDir);

        /* Set flag variables back to false */
        movingDisabled = shieldStunned;
        attacking = false; 
    }

    /* Method used to execute move as enemy */
    public IEnumerator executeMoveEnemy(int moveID, Transform tgt) {
        if((moveID < 0 || moveID >= 3) || tgt == null)
            throw new System.Exception("Error: Invalid move ID");

        /* Set attacking to true */
        attacking = true;

        /* Get necessary data from move */
        IMove curMove = moves[moveID];
        movingDisabled = shieldStunned || curMove.isMovementDisabled();

        yield return curMove.executeMoveEnemy(tgt);

        /* Set flag variables back to false */
        movingDisabled = shieldStunned;
        attacking = false; 
    }

    /* Method used to execute move as an assist move */
    public IEnumerator executeAssistMove(int moveID, int hDir, int vDir) {
        if((moveID < 0 || moveID >= 3) && (hDir != 0 || vDir != 0))
            throw new System.Exception("Error: Invalid move ID");

        /* Set attacking to true */
        attacking = true;

        /* Get necessary data from move */
        IMove curMove = moves[moveID];
        movingDisabled = shieldStunned || curMove.isMovementDisabled();

        yield return curMove.executeAssistMove(hDir, vDir);

        /* Set flag variables back to false */
        movingDisabled = shieldStunned;
        attacking = false; 
    }

    /* Helper method to check if move is valid
        A move is valid IFF the move slot is not null AND it can be run at this moment*/
    public bool canUseMove(int moveID) {
        if(moveID < 0 || moveID >= 3)
            return false;
        
        return moves[moveID] != null && !attacking && moves[moveID].canRun();
    }

    /* Public method to execute player animation for swapping: WILL BE IN SEPERATE CLASS */
    public IEnumerator runSwapAnimation() {
        Color prevColor = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = Color.magenta;
        movingDisabled = true;

        yield return new WaitForSeconds(0.35f);

        GetComponent<SpriteRenderer>().color = prevColor;
        movingDisabled = false;

    }

    /* Move map method: converts an input string to a set move 
        Pre: moveName must be a name of an appropriate move found in the move inventory */
    private IMove nameToMove(string moveName) {
        switch (moveName) {
            case "Pound":
                return new BasicMeleeAttack(25, 175f, this);
            case "BulletSeed":
                return new BulletSeed(this, ControlMap.ABILITY_2);
            case "HyperVoice":
                Transform hyperVoice = Resources.Load<Transform>("MoveHitboxes/HyperVoiceHitbox");
                return new CircleAoE(10f, true, 100, 500f, false, hyperVoice, this);
            case "None":
                return null;
            default:
                throw new System.Exception("ERROR: " + moveName +" not found in move inventory");


        }
    }


    //  ---------------------
    //  UI Methods and move regeneration regarding player only
    //  ---------------------

    /* Method used to update UI abilities / moves
        Pre: The array sent in MUST be of size 3 and all filled
        Post: UI abilities will now be updated */
    public void playerRegen(UIAbility[] abilities) {
        Debug.Assert(abilities.Length == 3);

        /* Regenerate bars */
        regenBars();

        /* Regenerating move CD and resources */
        for(int i = 0; i < abilities.Length; i++) {
            Debug.Assert(abilities[i] != null);
            
            if (moves[i] != null) {
                moves[i].regen();
                moves[i].updateMoveUI(abilities[i]);
            }
        }
    }

    /* Method used to set up UI with their abilities
        Pre: array part of UI resources MUST be size 3, nothing is null
        Post: This entity will now be assigned this Resources UI */
    public void setUpUI(UIResources resources) {
        Debug.Assert(resources.abilities.Length == 3);
        Debug.Assert(resources.healthBar != null);
        Debug.Assert(resources.armorBar != null);

        /* Set up healthBars and armorBars */
        healthBar = resources.healthBar;
        armorBar = resources.armorBar;

        /* Update bars */
        healthBar.fillAmount = (float)curHealth / (float)maxHealth;
        armorBar.fillAmount = (float)curArmor / (float)maxArmor;

        /* Set up abilities */
        for(int i = 0; i < resources.abilities.Length; i++) {
            Debug.Assert(resources.abilities[i] != null);

            if (moves[i] != null)
                moves[i].setUpUI(resources.abilities[i]);
            else
                resources.abilities[i].setNone();
        }
    }
}
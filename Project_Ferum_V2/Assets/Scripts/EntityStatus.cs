using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private IMove[] moves = new IMove[3];

    /* Stat effects - to be added */


    /* Flags */
    private bool invincibility;
    private bool movingDisabled = false;
    private bool attacking = false;
    private bool shieldStunned = false;

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

    /* Accessor methods to health % */
    public float getHealthPercent() {
        return (float)curHealth / (float)maxHealth;
    }

    /* Accessor method to armor percent */
    public float getArmorPercent() {
        return (float)curArmor / (float)maxArmor;
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


    //  ---------------------
    //  Actual methods
    //  ---------------------

    // Start is called before the first frame update
    void Awake()
    {
        curHealth = maxHealth;
        maxArmor = Mathf.Max(def, sDef);
        curArmor = maxArmor;

        /* Sets moves for player: TO BE MOVED ELSEWHERE */
        moves[0] = new BasicMeleeAttack(25, 175f, this);              // POUND
        moves[1] = new BulletSeed(this, ControlMap.ABILITY_2);        // BULLET SEED

        //HYPER VOICE
        Transform hyperVoice = Resources.Load<Transform>("MoveHitboxes/HyperVoiceHitbox");
        moves[2] = new CircleAoE(10f, true, 100, 500f, false, hyperVoice, this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /* Regenerating move CD and resources */
        for(int i = 0; i < moves.Length; i++) {
            if (moves[i] != null) {
                moves[i].regen();
            }
        }
    }

    /* Method used to apply damage to this entity
        Pre: this entity has been hit by a hostile hitbox
        Post: this entity will now receive damage. If the player actually took damage and is still alive, return true, else return false */
    public bool applyDamage(int damage) {
        /* Only allow invincibility if target is a player */
        if (!invincibility || transform.tag == GeneralConstants.ENEMY_TAG) {
            StartCoroutine(invincibilityFrames());

            curHealth -= damage;
            curArmor -= (damage * 3) / 2;
            Debug.Log(curHealth);

            if (curHealth <= 0) {               //Case where this entity dies from this move
                gameObject.SetActive(false);
                return false;
            }else{                              //Case where entity still lives 
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
    private const float ARMOR_SHATTER_DURATION = 1.75f;

    IEnumerator shieldStun() {
        shieldStunned = true;
        movingDisabled = true;
        Debug.Log("Armor shattered!");

        yield return new WaitForSeconds(ARMOR_SHATTER_DURATION);

        Debug.Log("Armor Restored");
        curArmor = maxArmor;
        shieldStunned = false;
        movingDisabled = false;
    }


    /* Method used to execute a certain move
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

    /* Helper method to check if move is valid
        A move is valid IFF the move slot is not null AND it can be run at this moment*/
    public bool canUseMove(int moveID) {
        if(moveID < 0 || moveID >= 3)
            return false;
        
        return moves[moveID] != null && !attacking && moves[moveID].canRun();
    }
}

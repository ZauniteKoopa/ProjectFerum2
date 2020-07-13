using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Public interface that is used for ALL moves / abilities that a player or enemy uses */
public abstract class IMove
{
    /* Boolean that represent play'ers movement during move animation */
    private bool movementDisabled;

    /* IMove constructor */
    public IMove(bool moveType) {
        movementDisabled = moveType;
    }

    //Calculates damage for a given move based on the official pokemon damage formula
    //  Pre: 0 < lvl <= 100, 0 < power, attDefRatio = attacker attack / victim defense
    //  Post: Returns an int representing the amount of damage applied to enemy
    public int damageCalc(int level, int power, EntityStatus attacker, EntityStatus tgt, bool isPhysical) {
        /* Get necessariy stats from entity*/
        int entityAttack, enemyDef;

        if (isPhysical) {
            entityAttack = attacker.getStat((int)GeneralConstants.statIDs.ATTACK);
            enemyDef = tgt.getStat((int)GeneralConstants.statIDs.DEFENSE);
        }else {
            entityAttack = attacker.getStat((int)GeneralConstants.statIDs.SP_ATTACK);
            enemyDef = tgt.getStat((int)GeneralConstants.statIDs.SP_DEFENSE);
        }

        float attDefRatio = (float)entityAttack / (float)enemyDef;

        /* Calculate damage */
        float damage = (0.5f * level + 2) * power * attDefRatio * 0.06f;
        damage += 1;
        return (int)damage;
    }

    //Returns a vector of magnitude knockbackVal that goes from the entity to its enemy
    //  Pre: entityPos and enemyPos are 2D transform positions, kncobackVal > 0
    //  Post: Returns a vector of magnitude knockbackVal that points from entity to enemy
    public Vector2 dirKnockbackCalc(Vector2 entityPos, Vector2 enemyPos, float knockbackVal) {
        Vector2 result = new Vector2(enemyPos.x - entityPos.x, enemyPos.y - entityPos.y);
        result.Normalize();
        return result *= knockbackVal;
    }

    //Checks IMove's movement status during player's animation
    public bool isMovementDisabled() {
        return movementDisabled;
    }

    /* Returns entity's suggested orientation given the target's position relative to entity
        Pre: The vector2 represents the distance between tgt and entity
        Post: returns an integer representing orientation using the following mapping
        
            0 or 4 -> no direction: 0
            1 - 3 inclusive -> Right or Up direction: 1
            5 - 7 inclusive -> left or down direction: -1 */
    
    private const int DEGREE_DIV = 45;
    private const float OFFSET = 22.5f;

    protected int getAttackOrientation(Vector2 dist, bool isHorizontal) {
        //Calculate the angle of the vector 
        float deg = Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg;
        deg = (deg < 0) ? deg + 360 + OFFSET : deg + OFFSET;        //Add offset for key values to align
        deg = (deg >= 360) ? deg - 360 : deg;

        //Calculate the key value: 0 - 7
        int keyValue = (int)(deg / DEGREE_DIV);
        keyValue = (isHorizontal) ? (keyValue + 2) % 8 : keyValue;  //Adjust keyvalue if looking for horizontal value  

        //Calculate the new attack vertical and attack horizontal orientation
        return (keyValue % 4 == 0) ? 0 : -1 * (int)Mathf.Sign(keyValue - 4f);
    }

    /* Method that assigns appropriate hitbox tag depending on entity's tag */
    protected string assignHitboxTag(string entityTag) {
        if (entityTag == GeneralConstants.PLAYER_TAG)
            return GeneralConstants.PLAYER_ATTK_TAG;
        else
            return GeneralConstants.ENEMY_ATTK_TAG;
    }
    


    //  --------------------
    //  Abstract methods to be implemented
    //  --------------------

    /* Method that checks whether or not a move can be run or not */
    public abstract bool canRun();

    /* Method used to execute a move from the player's perspective */
    public abstract IEnumerator executeMovePlayer(int hDir, int vDir);

    /* Method used to execute a move from an enemy's perspective */
    public abstract IEnumerator executeMoveEnemy(Transform tgt);

    /* Method used to regenerate cooldowns / ammo (not used in melee attacks) */
    public abstract void regen();

    /* Method called by a hitbox for move object to calculate and do "damage to target"
        Pre: hitbox associated with move has hit its associated target
        Post: Target will now receive the move's effects */
    public abstract void enactEffects(EntityStatus tgt);
}

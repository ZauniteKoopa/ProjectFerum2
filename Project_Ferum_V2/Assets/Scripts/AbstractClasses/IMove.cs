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
    public int damageCalc(int level, int power, int entityAttack, int enemyDef) {
        float attDefRatio = (float)entityAttack / (float)enemyDef;

        float damage = (0.5f * level + 2) * power * attDefRatio * 0.06f;
        damage += 1;
        return (int)damage;
    }

    //Checks IMove's movement status during player's animation
    public bool isMovementDisabled() {
        return movementDisabled;
    }

    /* Method that checks whether or not a move can be run or not */
    public abstract bool canRun();

    /* Method used to execute a move from the player's perspective */
    public abstract IEnumerator executeMovePlayer(int hDir, int vDir);

    /* Method used to regenerate cooldowns / ammo (not used in melee attacks) */
    public abstract void regen();

    /* Method called by a hitbox for move object to calculate and do "damage to target"
        Pre: hitbox associated with move has hit its associated target
        Post: Target will now receive the move's effects */
    public abstract void enactEffects(EntityStatus tgt);
}

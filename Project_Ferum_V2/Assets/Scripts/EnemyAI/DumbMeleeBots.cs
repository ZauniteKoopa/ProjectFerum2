using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbMeleeBots : AbstractEnemy
{
    //Move IDs for use
    private const int MELEE_ATTK_ID = 0;
    private const int SHIELD_ID = 1;
    private const int CHARGE_DASH_ID = 2;

    //Method for movement
    public override void movement() {
        if (isAgitated()) {
            Vector3 dirVect = getTgtPos() - transform.position;
            dirVect.Normalize();

            transform.position += (dirVect * getMoveSpeed());
        }
    }

    //Method for basic attacking
    public override void attack() {
        executeMove(CHARGE_DASH_ID);
    }

    //Method for reacting to player
    public override bool reactToPlayer() {
        executeMove(MELEE_ATTK_ID);
        return true;
    }

    //Method to react to player attack
    public override bool reactToAttack(Transform attk) {
        executeMove(SHIELD_ID, attk);
        return true;
    }

}

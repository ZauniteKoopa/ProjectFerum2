using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleAttackPunchingBag : AbstractEnemy
{
    //Melee attack ID number
    private const int ATTACK_ID = 0;

    /* Do nothing */
    public override void movement() {

    }

    /* Use melee attack */
    public override void attack() {
        executeMove(ATTACK_ID);
    }
}

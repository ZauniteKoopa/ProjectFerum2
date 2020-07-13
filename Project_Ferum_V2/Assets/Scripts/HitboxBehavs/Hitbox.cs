using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    IMove myMove = null;

    /* Sets HitBox's move so it knows where to send a message to upon hitting tgt */
    public void setMove(IMove newMove) {
        myMove = newMove;
    }

    /* Method to send message to assigned move to apply affects to target
        Pre: myMove != null && tgt != null */
    protected void applyEffects(EntityStatus tgt) {
        myMove.enactEffects(tgt);
    }

    /* Method used to see if you hit an enemy or not */
    protected bool enemyHit(string tgtTag) {
        if (tag == GeneralConstants.PLAYER_ATTK_TAG) {
            return tgtTag == GeneralConstants.ENEMY_TAG;
        } else {
            return tgtTag == GeneralConstants.PLAYER_TAG;
        }
    }
}

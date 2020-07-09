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
}

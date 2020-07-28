using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    IMove myMove = null;

    //Priority is on a scale of 1 to 10
    [SerializeField]
    int movePriority = 0;

    //Flagthat indicates this hitbox can interact withother hitboxes of the same type
    [SerializeField]
    bool friendlyFire = false;

    /* Sets HitBox's move so it knows where to send a message to upon hitting tgt */
    public void setMove(IMove newMove) {
        myMove = newMove;
    }

    /* Set priority */
    public void setPriority(int newPriority) {
        movePriority = newPriority;
    }

    /* Method to send message to assigned move to apply affects to target
        Pre: myMove != null */
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

    /* Method used to see if you get hit by a valid, interactable attack */
    protected bool hitAttack(Collider2D collider) {
        if (!hitAttackMutual(collider.tag))
            return false;

        Hitbox colliderHB = collider.GetComponent<Hitbox>();
        string tgtTag = collider.tag;
        
        if (tag == GeneralConstants.PLAYER_ATTK_TAG) {
            return tgtTag == GeneralConstants.ENEMY_ATTK_TAG || colliderHB.friendlyFire;
        } else {
            return tgtTag == GeneralConstants.PLAYER_ATTK_TAG || colliderHB.friendlyFire;
        }
    }

    /* Method used to see if hit by another attack regardless of attack type */
    protected bool hitAttackMutual(string tgtTag) {
        return tgtTag == GeneralConstants.ENEMY_ATTK_TAG || tgtTag == GeneralConstants.PLAYER_ATTK_TAG;
    }

    /* Method used to check if hitting another hitbox which one has more priority */
    protected bool overpoweredBy(Hitbox tgt) {
        return movePriority <= tgt.movePriority;
    }
}

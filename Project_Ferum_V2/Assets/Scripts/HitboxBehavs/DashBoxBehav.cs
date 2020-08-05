using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashBoxBehav : Hitbox
{
    //Activates hitbox
    public void activateHitbox(string attackTag, IMove move, int priority) {
        setPriority(priority);
        setMove(move);
        tag = attackTag;
        gameObject.SetActive(true);
    }

    //Deactivates hitbox
    public void deactivateHitbox() {
        gameObject.SetActive(false);
    }

    //On collision with enemy
    void OnTriggerEnter2D(Collider2D tgt) {
        if(enemyHit(tgt.tag)) {
            EntityStatus tgtStatus = tgt.GetComponent<EntityStatus>();
            applyEffects(tgtStatus);
            deactivateHitbox();
        }

        /* Behavior when hitting another attack */
        if (hitAttack(tgt) && overpoweredBy(tgt.GetComponent<Hitbox>())) {
            applyEffects(null);
        }
    }

}

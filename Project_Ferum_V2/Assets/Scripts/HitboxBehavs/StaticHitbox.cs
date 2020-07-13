using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticHitbox : Hitbox
{
    void OnTriggerEnter2D(Collider2D tgt) {

        /* Behavior when an enemy is hit */
        if(enemyHit(tgt.tag)) {
            EntityStatus tgtStatus = tgt.GetComponent<EntityStatus>();
            applyEffects(tgtStatus);
        }
    }
}

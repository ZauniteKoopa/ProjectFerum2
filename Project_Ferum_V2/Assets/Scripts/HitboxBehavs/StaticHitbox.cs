using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticHitbox : Hitbox
{
    HashSet<Collider2D> hit = new HashSet<Collider2D>();

    //Resets hitbox to be used again. MUST be called before using
    public void resetHitbox() {
        hit.Clear();
    }

    //called upon hitting an enemy
    void OnTriggerEnter2D(Collider2D tgt) {

        /* Behavior when an enemy is hit */
        if(enemyHit(tgt.tag) && !hit.Contains(tgt)) {
            hit.Add(tgt);
            EntityStatus tgtStatus = tgt.GetComponent<EntityStatus>();
            applyEffects(tgtStatus);
        }
    }
}

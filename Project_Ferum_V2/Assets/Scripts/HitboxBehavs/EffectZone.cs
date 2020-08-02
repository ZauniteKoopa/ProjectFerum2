using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectZone : Hitbox
{
    // Duration keeping
    private float timer = 0f;
    private float duration;

    // Establish properties
    public void setProperties(IMove move, float maxDuration) {
        duration = maxDuration;
        setMove(move);
    }

    //On update: increment timer
    void FixedUpdate() {
        timer += Time.deltaTime;

        if (timer > duration) {
            Destroy(gameObject);
        }
    }

    //On Trigger Enter: enactEffects
    void OnTriggerEnter2D(Collider2D tgt) {
        /* Behavior when an enemy enters zone */
        if(enemyHit(tgt.tag)) {
            EntityStatus tgtStatus = tgt.GetComponent<EntityStatus>();
            applyEffects(tgtStatus);
        }
    }

    //On Trigger exits: enactEffects
    //On Trigger Enter: enactEffects
    void OnTriggerExit2D(Collider2D tgt) {
        /* Behavior when an enemy exits zone */
        if(enemyHit(tgt.tag)) {
            EntityStatus tgtStatus = tgt.GetComponent<EntityStatus>();
            applyEffects(tgtStatus);
        }
    }
}

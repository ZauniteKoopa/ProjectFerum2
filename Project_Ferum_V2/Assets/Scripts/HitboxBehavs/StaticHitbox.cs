﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticHitbox : Hitbox
{
    void OnTriggerEnter2D(Collider2D tgt) {

        /* Behavior when an enemy is hit */
        if(tgt.tag == GeneralConstants.ENEMY_TAG) {
            EntityStatus tgtStatus = tgt.GetComponent<EntityStatus>();
            applyEffects(tgtStatus);
        }
    }
}

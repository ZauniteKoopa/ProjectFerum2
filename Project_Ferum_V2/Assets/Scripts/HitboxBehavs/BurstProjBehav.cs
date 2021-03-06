﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class describing a projectile that blows up upon collision with a wall
public class BurstProjBehav : ProjectileBehav
{
    //Variables
    [SerializeField]
    Transform splashBox = null;

    //Variables for timers
    private const float BLAST_DURATION = 0.5f;
    private float blastTimer = 0f;
    bool moving = true;

    //Hashset for collisions
    private HashSet<Collider2D> hit = new HashSet<Collider2D>();

    /* Fixed update */
    void FixedUpdate() {

        if (moving) {                           //Projectile mode
            moveProjectile();
        } else {                                //Blast mode
            blastTimer += Time.deltaTime;
            if (blastTimer >= BLAST_DURATION) {
                Destroy(gameObject);
            }
        }
    }

    // Method called when collides with either an enemy or a wall
    void OnTriggerEnter2D(Collider2D tgt) {
        /* Behavior when an enemy is hit */
        if(enemyHit(tgt.tag)) {
            attkInitialEnemy(tgt);
            triggerSplashHitbox();
        }

        /* Behavior when wall is hit */
        if(tgt.tag == GeneralConstants.WALL_TAG) {
            triggerSplashHitbox();
        }

        /* Behavior when hitting another attack */
        if (hitAttack(tgt) && overpoweredBy(tgt.GetComponent<Hitbox>())) {
            triggerSplashHitbox();
        }
    }

    //Protected method to add enemy to hit
    protected void attkInitialEnemy(Collider2D tgt) {
        hit.Add(tgt);
        EntityStatus tgtStatus = tgt.GetComponent<EntityStatus>();
        applyEffects(tgtStatus);
    }

    //Method for triggering splash hitbox
    protected void triggerSplashHitbox() {
        //Disable initial projectile hitbox
        moving = false;
        blastTimer = 0f;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        //Enable splash box's hitbox
        splashBox.gameObject.SetActive(true);
    }

    //Method for splash hitbox to use to enact damage
    public void applySplashDamage(Collider2D tgt) {
        if(enemyHit(tgt.tag) && !hit.Contains(tgt)) {
            hit.Add(tgt);
            EntityStatus tgtStatus = tgt.GetComponent<EntityStatus>();
            applyEffects(tgtStatus);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehav : Hitbox
{
    //speed of the projectile per frame
    private Vector3 speedVector;

    //Time out variables
    private const float TIMEOUT = 6f;
    private float timer = 0f;

    /* Sets properties of this projectile */
    public void setProperties(IMove move, float speed, Vector3 vect) {
        vect.Normalize();

        setMove(move);
        speedVector = speed * vect;
    }

    /* Fixed update */
    void FixedUpdate() {
        transform.position += speedVector;

        /* Update timer */
        timer += Time.deltaTime;
        if (timer >= TIMEOUT) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D tgt) {

        /* Behavior when an enemy is hit */
        if(tgt.tag == GeneralConstants.ENEMY_TAG) {
            EntityStatus tgtStatus = tgt.GetComponent<EntityStatus>();
            applyEffects(tgtStatus);
            Destroy(gameObject);
        }

        /* Behavior when wall is hit */
        if(tgt.tag == GeneralConstants.WALL_TAG) {
            Destroy(gameObject);
        }
    }
}

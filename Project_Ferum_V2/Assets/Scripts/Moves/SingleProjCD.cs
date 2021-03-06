﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleProjCD : CooldownMove
{
    //Variables concerning move
    private int power;
    private float knockback;
    private const float KB_DURATION = 0.06f;

    //Reference variables
    private EntityStatus status;
    private Transform hitbox;
    private Transform curHitbox;

    //Variables concerning execution
    private const float ATTACK_ANIM_TIME = 0.15f;

    //Move constructor
    public SingleProjCD(EntityStatus es, int pwr, float kb, float maxCD, string hitboxSrc) : base(maxCD, true) {
        status = es;
        power = pwr;
        knockback = kb;
        hitbox = Resources.Load<Transform>(hitboxSrc);
        curHitbox = null;
    }

    //Allows player to execute move
    public override IEnumerator executeMovePlayer() {
        createProjectile(getVectorToMouse(status.transform));
        startCDTimer();

        /* Do animation */
        yield return playerWaitForSec(ATTACK_ANIM_TIME, status, getMouseInputKey());
    }

    //Allows enemy to execute move
    public override IEnumerator executeMoveEnemy(Transform tgt) {
        /* Get direction vector */
        Vector3 dirVect = tgt.position - status.transform.position;
        createProjectile(dirVect);

        /* Do animation */
        yield return new WaitForSeconds(ATTACK_ANIM_TIME);
    }

    //Allows player's assist to execute move
    public override IEnumerator executeAssistMove() {
        createProjectile(getVectorToMouse(status.transform));
        startCDTimer();

        /* Do animation */
        yield return new WaitForSeconds(ATTACK_ANIM_TIME);
    }

    //Creates projectile
    private void createProjectile(Vector3 dirVect) {
        /* Set properties of projectile and detach from parent*/
        curHitbox = Object.Instantiate(hitbox, status.transform);
        curHitbox.tag = assignHitboxTag(status.tag);
        curHitbox.GetComponent<ProjectileBehav>().setProperties(this, dirVect);
        curHitbox.parent = null;
    }

    //Enact effects on enemy
    public override void enactEffects(EntityStatus tgt) {
        int damage = damageCalc(power, status, tgt, false);
        bool applyKB = tgt.applyDamage(damage);

        if (applyKB) {
            Rigidbody2D rb = tgt.GetComponent<Rigidbody2D>();
            Vector3 dirVect = tgt.transform.position - curHitbox.position;
            dirVect.Normalize();
            dirVect *= knockback;

            tgt.StartCoroutine(tgt.receiveKnockback(dirVect, KB_DURATION));
        }
    }
}

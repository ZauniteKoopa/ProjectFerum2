using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleAoE : CooldownMove
{
    /* Private instance variables regarding move's effect on enemy */
    private int pwr;
    private float kb;
    private bool isPhy;
    private const float MOVE_DURATION = 0.2f;

    /* Knockback variables */
    private float KB_DURATION = 0.125f;

    /* Reference Variables */
    private EntityStatus myStatus;
    private Transform hitbox;

    /* Basic constructor */
    public CircleAoE(float cd, bool moveDisabled, int power, float knockback, bool isPhy, string hbSrc, EntityStatus es) : base(cd, moveDisabled){
        pwr = power;
        kb = knockback;
        this.isPhy = isPhy;
        hitbox = Resources.Load<Transform>(hbSrc);
        myStatus = es;
    }

    /* Move that allows player to execute move */
    public override IEnumerator executeMovePlayer() {
        /* Set hitbox properties */
        Transform curHitbox = Object.Instantiate(hitbox, myStatus.transform);
        curHitbox.tag = assignHitboxTag(myStatus.tag);
        curHitbox.GetComponent<Hitbox>().setMove(this);
        curHitbox.GetComponent<StaticHitbox>().resetHitbox();

        /* Duration of move */
        yield return playerWaitForSec(MOVE_DURATION, myStatus, getMouseInputKey());

        /* Destroy hitbox and Activate cooldown */
        Object.Destroy(curHitbox.gameObject);
        startCDTimer();
    }

    /* IEnumerator that allows an enemy / AI to attack */
    public override IEnumerator executeMoveEnemy(Transform tgt) {
        /* Set hitbox properties */
        Transform curHitbox = Object.Instantiate(hitbox, myStatus.transform);
        curHitbox.tag = assignHitboxTag(myStatus.tag);
        curHitbox.GetComponent<Hitbox>().setMove(this);
        curHitbox.GetComponent<StaticHitbox>().resetHitbox();

        /* Duration of move */
        yield return new WaitForSeconds(MOVE_DURATION);

        /* Destroy hitbox and Activate cooldown */
        Object.Destroy(curHitbox.gameObject);
    }

    /* Move that allows player to execute this move as an assist move */
    public override IEnumerator executeAssistMove() {
        yield return executeMoveEnemy(null);
        startCDTimer();
    }

    /* Does damage to enemy */
    public override void enactEffects(EntityStatus tgt) {
        int damage = damageCalc(pwr, myStatus, tgt, isPhy);
        bool applyKB = tgt.applyDamage(damage);

        if (applyKB) {
            Vector2 kbVector = dirKnockbackCalc(myStatus.transform.position, tgt.transform.position, kb);
            tgt.StartCoroutine(tgt.receiveKnockback(kbVector, KB_DURATION));
        }
    }
}

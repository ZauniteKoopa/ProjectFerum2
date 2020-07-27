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

    /* Reference Variables */
    private EntityStatus myStatus;
    private Transform hitbox;

    /* Basic constructor */
    public CircleAoE(float cd, bool moveDisabled, int power, float knockback, bool isPhy, Transform hb, EntityStatus es) : base(cd, moveDisabled){
        pwr = power;
        kb = knockback;
        this.isPhy = isPhy;
        hitbox = hb;
        myStatus = es;
    }

    /* Move that allows player to execute move */
    public override IEnumerator executeMovePlayer(int hDir, int vDir) {
        yield return executeMoveEnemy(null);
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
    public override IEnumerator executeAssistMove(int hDir, int vDir) {
        yield return executeMoveEnemy(null);
        startCDTimer();
    }

    /* Does damage to enemy */
    public override void enactEffects(EntityStatus tgt) {
        int damage = damageCalc(myStatus.getLevel(), pwr, myStatus, tgt, isPhy);
        bool applyKB = tgt.applyDamage(damage);

        if (applyKB) {
            myStatus.StartCoroutine(applyKnockback(tgt));
        }
    }

    /* Method to apply knockback to target if applyDamage returned true (attack hit the tgt and tgt is still alive)*/
    private float KB_DURATION = 0.125f;

    IEnumerator applyKnockback(EntityStatus tgt) {
        /* Apply knockback vector to entity */
        Vector2 kbVector = dirKnockbackCalc(myStatus.transform.position, tgt.transform.position, kb);
        Rigidbody2D rb = tgt.GetComponent<Rigidbody2D>();
        rb.AddForce(kbVector);

        /* Wait for duration */
        yield return new WaitForSeconds(KB_DURATION);

        /* cancel knockback */
        rb.velocity = Vector2.zero;
    }
}

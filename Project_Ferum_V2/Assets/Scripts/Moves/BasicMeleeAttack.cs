using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeAttack : FrameMove
{
    /* Variables regarding power and stats to be set in constructor */
    private int power;
    private float knockback;

    /* Reference variables */
    private EntityStatus myStatus;
    private Transform hitbox;


    /* Constructor */
    public BasicMeleeAttack(int pwr, float kb, EntityStatus entity) : base(true) {
        myStatus = entity;

        hitbox = entity.transform.GetChild(0);
        hitbox.tag = assignHitboxTag(entity.tag);

        knockback = kb;
        power = pwr;
    }

    /* IEnumerator that allows player to execute move */
    private const float HITBOX_OFFSET = 0.5f;

    public override IEnumerator executeMovePlayer(int hDir, int vDir) {
        /* Set hitbox's assigned move to this*/
        hitbox.GetComponent<Hitbox>().setMove(this);

        /* Reposition hitbox*/
        Vector3 dirVector = new Vector3(hDir, vDir, 0);
        dirVector.Normalize();
        dirVector *= HITBOX_OFFSET;
        hitbox.position += dirVector;

        /* Activate hitbox*/
        hitbox.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        hitbox.gameObject.SetActive(false);

        /* Reverse reposition*/
        hitbox.position -= dirVector;
    }

    /* IEnumerator that allows an enemy / AI to attack */
    public override IEnumerator executeMoveEnemy(Transform tgt) {
        Vector3 tgtPos = tgt.position;
        Vector3 myPos = myStatus.transform.position;
        Vector2 distVector = new Vector2(tgtPos.x - myPos.x, tgtPos.y - myPos.y);

        int vDir = getAttackOrientation(distVector, false);
        int hDir = getAttackOrientation(distVector, true);
        yield return executeMovePlayer(hDir, vDir);
    }

    /* IEnumerator that allows player to use this move as an assist move */
    public override IEnumerator executeAssistMove(int hDir, int vDir) {
        yield return executeMovePlayer(hDir, vDir);
    }

    /* Does damage to target */
    public override void enactEffects(EntityStatus tgt) {
        int damage = damageCalc(myStatus.getLevel(), power, myStatus, tgt, true);
        bool applyKB = tgt.applyDamage(damage);

        if (applyKB) {
            myStatus.StartCoroutine(applyKnockback(tgt));
        }
    }

    /* Method to apply knockback to target if applyDamage returned true (attack hit the tgt and tgt is still alive)*/
    private float KB_DURATION = 0.125f;

    IEnumerator applyKnockback(EntityStatus tgt) {
        /* Apply knockback vector to entity */
        Vector2 kbVector = dirKnockbackCalc(myStatus.transform.position, tgt.transform.position, knockback);
        Rigidbody2D rb = tgt.GetComponent<Rigidbody2D>();
        rb.AddForce(kbVector);

        /* Wait for duration */
        yield return new WaitForSeconds(KB_DURATION);

        /* cancel knockback */
        rb.velocity = Vector2.zero;
    }
}

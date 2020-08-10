using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeAttack : FrameMove
{
    /* Variables regarding power and stats to be set in constructor */
    private int power;
    private float knockback;
    private int priority;
    private float KB_DURATION = 0.125f;

    /* Reference variables */
    private EntityStatus myStatus;
    private Transform hitbox;

    /* Constructor */
    public BasicMeleeAttack(int pwr, float kb, EntityStatus entity, int prio) : base(true) {
        myStatus = entity;

        hitbox = entity.transform.GetChild(0);
        hitbox.tag = assignHitboxTag(entity.tag);
        priority = prio;

        knockback = kb;
        power = pwr;
    }

    /* IEnumerator that allows player to execute move */
    public override IEnumerator executeMovePlayer() {
        Vector3 distVector = getVectorToMouse(myStatus.transform);
        int vDir = getAttackOrientation(distVector, false);
        int hDir = getAttackOrientation(distVector, true);
        yield return doMeleeMove(hDir, vDir);
    }

    /* IEnumerator that allows an enemy / AI to attack */
    public override IEnumerator executeMoveEnemy(Transform tgt) {
        Vector3 tgtPos = tgt.position;
        Vector3 myPos = myStatus.transform.position;
        Vector2 distVector = new Vector2(tgtPos.x - myPos.x, tgtPos.y - myPos.y);

        int vDir = getAttackOrientation(distVector, false);
        int hDir = getAttackOrientation(distVector, true);
        yield return doMeleeMove(hDir, vDir);
    }

    /* IEnumerator that allows player to use this move as an assist move */
    public override IEnumerator executeAssistMove() {
        yield return executeMovePlayer();
    }

    /* Private helper method to do melee attack */
    private const float HITBOX_OFFSET = 0.5f;

    private IEnumerator doMeleeMove(int hDir, int vDir) {
        /* Set hitbox's assigned move to this*/
        hitbox.GetComponent<Hitbox>().setMove(this);
        hitbox.GetComponent<Hitbox>().setPriority(priority);
        hitbox.GetComponent<StaticHitbox>().resetHitbox();

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

    /* Does damage to target */
    public override void enactEffects(EntityStatus tgt) {
        int damage = damageCalc(power, myStatus, tgt, true);
        bool applyKB = tgt.applyDamage(damage);

        if (applyKB) {
            Vector2 kbVector = dirKnockbackCalc(myStatus.transform.position, tgt.transform.position, knockback);
            tgt.StartCoroutine(tgt.receiveKnockback(kbVector, KB_DURATION));
        }
    }    
}

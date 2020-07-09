using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleAoE : CooldownMove
{
    /* Private instance variables regarding move's effect on enemy */
    private int pwr;
    private float kb;
    private bool isPhy;

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
    private const float MOVE_DURATION = 0.55f;

    public override IEnumerator executeMovePlayer(int hDir, int vDir) {
        /* Set hitbox properties */
        Transform curHitbox = Object.Instantiate(hitbox, myStatus.transform);
        curHitbox.GetComponent<Hitbox>().setMove(this);

        /* Duration of move */
        yield return new WaitForSeconds(MOVE_DURATION);

        /* Destroy hitbox and Activate cooldown */
        Object.Destroy(curHitbox.gameObject);
        startCDTimer();
    }

    /* Does damage to enemy */
    public override void enactEffects(EntityStatus tgt) {
        int damage = damageCalc(myStatus.getLevel(), pwr, myStatus, tgt, isPhy);
        tgt.applyDamage(damage);
    }
}

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
        knockback = kb;
        power = pwr;
    }

    /* IEnumerator that allows player to execute move */
    private const float HITBOX_OFFSET = 0.5f;

    public override IEnumerator executeMovePlayer(int hDir, int vDir) {
        Vector3 dirVector = new Vector3(hDir, vDir, 0);
        dirVector.Normalize();
        dirVector *= HITBOX_OFFSET;

        hitbox.position += dirVector;

        hitbox.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        hitbox.gameObject.SetActive(false);

        hitbox.position -= dirVector;
    }

    /* Does damage to enemy */
    public override void enactEffects(EntityStatus tgt) {

    }
}

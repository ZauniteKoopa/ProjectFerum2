using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSeed : AmmoMove
{
    /* In Build Move Constants for Bullet Seed */
    private const int POWER = 10;
    private const float PROJ_SPEED = 0.165f;
    private const float FIRE_RATE = 0.3f;

    /* Ammo regen variables */
    private const int MAX_AMMO = 20;
    private const float REGEN_RATE = 2f;

    /* Reference Variables */
    private EntityStatus myStatus;
    private Transform hitbox;
    private string input;


    /* Bullet Seed Constructor */
    public BulletSeed(EntityStatus entity, string buttonInput) : base(MAX_AMMO, REGEN_RATE, false) {
        myStatus = entity;
        hitbox = Resources.Load<Transform>("MoveHitboxes/Bullet");
        input = buttonInput;
    }

    /* Allows player to shoot */
    public override IEnumerator executeMovePlayer(int hDir, int vDir) {
        while(Input.GetKey(input) && canRun() && !myStatus.armorBroke()) {
            /* Get direction vector */
            Vector3 dirVect = new Vector3(hDir, vDir, 0);

            /* Set properties of projectile and detach from parent*/
            Transform curBullet = Object.Instantiate(hitbox, myStatus.transform);
            curBullet.tag = assignHitboxTag(myStatus.tag);
            curBullet.GetComponent<ProjectileBehav>().setProperties(this, PROJ_SPEED, dirVect);
            curBullet.parent = null;

            /* Consume a bullet */
            useAmmo();

            /* Wait for next chance to fire */
            yield return new WaitForSeconds(FIRE_RATE); 
        }
    }

    /* IEnumerator that allows an enemy / AI to attack */
    private int MIN_SHOTS = 2;
    private int MAX_SHOTS = 5;

    public override IEnumerator executeMoveEnemy(Transform tgt) {
        // calculate how many shots fired
        int shotLimit = Random.Range(MIN_SHOTS, MAX_SHOTS + 1);
        int curShot = 0;

        while(curShot <= shotLimit && !myStatus.armorBroke()) {
            /* Get direction vector */
            Vector3 dirVect = tgt.position - myStatus.transform.position;

            /* Set properties of projectile and detach from parent*/
            Transform curBullet = Object.Instantiate(hitbox, myStatus.transform);
            curBullet.tag = assignHitboxTag(myStatus.tag);
            curBullet.GetComponent<ProjectileBehav>().setProperties(this, PROJ_SPEED, dirVect);
            curBullet.parent = null;

            /* Wait for next chance to fire */
            yield return new WaitForSeconds(FIRE_RATE); 

            /* Increment curShot counter */
            curShot++;
        }

    }

    /* Does damage to enemy */
    public override void enactEffects(EntityStatus tgt) {
        int damage = damageCalc(myStatus.getLevel(), POWER, myStatus, tgt, true);
        tgt.applyDamage(damage);
    }
}

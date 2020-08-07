using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : AmmoMove
{
    /* variables for bullet-type moves*/
    private int power;
    private float fireRate;

    /* Reference Variables */
    private EntityStatus myStatus;
    private Transform hitbox;

    /* Variables indicating if the move is physical or not */
    private bool isPhy;


    /* Bullet Seed Constructor */
    public BulletMove(EntityStatus entity, int pwr, int maxAmmo, float regenRate, float fr, bool isPhysical, string hitboxSrc) : base(maxAmmo, regenRate, false) {
        myStatus = entity;
        hitbox = Resources.Load<Transform>(hitboxSrc);
        fireRate = fr;
        isPhy = isPhysical;
        power = pwr;
    }

    /* Allows player to shoot */
    public override IEnumerator executeMovePlayer() {
        int mouseInput = getMouseInputKey();

        while(Input.GetMouseButton(mouseInput) && canRun() && !myStatus.armorBroke()) {
            /* Get direction vector */
            Vector3 dirVect = getVectorToMouse(myStatus.transform);

            /* Set properties of projectile and detach from parent*/
            Transform curBullet = Object.Instantiate(hitbox, myStatus.transform);
            curBullet.tag = assignHitboxTag(myStatus.tag);
            curBullet.GetComponent<ProjectileBehav>().setProperties(this, dirVect);
            curBullet.parent = null;

            /* Consume a bullet */
            useAmmo();

            /* Wait for next chance to fire */
            yield return new WaitForSeconds(fireRate); 
        }
    }

    /* IEnumerator that allows an enemy / AI to attack */
    private const int MIN_SHOTS = 4;
    private const int MAX_SHOTS = 5;

    public override IEnumerator executeMoveEnemy(Transform tgt) {
        // calculate how many shots fired
        int shotLimit = Random.Range(MIN_SHOTS, MAX_SHOTS + 1);
        int curShot = 0;

        while(curShot < shotLimit && !myStatus.armorBroke()) {
            /* Get direction vector */
            Vector3 dirVect = tgt.position - myStatus.transform.position;

            /* Set properties of projectile and detach from parent*/
            Transform curBullet = Object.Instantiate(hitbox, myStatus.transform);
            curBullet.tag = assignHitboxTag(myStatus.tag);
            curBullet.GetComponent<ProjectileBehav>().setProperties(this, dirVect);
            curBullet.parent = null;

            /* Wait for next chance to fire */
            yield return new WaitForSeconds(fireRate); 

            /* Increment curShot counter */
            curShot++;
        }
    }

    /* IEnumerator that allows player to use this move as an assist move */
    private const int ASSIST_SHOTS = 4;

    public override IEnumerator executeAssistMove() {
        // calculate how many shots fired and get dir Vector
        int curShot = 0;
        Vector3 dirVect = getVectorToMouse(myStatus.transform);

        //Loop to run bullets
        while(canRun() && curShot < ASSIST_SHOTS && !myStatus.armorBroke()) {
            /* Set properties of projectile and detach from parent*/
            Transform curBullet = Object.Instantiate(hitbox, myStatus.transform);
            curBullet.tag = assignHitboxTag(myStatus.tag);
            curBullet.GetComponent<ProjectileBehav>().setProperties(this, dirVect);
            curBullet.parent = null;

            /* Use ammo */
            useAmmo();

            /* Wait for next chance to fire */
            yield return new WaitForSeconds(fireRate); 

            /* Increment curShot counter */
            curShot++;
        }
    }

    /* Does damage to enemy */
    public override void enactEffects(EntityStatus tgt) {
        int damage = damageCalc(myStatus.getLevel(), power, myStatus, tgt, isPhy);
        tgt.applyDamage(damage);
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashMove : AmmoMove
{
    //Innate move variables
    private int power;
    private float dashForce;

    //Reference variables
    private EntityStatus status;
    private Transform hitbox;

    //Variables for dash characteristics
    private float dashDuration;
    private bool hitTgt;

    //DashMove constructor
    public DashMove(EntityStatus es, int pwr, int numDashes, float regen, float kbForce, float duration) : base(numDashes, regen, true){
        status = es;
        power = pwr;
        dashForce = kbForce;
        dashDuration = duration;
        hitbox = status.transform.GetChild(1);
    }

    /* Allows player to shoot */
    public override IEnumerator executeMovePlayer(int hDir, int vDir) {
        //Get directional vector
        Vector3 dirVector = new Vector3(hDir, vDir, 0);
        dirVector.Normalize();

        //Set Dashbox characteristics
        hitbox.GetComponent<DashBoxBehav>().activateHitbox(assignHitboxTag(status.tag), this);

        //Set up dash and then exert force
        float timer = 0f;
        hitTgt = false;
        Rigidbody2D rb = status.GetComponent<Rigidbody2D>();
        rb.AddForce(dirVector * dashForce);
        useAmmo();

        //Calculate dash suration
        while(!hitTgt && timer < dashDuration && !status.armorBroke()) {
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
        }

        //If target not hit, stop the dash by setting velocity to 0
        if(!hitTgt) {
            rb.velocity = Vector3.zero;
            hitbox.GetComponent<DashBoxBehav>().deactivateHitbox();
        }
    }

    //Allows enemy to execute this move
    public override IEnumerator executeMoveEnemy(Transform tgt) {
        yield return 0;
    }

    //Allows player to execute this move as an assist move
    public override IEnumerator executeAssistMove(int hDir, int vDir) {
        yield return executeMovePlayer(hDir, vDir);
    }

    /* Does damage to enemy */
    public override void enactEffects(EntityStatus tgt) {
        //Deactivate hitbox
        status.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        hitbox.GetComponent<DashBoxBehav>().deactivateHitbox();
        hitTgt = true;

        //Apply damage
        int damage = damageCalc(status.getLevel(), power, status, tgt, true);
        bool enemyLived = tgt.applyDamage(damage);

        //Apply recoil forces
        status.StartCoroutine(applyRecoilForces(enemyLived, tgt));
    }

    /* Apply recoil forces */
    private const float RECOIL_DURATION = 0.1f;
    private const float RECOIL_FORCE = 250f;

    IEnumerator applyRecoilForces(bool enemyLived, EntityStatus tgt) {
        //Enact player recoil
        Vector3 playerRecoil = dirKnockbackCalc(tgt.transform.position, status.transform.position, RECOIL_FORCE);
        status.GetComponent<Rigidbody2D>().AddForce(playerRecoil);

        //Enact enemy recoil if necessary
        if (enemyLived) {
            Vector3 enemyRecoil = dirKnockbackCalc(status.transform.position, tgt.transform.position, RECOIL_FORCE);
            tgt.GetComponent<Rigidbody2D>().AddForce(enemyRecoil);
        }

        yield return new WaitForSeconds(RECOIL_DURATION);

        //Set all velocities to 0
        status.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        if (enemyLived)
            tgt.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }
}

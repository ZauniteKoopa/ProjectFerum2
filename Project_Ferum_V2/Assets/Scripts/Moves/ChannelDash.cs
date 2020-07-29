using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelDash : CooldownMove
{
    //Innate power variables
    private int minPower;
    private int maxPower;
    private int minPriority;
    private int maxPriority;
    private const float MIN_DASH = 600f;
    private const float MAX_DASH = 950f;

    private const float KB_DURATION = 0.08f;
    private const float DASH_DURATION = 0.4f;
    private const float MIN_KB = 500f;
    private const float MAX_KB = 900f;

    //Channel variables
    private float maxChannel;
    private float curChannel;

    //Reference variables
    private EntityStatus status;
    private Transform hitbox;

    //Variables for channelDash to use
    private bool hitTgt;

    //Constructor variable
    public ChannelDash(EntityStatus es, float cd, int maxPwr, int minPwr, int maxPrio, int minPrio, float maxC) : base(cd, true) {
        status = es;

        hitbox = status.transform.GetChild(1);
        maxPower = maxPwr;
        minPower = minPwr;
        minPriority = minPrio;
        maxPriority = maxPrio;
        maxChannel = maxC;
    }

    /* IEnumerator that allows execution of mainFighter */
    public override IEnumerator executeMovePlayer(int tempHDir, int tempVDir) {
        //Set up channel
        curChannel = 0f;
        string input = getInputKey();
        int hDir = tempHDir;
        int vDir = tempVDir;
        Color prevColor = status.GetComponent<SpriteRenderer>().color;

        status.setUnflinching(true);
        status.setChannelActive(true);
        status.GetComponent<SpriteRenderer>().color = Color.magenta;

        //Channel loop
        while (!status.armorBroke() && status.getHealth() > 0 && Input.GetKey(input)) {
            yield return new WaitForFixedUpdate();

            /* Update channel and associated UI */
            curChannel += Time.deltaTime;
            status.setChannelProgress(curChannel, maxChannel);

            /* Update movements */
            int prevHDir = hDir;
            int prevVDir = vDir;

            /* Vertical movement */
            if (Input.GetKey(ControlMap.MOVE_UP)) {
                vDir = 1;
            } else if (Input.GetKey(ControlMap.MOVE_DOWN)) {
                vDir = -1;
            } else {
                vDir = 0;
            }

            /* Horizontal movement */
            if (Input.GetKey(ControlMap.MOVE_RIGHT)) {
                hDir = 1;
            } else if (Input.GetKey(ControlMap.MOVE_LEFT)) {
                hDir = -1;
            } else {
                hDir = 0;
            }

            /* If player didn't move, go back to previous dir values */
            if(hDir == 0 && hDir == vDir) {
                hDir = prevHDir;
                vDir = prevVDir;
            }
        }

        status.setChannelActive(false);
        status.GetComponent<SpriteRenderer>().color = prevColor;

        if (!status.armorBroke() && status.getHealth() > 0)
            yield return executeDash(hDir, vDir);

        startCDTimer();
        status.setUnflinching(false);
    }

    /* IEnumerator to allow enemy to move */
    public override IEnumerator executeMoveEnemy(Transform tgt) {
        yield return 0;
    }

    /* IEnumerator to allow assist move execution */
    public override IEnumerator executeAssistMove(int hDir, int vDir) {
        //Set up channel
        curChannel = 0f;
        string input = getAssistInputKey();
        Color prevColor = status.GetComponent<SpriteRenderer>().color;

        status.setUnflinching(true);
        status.setChannelActive(true);
        status.GetComponent<SpriteRenderer>().color = Color.magenta;

        //Channel loop
        while (!status.armorBroke() && status.getHealth() > 0 && Input.GetKey(input)) {
            yield return new WaitForFixedUpdate();

            /* Update channel and associated UI */
            curChannel += Time.deltaTime;
            status.setChannelProgress(curChannel, maxChannel);
        }

        status.setChannelActive(false);
        status.GetComponent<SpriteRenderer>().color = prevColor;

        if (!status.armorBroke() && status.getHealth() > 0)
            yield return executeDash(hDir, vDir);
        
        startCDTimer();
        status.setUnflinching(false);
    }


    /* Helper IEnumerator that allows dashing */
    IEnumerator executeDash(int hDir, int vDir) {
        /* Calculate values concerning channelPercent */
        float channelPercent = (curChannel / maxChannel > 1f) ? 1f : curChannel / maxChannel;
        float dashForce = MIN_DASH + (MAX_DASH - MIN_DASH) * channelPercent;
        float fPriority = (float)minPriority + ((float)(maxPriority - minPriority) * channelPercent);
        int priority = Mathf.RoundToInt(fPriority);

        //Get directional vector
        Vector3 dirVector = new Vector3(hDir, vDir, 0);
        dirVector.Normalize();

        //Set Dashbox characteristics
        hitbox.GetComponent<DashBoxBehav>().activateHitbox(assignHitboxTag(status.tag), this, priority);

        //Set up dash and then exert force
        float timer = 0f;
        hitTgt = false;
        Rigidbody2D rb = status.GetComponent<Rigidbody2D>();
        rb.AddForce(dirVector * dashForce);

        //Calculate dash duration
        while(!hitTgt && timer < DASH_DURATION && !status.armorBroke()) {
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
        }

        //If target not hit, stop the dash by setting velocity to 0
        if(!hitTgt) {
            rb.velocity = Vector3.zero;
            hitbox.GetComponent<DashBoxBehav>().deactivateHitbox();
        }
    }

    /* Enact effects method */
    public override void enactEffects(EntityStatus tgt) {
        //Deactivate hitbox
        status.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        hitbox.GetComponent<DashBoxBehav>().deactivateHitbox();
        hitTgt = true;

        //Apply damage
        if (tgt != null) {
            float channelPercent = (curChannel / maxChannel >= 1f) ? 1f : curChannel / maxChannel;
            float fPower = (float)minPower + ((float)(maxPower - minPower) * channelPercent);
            int power = Mathf.RoundToInt(fPower);

            int damage = damageCalc(status.getLevel(), power, status, tgt, true);
            bool enemyLived = tgt.applyDamage(damage);

            //Apply player recoil forces
            Vector3 playerRecoil = dirKnockbackCalc(tgt.transform.position, status.transform.position, MIN_KB);
            status.StartCoroutine(status.receiveKnockback(playerRecoil, KB_DURATION));

            //Apply enemy recoil forces IF enemy survived
            if (enemyLived) {
                float recoilForce = MIN_KB + channelPercent * (MAX_KB - MIN_KB);
                Vector3 enemyRecoil = dirKnockbackCalc(status.transform.position, tgt.transform.position, recoilForce);
                status.StartCoroutine(tgt.receiveKnockback(enemyRecoil, KB_DURATION));
            }
        }
    }
}

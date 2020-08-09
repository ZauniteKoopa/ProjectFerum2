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
    private const float MIN_DASH = 400f;
    private const float MAX_DASH = 800f;

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
    public override IEnumerator executeMovePlayer() {
        //Set up channel
        curChannel = 0f;
        int mouseInput = getMouseInputKey();
        Color prevColor = status.GetComponent<SpriteRenderer>().color;

        status.setUnflinching(true);
        status.setChannelActive(true);
        status.GetComponent<SpriteRenderer>().color = Color.magenta;

        //Channel loop
        while (!status.armorBroke() && status.getHealth() > 0 && Input.GetMouseButton(mouseInput)) {
            yield return new WaitForFixedUpdate();

            /* Update channel and associated UI */
            curChannel += Time.deltaTime;
            status.setChannelProgress(curChannel, maxChannel);
        }

        status.setChannelActive(false);
        status.GetComponent<SpriteRenderer>().color = prevColor;

        if (!status.armorBroke() && status.getHealth() > 0) {
            Vector3 dirVector = getVectorToMouse(status.transform);
            yield return executeDash(dirVector);
        }

        startCDTimer();
        status.setUnflinching(false);
    }

    /* IEnumerator to allow enemy to move */
    public override IEnumerator executeMoveEnemy(Transform tgt) {
        //Set up channel
        curChannel = 0f;
        Color prevColor = status.GetComponent<SpriteRenderer>().color;

        status.setUnflinching(true);
        status.setChannelActive(true);
        status.GetComponent<SpriteRenderer>().color = Color.magenta;

        //Channel loop
        while (!status.armorBroke() && status.getHealth() > 0 && curChannel < maxChannel) {
            yield return new WaitForFixedUpdate();

            /* Update channel and associated UI */
            curChannel += Time.deltaTime;
            status.setChannelProgress(curChannel, maxChannel);
        }

        status.setChannelActive(false);
        status.GetComponent<SpriteRenderer>().color = prevColor;

        if (!status.armorBroke() && status.getHealth() > 0) {
            Vector3 dirVector = tgt.position - status.transform.position;
            yield return executeDash(dirVector);
        }

        status.setUnflinching(false);

    }

    /* IEnumerator to allow assist move execution */
    public override IEnumerator executeAssistMove() {
        //Set up channel
        curChannel = 0f;
        string input = getAssistInputKey();
        Color prevColor = status.GetComponent<SpriteRenderer>().color;

        status.setUnflinching(true);
        status.setChannelActive(true);
        status.GetComponent<SpriteRenderer>().color = Color.magenta;
        Vector3 dirVector = getVectorToMouse(status.transform);

        //Channel loop
        while (!status.armorBroke() && status.getHealth() > 0 && !Input.GetKeyDown(input) && curChannel < maxChannel) {
            yield return new WaitForFixedUpdate();

            /* Update channel and associated UI */
            curChannel += Time.deltaTime;
            status.setChannelProgress(curChannel, maxChannel);
        }

        status.setChannelActive(false);
        status.GetComponent<SpriteRenderer>().color = prevColor;

        if (!status.armorBroke() && status.getHealth() > 0) {
            yield return executeDash(dirVector);
        }
        
        startCDTimer();
        status.setUnflinching(false);
    }


    /* Helper IEnumerator that allows dashing */
    IEnumerator executeDash(Vector3 dirVector) {
        /* Calculate values concerning channelPercent */
        float channelPercent = (curChannel / maxChannel > 1f) ? 1f : curChannel / maxChannel;
        float dashForce = MIN_DASH + (MAX_DASH - MIN_DASH) * channelPercent;
        float fPriority = (float)minPriority + ((float)(maxPriority - minPriority) * channelPercent);
        int priority = Mathf.RoundToInt(fPriority);

        //Get directional vector
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

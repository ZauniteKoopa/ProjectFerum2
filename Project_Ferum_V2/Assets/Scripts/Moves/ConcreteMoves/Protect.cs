using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protect : CooldownMove
{
    //Reference variables
    private EntityStatus status;
    private Transform hitbox;

    //Innate constant variables
    private const float PROTECT_DURATION = 2.5f;
    private const float PROTECT_COOLDOWN = 16f;
    private const int PROTECT_PRIORITY = 11;

    //Protect Constructor
    public Protect(EntityStatus es) : base(PROTECT_COOLDOWN, false){
        status = es;
        hitbox = status.transform.GetChild(1);
    }

    //Allows player to run move
    public override IEnumerator executeMovePlayer() {        
        //Get color info
        SpriteRenderer render = status.GetComponent<SpriteRenderer>();
        Color prevColor = render.color;
        render.color = Color.gray;

        //Set up loop and begin duration
        float timer = 0f;
        status.setInvincibility(true);
        hitbox.GetComponent<DashBoxBehav>().activateHitbox(assignHitboxTag(status.tag), this, PROTECT_PRIORITY);

        //Channel UI
        status.setChannelActive(true);

        while (timer < PROTECT_DURATION && Input.GetMouseButton(0)) {
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
            status.setChannelProgress(PROTECT_DURATION - timer, PROTECT_DURATION);
        }

        //Channel UI disable
        status.setChannelActive(false);

         //Set invincibility to false
        hitbox.GetComponent<DashBoxBehav>().deactivateHitbox();
        status.setInvincibility(false);
        render.color = prevColor;
        startCDTimer();
    }

    //Allows enemy to move
    public override IEnumerator executeMoveEnemy (Transform tgt) {        
        //Get color info
        SpriteRenderer render = status.GetComponent<SpriteRenderer>();
        Color prevColor = render.color;
        render.color = Color.gray;

        //Set up loop and begin duration
        float timer = 0f;
        status.setInvincibility(true);
        hitbox.GetComponent<DashBoxBehav>().activateHitbox(assignHitboxTag(status.tag), this, PROTECT_PRIORITY);

        //Channel UI
        status.setChannelActive(true);

        while (timer < PROTECT_DURATION) {
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
            status.setChannelProgress(PROTECT_DURATION - timer, PROTECT_DURATION);
        }

        //Channel UI disable
        status.setChannelActive(false);

         //Set invincibility to false
        hitbox.GetComponent<DashBoxBehav>().deactivateHitbox();
        status.setInvincibility(false);
        render.color = prevColor;
    }

    //Allow player to use move as an assist
    public override IEnumerator executeAssistMove() {
        yield return executeMoveEnemy(null);
        startCDTimer();
    }

    //Obligatory enactEffects method
    public override void enactEffects(EntityStatus tgt) {}
}

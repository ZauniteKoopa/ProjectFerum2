using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protect : CooldownMove
{
    //Reference variables
    private EntityStatus status;

    //Innate constant variables
    private const float PROTECT_DURATION = 2.5f;
    private const float PROTECT_COOLDOWN = 16f;

    //Protect Constructor
    public Protect(EntityStatus es) : base(PROTECT_COOLDOWN, false){
        status = es;
    }

    //Allows player to run move
    public override IEnumerator executeMovePlayer(int hDir, int vDir) {
        //Get input
        string input = getInputKey();
        
        //Get color info
        SpriteRenderer render = status.GetComponent<SpriteRenderer>();
        Color prevColor = render.color;
        render.color = Color.gray;

        //Set up loop and begin duration
        float timer = 0f;
        status.setInvincibility(true);

        while (timer < PROTECT_DURATION && Input.GetKey(input)) {
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
        }

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

        while (timer < PROTECT_DURATION) {
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
        }

        status.setInvincibility(false);
        render.color = prevColor;
    }

    //Allow player to use move as an assist
    public override IEnumerator executeAssistMove(int hDir, int vDir) {
        yield return executeMoveEnemy(null);
        startCDTimer();
    }

    //Obligatory enactEffects method
    public override void enactEffects(EntityStatus tgt) {}
}

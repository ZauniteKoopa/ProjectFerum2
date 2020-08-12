using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Static channel move where 
public abstract class ChannelMove : CooldownMove
{
    //Variables concerning channel time
    private float maxChannel;
    private float curChannel;

    //Reference variables
    private EntityStatus status;

    //Constants for progress decay
    private const float DECAY_RATE = 0.85f;
    private const float DECAY_PER_RATE = 0.2f;
    private const float INITIAL_DECAY = 0.15f;
    private float decayTimer;
    private bool charging;


    //Public constructor of ChannelMove
    public ChannelMove(EntityStatus es, float cooldown, float channelTime) : base(cooldown, true) {
        maxChannel = channelTime;
        status = es;
        curChannel = 0f;
        decayTimer = 0f;
        charging = false;
    }

    //Method to do decay 
    public override void regen() {
        //Regenerate cooldown
        base.regen();

        //Do decay
        if (!charging && curChannel > 0f) {
            decayTimer += Time.deltaTime;

            if (decayTimer >= DECAY_RATE) {
                curChannel -= DECAY_PER_RATE;

                //In case we hit rock bottom
                if (curChannel < 0f)
                    curChannel = 0f;
                
                decayTimer = 0f;
            }
        }
    }

    //Method to execute as a main fighter for player
    public override IEnumerator executeMovePlayer() {
        Color prevColor = status.GetComponent<SpriteRenderer>().color;
        status.GetComponent<SpriteRenderer>().color = Color.magenta;
        status.setChannelActive(true);
        charging = true;
        status.setChannelProgress(curChannel, maxChannel);
        int mouseInput = getMouseInputKey();
        bool cancelled = false;

        //Channel
        while (!status.armorBroke() && status.getHealth() > 0 && Input.GetMouseButton(mouseInput) && curChannel < maxChannel && !cancelled) {
            yield return new WaitForFixedUpdate();

            curChannel += Time.deltaTime;
            status.setChannelProgress(curChannel, maxChannel);
            cancelled = Input.GetMouseButton((mouseInput + 1) % 2);
        }

        //Dismantle channeling stage
        status.GetComponent<SpriteRenderer>().color = prevColor;
        status.setChannelActive(false);
        charging = false;

        //Check for success case
        if (curChannel >= maxChannel) {
            curChannel = 0f;
            executeFinishedChannel();
            startCDTimer();
        } else {
            curChannel -= (curChannel * INITIAL_DECAY);
        }

        if (cancelled)
            status.cancelMove((mouseInput + 1) % 2);
    }

    // Method to execute as an enemy
    public override IEnumerator executeMoveEnemy(Transform tgt) {
        Color prevColor = status.GetComponent<SpriteRenderer>().color;
        status.GetComponent<SpriteRenderer>().color = Color.magenta;
        status.setChannelActive(true);
        charging = true;
        status.setChannelProgress(curChannel, maxChannel);

        //Channel
        while (!status.armorBroke() && status.getHealth() > 0 && curChannel < maxChannel) {
            yield return new WaitForFixedUpdate();

            curChannel += Time.deltaTime;
            status.setChannelProgress(curChannel, maxChannel);
        }

        //Dismantle channeling stage
        status.GetComponent<SpriteRenderer>().color = prevColor;
        status.setChannelActive(false);
        charging = false;

        //Check for success case
        if (curChannel >= maxChannel) {
            curChannel = 0f;
            executeFinishedChannel();
            startCDTimer();
        }
    }

    //Method to execute move as an assist fighter for player
    public override IEnumerator executeAssistMove() {
        string input = getAssistInputKey();
        Color prevColor = status.GetComponent<SpriteRenderer>().color;
        status.GetComponent<SpriteRenderer>().color = Color.magenta;
        status.setChannelActive(true);
        charging = true;
        status.setChannelProgress(curChannel, maxChannel);

        //Channel
        while (!status.armorBroke() && status.getHealth() > 0 && !Input.GetKeyDown(input) && curChannel < maxChannel) {
            yield return new WaitForFixedUpdate();

            curChannel += Time.deltaTime;
            status.setChannelProgress(curChannel, maxChannel);
        }

        //Dismantle channeling stage
        status.GetComponent<SpriteRenderer>().color = prevColor;
        status.setChannelActive(false);
        charging = false;

        //Check for success case
        if (curChannel >= maxChannel) {
            curChannel = 0f;
            executeFinishedChannel();
            startCDTimer();
        }
    }

    //Method to execute finished channel (action you do when channel == 100%)
    public abstract void executeFinishedChannel();
}

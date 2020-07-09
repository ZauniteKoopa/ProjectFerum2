using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CooldownMove : IMove
{
    /* Cooldown timer variables */
    private float maxCooldown;
    private float cdTimer;

    /* Flag that tells you if it can run or not */
    private bool isReady;

    /* Abstract Class constructor */
    public CooldownMove(float MAX_CD, bool moveDisabled) : base(moveDisabled)
    {
        maxCooldown = MAX_CD;
        cdTimer = 0f;
        isReady = true;
    }

    /* Overriden regen method: if move is not ready, update cdTimer */
    public override void regen() {
        if (!isReady) {
            cdTimer += Time.deltaTime;

            if (cdTimer >= maxCooldown) {
                isReady = true;
            }
        }
    }

    /* Overriden accessor method: checks if the move is ready to be used */
    public override bool canRun() {
        return isReady;
    }

    /* Class specific method to start CD timer
        Pre: entity has started using the move
        Post: timer will start by setting isReady to false */
    protected void startCDTimer() {
        cdTimer = 0f;
        isReady = false;
    }

}

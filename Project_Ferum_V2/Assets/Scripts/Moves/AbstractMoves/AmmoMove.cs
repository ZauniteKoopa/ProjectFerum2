using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AmmoMove : IMove
{
    /* Ammo supply */
    private int curAmmo;
    private int maxAmmo;
    
    /* Ammo regen rate (every X seconds) */
    private float regenRate;
    private float regenTimer;
    private const float FULL_RECHARGE_FACTOR = 2f;

    /* Ammo move constructor */
    public AmmoMove(int MAX_AMMO, float REGEN_RATE, bool moveDisabled) : base(moveDisabled) {
        maxAmmo = MAX_AMMO;
        curAmmo = MAX_AMMO;

        regenRate = REGEN_RATE;
        regenTimer = 0f;
    }

    /* Ammo regeneration */
    public override void regen() {
        if (curAmmo < maxAmmo) {
            regenTimer += Time.deltaTime;

            /* Regen timer */
            if (curAmmo == 0 && regenTimer >= regenRate * FULL_RECHARGE_FACTOR) {
                curAmmo = (maxAmmo * 3) / 5;         //Will refill 60% of ammo gained
                regenTimer = 0f;

            } else if (curAmmo > 0 && regenTimer >= regenRate) {
                curAmmo = (curAmmo == maxAmmo) ? maxAmmo : curAmmo + 1;
                regenTimer = 0f;
            }
        }
    }

    /* Checks if move can run: sees if we actually have ammo */
    public override bool canRun() {
        return curAmmo > 0;
    }

    /* Method to update UI */
    public override void updateMoveUI(UIAbility icon) {
        float fill = (curAmmo == 0) ? regenTimer / (FULL_RECHARGE_FACTOR * regenRate) : 1f;
        icon.setCooldownUI(fill);
        icon.setAmmo(curAmmo);
    }

    /* Method to set up UI */
    public override void setUpUI(UIAbility icon) {
        float fill = (curAmmo == 0) ? regenTimer / (FULL_RECHARGE_FACTOR * regenRate) : 1f;
        icon.setCooldownUI(fill);
        icon.setAmmo(curAmmo);
    }

    /* Use one ammo */
    protected void useAmmo() {
        curAmmo--;

        if (curAmmo == 0f) {
            regenTimer = 0f;
        }
        //Debug.Log("Ammo Used! Ammo left: " + curAmmo);
    }

}

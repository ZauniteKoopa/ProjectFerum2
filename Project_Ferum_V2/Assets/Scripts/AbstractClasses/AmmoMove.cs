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

            /* Increment current ammo count by one once passed regen rate */
            if (regenTimer >= regenRate) {
                curAmmo = (curAmmo == maxAmmo) ? maxAmmo : curAmmo + 1;
                regenTimer = 0f;
            }
        }
    }

    /* Checks if move can run: sees if we actually have ammo */
    public override bool canRun() {
        return curAmmo > 0;
    }

    protected void useAmmo() {
        curAmmo--;
        //Debug.Log("Ammo Used! Ammo left: " + curAmmo);
    }

}

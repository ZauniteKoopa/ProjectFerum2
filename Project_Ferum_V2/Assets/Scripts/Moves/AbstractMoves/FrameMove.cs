using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FrameMove : IMove
{
    /* Frame move constructor */
    public FrameMove(bool moveDisabled) : base(moveDisabled) {}

    /* Overriden regen method: does nothing */
    public override void regen()  {}

    /* Frame based move can always be run unless player is in middle of action */
    public override bool canRun() {
        return true;
    }

    /* Method to update UI */
    public override void updateMoveUI(UIAbility icon) {}

    /* Method to set up UI */
    public override void setUpUI(UIAbility icon) {
        icon.setCooldownUI(1f);
        icon.clearAmmo();
    }
}

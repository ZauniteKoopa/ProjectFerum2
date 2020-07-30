using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : TimedEffect
{
    //Public Constructor
    public StatusEffect(int statusType, float duration) : base(statusType, duration) {}

    //Method to apply effect
    public override float applyEffect(float curNum) {
        return curNum + 1;
    }

    //Method to reverse effect
    public override float reverseEffect(float curNum) {
        return curNum - 1;
    }

    //Method to check if this is a buff
    public override bool isBuff() {
        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatEffect : TimedEffect
{
    // Factor used to change stats (If value is 2.0f, double the associated stat)
    private float statChangeFactor;

    // Public constructor
    public StatEffect(int statID, float duration, float change) : base(statID, duration) {
        statChangeFactor = change;
    }

    //Method to apply stat effect
    public override float applyEffect(float curStatValue) {
        return curStatValue * statChangeFactor;
    }

    //Method to reverse stat effect
    public override float reverseEffect(float curStatValue) {
        return curStatValue * (1f / statChangeFactor);
    }

    //Method to check if this is a buff or not
    public override bool isBuff() {
        return statChangeFactor > 1f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimedEffect
{
    //Method that discerns what type of effect this is
    int effectType;
    
    //Methods that keep track of duration and time
    private float duration;
    private float curEndTime;

    //Public constructor
    public TimedEffect(int type, float dur) {
        effectType = type;
        duration = dur;
        curEndTime = 0f;
    }

    //Method to set an end time for this TimedEffect
    public void setEndTime(float startTime) {
        curEndTime = startTime + duration;
    }

    //Method to check if the end time has been reached by curTime
    public bool finished(float curTime) {
        return curEndTime <= curTime;
    }

    //Compares 2 Timed effects to each other.
    //  If this timedEffect finishes earlier than the other, return true
    public bool lessThan(TimedEffect other) {
        return curEndTime <= other.curEndTime;
    }

    //Accessor method to effect type
    public int getEffectType() {
        return effectType;
    }



    //------------------------
    //  Abstract methods for children to implement
    //------------------------

    // Method to apply the effect
    public abstract float applyEffect(float curStatValue);

    // Method to reverse the effect
    public abstract float reverseEffect(float curStatValue);

    //Method to check if this is a buff
    public abstract bool isBuff();

}

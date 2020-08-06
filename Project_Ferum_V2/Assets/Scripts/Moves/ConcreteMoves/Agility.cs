using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agility : StatEffectChannel
{
    //Move properties
    private const float COOLDOWN = 13.5f;
    private const float CHANNEL_TIME = 1f;

    //Stat effect properties
    private const int STAT_ID = (int)GeneralConstants.statIDs.SPEED;
    private const float EFFECT_DURATION = 6f;
    private const float EFFECT_FACTOR = 1.5f;

    //Move constructor
    public Agility(EntityStatus es) : base(es, COOLDOWN, CHANNEL_TIME, STAT_ID, EFFECT_DURATION, EFFECT_FACTOR) {}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainDance : ZonalEffectChannel
{
    //Parameter variables for Rain Dance
    private const float COOLDOWN = 18f;
    private const float CHANNEL_TIME = 1.25f;

    private const int STAT_TYPE = (int)GeneralConstants.statIDs.SPEED;
    private const float STAT_FACTOR = 0.65f;
    private const float ZONE_DURATION = 6f;
    

    //Constructor
    public RainDance(EntityStatus es) : base(es, COOLDOWN, CHANNEL_TIME, STAT_TYPE, STAT_FACTOR, ZONE_DURATION, "MoveHitboxes/EffectZone") {}
}

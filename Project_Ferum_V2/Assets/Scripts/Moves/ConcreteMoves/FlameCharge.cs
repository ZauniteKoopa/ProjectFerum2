using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameCharge : ChannelDash
{
    //Move properties
    private const float COOLDOWN = 10.5f;

    private const int MAX_PWR = 75;
    private const int MIN_PWR = 40;
    private const int MAX_PRIO = 8;
    private const int MIN_PRIO = 4;

    private const float MAX_CHANNEL_DURATION = 1.5f;

    //Move constructor
    public FlameCharge(EntityStatus es) : base(es, COOLDOWN, MAX_PWR, MIN_PWR, MAX_PRIO, MIN_PRIO, MAX_CHANNEL_DURATION) {}
}

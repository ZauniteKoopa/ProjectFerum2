using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameBurst : BulletMove
{
    //Move parameters
    private const int PWR = 18;
    private const int MAX_AMMO = 5;
    private const float REGEN_RATE = 5f;
    private const float FIRE_RATE = 0.7f;
    private const bool IS_PHY = false;

    //Constructor
    public FlameBurst(EntityStatus es) : base(es, PWR, MAX_AMMO, REGEN_RATE, FIRE_RATE, IS_PHY, "MoveHitboxes/BurstProj") {}
}

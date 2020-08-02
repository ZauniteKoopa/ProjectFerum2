using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameBurst : BulletMove
{
    //Move parameters
    private const int PWR = 20;
    private const int MAX_AMMO = 12;
    private const float REGEN_RATE = 2.25f;
    private const float FIRE_RATE = 0.5f;
    private const bool IS_PHY = false;

    //Constructor
    public FlameBurst(EntityStatus es) : base(es, PWR, MAX_AMMO, REGEN_RATE, FIRE_RATE, IS_PHY, "MoveHitboxes/BurstProj") {}
}

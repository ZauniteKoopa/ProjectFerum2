using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSeed : BulletMove
{
    //Move parameters
    private const int PWR = 12;
    private const int MAX_AMMO = 20;
    private const float REGEN_RATE = 3f;
    private const float FIRE_RATE = 0.3f;
    private const bool IS_PHY = true;

    //Constructor
    public BulletSeed(EntityStatus es) : base(es, PWR, MAX_AMMO, REGEN_RATE, FIRE_RATE, IS_PHY, "MoveHitboxes/Bullet") {}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameBurst : SingleProjCD
{
    //Move parameters
    private const int PWR = 40;
    private const float KB = 250f;
    private const float COOLDOWN = 3.5f;

    //Constructor
    public FlameBurst(EntityStatus es) : base(es, PWR, KB, COOLDOWN, "MoveHitboxes/BurstProj") {}
}

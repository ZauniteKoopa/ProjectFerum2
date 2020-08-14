using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPulse : SingleProjCD
{
    //Move properties
    private const int PWR = 40;
    private const float KB = 400f;
    private const float COOLDOWN = 7f;

    //Move constructor
    public WaterPulse(EntityStatus es) : base(es, PWR, KB, COOLDOWN, "MoveHitboxes/WaterPulse") {}
}

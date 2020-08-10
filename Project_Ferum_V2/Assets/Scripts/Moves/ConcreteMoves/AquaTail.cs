using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AquaTail : CircleAoE
{
    //Constants for this move
    private const float COOLDOWN = 0.8f;
    private const bool MOVE_DISABLED = false;

    private const int PWR = 55;
    private const float KB = 400f;
    private const bool IS_PHY = true;

    //Constructor
    public AquaTail(EntityStatus es) : base(COOLDOWN, MOVE_DISABLED, PWR, KB, IS_PHY, "MoveHitboxes/AquaTailHitbox", es) {}
}

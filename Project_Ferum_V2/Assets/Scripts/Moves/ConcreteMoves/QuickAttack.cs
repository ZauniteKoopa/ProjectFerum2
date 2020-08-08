using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickAttack : DashMove
{
    //Move properties
    private const int PWR = 15;
    private const int NUM_DASHES = 3;
    private const float DASH_REGEN = 2.5f;
    private const float KB = 650f;
    private const float DASH_DUR = 0.15f;
    private const int PRIO = 2;

    //Constructor
    public QuickAttack(EntityStatus es) : base(es, PWR, NUM_DASHES, DASH_REGEN, KB, DASH_DUR, PRIO) {}

}

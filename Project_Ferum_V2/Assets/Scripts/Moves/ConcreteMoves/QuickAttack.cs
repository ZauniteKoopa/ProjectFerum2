﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickAttack : DashMove
{
    //Move properties
    private const int PWR = 20;
    private const int NUM_DASHES = 5;
    private const float DASH_REGEN = 1.75f;
    private const float KB = 650f;
    private const float DASH_DUR = 0.15f;
    private const int PRIO = 2;

    //Constructor
    public QuickAttack(EntityStatus es) : base(es, PWR, NUM_DASHES, DASH_REGEN, KB, DASH_DUR, PRIO) {}

}

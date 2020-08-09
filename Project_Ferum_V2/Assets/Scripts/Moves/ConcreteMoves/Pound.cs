using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pound : BasicMeleeAttack
{
    //Move parameters
    private const int PWR = 35;
    private const float KB_FORCE = 300f;
    private const int PRIO = 4;

    //Constructor
    public Pound(EntityStatus es) : base(PWR, KB_FORCE, es, PRIO) {}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Hashset manager for collisions hit for a move that has multiple hitboxes
public interface MultiPhaseHitbox
{
    //Method for splash hitbox to use to enact damage
    void applySplashDamage(Collider2D tgt);
}

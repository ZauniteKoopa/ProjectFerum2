using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryHitbox : MonoBehaviour
{
    //Reference variable to main hitbox script
    [SerializeField]
    private BurstProjBehav mainHB = null;

    //On collision with anything, contact mainHB to apply splash damage
    void OnTriggerEnter2D(Collider2D tgt) {
        mainHB.applySplashDamage(tgt);
    }

}

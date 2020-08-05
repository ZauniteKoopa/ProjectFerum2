using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PulseBehav : BurstProjBehav
{
    //Health variables
    [SerializeField]
    private float maxHealth = 0;
    private float curHealth;

    //Invulnerability variables
    private bool invincible = false;
    private const float INVINCIBILITY_TIME = 0.1f;

    //UI variables
    [SerializeField]
    private Image healthBar = null;

    //On awake set up health
    void Awake() {
        curHealth = maxHealth;
        healthBar.fillAmount = 1f;
    }

    //On collision with an enemy or attack
    void OnTriggerEnter2D(Collider2D tgt) {
        /* Behavior when an enemy is hit */
        if(enemyHit(tgt.tag)) {
            attkInitialEnemy(tgt);
            triggerSplashHitbox();
        }

        /* Behavior when wall is hit */
        // if(tgt.tag == GeneralConstants.WALL_TAG) {
        //     triggerSplashHitbox();
        // }

        /* Behavior when hit by any attack */
        if (hitAttackMutual(tgt.tag) && !invincible) {
            if (overpoweredBy(tgt.GetComponent<Hitbox>())) {
                triggerSplashHitbox();
            } else {
                StartCoroutine(receiveDamage());
            }
        }
    }

    //Initial projectile receives damage
    IEnumerator receiveDamage() {
        curHealth--;
        healthBar.fillAmount = curHealth / maxHealth;

        //If alive, activate invincibility. Else, trigger splash box
        if (curHealth > 0) {
            invincible = true;
            yield return new WaitForSeconds(INVINCIBILITY_TIME);
            invincible = false;
        } else {
            triggerSplashHitbox();
        }
    }
}

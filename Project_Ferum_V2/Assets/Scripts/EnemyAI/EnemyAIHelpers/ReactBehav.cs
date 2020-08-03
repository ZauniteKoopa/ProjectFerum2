using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Allows enemies to react to player and player attacks
public class ReactBehav : MonoBehaviour
{
    //Timer variables
    [SerializeField]
    private float reactionTiming = 0f;
    private float rTimer = 0f;
    private bool canReact = true;

    //Enemy that this reactor belongs to
    [SerializeField]
    private AbstractEnemy boss = null;

    // On trigger: have enemy react
    void OnTriggerEnter2D(Collider2D tgt) {
        if (canReact) {
            if (tgt.tag == GeneralConstants.PLAYER_TAG || tgt.tag == GeneralConstants.PLAYER_ATTK_TAG) {
                boss.react(tgt);
                canReact = false;
            }
        }
    }

    //Updates rTimer that will enable enemy to react
    void FixedUpdate() {
        if (!canReact) {
            rTimer += Time.deltaTime;

            if (rTimer >= reactionTiming) {
                rTimer = 0f;
                canReact = true;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TgtCollector : MonoBehaviour
{
    [SerializeField]
    private AbstractEnemy enemy = null;
    private bool foundTgt = false;

    //OnTriggerEnter: If a player enters sensor zone, set that player as enemy's tgt
    void OnTriggerEnter2D(Collider2D tgt) {
        if (!foundTgt && tgt.tag == GeneralConstants.PLAYER_TAG) {
            enemy.setTgt(tgt.transform);
            foundTgt = true;
        }
    }
}

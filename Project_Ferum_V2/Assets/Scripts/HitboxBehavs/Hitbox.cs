using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    IMove myMove = null;

    /* Sets HitBox's move so it knows where to send a message to upon hitting tgt */
    public void setMove(IMove newMove) {
        myMove = newMove;
    }

    /* Collision method with enemy */
    void OnTriggerEnter2D() {
        
    }

}

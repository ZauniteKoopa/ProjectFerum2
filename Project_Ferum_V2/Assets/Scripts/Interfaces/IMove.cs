using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Public interface that is used for ALL moves / abilities that a player or enemy uses */
public interface IMove
{
    /* Method that checks whether or not a move can be run or not */
    bool canRun();

    /* Method used to execute a move from the player's perspective */
    void executeMovePlayer(int hDir, int vDir);

    /* Method used to regenerate cooldowns / ammo (not used in melee attacks) */
    void regen();
}

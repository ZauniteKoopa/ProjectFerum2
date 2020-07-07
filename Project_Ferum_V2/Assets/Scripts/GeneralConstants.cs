using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralConstants
{
    /* General tags */
    public const string PLAYER_TAG = "Player";
    public const string ENEMY_TAG = "Enemy";
    public const string PLAYER_ATTK_TAG = "PlayerAttack";
    public const string ENEMY_ATTK_TAG = "EnemyAttack";

    /* Stat IDs */
    public enum statIDs
    {
        ATTACK,
        DEFENSE,
        SP_ATTACK,
        SP_DEFENSE,
        SPEED
    }
}

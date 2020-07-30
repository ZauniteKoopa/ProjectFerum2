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
    public const string WALL_TAG = "Wall";

    /* Stat IDs */
    public enum statIDs
    {
        ATTACK,
        DEFENSE,
        SP_ATTACK,
        SP_DEFENSE,
        SPEED,
        NUM_STATS
    }

    //Effect IDs
    public const int POISON_ID = 5;
    public const int PARALYSIS_ID = 6;
    public const int BURN_ID = 7;
    public const int NUM_EFFECTS = 8;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStatus : MonoBehaviour
{
    /* Level */
    [SerializeField]
    private int lvl = 0;
    [Space(20)]

    /* Stats */
    [SerializeField]
    private int attk = 0;
    [SerializeField]
    private int def = 0;
    [SerializeField]
    private int sAttk = 0;
    [SerializeField]
    private int sDef = 0;
    [SerializeField]
    private int speed = 0;
    [Space(20)]

    /* Health */
    [SerializeField]
    private float maxHealth = 0f;
    [SerializeField]
    private float curHealth = 0f;
    [Space(20)]

    /* Armor */
    [SerializeField]
    private float maxArmor = 0f;
    [SerializeField]
    private float curArmor = 0f;

    /* Stat effects - to be added */

    /* Accessor method to level */
    public int getLevel() {
        return lvl;
    }

    /* Accessor methods to stats */
    public int getStat(int statID) {
        switch(statID)
        {
            case (int)GeneralConstants.statIDs.ATTACK:
                return attk;
            case (int)GeneralConstants.statIDs.DEFENSE:
                return def;
            case (int)GeneralConstants.statIDs.SP_ATTACK:
                return sAttk;
            case (int)GeneralConstants.statIDs.SP_DEFENSE:
                return sDef;
            case (int)GeneralConstants.statIDs.SPEED:
                return speed;
            default:
                throw new System.Exception("Error: Invalid stat ID");
        }
    }

    /* Accessor methods to health % */
    public float getHealthPercent() {
        return (float)curHealth / (float)maxHealth;
    }

    /* Accessor method to armor percent */
    public float getArmorPercent() {
        return (float)curArmor / (float)maxArmor;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* Method used to apply damage to this entity
        Pre: this entity has been hit by a hostile hitbox
        Post: this entity will now receive damage. If entity dies from move, return true. False otherwise */
    public bool applyDamage(int damage) {
        curHealth -= damage;
        curArmor -= (damage * 5) / 4;

        if (curHealth <= 0) {               //Case where this entity dies from this move
            return true;
        }else{                              //Case where entity still lives  
            return false;
        }
    }
}

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
    private float curHealth = 0f;
    [Space(20)]

    /* Armor */
    [SerializeField]
    private float maxArmor = 0f;
    private float curArmor = 0f;

    /* Stat effects - to be added */


    /* Flags */
    private bool invincibility;

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
    void Awake()
    {
        curHealth = maxHealth;
        curArmor = maxArmor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* Method used to apply damage to this entity
        Pre: this entity has been hit by a hostile hitbox
        Post: this entity will now receive damage. If entity dies from move or is invincible, return true. False otherwise */
    public bool applyDamage(int damage) {
        if (!invincibility) {
            StartCoroutine(invincibilityFrames());

            curHealth -= damage;
            curArmor -= (damage * 5) / 4;
            Debug.Log(curHealth);

            if (curHealth <= 0) {               //Case where this entity dies from this move
                gameObject.SetActive(false);
                return true;
            }else{                              //Case where entity still lives  
                return false;
            }
        }

        return true;
    }

    /* Allow for small invincibility period upon getting hit */
    private float INVINCIBILITY_DURATION = 0.1f;

    IEnumerator invincibilityFrames() {
        invincibility = true;
        yield return new WaitForSeconds(INVINCIBILITY_DURATION);
        invincibility = false;
    }
}

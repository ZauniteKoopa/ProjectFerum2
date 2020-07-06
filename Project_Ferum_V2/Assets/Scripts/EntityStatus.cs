using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStatus : MonoBehaviour
{
    /* Stats */
    [SerializeField]
    private int attk, def, sAttk, sDef, speed;
    [Space(20)]

    /* Resources */
    [SerializeField]
    private float maxHealth, maxArmor;
    private float curHealth;
    private float curArmor;

    /* Stat effects - to be added */

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

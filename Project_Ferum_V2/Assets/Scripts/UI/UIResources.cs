﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Class just filled with public variables to update
public class UIResources : MonoBehaviour
{
    //Variables that keep track of healthBar and armorBars
    [SerializeField]
    public Image healthBar, armorBar;

    [SerializeField]
    private Image characterIcon = null;

    //An array of 3 abiities
    public UIAbility[] abilities = new UIAbility[3];

    //Set UI when entity dies
    public void setDead() {
        healthBar.fillAmount = 0f;
        armorBar.fillAmount = 0f;

        for(int i = 0; i < abilities.Length; i++) {
            abilities[i].setNone();
        }

        if (characterIcon != null){
            characterIcon.color = Color.black;
        }
    }

    //Disable UI Resources when not being used
    public void enableUI() {
        gameObject.SetActive(true);
    }

    //Change Character Icon
    public void changeIcon(Color color) {
        characterIcon.color = color;
    }

}

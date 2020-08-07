using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAbility : MonoBehaviour
{
    [SerializeField]
    private Image cd = null;
    [SerializeField]
    private Text ammo = null;

    public void setAmmo(int newAmmo) {
        Debug.Assert(newAmmo >= 0);

        ammo.text = "" + newAmmo;
    }
    
    public void setCooldownUI(float cdDone) {
        cd.fillAmount = 1f - cdDone;
    }

    public void clearAmmo() {
        ammo.text = "";
    }

    public void setNone() {
        gameObject.SetActive(false);
    }

    public void setActive() {
        gameObject.SetActive(true);
    }
}

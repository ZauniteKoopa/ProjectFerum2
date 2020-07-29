using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChannelUI : MonoBehaviour
{
    [SerializeField]
    private Image channelBar = null;

    //Sets channeling bar active
    public void setActive(bool state) {
        gameObject.SetActive(state);
    }

    //Sets channel bar depending on cur and max
    public void setChannel(float cur, float max) {
        channelBar.fillAmount = cur / max;
    }
}

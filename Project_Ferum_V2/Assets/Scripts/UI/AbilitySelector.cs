using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySelector : MonoBehaviour
{
    [SerializeField]
    private GameObject[] selections = new GameObject[3];
    private int moveIndex = 0;

    private int numMoves = 0;

    //Method to change
    public void changeSelectAbility(int index) {
        Debug.Assert(index >= 0 && index < 3);

        //Disable cur selection
        selections[moveIndex].SetActive(false);

        //Set new index
        moveIndex = index;
        selections[moveIndex].SetActive(true);
    }

    //Method to shift left
    public void shiftLeft() {
        int newIndex = (moveIndex - 1 + numMoves) % numMoves;
        changeSelectAbility(newIndex);
    }

    //Method to shift right
    public void shiftRight() {
        int newIndex = (moveIndex + 1 + numMoves) % numMoves;
        changeSelectAbility(newIndex);
    }

    //Accessor method for moveIndex
    public int getMoveIndex() {
        return moveIndex;
    }

    //Reset selector to fighter
    public void setToFighter(int fighterMoves) {
        numMoves = fighterMoves;
        changeSelectAbility(0);
    }
}

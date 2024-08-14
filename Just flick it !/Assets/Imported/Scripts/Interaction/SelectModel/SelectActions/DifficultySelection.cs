using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using static GameCycler;

public class DifficultySelection : SwitchAction
{
    [Header("Game setter")]
    public GameSelector menu;
    [SerializeField]
    private Difficulty SelectedDifficulty = Difficulty.Normal;
    public Difficulty RegisteredDifficulty
    {
        get { return SelectedDifficulty; }
        set
        {
            SelectedDifficulty = value;
        }
    }

    public override void onMouseDown()
    {
        //change status
        if (isHovered)
        {
            if (!IsOn) //select it (show options)
            {
                //apply the selection
                menu.SeDiff = RegisteredDifficulty;


                //unselect other switches
                // look every container
                foreach (Transform switchcontainer in transform.parent.parent)
                {
                    // look into every container
                    foreach (Transform child in switchcontainer)
                    {
                        // is it the switch
                        if (child.name.Contains("select") && child.name != transform.name)
                        {
                            child.GetComponent<SwitchAction>().FlickOff(true);
                        }
                    }
                }
                FlickOn(false);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayGame : SwitchAction
{
    public GameSelector GameSelect;
    public override void onMouseDown()
    {
        //change status
        if (isHovered)
        {
            IsOn = !IsOn;
            GameSelect.ready = IsOn;
        }
    }
}

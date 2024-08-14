using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class SwitchAction : SelectAction
{
    public GameCycler gamecycler;

    public bool IsOn;
    public bool isHovered = false;

    public void FlickOn(bool muted)
    {
        IsOn=true;

        //switch animation
        transform.GetComponent<AudioSource>().clip = gamecycler.FlickOnAudio;
        if (!muted) {
            transform.GetComponent<AudioSource>().Play();
        }
        transform.Find("Light").GetComponent<Renderer>().material = gamecycler.GreenMaterial;
        transform.GetComponent<Outline>().OutlineColor = gamecycler.GreenMaterial.color;
        transform.Find("Switch").localPosition = gamecycler.FlickOnModel.transform.Find("Switch").localPosition;
        transform.Find("Switch").localRotation = gamecycler.FlickOnModel.transform.Find("Switch").localRotation;
    }

    public void FlickOff(bool muted)
    {
        IsOn = false;

        //switch animation
        transform.GetComponent<AudioSource>().clip = gamecycler.FlickOffAudio;
        if (!muted)
        {
            transform.GetComponent<AudioSource>().Play();
        }
        transform.Find("Light").GetComponent<Renderer>().material = gamecycler.RedMaterial;
        transform.GetComponent<Outline>().OutlineColor = gamecycler.RedMaterial.color;
        transform.Find("Switch").localPosition = gamecycler.FlickOffModel.transform.Find("Switch").localPosition;
        transform.Find("Switch").localRotation = gamecycler.FlickOffModel.transform.Find("Switch").localRotation;
    }
    

    public override void onMouseDown()
    {
        if (IsOn)
        {
            FlickOff(false);
        } else
        {
            FlickOn(false);
        }
    }

    public override void onMouseUp()
    {
        
    }

    public override void onMouseExit()
    {
        // Reset the color when the mouse exits
        GetComponent<Outline>().OutlineWidth = 0;
        isHovered = false;
    }

    public override void onMouseEnter()
    {
        // Highlight the model when the mouse hovers over it
        GetComponent<Outline>().OutlineWidth = 3;
        GetComponent<Outline>().OutlineColor = transform.Find("Light").GetComponent<Renderer>().material.color;
        isHovered = true;
    }

    public override void onMouseDrag()
    {
        
    }
}




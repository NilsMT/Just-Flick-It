using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static GameCycler;

public class GameSelector : MonoBehaviour
{
    [Header("Inputs")]
    public Game SeType = Game.TutorialEmpty;
    public Difficulty SeDiff = Difficulty.Easy;

    //Custom imported
    public string matrixinput;
    //Custom settings
    public bool customsettings;
    public string inputid;
    public string inputseed = "0";
    [Range(0.1f, 100.0f)]
    public float SpecialPercentage = 0.0f;
    [Range(0.0f, 99.9f)]
    public float VoidPercentage = 0.0f;
    [Range(0f, 100.0f)]
    public float EventLikeliness = 0f;
    public string inputevent;
    public string inputcolumns = "1";
    public string inputrows = "1";

    public bool ok;
    public bool ready;
    [Header("Objects")]
    public GameSelection tutorialswitch;
    public GameSelection classicswitch;
    public GameObject diffcontainer;
    public PlayGame playswitch;
    public GameCycler gamecycler;

    public DifficultySelection diffdefault;


    List<String> errorlist = new List<string>();

    // tell to start the game
    void Update()
    {
        if (ready)
        {
            ok = true;
            string[,] matr = { { } };
            int rseed = 0;
            int rcol = 0;
            int rrow = 0;
            int revent = 0;


            if (SeType == Game.Custom) {
                //create the matrix
                matr = gamecycler.ImportMatrix(matrixinput);
                //check if correct
                if (matr.GetLength(0) == 0 && matr.GetLength(1) == 0)
                {
                    errorlist.Add("The custom matrix doesnt respect the proper format (exemple of a 3x3 matrix : \"E1,C1,E0;L1,C0,S0;,,T0\")");
                    ok = false;
                }
            }

            if (customsettings)
            {
                //check correct seed is int
                try
                {
                    rseed = int.Parse(inputseed);
                }
                catch (Exception e)
                {
                    errorlist.Add("The seed is not an number");
                    ok = false;
                }
                //check correct id ==> A-Z, 4 letters
                Regex r = new Regex(@"^[A-Z]*$");

                bool onlyup = r.IsMatch(inputid);
                bool lenfour = (inputid.Length == 4);

                if (!lenfour || !onlyup)
                {
                    if (!lenfour)
                    {
                        errorlist.Add("The ID lenght is not 4");
                    }
                    if (!onlyup)
                    {
                        errorlist.Add("The ID doesnt only contain UPPERCASE Letters");
                    }
                    ok = false;
                }
                //check correct columns + rows + eventleft ==> all superior to 0 and are int
                try
                {
                    rcol = int.Parse(inputcolumns);
                    if (rcol < 0)
                    {
                        errorlist.Add("The number of columns is under 0");
                        ok = false;
                    }
                }
                catch
                {
                    errorlist.Add("The number of columns is not a number");
                    ok = false;
                }

                try
                {
                    rrow = int.Parse(inputrows);
                    if (rrow < 0)
                    {
                        errorlist.Add("The number of rows is under 0");
                        ok = false;
                    }
                }
                catch
                {
                    errorlist.Add("The number of rows is not a number");
                    ok = false;
                }

                try
                {
                    revent = int.Parse(inputevent);
                    if (revent < 0)
                    {
                        errorlist.Add("The number of event left is under 0");
                        ok = false;
                    }
                }
                catch
                {
                    errorlist.Add("The number of event left is not a number");
                    ok = false;
                }
            }

            ////////////////////////

            if (ok)
            {
                ok = false;
                ready = false;
                gamecycler.PlayGame(SeType, SeDiff, matr, customsettings, inputid, rseed, SpecialPercentage, VoidPercentage, EventLikeliness, revent, rcol, rrow);
            } else
            {
                ready = false;
                ok = false;
                //error effect
                StartCoroutine(ErrorEffect());

            }
            
        } else
        {
            if (classicswitch.IsOn)
            {
                //not displayed so display it + select
                if (!diffcontainer.activeSelf) {
                    diffcontainer.SetActive(true);
                    //tick
                    diffdefault.isHovered = true;
                    diffdefault.onMouseDown();
                    diffdefault.isHovered = false;
                }
            } else
            {
                //displayed so hide it + select
                if (diffcontainer.activeSelf)
                {
                    diffcontainer.SetActive(false);
                    //tick
                    diffdefault.isHovered = true;
                    diffdefault.onMouseDown();
                    diffdefault.isHovered = false;
                }
            }
        }
    }

    IEnumerator ErrorEffect()
    {
        CameraAnimation CameraAnimation = gamecycler.Camera.GetComponent<CameraAnimation>();
        AudioController sndpl = gamecycler.SoundPlayer.GetComponent<AudioController>();
        Color originalcolor = gamecycler.Camera.GetComponent<Light>().color;
        CameraAnimation.changeColor(Color.red);
        CameraAnimation.animationtime = sndpl.MalfunctionAlarm.length;
        gamecycler.Camera.GetComponent<Light>().intensity *= 1.5f;
        StartCoroutine(sndpl.PlayMalfunction());
        CameraAnimation.shake(false);
        gamecycler.Camera.DisableMovements();

        playswitch.GetComponent<SwitchAction>().FlickOff(false);
        playswitch.GetComponent<SelectModel>().disableAction();

        //display errors
        foreach (string err in errorlist)
        {
           Debug.Log(err);
        }

        //wait for animation time
        yield return new WaitForSeconds(CameraAnimation.animationtime);

        //resume
        CameraAnimation.changeColor(originalcolor);
        gamecycler.Camera.GetComponent<Light>().intensity /= 1.5f;
        gamecycler.Camera.GetComponent<CameraController>().EnableMovements();

        playswitch.GetComponent<SelectModel>().enableAction();
    }
}



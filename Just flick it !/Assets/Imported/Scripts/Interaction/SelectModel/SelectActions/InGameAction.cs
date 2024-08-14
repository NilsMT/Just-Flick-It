using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameAction : SwitchAction
{
    public List<int> Coords;
    public int Status;

    [Space(10)]
    public TextMeshPro swid;
    public enum SwitchType
    {
        Empty,
        Line,
        Triangle,
        Square,
        Circle
    }

    [SerializeField]
    private SwitchType Type;
    public SwitchType SelectedType
    {
        get { return Type; }
        set
        {
            Type = value;
        }
    }
    public void UpdateSwitch(bool init,bool muted)
    {
        //update view
        Renderer LineIcon = transform.Find("L").GetComponent<Renderer>();
        Renderer TriangleIcon = transform.Find("T").GetComponent<Renderer>();
        Renderer SquareIcon = transform.Find("S").GetComponent<Renderer>();
        Renderer CircleIcon = transform.Find("C").GetComponent<Renderer>();
        
        //Set Default to empty
        LineIcon.material = gamecycler.RedMaterial;
        TriangleIcon.material = gamecycler.RedMaterial;
        SquareIcon.material = gamecycler.RedMaterial;
        CircleIcon.material = gamecycler.RedMaterial;
        string type = "E";

        switch (SelectedType)
        {
            case SwitchType.Line:
                LineIcon.material = gamecycler.GreenMaterial; 
                type = "L";
                break;
            case SwitchType.Triangle:
                TriangleIcon.material = gamecycler.GreenMaterial;
                type = "T";
                break;
            case SwitchType.Square:
                SquareIcon.material = gamecycler.GreenMaterial;
                type = "S";
                break;
            case SwitchType.Circle:
                CircleIcon.material = gamecycler.GreenMaterial;
                type = "C";
                break;
            case SwitchType.Empty:
                break;
            default:
                Debug.LogWarning("Unknown Switch Type : " + SelectedType);
                break;
        }

        if (Status == 0)
        {
            FlickOff(muted);
        }
        else if (Status == 1)
        {
            FlickOn(muted);
        }
        //

        
        if (transform.GetComponentInParent<GeneratorHandler>() != null)
        {
            //text
            swid.text = name.Replace("-" + gamecycler.GameBoard.ID, "");

            //check if the game is solved

            if (init == false)
            {
                if (gamecycler.GameBoard.CurrentGameMatrix.GetLength(0) > 0 && gamecycler.GameBoard.CurrentGameMatrix.GetLength(1) > 0)
                {
                    gamecycler.GameBoard.UpdateMatrix(Coords[0], Coords[1], Status, type, gamecycler.GameBoard.CurrentGameMatrix);
                    if (transform.GetComponentInParent<AnswerHandler>() != null && gamecycler.GameBoard.AnswerGameMatrix != null)
                    {
                        AnswerHandler solver = transform.GetComponentInParent<AnswerHandler>();
                        bool res = solver.IsProblemSolved(gamecycler.GameBoard.AnswerGameMatrix, gamecycler.GameBoard.CurrentGameMatrix);
                        if (res)
                        {
                            Console.WriteLine("Board complete !");
                            gamecycler.GameBoard.EndGame();
                        }
                    }
                }
            }
        }
    }

    public override void onMouseDown()
    {
        //change status
        if (isHovered)
        {
            if (Status == 0)
            {
                Status = 1;
            }
            else
            {
                Status = 0;
            }
            //Handle event here
            if (UnityEngine.Random.Range(0f, 100.0f) <= gamecycler.GameBoard.EventLikeliness && gamecycler.GameBoard.eventLeft > 0)
            {
                gamecycler.GameBoard.eventLeft -= 1;
                int choice = UnityEngine.Random.Range(1, 4);
                switch (choice)
                {
                    case 1:
                        gamecycler.GameBoard.Malfunction(); break;
                    case 2:
                        gamecycler.GameBoard.Earthquake(); break;
                    case 3:
                        gamecycler.GameBoard.PowerOutage(); break;
                    default: Debug.LogWarning("Event choice not binded : " + choice); break;
                }
            } else
            {
                UpdateSwitch(false,false);
            }
        }
    }
    private void Start()
    {
        UnityEngine.Random.InitState(gamecycler.GameBoard.Seed);
    }
}

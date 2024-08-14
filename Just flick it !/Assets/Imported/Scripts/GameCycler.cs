using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static GameCycler;

public class GameCycler : MonoBehaviour
{
    public enum Difficulty
    {
        Easy,
        Normal,
        Hard,
        Hardcore
    }

    public enum Game
    {
        Custom,
        Classic,
        TutorialEmpty,
        TutorialLine1,
        TutorialLine2,
        TutorialTriangle1,
        TutorialTriangle2,
        TutorialSquare,
        TutorialCircle1,
        TutorialCircle2,
        TutorialPriority1,
        TutorialPriority2
    }

    [Header("global ressources")]
    public GeneratorHandler GameBoard;
    public GameSelector MenuBoard;
    public Canvas GameUI;
    public CameraController Camera;
    public GameObject SwitchBase;
    public GameObject GameBG;
    public Material RedMaterial;
    public Material GreenMaterial;
    public Transform FlickOffModel;
    public Transform FlickOnModel;
    public AudioClip FlickOffAudio;
    public AudioClip FlickOnAudio;
    public GameObject SoundPlayer;
    public GameObject AnswerVisualiser;

    private Game transferedgame;

    //Turn string to matrix
    public string[,] ImportMatrix(string exportedString)
    {
        string[] rows = exportedString.Split(';');
        int rowCount = rows.Length;

        // If the exported string is empty, return an empty matrix
        if (rowCount == 0)
        {
            return new string[0, 0];
        }

        int colCount = rows[0].Split(',').Length;

        string[,] matrix = new string[rowCount, colCount];

        for (int i = 0; i < rowCount; i++)
        {
            string[] elements = rows[i].Split(',');

            // Check if the row has the correct number of elements
            if (elements.Length != colCount)
            {
                return new string[0, 0];
            }

            for (int j = 0; j < colCount; j++)
            {
                if (elements[j].Equals("E1") || elements[j].Equals("E0") || elements[j].Equals("L0") || elements[j].Equals("L1") || elements[j].Equals("T0") || elements[j].Equals("T1") || elements[j].Equals("S0") || elements[j].Equals("S1") || elements[j].Equals("C0") || elements[j].Equals("C1") || elements[j].Equals(""))
                {
                    matrix[i, j] = elements[j];
                }
                else //Not a proper definition of the element
                {
                    return new string[0, 0];
                }
            }
        }

        return matrix;
    }

    //Turn matrix to string
    public string ExportMatrix(int[,] matrix)
    {
        StringBuilder sb = new StringBuilder();

        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                sb.Append(matrix[i, j]);
                if (j < cols - 1)
                {
                    sb.Append(",");
                }
            }
            if (i < rows - 1)
            {
                sb.Append(";");
            }
        }

        return sb.ToString();
    }
    public void PlayGame(Game SeType, Difficulty SeDiff, string[,] matr, bool customsettings, string idinput, int seed, float SpecialPercentage, float VoidPercentage, float EventLikeliness, int eventLeft, int columns, int rows)
    {
        MenuBoard.enabled = false;


        //transfer
        GameBoard.RegisteredDifficulty = SeDiff;
        GameBoard.RegisteredGame = SeType;

        transferedgame = SeType;

        if (SeType==Game.Custom)
        {
            GameBoard.StarterGameMatrix = matr;
        }
        
        if (customsettings)
        {
            GameBoard.CustomSettingsEnabled = true;
            GameBoard.ID = idinput;
            GameBoard.Seed = seed;
            GameBoard.SpecialPercentage = SpecialPercentage;
            GameBoard.VoidPercentage = VoidPercentage;
            GameBoard.EventLikeliness = EventLikeliness;
            GameBoard.eventLeft = eventLeft;
            GameBoard.columns = columns;
            GameBoard.rows = rows;
        }

        MenuBoard.playswitch.FlickOff(true);
        //enable it
        GameBoard.enabled = true;
        GameUI.enabled = true;
    }
    public void GoToMenu()
    {
        GameBoard.enabled = false;
        GameBoard.enabled = false;


        MenuBoard.enabled = true;

        //center the camera toward the MenuBoard
        Camera.DisableMovements();
        Camera.SetSubject(MenuBoard.gameObject);
        Camera.CenterCameraPos();
        //check tutorial
        MenuBoard.tutorialswitch.isHovered = true;
        MenuBoard.tutorialswitch.onMouseDown();
        MenuBoard.tutorialswitch.isHovered = false;
    }

    public void Start()
    {
        GoToMenu();
    }

    public void Update()
    {
        if (GameUI.enabled)
        {
            if (GameBoard.RegisteredGame!=transferedgame)
            {
                GoToMenu();
                transferedgame = GameBoard.RegisteredGame;
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.WSA;
using static UnityEngine.GraphicsBuffer;
using static GameCycler;

public class GeneratorHandler : MonoBehaviour
{
    [Header("Custom settings (Only work when in classic gamemode)")]
    public bool CustomSettingsEnabled = true;
    //Game Settings
    
    [Header("Game settings")]
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
    
    [SerializeField]
    private Game GameType = Game.Classic;
    public Game RegisteredGame
    {
        get { return GameType; }
        set
        {
            GameType = value;
        }
    }
    //Board settings
    public int Seed = 0;
    public string ID = "ABCD";
    //Special switch rarity
    [Range(0.1f, 100.0f)]
    public float SpecialPercentage = 0.0f;
    //Range for empty
    [Range(0.0f, 99.9f)]
    public float VoidPercentage = 0.0f;
    //Range for event likeliness
    [Range(0f, 100.0f)]
    public float EventLikeliness = 0f;
    public int eventLeft;
    //Board dimensions
    public int columns = 1;
    public int rows = 1;

    public float SpacingY = 1f;
    public float SpacingX = 1f;
    public float BackgroundBoardPadding = 50f;

    // Game objects
    [Header("Game Objects")]
    public GameCycler gamecycler;
    public Vector3 modelsize;

    [Space(10)]
    public string[,] StarterGameMatrix;
    public string[,] CurrentGameMatrix;
    public string[,] AnswerGameMatrix;

    //optionnal
    [Header("External Assets")]
    public TextMeshProUGUI SeedTxt;
    public TextMeshProUGUI IdTxt;
    public TextMeshProUGUI DifficultyTxt;
    public RawImage DifficultyImage;
    public Texture2D CustomDifficulty;
    public Texture2D TutorialDifficulty;
    public Texture2D EasyDifficulty;
    public Texture2D NormalDifficulty;
    public Texture2D HardDifficulty;
    public Texture2D HardcoreDifficulty;
    /// <summary>
    /// GAME EVENTS
    /// </summary>

    //Malfunction event | invert all status and recalculate solution
    public void Malfunction()
    {
        StartCoroutine(DoMalfunction());
    }

    IEnumerator DoMalfunction()
    {
        Debug.Log("Malfunction On Going");

        // Shake the camera
        CameraAnimation CameraAnimation = gamecycler.Camera.GetComponent<CameraAnimation>();
        AudioController sndpl = gamecycler.SoundPlayer.GetComponent<AudioController>();

        Color originalcolor = gamecycler.Camera.GetComponent<Light>().color;

        CameraAnimation.changeColor(Color.red);

        CameraAnimation.animationtime = sndpl.MalfunctionAlarm.length;

        gamecycler.Camera.GetComponent<Light>().intensity *= 1.5f;

        StartCoroutine(sndpl.PlayMalfunction());

        CameraAnimation.shake(false);
        gamecycler.Camera.GetComponent<CameraController>().DisableMovements();

        // Suspend switches interaction
        StartCoroutine(SwitchesInteraction(false));

        //Modify switches
        Transform[] models = transform.GetComponentsInChildren<Transform>();
        foreach (Transform model in models)
        {
            InGameAction SwitchData = model.GetComponent<InGameAction>();
            if (SwitchData != null)
            {
                if (SwitchData.Status == 1)
                {
                    SwitchData.Status = 0;
                }
                else if (SwitchData.Status == 0)
                {
                    SwitchData.Status = 1;
                }
                SwitchData.UpdateSwitch(false, true);
            }
        }
        // Wait for the camera shake to finish
        yield return new WaitForSeconds(CameraAnimation.animationtime);

        //resume activity
        CameraAnimation.changeColor(originalcolor);
        gamecycler.Camera.GetComponent<Light>().intensity /= 1.5f;

        // Resume switches interaction
        StartCoroutine(SwitchesInteraction(true));
        gamecycler.Camera.GetComponent<CameraController>().EnableMovements();

        UpdateAnswer(CurrentGameMatrix);
    }

    //Earthquake event | 6-16 switches change places
    public void Earthquake()
    {
        StartCoroutine(DoEarthquake());
    }

    IEnumerator DoEarthquake()
    {
        Debug.Log("Earthquake On Going");

        // Shake the camera
        CameraAnimation CameraAnimation = gamecycler.Camera.GetComponent<CameraAnimation>();
        AudioController sndpl = gamecycler.SoundPlayer.GetComponent<AudioController>();

        CameraAnimation.animationtime = sndpl.Earthquake.length;

        StartCoroutine(sndpl.PlayEarthquake());
        CameraAnimation.shake(true);
        gamecycler.Camera.GetComponent<CameraController>().DisableMovements();

        // Suspend switches interaction
        StartCoroutine(SwitchesInteraction(false));

        // Modify switches
        int switchestoupdate = UnityEngine.Random.Range(6, 17);
        int numberofswitches = transform.childCount;

        for (int i = 0; i < switchestoupdate; i++)
        {
            int a = UnityEngine.Random.Range(0, numberofswitches);
            int b = UnityEngine.Random.Range(0, numberofswitches);

            GameObject target1 = transform.GetChild(a).gameObject;
            GameObject target2 = transform.GetChild(b).gameObject;

            InGameAction switchData1 = target1.GetComponent<InGameAction>();
            InGameAction switchData2 = target2.GetComponent<InGameAction>();

            if (switchData1 != switchData2 && switchData1 != null && switchData2 != null)
            {
                List<int> prevcord = switchData1.Coords;
                Vector3 prevpos = target1.transform.localPosition;

                target1.transform.localPosition = target2.transform.localPosition;
                switchData1.Coords = switchData2.Coords;

                target2.transform.localPosition = prevpos;
                switchData2.Coords = prevcord;

                switchData1.UpdateSwitch(false, true);
                switchData2.UpdateSwitch(false, true);
            }
        }

        // Wait for the camera shake to finish
        yield return new WaitForSeconds(CameraAnimation.animationtime);

        // Resume switches interaction
        StartCoroutine(SwitchesInteraction(true));
        gamecycler.Camera.GetComponent<CameraController>().EnableMovements();

        UpdateAnswer(CurrentGameMatrix);
    }

    //Power outage event | change type by one (1-3 time)
    public void PowerOutage()
    {
        StartCoroutine(DoPowerOutage());
    }
    IEnumerator DoPowerOutage()
    {
        Debug.Log("Power Outage On Going");

        // Shake the camera
        CameraAnimation CameraAnimation = gamecycler.Camera.GetComponent<CameraAnimation>();
        AudioController sndpl = gamecycler.SoundPlayer.GetComponent<AudioController>();

        CameraAnimation.animationtime = 0;

        foreach (var clip in sndpl.PowerOutageClips)
        {
            CameraAnimation.animationtime += clip.length;
        }

        StartCoroutine(sndpl.PlayPowerOutage());

        CameraAnimation.enableLight(false);
        gamecycler.Camera.GetComponent<CameraController>().DisableMovements();
        CameraAnimation.shake(false);

        // Suspend switches interaction
        StartCoroutine(SwitchesInteraction(false));

        //Modify switches
        int timetoupdate = UnityEngine.Random.Range(1, 4);

        for (int i = 0; i < timetoupdate; i++)
        {
            Transform[] models = transform.GetComponentsInChildren<Transform>();
            foreach (Transform model in models)
            {
                InGameAction SwitchData = model.GetComponent<InGameAction>();
                if (SwitchData != null)
                {
                    switch (SwitchData.SelectedType)
                    {
                        case InGameAction.SwitchType.Empty:
                            SwitchData.SelectedType = InGameAction.SwitchType.Line;
                            break;
                        case InGameAction.SwitchType.Line:
                            SwitchData.SelectedType = InGameAction.SwitchType.Triangle;
                            break;
                        case InGameAction.SwitchType.Triangle:
                            SwitchData.SelectedType = InGameAction.SwitchType.Square;
                            break;
                        case InGameAction.SwitchType.Square:
                            SwitchData.SelectedType = InGameAction.SwitchType.Circle;
                            break;
                        case InGameAction.SwitchType.Circle:
                            SwitchData.SelectedType = InGameAction.SwitchType.Empty;
                            break;
                        default:
                            Debug.LogWarning("Unknown Switch Type");
                            break;
                    }
                    SwitchData.UpdateSwitch(false, true);
                }
            }
        }
        // Wait for the camera shake to finish
        yield return new WaitForSeconds(CameraAnimation.animationtime);

        // Resume switches interaction
        StartCoroutine(SwitchesInteraction(true));
        gamecycler.Camera.GetComponent<CameraController>().EnableMovements();

        //Create the answer matrix
        UpdateAnswer(CurrentGameMatrix);

        CameraAnimation.enableLight(true);
    }

    //Is called when the matrix as been created
    void InitGame()
    {
        //UI
        IdTxt.text = "ID : #" + ID;
        

        if (RegisteredGame==Game.Custom)
        {
            DifficultyImage.texture = CustomDifficulty;
            DifficultyTxt.text = "Difficulty : " + RegisteredGame.ToString();
            SeedTxt.text = "";
        } else if (RegisteredGame.ToString().Contains("Tutorial")) {
            DifficultyImage.texture = TutorialDifficulty;
            DifficultyTxt.text = "Difficulty : " + RegisteredGame.ToString();
            SeedTxt.text = "";
        }
        else
        {
            switch (RegisteredDifficulty)
            {
                case Difficulty.Easy:
                    DifficultyImage.texture = EasyDifficulty;
                    break;
                case Difficulty.Normal:
                    DifficultyImage.texture = NormalDifficulty;
                    break;
                case Difficulty.Hard:
                    DifficultyImage.texture = HardDifficulty;
                    break;
                case Difficulty.Hardcore:
                    DifficultyImage.texture = HardcoreDifficulty;
                    break;
                default:
                    Debug.LogWarning("Unknown Game Type");
                    break;
            }
            DifficultyTxt.text = "Difficulty : " + RegisteredDifficulty.ToString();
        }
        //Game matrix
        CurrentGameMatrix = CopyArray(StarterGameMatrix);
        
        //Board sizing
        float cx = ((columns) * (modelsize.x + SpacingX));
        float cy = ((rows) * (modelsize.y + SpacingY));

        transform.localScale = new Vector3(cx, cy, 1);

        //Background sizing
        gamecycler.GameBG.transform.localScale = new Vector3(cx + (BackgroundBoardPadding * 2), cy + (BackgroundBoardPadding * 2), 1);
        gamecycler.GameBG.transform.position = transform.position;
        gamecycler.GameBG.transform.position -= new Vector3(0, 0, transform.localScale.z / 2);
        //texture tiling
        gamecycler.GameBG.GetComponent<Renderer>().material.mainTextureScale = new Vector2(gamecycler.GameBG.transform.localScale.x / 4, gamecycler.GameBG.transform.localScale.y / 4);


        UpdateAnswer(StarterGameMatrix);
    }

    //Is called when the board is created and everything is ready
    void StartGame()
    {
        gamecycler.Camera.transform.GetComponent<CameraController>().CenterCameraPos();
    }

    //Is called when the player reset the game board
    void Reset()
    {
        ClearBoard(transform);
        CreateBoard(StarterGameMatrix,transform, false);
    }

    //Is called when the board is solved
    public void EndGame()
    {
        ClearBoard(transform);

        switch (RegisteredGame)
        {
            case Game.Classic:
                RegisteredGame = Game.Classic;
                break;
            case Game.TutorialEmpty:
                RegisteredGame = Game.TutorialLine1;
                break;
            case Game.TutorialLine1:
                RegisteredGame = Game.TutorialLine2;
                break;
            case Game.TutorialLine2:
                RegisteredGame = Game.TutorialTriangle1;
                break;
            case Game.TutorialTriangle1:
                RegisteredGame = Game.TutorialTriangle2;
                break;
            case Game.TutorialTriangle2:
                RegisteredGame = Game.TutorialSquare;
                break;
            case Game.TutorialSquare:
                RegisteredGame = Game.TutorialCircle1;
                break;
            case Game.TutorialCircle1:
                RegisteredGame = Game.TutorialCircle2;
                break;
            case Game.TutorialCircle2:
                RegisteredGame = Game.TutorialPriority1;
                break;
            case Game.TutorialPriority1:
                RegisteredGame = Game.TutorialPriority2;
                break;
            case Game.TutorialPriority2:
                SelectedDifficulty = Difficulty.Easy;
                RegisteredGame = Game.Classic;
                break;
            case Game.Custom:
                RegisteredGame = Game.Classic;
                break;
            default:
                Debug.LogWarning("Unknown Game type");
                break;
        }
        gamecycler.Camera.GetComponent<CameraController>().DisableMovements();
        ChooseGame();
    }

    /// <summary>
    /// GAME METHODS
    /// </summary>
    void UpdateAnswer(string[,] matrix)
    {
        //Game Solver
        AnswerGameMatrix = CopyArray(matrix);
        AnswerHandler Solver = transform.gameObject.GetComponent<AnswerHandler>();

        //Create the answer matrix
        Solver.CreateAnswer(AnswerGameMatrix);
        //Visualize the answer
        if (GetComponent<AnswerHandler>().AnswerVisualization == true)
        {

            foreach (Transform item in gamecycler.AnswerVisualiser.transform)
            {
                Destroy(item.gameObject);
            }

            gamecycler.AnswerVisualiser.transform.position = transform.position;
            gamecycler.AnswerVisualiser.transform.localScale = transform.localScale;
            gamecycler.AnswerVisualiser.transform.rotation = transform.rotation;
            //Offset it on the top
            gamecycler.AnswerVisualiser.transform.position += new Vector3(0, Solver.transform.localScale.y + SpacingY, 0);

            //read the answer
            CreateBoard(AnswerGameMatrix, gamecycler.AnswerVisualiser.transform, true);
        }
    }
    IEnumerator SwitchesInteraction(bool canInteract)
    {
        Transform[] models = transform.GetComponentsInChildren<Transform>();

        foreach (Transform model in models)
        {
            SelectModel sel = model.GetComponent<SelectModel>();
            if (sel != null)
            {
                if (canInteract)
                {
                    sel.enableAction();
                }
                else
                {
                    sel.disableAction();
                }
            }
        }
        yield return null;
    }

    string[,] CopyArray(string[,] original)
    {
        string[,] destination = new string[original.GetLength(0), original.GetLength(1)];
        for (int i = 0; i < original.GetLength(0); i++)
        {
            for (int j = 0; j < original.GetLength(1); j++)
            {
                destination[i, j] = original[i, j];
            }
        }
        return destination;
    }

    //Get model size
    Bounds CalculateBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            return new Bounds();
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds;
    }

    //Change the string at the specified coords (a switch has change state/type)
    public void UpdateMatrix(int i,int j, int status,string type,string[,] matrix)
    {
        matrix[i,j] = type+status.ToString();
    }

    //return the switch model that got the specified coords or null
    GameObject GetObjectByCoords(int row, int column)
    {
        foreach (Transform item in transform)
        {
            if (item.GetComponent<InGameAction>() != null)
            {
                InGameAction script = item.GetComponent<InGameAction>();
                if (script.Coords[0] == row && script.Coords[1] == column)
                {
                    return item.gameObject;
                }
            }

        }
        return null;
    }

    /// <summary>
    /// GAME GENERATION
    /// </summary>

    //Choose which game to generate
    void ChooseGame()
    {
        if (RegisteredGame==Game.Classic)
        {
            CreateBoard(GenerateClassicGame(), transform, false);
        } else if (RegisteredGame==Game.Custom) //////////////////////////////////////////////////////To write it so it read a file
        {
           
            Seed = 0;
            CreateBoard(StarterGameMatrix, transform, false);
        } else
        {
            string[,] TutorialGameMatrix = GenerateTutorialGame();
            CreateBoard(TutorialGameMatrix, transform, false);
        };
    }

    //generate predetermined matrix
    string[,] GenerateTutorialGame()
    {
        EventLikeliness = 0f;
        eventLeft = 0;
        switch (RegisteredGame)
        {
            case Game.TutorialEmpty:
                StarterGameMatrix = new string[1, 1] { { "E0" } };
                break;
            case Game.TutorialLine1:
                StarterGameMatrix = new string[3, 1] { { "L1" }, { "L0" }, { "L1" } };
                break;
            case Game.TutorialLine2:
                StarterGameMatrix = new string[2, 1] { { "L1" }, { "L0" } };
                break;
            case Game.TutorialTriangle1:
                StarterGameMatrix = new string[2, 3] { { "","E1","" }, { "E1","T0","E1" } };
                break;
            case Game.TutorialTriangle2:
                StarterGameMatrix = new string[1, 3] { { "E1", "T0", "E1" } };
                break;
            case Game.TutorialSquare:
                StarterGameMatrix = new string[3, 3] { { "E0", "E0", "E0" }, { "E0", "S0", "E0" }, { "E0", "E0", "E0" } };
                break;
            case Game.TutorialCircle1:
                StarterGameMatrix = new string[3, 3] { { "", "E1", "" }, { "E1", "C0", "E1" }, { "", "E0", "" } };
                break;
            case Game.TutorialCircle2:
                StarterGameMatrix = new string[3, 3] { { "", "E0", "" }, { "E1", "C0", "E1" }, { "", "E0", "" } };
                break;
            case Game.TutorialPriority1:
                ID = "ABCD";
                SeedTxt.text = "ID : " + ID;
                StarterGameMatrix = new string[3, 3] { { "L0", "E1", "" }, { "L1", "T0", "E0" }, { "L1", "", "" } };
                break;
            case Game.TutorialPriority2:
                ID = "DCBA";
                SeedTxt.text = "ID : " + ID;
                StarterGameMatrix = new string[3, 3] { { "L0", "E0", "" }, { "L0", "T0", "E0" }, { "L1", "", "" } };
                break;
            default:
                Debug.LogWarning("Input does not match any Game type.");
                break;
        }
        return StarterGameMatrix;
    }

    // Generate a random matrix according to game settings
    string[,] GenerateClassicGame()
    {
        if (!CustomSettingsEnabled)
        {
            //ID Generation
            string allLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            List<char> availableLetters = new List<char>(allLetters);
            ID = "";

            for (int w = 0; w < 4; w++)
            {
                int randomIndex = UnityEngine.Random.Range(0, availableLetters.Count);
                char selectedLetter = availableLetters[randomIndex];
                availableLetters.RemoveAt(randomIndex);
                ID += selectedLetter;
            }
            //Seed Generation
            Seed = System.Environment.TickCount;
            //Board dimensions
            if (RegisteredDifficulty == Difficulty.Easy)
            {
                eventLeft = 1;
                EventLikeliness = 5f;
                StarterGameMatrix = new string[6, 15];
            }
            else if (RegisteredDifficulty == Difficulty.Normal)
            {
                eventLeft = 2;
                EventLikeliness = 10f;
                StarterGameMatrix = new string[10, 16];
            }
            else if (RegisteredDifficulty == Difficulty.Hard)
            {
                eventLeft = 3;
                EventLikeliness = 10f;
                StarterGameMatrix = new string[6, 17];
            }
            else if (RegisteredDifficulty == Difficulty.Hardcore)
            {
                eventLeft = 4;
                EventLikeliness = 15f;
                StarterGameMatrix = new string[10, 18];
            }
            //Special Rarity + Empty
            if (SelectedDifficulty == Difficulty.Easy || SelectedDifficulty == Difficulty.Hard)
            {
                SpecialPercentage = 50f;
                VoidPercentage = 25f;
            }
            else if (SelectedDifficulty == Difficulty.Normal || SelectedDifficulty == Difficulty.Hardcore)
            {
                SpecialPercentage = 75f;
                VoidPercentage = 5f;
            }
        } else
        {
            if ((rows*columns)>999) //set the limit at 999 cells
            {
                Debug.LogWarning("Dimensions are too big, adjusting it from : " + rows.ToString() + "," + columns.ToString());
                if (rows<columns) //if there is more columns than row, ratio is columns per row
                {
                    int ratio = columns / rows;
                    while ((rows * columns) > 999)
                    {
                        rows -= 1;
                        columns -= ratio;
                    }
                }
                else  //else, there is more row than columns, ratio is rows per column
                {
                    int ratio = rows / columns;
                    while ((rows * columns) > 999)
                    {
                        columns -= 1;
                        rows -= ratio;
                    }
                }
                Debug.LogWarning("To : " + rows.ToString() + "," + columns.ToString());
            }

            StarterGameMatrix = new string[rows, columns];
        }
        
        SeedTxt.text = "Seed : " + Seed;


        UnityEngine.Random.InitState(Seed); //INITIATE THE SEED
        


        //generate it
        int i, j;
        for (i = 0; i < StarterGameMatrix.GetLength(0); i++)
        {
            for (j = 0; j < StarterGameMatrix.GetLength(1); j++)
            {
                string status = UnityEngine.Random.Range(0, 2).ToString();
                string type = "";
                int choice = UnityEngine.Random.Range(0, 4);
                if (UnityEngine.Random.Range(0f, 100.0f) <= VoidPercentage)
                {
                    StarterGameMatrix[i, j] = "";
                }
                else if (UnityEngine.Random.Range(0f, 100.0f) <= SpecialPercentage)
                {
                    List<string> typelist = new List<string> { "L","T","S","C" };
                    type = typelist[choice];
                    StarterGameMatrix[i, j] = type + status;
                } else 
                {
                    type = "E";
                    StarterGameMatrix[i, j] = type + status;
                }
            }
        }
        return StarterGameMatrix;
    }

    /// Read the matrix and clones the switches to the instance
    void CreateBoard(string[,] matrix, Transform instance,bool AnswerCreation)
    {
        instance.GetComponent<Renderer>().enabled = true;
        if (AnswerCreation == false)
        {
            rows = StarterGameMatrix.GetLength(0);
            columns = StarterGameMatrix.GetLength(1);
            InitGame();
        }
        int matrixrows = matrix.GetLength(0);
        int matrixcolumns = matrix.GetLength(1);
        //size the board
        float cx = ((matrixcolumns) * (modelsize.x + SpacingX));
        float cy = ((matrixrows) * (modelsize.y + SpacingY));
        instance.localScale = new Vector3(cx, cy, 1);
        //read the matrix
        int i, j;
        for (i = 0; i < matrixrows; i++)
        {
            for (j = 0; j < matrixcolumns; j++)
            {
                // Create model and position it
                float x = -(j * (modelsize.x + SpacingX)) + ((instance.localScale.x - modelsize.x - SpacingX) / 2);
                float y = -(i * (modelsize.y + SpacingY)) + ((instance.localScale.y - modelsize.y - SpacingY) / 2);
                float z = instance.localScale.z;
                GameObject clone = Instantiate(gamecycler.SwitchBase);
                clone.transform.position = instance.position;
                clone.transform.localPosition += new Vector3(x, y, z);
                clone.transform.SetParent(instance);
                ////after positionning , doing stuff
                clone.GetComponent<InGameAction>().enabled = true;
                //init variables
                clone.GetComponent<InGameAction>().Coords = new List<int> { i, j };
                clone.name = "#" + ((i) * matrixcolumns + (j + 1));
                //getting type and status
                string Switch = matrix[i, j];
                string types = "ELTSC";
                if (Switch!="") {
                    char type = Switch[0];
                    char status = Switch[1];
                    //reading status
                    if (status == '1')
                    {
                        clone.GetComponent<InGameAction>().Status = 1;
                    }
                    else if (status == '0')
                    {
                        clone.GetComponent<InGameAction>().Status = 0;
                    }
                    //reading the type
                    if (type == types[0])
                    {
                        clone.GetComponent<InGameAction>().SelectedType = InGameAction.SwitchType.Empty;
                    }
                    else if (type == types[1])
                    {
                        clone.GetComponent<InGameAction>().SelectedType = InGameAction.SwitchType.Line;
                    }
                    else if (type == types[2])
                    {
                        clone.GetComponent<InGameAction>().SelectedType = InGameAction.SwitchType.Triangle;
                    }
                    else if (type == types[3])
                    {
                        clone.GetComponent<InGameAction>().SelectedType = InGameAction.SwitchType.Square;
                    }
                    else if (type == types[4])
                    {
                        clone.GetComponent<InGameAction>().SelectedType = InGameAction.SwitchType.Circle;
                    }
                    clone.GetComponent<InGameAction>().UpdateSwitch(true,true);
                    clone.GetComponent<SelectModel>().enableAction();
                    if (AnswerCreation == true)
                    {
                        clone.GetComponent<InGameAction>().swid.text = clone.name;
                        Destroy(clone.GetComponent<SelectModel>());
                        Destroy(clone.GetComponent<InGameAction>());
                    }
                } else
                {
                    Destroy(clone);
                }
            }
        }
        if (AnswerCreation == false) {
            StartGame();
        }
    }

    //Remove switches from the instance
    void ClearBoard(Transform gen)
    {
        //clear childs
        foreach (Transform item in gen)
        {
            Destroy(item.gameObject);
        }
        //reset matrix
        StarterGameMatrix = null;
        AnswerGameMatrix = null;
        CurrentGameMatrix = null;

        ////////Do it for the answer
        Transform Answ = null;
        AnswerHandler answerHandler = gen.GetComponent<AnswerHandler>();
        if (answerHandler != null && gamecycler.AnswerVisualiser != null && answerHandler.AnswerVisualization)
        {
            Answ = gamecycler.AnswerVisualiser.transform;
        }
        if (Answ != null)
        {
            //clear childs
            foreach (Transform item in Answ)
            {
                Destroy(item.gameObject);
            }

            //reset size
            Answ.localScale = Vector3.one;
            Answ.GetComponent<Renderer>().enabled = false;
        }

        //reset size
        gen.localScale = Vector3.one;
        gen.GetComponent<Renderer>().enabled = false;
    }

    //Called when the script start
    void OnEnable()
    {
        gamecycler.Camera.transform.GetComponent<CameraController>().SetSubject(transform.gameObject);
        modelsize = CalculateBounds(gamecycler.SwitchBase).size;
        ChooseGame();
    }

    void OnDisable ()
    {
        ClearBoard(transform);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }
}
using Palmmedia.ReportGenerator.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Moving Keybinds")]
    public KeyCode moveUpKey = KeyCode.W;
    public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveDownKey = KeyCode.S;
    public KeyCode moveRightKey = KeyCode.D;

    [Header("Zoom Settings")]
    public int StartingCameraZ = 50;

    

    [Header("Moving Settings")]
    public float ZoomSensitivity = 100.0f;
    public float MovementSensitivity = 100.0f;

    [Header("Limits")]
    public float MinCameraZ;
    public float MaxCameraZ;

    public float MinCameraX;
    public float MaxCameraX;

    public float MinCameraY;
    public float MaxCameraY;

    private Vector3 InitCameraPosition;

    private GameObject Subject;
    private bool moveenabled;

    [Header("Others")]
    public GameCycler gamecycler;
    //setter of the Subject
    public void SetSubject(GameObject sub)
    {
        Subject = sub;
    }

    //enable/disable the camera
    public void EnableMovements()
    {
        moveenabled = true;
    }

    public void DisableMovements()
    {
        moveenabled = false;
    }

    //center the cam on the board
    public void CenterCameraPos()
    {
        InitCameraPosition = Subject.transform.position + new Vector3(0, 0, StartingCameraZ);
        transform.position = InitCameraPosition;
        //manage the limits
        MaxCameraX = Subject.transform.localScale.x / 2;
        MaxCameraY = Subject.transform.localScale.y / 2;

        MinCameraX = -MaxCameraX;
        MinCameraY = -MaxCameraY;
        //allow the camera to move
        EnableMovements();
    }

    //called when enabling the script
    void Update()
    {
        if (moveenabled==true) {
            // Déplacement sur l'axe X et Y
            Vector3 movement = Vector3.zero;

            if (Input.GetKey(moveUpKey))
                movement += Vector3.up;
            if (Input.GetKey(moveDownKey))
                movement += Vector3.down;
            if (Input.GetKey(moveLeftKey))
                movement += Vector3.right;
            if (Input.GetKey(moveRightKey))
                movement += Vector3.left;

            // Applique la sensibilité
            movement *= MovementSensitivity * Time.deltaTime;

            // Applique le mouvement en respectant les limites
            Vector3 newPosition = transform.position + movement;
            newPosition.x = Mathf.Clamp(newPosition.x, InitCameraPosition.x + MinCameraX, InitCameraPosition.x + MaxCameraX);
            newPosition.y = Mathf.Clamp(newPosition.y, InitCameraPosition.y + MinCameraY, InitCameraPosition.y + MaxCameraY);
            transform.position = newPosition;

            // Mouvement sur l'axe Z (avec la molette de la souris)
            float scroll = -Input.GetAxis("Mouse ScrollWheel");
            Vector3 zoomVector = Vector3.forward * scroll * ZoomSensitivity;

            // Applique le mouvement en respectant les limites
            newPosition = transform.position + zoomVector;
            newPosition.z = Mathf.Clamp(newPosition.z, InitCameraPosition.z + MinCameraZ, InitCameraPosition.z + MaxCameraZ);
            transform.position = newPosition;

            // Déplace la balise de son
            gamecycler.SoundPlayer.transform.position = newPosition+new Vector3(0,0,15);
        }
    }
}
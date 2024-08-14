using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class SelectModel : MonoBehaviour
{
    public SelectAction action;
    private void OnMouseDown()
    {
        if (action.RegisteredActionMode == SelectAction.ActionMode.Click)
        {
            action.onMouseDown();
        }
    }

    private void OnMouseUp()
    {
        if (action.RegisteredActionMode == SelectAction.ActionMode.Click)
        {
            action.onMouseUp();
        }
    }
    private void OnMouseExit()
    {
        action.onMouseExit();
    }
    private void OnMouseEnter()
    {
        action.onMouseEnter();
    }
    private void OnMouseDrag()
    {
        if (action.RegisteredActionMode==SelectAction.ActionMode.Drag)
        {
            action.onMouseDrag();
        }
    }

    public void enableAction()
    {
        transform.GetComponent<Collider>().enabled = true;
    }

    public void disableAction()
    {
        transform.GetComponent<Collider>().enabled = false;
    }
}

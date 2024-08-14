using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectAction : MonoBehaviour
{
    public enum ActionMode
    {
        Click,
        Drag
    }
    [SerializeField]
    protected ActionMode SelectedActionMode = ActionMode.Click;
    public ActionMode RegisteredActionMode
    {
        get { return SelectedActionMode; }
        set
        {
            SelectedActionMode = value;
        }
    }

    public abstract void onMouseDown();

    public abstract void onMouseUp();

    public abstract void onMouseExit();

    public abstract void onMouseEnter();
    public abstract void onMouseDrag();
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The class handle player operation:
///  - draw rectangle to select units
///  - mouse click(touch) to issue command(move, attack, ...)
/// </summary>
public class RTSGestureInput : MonoBehaviour {

    public GameObject CommandPointGizmo = null;
    public Command.CommandType DefaultCommandType = Command.CommandType.AttackAndMove;
    private bool s_LeftMouseDown = false;
    private Vector2 s_MouseDraggingStartPoint;
    
    private IList<SelectableUnit> selectedObjectList = new List<SelectableUnit>();
 

    void Start()
    {
 
    }

    /// <summary>
    /// HumanOperationHandler draws a rectangle to let player pick up
    /// the game units which are marked by "SelectableUnit".
    /// </summary>
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.touches.Length > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                
                //DebuggerGUIText.instance.guiText.text = "Touch count:" + Input.touches.Length;
                //Debug.Log("Phase: " + Input.touches[0].phase.ToString()); 
                Touch touch = Input.GetTouch(0);
                s_LeftMouseDown = true;
                s_MouseDraggingStartPoint = GameGUIHelper.ConvertScreenTouchCoordToGUICoord(new Vector2(touch.position.x, touch.position.y));
            }
            if (Input.touches.Length > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Touch touch = Input.GetTouch(0);
                s_LeftMouseDown = false;
                Vector2 mousePos = new Vector2(touch.position.x, touch.position.y);
                Vector2 mousePos_GUI = GameGUIHelper.ConvertScreenTouchCoordToGUICoord(mousePos);
                //If single clicking, issue a command 
                if (mousePos_GUI.x == s_MouseDraggingStartPoint.x && mousePos_GUI.y == s_MouseDraggingStartPoint.y)
                {
                    DispatchCommand(mousePos);
                }
                else
                {
                    selectUnits(s_MouseDraggingStartPoint, mousePos_GUI);
                }
            }
     
        }
        //GetMouseButtonDown/GetMouseButtonUp - return true AT THE FRAME 
        //when user press down mouse button. Useful to detect drag event starting!!! 
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            if (Input.GetMouseButtonDown(0) == true)
            {
                s_LeftMouseDown = true;
                //Convert screen coordinate to GUI coordinate
                s_MouseDraggingStartPoint = GameGUIHelper.ConvertScreenTouchCoordToGUICoord(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            }
            if (Input.GetMouseButtonUp(0) == true)
            {

                s_LeftMouseDown = false;
                Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                Vector2 mousePos_GUI = GameGUIHelper.ConvertScreenTouchCoordToGUICoord(mousePos);
                //If single clicking, issue a command 
                if (mousePos_GUI.x == s_MouseDraggingStartPoint.x && mousePos_GUI.y == s_MouseDraggingStartPoint.y)
                {
                    DispatchCommand(mousePos);
                }
                else
                {
                    selectUnits(s_MouseDraggingStartPoint, mousePos_GUI);
                }
            }
        }
    }


    void OnGUI() 
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount > 0 && s_LeftMouseDown)
            {
                Touch touch = Input.GetTouch(0);
                float mouseX = touch.position.x;
                float mouseY = Screen.height - touch.position.y;
                float pivotX = s_MouseDraggingStartPoint.x;
                float pivotY = s_MouseDraggingStartPoint.y;
                //GUI.DrawTexture(new Rect(left, top, width, height), texture);
                //Top line
                GameGUIHelper.DrawHorizontalLine(pivotX, pivotY, mouseX - pivotX);
                //Right line
                GameGUIHelper.DrawVerticalLine(mouseX, pivotY, mouseY - pivotY);
                //Bottom line
                GameGUIHelper.DrawHorizontalLine(pivotX, mouseY, mouseX - pivotX);
                //Left line
                GameGUIHelper.DrawVerticalLine(pivotX, pivotY, mouseY - pivotY);
            }
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            //Draw the selection rectangle in OnGUI routine
            if (Input.GetMouseButton(0) == true && s_LeftMouseDown)
            {
                float mouseX = Input.mousePosition.x;
                float mouseY = Screen.height - Input.mousePosition.y;
                float pivotX = s_MouseDraggingStartPoint.x;
                float pivotY = s_MouseDraggingStartPoint.y;
                //GUI.DrawTexture(new Rect(left, top, width, height), texture);
                //Top line
                GameGUIHelper.DrawHorizontalLine(pivotX, pivotY, mouseX - pivotX);
                //Right line
                GameGUIHelper.DrawVerticalLine(mouseX, pivotY, mouseY - pivotY);
                //Bottom line
                GameGUIHelper.DrawHorizontalLine(pivotX, mouseY, mouseX - pivotX);
                //Left line
                GameGUIHelper.DrawVerticalLine(pivotX, pivotY, mouseY - pivotY);
            }
        }
    }
    /// <summary>
    /// Send a command
    /// </summary>
    /// <param name="mousePosition"></param>
    public void DispatchCommand(Vector2 mousePosition)
    {
        if (this.selectedObjectList == null || selectedObjectList.Count == 0)
        {
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit, 10000);
        if (isHit)
        {
            Vector3 hitPos = hit.point + new Vector3(0, 0.25f, 0); ;
            GameObject.Instantiate(CommandPointGizmo, hitPos, Quaternion.identity);
            foreach (SelectableUnit unit in selectedObjectList)
            {
                unit.ExecuteCommand(new Command(DefaultCommandType, hitPos));
            }
        }
    }

    /// <summary>
    /// Invokes when user button up, find tag "selectableUnits" and mark it selected
    /// Given two vectors, select game units which fall in the two vectors
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    private void selectUnits(Vector2 v1, Vector2 v2)
    {
        GameObject[] selectableUnits = GameObject.FindGameObjectsWithTag("SelectableUnit");
        if (selectableUnits != null && selectableUnits.Length > 0)
        {
            selectedObjectList.Clear();
            foreach (GameObject gameobject in selectableUnits)
            {
                SelectableUnit unit = gameobject.GetComponent<SelectableUnit>();
                if (unit == null)
                {
                    continue;
                }
                Vector2 screenPos = Camera.main.WorldToScreenPoint(gameobject.transform.position);
                Vector2 guiPos = GameGUIHelper.ConvertScreenTouchCoordToGUICoord(screenPos);

                float minimumX = Mathf.Min(v1.x, v2.x);
                float minimumY = Mathf.Min(v1.y, v2.y);
                float maximumX = Mathf.Max(v1.x, v2.x);
                float maximumY = Mathf.Max(v1.y, v2.y);

                //Debug.Log(string.Format("Obj X: {0}, Obj Y: {1} From X: {2} From Y: {3} End X: {4} End Y: {5}",
                //        guiPos.x, guiPos.y, minimumX, minimumY, maximumX, maximumY));
                unit.isSelected = (guiPos.x >= minimumX && guiPos.y >= minimumY
                        && guiPos.x <= maximumX && guiPos.y <= maximumY);
                if (unit.isSelected)
                {
                    selectedObjectList.Add(unit);
                }
            }
        }
    }
 
}

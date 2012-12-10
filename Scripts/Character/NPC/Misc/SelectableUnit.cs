using UnityEngine;
using System.Collections;

/// <summary>
/// SelectableUnit
///  - Provide visual hint on the units when being selected.
///  - Receive commands from player, pass command to relative AI components.
/// </summary>
[RequireComponent(typeof(AI))]
public class SelectableUnit : MonoBehaviour 
{
    public bool isSelected = false;
    private static Texture2D s_Icon_Selected = null;
    private AI baseAI = null;
    // Use this for initialization
    void Start()
    {
        if (this.gameObject.tag != "SelectableUnit")
        {
            this.gameObject.tag = "SelectableUnit";
        }
        baseAI = this.GetComponent<AI>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DispathCommand(Command c)
    {
        baseAI.DispatchCommand(c);
    }
    public void ExecuteCommand(Command c)
    {
        baseAI.ExecuteCommand(c,true);
    }
    #region GUI methods

    void OnGUI()
    {
        if (this.isSelected)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
            if (s_Icon_Selected == null)
            {
                s_Icon_Selected = new Texture2D(1, 1);
                s_Icon_Selected.SetPixel(0, 0, Color.cyan);
                s_Icon_Selected.Apply();
            }
            GameGUIHelper.DrawDot(new Vector2(screenPos.x, screenPos.y), s_Icon_Selected);
        }
    }

    #endregion
}

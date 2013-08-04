using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The manager of virtual buttons.
/// </summary>
public class JoyButtonManager : MonoBehaviour {

    JoyButton[] joyButtons = null;

    void Awake()
    {
        SortJoyButtonsByPriority(this.GetComponents<JoyButton>());
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        CheckTouch();
	}

    /// <summary>
    /// JoyButton with higher priority put in lower index, to ensure high priority button
    /// get processed first.
    /// </summary>
    /// <param name="jBs"></param>
    private void SortJoyButtonsByPriority(JoyButton[] jBs)
    {
        IList<JoyButton> tempList = new List<JoyButton>();
        foreach (JoyButton jb in jBs)
        {
            if (tempList.Count == 0)
            {
                tempList.Add(jb);
            }
            else
            {
                bool jbInserted = false;
                for (int i = 0; i < tempList.Count; i++)
                {
                    JoyButton _queuedJb = tempList[i];
                    //Always put the higher priority JoyButton ahead
                    if (_queuedJb.Priority < jb.Priority)
                    {
                        tempList.Insert(i, jb);
                        jbInserted = true;
                        break;
                    }
                }
                if(jbInserted == false)
                    tempList.Add(jb);
            }
        }
        joyButtons = new JoyButton[tempList.Count];
        tempList.CopyTo(joyButtons, 0);
    }
	
	public JoyButton GetButton(string name)
	{
		foreach(JoyButton b in joyButtons)
		{
			if(b.JoyButtonName == name)
			{
				return b;
			}
		}
		return null;
	}
	
    /// <summary>
    /// This method should be called per-frame.
    /// Loops each touch (if any), and check the touch position against every JoyButton's area.
    /// </summary>
    private void CheckTouch()
    {
        foreach (Touch t in Input.touches)
        {
            foreach (JoyButton joyButton in joyButtons)
            {
                if (joyButton.CheckTouch(t))
                {
                    joyButton.ProcessTouch(t);
                    break;
                }
            }
        }
    }
}

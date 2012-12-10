using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUIText))]
public class DebuggerGUIText : MonoBehaviour {

    public enum TextPosition
    {
        topLeft = 1,
        topRight = 2,
        bottomRight = 3,
        bottomLeft = 4
    }
    public TextPosition textPositionOnScreen = TextPosition.topRight;
    public string DebugText;
    public static DebuggerGUIText instance;
	// Use this for initialization
	void Start () {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        switch (textPositionOnScreen)
        {
            case TextPosition.topRight:
                this.guiText.pixelOffset = new Vector2(screenWidth / 2-100, screenHeight / 2);
                break;
            case TextPosition.topLeft:
                this.guiText.pixelOffset = new Vector2(-screenWidth / 2, screenHeight / 2);
                break;
            case TextPosition.bottomLeft:
                this.guiText.pixelOffset = new Vector2(-screenWidth / 2, -screenHeight / 2);
                break;
            case TextPosition.bottomRight:
                this.guiText.pixelOffset = new Vector2(screenWidth / 2, -screenHeight / 2);
                break;
        }
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    static DebuggerGUIText GetInstance()
    {
        return instance;
    }
}

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PrograssBar : MonoBehaviour {

    public Rect LocationRect;
    public Texture texture;
    public ScaleMode mode;
    public bool UseAlpha;
    
    [HideInInspector]
    public float MaxValue = 1f;
    
    public float Value = 0.1f;

    public Color ForegroundColor;
    public Color BackgroundColor;

    void Awake()
    {
        
    }

    void OnGUI()
    {
        LocationRect.width = Mathf.Clamp(LocationRect.width, Screen.width / 5, Screen.width / 3);

        //Draw prograssbar background
        GUI.color = BackgroundColor;
        GUI.DrawTexture(LocationRect, texture, mode, UseAlpha,100);

        //Draw foreground:
        GUI.color = ForegroundColor;
        Rect r = new Rect(LocationRect);
        r.width = Mathf.Clamp01(Value / MaxValue) * LocationRect.width;
        GUI.DrawTexture(r, texture, mode, UseAlpha, 100);
    }
}

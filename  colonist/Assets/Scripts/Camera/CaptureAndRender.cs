using UnityEngine;
using System.Collections;

public class CaptureAndRender : MonoBehaviour {

    public Renderer TargetRenderer;

    IEnumerator RenderToTarget()
    {
        //ReadPixels should be called after current frame is finishing drawing.
        yield return new WaitForEndOfFrame();
        Texture2D screenTex = new Texture2D(Screen.width, Screen.height);
        screenTex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenTex.Apply();

        TargetRenderer.sharedMaterial.mainTexture = screenTex;
    }
}

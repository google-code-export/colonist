using UnityEngine;
using System.Collections;

public class CameraWhiteInOut : MonoBehaviour,I_GameEventReceiver {

    public Material WhiteInOutMaterial;
	/// <summary>
	/// The time ength for white in.
	/// WhiteIn means : immediately cover the camera in a black box, then gradually dismiss the cover,
	/// </summary>
    public float WhiteInLength = 3;
	
	/// <summary>
	/// The time length for white out.
	/// WhiteOut means: gradually cover the camera in a black box.
	/// </summary>
    public float WhiteOutLength = 3;
	
	/// <summary>
	/// if WhiteInAwake = true, the camera would white in at Start()
	/// </summary>
	public bool WhiteInStart = false;

    private GameObject cookShadersObject;

    private void CreateCameraCoverPlane () {
        if (cookShadersObject != null)
        {
            Destroy(cookShadersObject);
        }
        cookShadersObject = (GameObject)GameObject.CreatePrimitive(PrimitiveType.Cube);
        cookShadersObject.renderer.material = WhiteInOutMaterial;
        cookShadersObject.transform.parent = transform;
        cookShadersObject.transform.localPosition = Vector3.zero;
        //cookShadersObject.transform.localPosition.z += 1.55f;
        cookShadersObject.transform.localPosition = new Vector3(0, 0, /*cookShadersObject.transform.localPosition.z + 1.55f*/0);
        cookShadersObject.transform.localRotation = Quaternion.identity;
        //cookShadersObject.transform.local.localEulerAngles.z += 180;
        cookShadersObject.transform.localEulerAngles = new Vector3(
        cookShadersObject.transform.localEulerAngles.x,
        cookShadersObject.transform.localEulerAngles.y,
        cookShadersObject.transform.localEulerAngles.z + 180);

        //cookShadersObject.transform.localScale = Vector3.one *1.5f;
        float XScale = 1f, YScale = 1f, ZScale = 1f;
        cookShadersObject.transform.localScale = new Vector3(XScale, YScale, ZScale);
        //cookShadersObject.transform.localScale.x *= 1.6f;	
    }

	// Use this for initialization
	void Start () {
	   if(WhiteInStart)
		{
		  SendMessage("WhiteIn");
		}
	}
	
	// Update is called once per frame
	void Update () {
//        if (Input.GetKeyDown("w"))
//        {
//            StopAllCoroutines();
//            SendMessage("WhiteIn");
//        }
//        if (Input.GetKeyDown("s"))
//        {
//            StopAllCoroutines();
//            SendMessage("WhiteOut");
//        }
	}

    void DestroyCameraCoverPlane()
    {
        if (cookShadersObject)
            DestroyImmediate(cookShadersObject);
        cookShadersObject = null;
    }
    /// <summary>
    /// gradually dismiss the white cover on the camera.
    /// Using default WhiteIn setting
    /// </summary>
    /// <returns></returns>
    public IEnumerator WhiteIn () {
	     yield return StartCoroutine("WhiteInTime", this.WhiteInLength);
    }
	
    /// <summary>
    /// gradually dismiss the white cover on the camera.
    /// </summary>
    /// <returns></returns>	
	public IEnumerator WhiteInTime (float WhiteInLength) {
         CreateCameraCoverPlane ();
	     Material mat  = cookShadersObject.renderer.material;
         mat.SetColor("_Color", new Color(mat.color.r, mat.color.g, mat.color.b, 1.0f));	
	     yield return null;;
         Color c = new Color(mat.color.r, mat.color.g, mat.color.b, 1.0f);
         float FadeSpeed = 1 / WhiteInLength;
	     while (c.a > 0.0) {
            c.a -= Time.deltaTime * FadeSpeed;
            mat.SetColor("_Color", c);
		    yield return null;;
	     }
	     DestroyCameraCoverPlane ();
	}

    /// <summary>
    /// gradually cover the camera.
    /// If keepCovered = true, the camera will remained undercover until call WhiteIn.
    /// </summary>
    /// <returns></returns>
    public IEnumerator WhiteOut(bool keepCovered)
    {
	    CreateCameraCoverPlane ();
        Material mat = cookShadersObject.renderer.material;
        mat.SetColor("_Color", new Color(mat.color.r, mat.color.g, mat.color.b, 0f));	
        yield return null;
        Color c = new Color(mat.color.r, mat.color.g, mat.color.b, 0f);
        float FadeSpeed = 1 / WhiteOutLength;
        while (c.a < 1.0)
        {
           c.a += Time.deltaTime * FadeSpeed;
           mat.SetColor("_Color", c);
           yield return null;
	    }
		if(keepCovered == false)
           DestroyCameraCoverPlane();
    }
	public void OnGameEvent(GameEvent _event)
	{
		switch(_event.type)
		{
		case GameEventType.PlayerCameraWhiteIn:
			StartCoroutine("WhiteIn", _event.FloatParameter > 0 ? _event.FloatParameter : this.WhiteInLength);
			break;
		case GameEventType.PlayerCameraWhiteOut:
			StartCoroutine("WhiteOut", _event.BoolParameter);
			break;
		}
	}
}

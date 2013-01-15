using UnityEngine;
using System.Collections;

public class Predator3rdPersonVisualEffectController : MonoBehaviour
{
	public Renderer[] LeftClawTailRender;
	public Renderer[] RightClawTailRender;

    private bool leftClawEffectDisplay = false;
    private bool rightClawEffectDisplay = false;
    private float rightclaw_visual_time;
    private float leftclaw_visual_time;
    private float lastShowRightClawEffectTime;
    private float lastShowLeftClawEffectTime;

    void Awake()
    {
        HideLeftClawTrailRenderEffect();
        HideRightClawTrailRenderEffect();
    }

	// Use this for initialization
	void Start ()
	{
	  
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (leftClawEffectDisplay && (Time.time - lastShowLeftClawEffectTime) > leftclaw_visual_time)
        {
            HideLeftClawTrailRenderEffect();
        }
        if (rightClawEffectDisplay && (Time.time - lastShowRightClawEffectTime) > rightclaw_visual_time)
        {
            HideRightClawTrailRenderEffect();
        }
	}

    /// <summary>
    /// Show left claw trail render effect in _time seconds.
    /// </summary>
    /// <param name="_time"></param>
	public void ShowLeftClawTrailRenderEffect (float _time)
	{
		foreach (Renderer render in LeftClawTailRender) {
			render.enabled = true;
		}
        lastShowLeftClawEffectTime = Time.time;
        leftClawEffectDisplay = true;
        leftclaw_visual_time = _time;
		//Invoke ("HideLeftClawTrailRenderEffect", _time);
	}

    /// <summary>
    /// Show left claw trail render
    /// </summary>
    /// <param name="_time"></param>
    public void ShowLeftClawTrailRenderEffect()
    {
        foreach (Renderer render in LeftClawTailRender)
        {
            render.enabled = true;
        }
    }

    /// <summary>
    /// Show right claw trail render effect in _time seconds.
    /// </summary>
    /// <param name="_time"></param>
	public void ShowRightClawTrailRenderEffect (float _time)
	{
		foreach (Renderer render in RightClawTailRender) {
			render.enabled = true;
		}
        lastShowRightClawEffectTime = Time.time;
        rightClawEffectDisplay = true;
        rightclaw_visual_time = _time;
        //Invoke ("HideRightClawTrailRenderEffect", _time);
	}

    /// <summary>
    /// Show right claw trail render.
    /// </summary>
    /// <param name="_time"></param>
    public void ShowRightClawTrailRenderEffect()
    {
        foreach (Renderer render in RightClawTailRender)
        {
            render.enabled = true;
        }
    }

    public void ShowBothClawVisualEffects(float _time)
    {
        ShowRightClawTrailRenderEffect(_time);
        ShowLeftClawTrailRenderEffect(_time);
    }

    public void ShowBothClawVisualEffects()
    {
        ShowRightClawTrailRenderEffect();
        ShowLeftClawTrailRenderEffect();
    }

	public void HideRightClawTrailRenderEffect ()
	{
		foreach (Renderer render in RightClawTailRender) {
			render.enabled = false;
		}
        rightClawEffectDisplay = false;
	}

    public void HideLeftClawTrailRenderEffect()
    {
        foreach (Renderer render in LeftClawTailRender)
        {
            render.enabled = false;
        }
        leftClawEffectDisplay = false;
    }

    public void HideBothClawTrailRenderEffect()
    {
        HideRightClawTrailRenderEffect();
        HideLeftClawTrailRenderEffect();
    }

}

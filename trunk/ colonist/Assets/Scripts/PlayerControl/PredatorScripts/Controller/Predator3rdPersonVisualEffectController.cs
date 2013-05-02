using UnityEngine;
using System.Collections;

public class RenderControlData
{
	public string Name = "";
	public Renderer[] LeftClawTailRender;
	public bool display = false;
	public float StopTime = 0;
}

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
	
	private float rightClaw_Effect_StopTime = 0;
	private float leftClaw_Effect_StopTime = 0;
	
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
        if (leftClawEffectDisplay && Time.time  > leftClaw_Effect_StopTime)
        {
            HideLeftClawTrailRenderEffect();
        }
        if (rightClawEffectDisplay && Time.time  > rightClaw_Effect_StopTime)
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
        leftClaw_Effect_StopTime = Time.time + _time;
        leftClawEffectDisplay = true;
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
        rightClaw_Effect_StopTime = Time.time + _time;
        rightClawEffectDisplay = true; 
	}

    public void ShowBothClawVisualEffects(float _time)
    {
        ShowRightClawTrailRenderEffect(_time);
        ShowLeftClawTrailRenderEffect(_time);
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

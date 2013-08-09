using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SplitAttackButton : JoyButton {
	
	public Texture HintTexture;
	public float HintRotateAngluarSpeed = 150;
	
	bool CanDoSpecialSkillAttack = false;
	bool ShowHint = false;
	SpecialSkill_SplitAttack splitAttackController = null;
	
	void Awake()
	{
		splitAttackController = GetComponent<SpecialSkill_SplitAttack >();
	}
	
	void Start ()
	{
		GUIBound = this.GetAdaptiveBound();
	}
	
	void Update()
	{
		if(splitAttackController.CanDoSpecialAttackInTheMoment())
		{
			ShowHint = true;
			CanDoSpecialSkillAttack = true;
		}
		else 
		{
			ShowHint = false;
			CanDoSpecialSkillAttack = false;
		}
	}
	
	/// <summary>
	/// Call when touch.phase = Began
	/// </summary>
	/// <param name="touch"></param>
	public override void onTouchBegin (Touch touch)
	{
		base.onTouchBegin (touch);
	}
	
    /// <summary>
    /// Call when touch.phase = Move
    /// </summary>
    /// <param name="touch"></param>
    public override void onTouchMove(Touch touch)
	{
	}
    /// <summary>
    /// Call when touch.phase = Move
    /// </summary>
    /// <param name="touch"></param>
    public override void onTouchStationary(Touch touch)
	{
	}
	
	/// <summary>
	/// Call when touch.phase = End
	/// </summary>
	/// <param name="touch"></param>
	public override void onTouchEnd (Touch touch)
	{
		base.onTouchEnd (touch);
		if(CanDoSpecialSkillAttack)
		{
           splitAttackController.StartCoroutine(splitAttackController.DoSpecialAttack());
		}
	}
	
	void OnGUI ()
	{
		GUI.DrawTexture (GUIBound, ButtonTexture);
		if(ShowHint)
		{
			GUIUtility.RotateAroundPivot(Time.time % 360 * this.HintRotateAngluarSpeed, GUIBound.center);
			GUI.DrawTexture (GUIBound, HintTexture, ScaleMode.ScaleToFit,true);
		}
	}
}

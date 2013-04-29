using UnityEngine;
using System.Collections;

/// <summary>
/// NPC health bar. Which is displayed at the top place of the character's world position.
/// </summary>
public class NPCHealthBar : MonoBehaviour
{
	public Texture foregoundTexture;
	public Texture backgoundTexture;
	/// <summary>
	/// The size of the health bar, x = width, y = height.
	/// </summary>
	public Vector2 size = new Vector2(45, 8);
	/// <summary>
	/// The offset plus to calcuated position.
	/// </summary>
	public Vector2 offset = new Vector2(-24,-20);
	public Color StartColor = Color.green;
	public Color EndColor = Color.red;
	public ScaleMode scaleMode;
	public bool AplhaBlend;
	/// <summary>
	/// The length of the show time.
	/// When the unit receive damage, the NPC health bar will be displayed for %ShowTimeLength% seconds.
	/// </summary>
	public float ShowTimeLength = 1;
	
	UnitHealth unit;
	CharacterController controller = null;
	Vector3 headWorldPos = Vector3.zero;
	Vector2 HealthBarGUIPos = Vector2.zero;
	float percentage;
	bool show = false;
    float lastChangeDisplayTime = 0;
	void Awake ()
	{
		controller = GetComponent<CharacterController> ();
		unit = GetComponent<UnitHealth> ();
	}
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
    void ApplyDamage(DamageParameter damageParam)
	{
		show = true;
		lastChangeDisplayTime = Time.time;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		if (show) {
			headWorldPos = controller.transform.position + new Vector3 (controller.center.x, controller.center.y + controller.height / 2, controller.center.z);
			Camera camera = Camera.allCameras [0];
			Vector3 screenPos = camera.WorldToScreenPoint (headWorldPos);
			HealthBarGUIPos = GameGUIHelper.ConvertScreenTouchCoordToGUICoord (new Vector2 (screenPos.x, screenPos.y));
			percentage = unit.GetCurrentHP () / unit.GetMaxHP ();
		}
		if(show && (((Time.time - lastChangeDisplayTime) >= ShowTimeLength) || percentage <= 0) )
		{
			show = false;
		}
		
	}
	
	void OnGUI ()
	{
		if (show) {
			Rect r = new Rect (HealthBarGUIPos.x + offset.x, HealthBarGUIPos.y + offset.y, size.x, size.y);
			GUI.DrawTexture (r, backgoundTexture, scaleMode, AplhaBlend);
			Rect runtimeRect = new Rect (r);
			runtimeRect.width *= percentage;
			GUI.color = Color.Lerp (EndColor, StartColor, percentage);
			GUI.DrawTextureWithTexCoords (runtimeRect, foregoundTexture, runtimeRect, AplhaBlend);
		}
	}
}

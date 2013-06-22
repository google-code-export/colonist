using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Display enemy in screen.
/// </summary>
public class EnemyRadar : MonoBehaviour {
	
	/// <summary>
	/// The scan range in world.
	/// </summary>
	public float ScanRadius = 10;
	public Texture RadarDot = null;
	public float ScanInterval = 0.2f;
	public float RadarDotSize = 30f;
	UnitBase unit;
	IList<Rect> RadarPointList = new List<Rect>();
	float LastScanTime = 0;
	void Awake()
	{
		unit = transform.root.GetComponentInChildren<UnitBase>();
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if((Time.time - LastScanTime) >= ScanInterval)
		{
			Scan();
		}
	}
	
	void Scan()
	{
		RadarPointList.Clear();
		Collider[] enemyColliders = Physics.OverlapSphere(transform.position, ScanRadius, unit.EnemyLayer);
		foreach(Collider enemy in enemyColliders)
		{
		   Vector3 screenPoint = this.camera.WorldToScreenPoint(enemy.transform.position);
		   if(IsScreenPointInsideScreen(screenPoint) == false)
		   {
			  Vector2 GUIPoint = GameGUIHelper.ConvertScreenTouchCoordToGUICoord(screenPoint);
			  Vector2 radarScreenPoint = GetRadarPoint(GUIPoint, RadarDotSize);
			  RadarPointList.Add(new Rect(radarScreenPoint.x, radarScreenPoint.y, RadarDotSize, RadarDotSize));
		   }
		}
		LastScanTime = Time.time;
	}
	
	/// <summary>
	/// screenpoint = some point out of screen.
	/// offset = the size of the rect point.
	/// </summary>
	Vector2 GetRadarPoint(Vector2 screenpoint, float offset)
	{
		float x = Mathf.Clamp(screenpoint.x, 0, Screen.width - offset);
		float y = Mathf.Clamp(screenpoint.y, 0, Screen.height - offset);
		return new Vector2(x, y);
	}
	
	bool IsScreenPointInsideScreen(Vector2 screenpoint)
	{
		return (screenpoint.x <= Screen.width) && (screenpoint.x >= 0) && (screenpoint.y <= Screen.height) && (screenpoint.y >= 0);
	}
	
	void OnGUI()
	{
		if(RadarPointList.Count > 0)
		{
			foreach(Rect r in RadarPointList)
			{
				 Vector2 top = GUIUtility.ScreenToGUIPoint(new Vector2(r.x, r.y));
				 GUI.DrawTexture(new Rect(top.x, top.y, r.width, r.width), RadarDot);
			}
		}
	}
}

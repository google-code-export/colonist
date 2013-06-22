using UnityEngine;
using System.Collections;

[System.Serializable]
public class LanguagePicker
{
	public Collider obj;
	public SystemLanguage systemLanguage = SystemLanguage.Afrikaans;
}

/// <summary>
/// Alow player clicking the object to trigger a game event.
/// </summary>
[ExecuteInEditMode]
public class LanguageBillboard : MonoBehaviour
{
	
	public LanguagePicker[] languagePickers = new LanguagePicker[]{};
	public Material materialForNonselected = null;
	public Material materialForSelected = null;
	
	void Awake ()
	{
		
	}
	
	// Use this for initialization
	void Start ()
	{
	    SetMaterialsForLanguagePicker();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.touches != null && Input.touches.Length > 0) {
			foreach (Touch t in Input.touches) {
				Ray ray = Camera.allCameras [0].ScreenPointToRay (t.position);
				RaycastHit hitInfo;
				if (Physics.Raycast (ray, out hitInfo)) {
					foreach (LanguagePicker languagePicker in languagePickers) {
						if (languagePicker.obj == hitInfo.collider) {
							GameEvent e = new GameEvent (GameEventType.SetLanguage);
							e.StringParameter = languagePicker.systemLanguage.ToString();
							LevelManager.OnGameEvent (e , this);
						}
					}
					SetMaterialsForLanguagePicker();
				}
			}
		}
	}
	
	void SetMaterialsForLanguagePicker ()
	{
		foreach (LanguagePicker languagePicker in languagePickers) {
			if (languagePicker.systemLanguage == Persistence.GetPlayerLanguage ()) {
			    languagePicker.obj.renderer.material = this.materialForSelected;	
			}
			else{
				languagePicker.obj.renderer.material = this.materialForNonselected;
			}
		}
	}
}

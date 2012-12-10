using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LevelManager : MonoBehaviour {

    public static LevelManager Instance;

    public ScenEventListener EventListener;
	public Transform ControlDirectionPivot;
	
    void Awake()
    {
        Instance = this;
    }

	// Use this for initialization
	void Start () {
	    GameEvent(new GameEvent(GameEventType.LevelStart, this.gameObject));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnEnable()
	{
		Instance = this;
	}

    public static void GameEvent(GameEvent gameEvent)
    {
        try
        {
            if(Instance != null && Instance.EventListener != null)
            {
               Instance.EventListener.SendMessage("ScenarioEvent", gameEvent);
            }
        }
        catch (System.Exception exc)
        {
            Debug.LogError(exc);
        }
    }
}

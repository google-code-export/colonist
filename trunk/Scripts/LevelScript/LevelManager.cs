using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Game global manager
/// </summary>
[ExecuteInEditMode]
public class LevelManager : MonoBehaviour {

    public static LevelManager Instance;

    public ScenEventListener EventListener;
	public Transform ControlDirectionPivot;

    [HideInInspector]
    public IList<AI> AIs = new List<AI>();

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
        if(Instance != null && Instance.EventListener != null)
        {
           Instance.EventListener.SendMessage("ScenarioEvent", gameEvent);
        }
    }

    public static void RegisterAI(AI AI)
    {
        Instance.AIs.Add(AI);
    }

    public static void UnregisterAI(AI AI)
    {
        if (Instance.AIs.Contains(AI))
        {
            Instance.AIs.Remove(AI);
        }
    }

    public static void SendAIMessage(string message, object parameter)
    {
        for (int i=0; i<Instance.AIs.Count; i++)
        {
            AI ai = Instance.AIs[i];
            if (ai == null)
            {
                Instance.AIs.RemoveAt(i);
            }
            else
            {
                ai.SendMessage(message, parameter);
            }
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Game global manager
/// </summary>
[ExecuteInEditMode]
public class LevelManager : MonoBehaviour {

    public static LevelManager Instance;
	public LayerMask GroundLayer;
    public ScenEventListener EventListener;
	public Transform ControlDirectionPivot;
	public string PlayerTag = "Player";
	
    [HideInInspector]
    public IList<Unit> Units = new List<Unit>();
	[HideInInspector]
	public GameObject player = null;
	
	
	
    void Awake()
    {
        Instance = this;
		player = GameObject.FindGameObjectWithTag(PlayerTag);
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
			if(Application.isPlaying ==true)
			{
               Instance.EventListener.SendMessage("OnEvent", gameEvent);
			}
        }
    }

    public static void RegisterUnit(Unit unit)
    {
        Instance.Units.Add(unit);
    }

    public static void UnregisterUnit(Unit unit)
    {
        if (Instance.Units.Contains(unit))
        {
            Instance.Units.Remove(unit);
        }
    }

    public static void SendAIMessage(string message, object parameter)
    {
        for (int i=0; i<Instance.Units.Count; i++)
        {
            Unit unit = Instance.Units[i];
            if (unit == null)
            {
                Instance.Units.RemoveAt(i);
            }
            else
            {
                unit.SendMessage(message, parameter);
            }
        }
    }
}

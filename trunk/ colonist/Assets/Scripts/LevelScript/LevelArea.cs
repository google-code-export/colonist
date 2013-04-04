using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SpawnWave
{
	
	public int TotalWaveCount = 3;
	
	public SpawnEnemy SpawnPoint = null;
}

/// <summary>
/// LevelArea - one of the most important element to compose a complete level.
/// A LevelArea is a X-Z surface. When player enter/leave the area, things happen.
/// LevelArea depends on Collider.OnTriggerEnter / OnTriggerExit to work.
/// </summary>
[RequireComponent(typeof (Collider))]
public class LevelArea : MonoBehaviour {

	public string Name = "Default Area";
    /// <summary>
    /// If SuppressEvent = true, the event will not be sent, but cache in a queue, then it will be sent 
    /// once SuppressEvent = false
    /// </summary>
    public bool SuppressEvent = false;
    public bool IgnoreInvokerTag = false;
    public string[] InvokerTag = new string[] { "Player" };

    const string PlayerTag = "Player";
    /// <summary>
    /// list to store Suppressed Event 
    /// </summary>
    private IList<GameEvent> CacheEventList = new List<GameEvent>();
    /// <summary>
    /// list to store trigger gameobjects
    /// </summary>
    private IList<GameObject> triggerObjects = new List<GameObject>();
	
    void Awake()
    {
        collider.isTrigger = true;
    }

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (SuppressEvent == false && CacheEventList.Count > 0)
        {
            foreach (GameEvent e in CacheEventList)
            {
                LevelManager.GameEvent(e);
                CacheEventList.Remove(e);
            }
        }
	}

    void OnDrawGizmosSelected()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        bool FireEvent = IgnoreInvokerTag || Util.ArrayContains<string>(InvokerTag, other.tag);
        if (FireEvent)
        {
            GameEvent _event = null;
            if (other.tag == PlayerTag)
            {
                _event = new GameEvent(GameEventType.PlayerEnterArea, this.gameObject);
            }
            else
            {
                _event = new GameEvent(GameEventType.NPCEnterArea, this.gameObject);
            }
            SendEvent(_event);
        }
    }

    void OnTriggerExit(Collider other)
    {
        bool FireEvent = IgnoreInvokerTag || Util.ArrayContains<string>(InvokerTag, other.tag);
        if (FireEvent)
        {
            GameEvent _event = null;
            if (other.tag == PlayerTag)
            {
                _event = new GameEvent(GameEventType.PlayerLeaveArea, this.gameObject);
            }
            else
            {
                _event = new GameEvent(GameEventType.NPCLeaveArea, this.gameObject);
            } 
            SendEvent(_event);

        }
    }

    /// <summary>
    ///send immediately, or cache in list
    /// </summary>
    /// <param name="e"></param>
    void SendEvent(GameEvent e)
    {
        
        if (SuppressEvent)
        {
            CacheEventList.Add(e);
        }
        else
        {
            LevelManager.GameEvent(e);
        }
    }
}

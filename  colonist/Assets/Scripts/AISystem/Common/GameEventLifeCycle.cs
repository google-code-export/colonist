using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Send event on a unit's birth and death.
/// </summary>
public class GameEventLifeCycle : MonoBehaviour {
	
	/// <summary>
	/// The dynamical event to be sent on runtime.
	/// Note: to sent DynamicalEvent by animation event, the dynamical event must has a non-empty name.
	/// </summary>
	public GameEvent[] DynamicalEvent = new GameEvent[]{};
	public IDictionary<string, GameEvent> dynamicalEventDict = new Dictionary<string, GameEvent>();
	
	/// <summary>
	/// The game event to be sent on birth.
	/// </summary>
	public GameEvent[] eventOnBirth = new GameEvent[]{};
	
	/// <summary>
	/// The event to be sent on unit die.
	/// note: this requires the GameObject to have a ApplyDamage monobehavior.
	/// </summary>
	public GameEvent[] eventOnUnitDie = new GameEvent[]{};
	
	/// <summary>
	/// The event to be sent on destroy.
	/// </summary>
	public GameEvent[] eventOnDestroy = new GameEvent[]{};
	

	void Awake()
	{   
		foreach(GameEvent _e in DynamicalEvent)
		{
			_e.sender = this.gameObject;
			dynamicalEventDict.Add(_e.Name, _e);
		}
	}
	
	// Use this for initialization
	void Start () {
		Debug.Log("In start, gameObject:" + this.name);
		foreach(GameEvent _e in eventOnBirth)
		{
			_e.sender = this.gameObject;
			LevelManager.OnGameEvent(_e, this);
		}
	}
	
	void Update()
	{
	}
	
	void OnDestroy () {
		foreach(GameEvent _e in eventOnDestroy)
		{
			_e.sender = this.gameObject;
			LevelManager.OnGameEvent(_e, this);
		}
	}
	
	/// <summary>
	/// Usually, this method is invoked by AIApplyDamage.ApplyDamage()
	/// </summary>
	void Die(DamageParameter dp)
	{
		foreach(GameEvent _e in eventOnUnitDie)
		{
			_e.sender = this.gameObject;
			LevelManager.OnGameEvent(_e, this);
		}
	}
	
	public void AddEventOnBirth(GameEvent _e)
	{
		eventOnBirth = Util.AddToArray<GameEvent> (_e, eventOnBirth);
	}
	
	public void AddEventOnDestroy(GameEvent _e)
	{
		eventOnDestroy = Util.AddToArray<GameEvent> (_e, eventOnDestroy);
	}
	
	public void AddEventOnUnitDie(GameEvent _e)
	{
		eventOnUnitDie = Util.AddToArray<GameEvent> (_e, eventOnUnitDie);
	}
	
	/// <summary>
	/// Fires a dynamical event by event name.
	/// </summary>
	public void _GameEvent(string eventname)
	{
		LevelManager.OnGameEvent(dynamicalEventDict[eventname], this);
	}
}

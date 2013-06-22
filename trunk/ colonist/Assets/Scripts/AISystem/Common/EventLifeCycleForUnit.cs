using UnityEngine;
using System.Collections;


/// <summary>
/// Game event with HP percentage.
/// When HP lesser/equal/greater than a value, fires the event.
/// </summary>
[System.Serializable]
public class GameEventWithHPPercentage
{
	public string Name = "";
	public float FireEventHPPercentage = 0.5f;
	public ValueComparisionOperator comparisonOperator = ValueComparisionOperator.LessOrEqual;
	/// <summary>
	/// If OneOff = true, the events are only fired for one-shot.
	/// </summary>
	public bool OneOff = true;
	public GameEvent[] events = new GameEvent[]{};
}

/// <summary>
/// Event life cycle for unit.
/// Define a set of game event, match to HP perctange of unit.
/// For example, when a unit's HP lesser than 35%, fire an event
/// </summary>
public class EventLifeCycleForUnit : MonoBehaviour {
	
	public GameEventWithHPPercentage[] eventAtHPPerctange = new GameEventWithHPPercentage[] {};
	
	// Update is called once per frame
	void Update () {
		foreach(GameEventWithHPPercentage e in this.eventAtHPPerctange)
		{
		   float HPPercentage = this.GetComponent<UnitHealth>().GetCurrentHP()/this.GetComponent<UnitHealth>().GetMaxHP();
		   if(Util.CompareValue(HPPercentage, e.comparisonOperator, e.FireEventHPPercentage))
		   {
			  foreach(GameEvent ge in e.events)
			  {
					LevelManager.OnGameEvent(ge , this);
			  }
				//if oneOff = true, remove the GameEventWithHPPercentage
			  if(e.OneOff)
			  {
				 this.eventAtHPPerctange = Util.CloneExcept<GameEventWithHPPercentage>( this.eventAtHPPerctange, e);
			  }
		   }
		}
	}
}

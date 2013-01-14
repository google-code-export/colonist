using UnityEngine;
using System.Collections;

public abstract class UnitHealth :MonoBehaviour {
    public abstract void SetCurrentHP(float value);
    public abstract void SetMaxHP(float value);
    public abstract float GetCurrentHP();
    public abstract float GetMaxHP();
}

public abstract class UnitBase : UnitHealth
{
	public string Name = "Default Name";
	public ArmorType Armor = ArmorType.NoArmor_Human;
	    /// <summary>
    /// What's the layer of the enemy
    /// </summary>
    public LayerMask EnemyLayer;
    public LayerMask GroundLayer;
    public LayerMask WallLayer;
    /// <summary>
    /// Max Health point.
    /// </summary>
    public float MaxHP = 100;
	
    /// <summary>
    /// Health point.
    /// </summary>
    public float HP = 100;
	
	/// <summary>
	/// The move modifier.
	/// When unit moving, real speed = speed * MoveModifier
	/// </summary>
	[HideInInspector]
	public float SpeedModifier = 1;
}
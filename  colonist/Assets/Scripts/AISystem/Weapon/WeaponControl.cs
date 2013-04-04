using UnityEngine;
using System.Collections;

public class WeaponControl : MonoBehaviour {
	
	public GameObject ControllableWeapon = null;
	
	public bool InitOn = false;
	
	void Awake()
	{
		ControllableWeapon.active = InitOn;
	}
	
	void Weapon(float delay)
	{
		Invoke("WeaponOn", delay);
	}
	
	void WeaponOn()
	{
		ControllableWeapon.active = true;
	}
	
	void WeaponOff()
	{
		ControllableWeapon.active = false;
	}
}

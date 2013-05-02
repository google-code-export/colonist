using UnityEngine;
using System.Collections;

public class WeaponControl : MonoBehaviour {
	
	public GameObject ControllableWeapon = null;
	
	public TrailRenderEffectData trailRenderEffectData = null;
	
	public bool InitOn = false;
	
	void Awake()
	{
		ControllableWeapon.active = InitOn;
	}
	
	void Update()
	{
		if(trailRenderEffectData.enabled && trailRenderEffectData.IsActive == true && (Time.time - trailRenderEffectData.LastDisplayTime)>=trailRenderEffectData.DisplayLength)
		{
			trailRenderEffectData.trailRenderObject.enabled = false;
			trailRenderEffectData.IsActive = false;
		}
	}
	
	void _WeaponOn(float delay)
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
	
	void _TrailRenderOn(float length)
	{   
		if(trailRenderEffectData.enabled)
		{
		  trailRenderEffectData.trailRenderObject.enabled = true;
		  trailRenderEffectData.IsActive = true;
	  	  trailRenderEffectData.LastDisplayTime = Time.time;
		  trailRenderEffectData.DisplayLength = length;
		}
	}
}

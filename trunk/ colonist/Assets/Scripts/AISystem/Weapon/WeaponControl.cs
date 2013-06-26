using UnityEngine;
using System.Collections;

public interface WeaponControl {
	
	 IEnumerator _WeaponOn(float delay);
	
	 void _WeaponOff();

}

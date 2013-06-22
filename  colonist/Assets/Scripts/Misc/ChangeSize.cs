using UnityEngine;
using System.Collections;

public class ChangeSize : MonoBehaviour {
	
	public FloatCurve ChangeSizeCurve = new FloatCurve();
	
	// Use this for initialization
	void Start () {
	     StartCoroutine("ChangingSize");
	}
	
    IEnumerator ChangingSize()
    {
		float StartTime = Time.time;
		Vector3 _localScale = transform.localScale;
		while((Time.time - StartTime) <= Util.GetCurveMaxTime(ChangeSizeCurve.curve))
		{
			float sizeRate = ChangeSizeCurve.Evaluate(Time.time - StartTime);
			transform.localScale = new Vector3(_localScale.x * sizeRate, _localScale.y * 1, _localScale.z * sizeRate);
			yield return null;
		}
    }
	
}

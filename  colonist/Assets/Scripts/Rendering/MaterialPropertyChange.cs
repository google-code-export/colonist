using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// Material float property curve.
/// </summary>
[System.Serializable]
public class MaterialFloatPropertyCurve
{
	/// <summary>
	/// The name of this curve. Muse be unique.
	/// </summary>
	public string Name = "";
	
	/// <summary>
	/// The name of the property of the material. Something like Color, Strength
	/// </summary>
	public string PropertyName = "";
	
	/// <summary>
	/// The curve of the property value Vs time.
	/// </summary>
	public AnimationCurve curve = AnimationCurve.Linear(0,0,1,1);
	
	/// <summary>
	/// The index of the material to be changed with the curve.
	/// </summary>
	public int MaterialIndex = 0;
}

/// <summary>
/// Material color property curve.
/// </summary>
[System.Serializable]
public class MaterialColorPropertyCurve
{
	/// <summary>
	/// The name of the color curve.
	/// </summary>
	public string Name = "";
	
	/// <summary>
	/// The name of the color property.
	/// </summary>
	public string PropertyName = "";
	
	public Color StartColor = Color.white;
	
	public Color EndColor = Color.white;
	
	public float timeLength = 3;
	
	/// <summary>
	/// The index of the material to be changed with the curve.
	/// </summary>
	public int MaterialIndex = 0;
}

/// <summary>
/// MaterialPropertyChange handles the behavior to change a property value in a material.
/// Note: it aims to change an instance of the material, not to change the sharedmaterial in project asset folder.
/// </summary>
public class MaterialPropertyChange : MonoBehaviour {
	
	public MaterialFloatPropertyCurve[] materialFloatPropertyCurveArray = new MaterialFloatPropertyCurve[]{};
	
	public MaterialColorPropertyCurve[] materialColorPropertyCurveArray = new MaterialColorPropertyCurve[]{};
	
	IEnumerator ChangeFloatProperty(string curveName)
	{
	    MaterialFloatPropertyCurve floatPropertyCurve = materialFloatPropertyCurveArray.Where(x=>x.Name == curveName).First();
		float startTime = Time.time;
		while((Time.time - startTime) <= Util.GetCurveMaxTime(floatPropertyCurve.curve))
		{
			float _t = Time.time - startTime;
			float propertyValue = floatPropertyCurve.curve.Evaluate(_t);
			this.renderer.materials[floatPropertyCurve.MaterialIndex].SetFloat(floatPropertyCurve.PropertyName, propertyValue);
//			Debug.Log("Set float to:" + propertyValue);
			yield return null;
		}
	}
	
	IEnumerator ChangeColorProperty(string curveName)
	{
	    MaterialColorPropertyCurve colorPropertyCurve = materialColorPropertyCurveArray.Where(x=>x.Name == curveName).First();
		float startTime = Time.time;
		float maxTime = colorPropertyCurve.timeLength;
		while((Time.time - startTime) <= maxTime)
		{
			float _t = (Time.time - startTime) / maxTime;
			Color _color = Color.Lerp(colorPropertyCurve.StartColor, colorPropertyCurve.EndColor, _t);
			this.renderer.materials[colorPropertyCurve.MaterialIndex].SetColor(colorPropertyCurve.PropertyName, _color);
//			Debug.Log("Set color to:" + _color);
			yield return null;
		}
	}
}

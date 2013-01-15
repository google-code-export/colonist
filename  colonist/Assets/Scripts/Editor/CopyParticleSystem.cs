using UnityEngine;
using UnityEditor;
using System.Collections;

public class CopyParticleSystem  {

    [MenuItem("GameObject/ParticleSystem/Copy particle system component")]
    public static void CopyParticle()
    {
        GameObject[] gameObjects = Selection.gameObjects;
        GameObject particleObject = null;
        ParticleSystem particleSystem = null;
        foreach (GameObject o in gameObjects)
        {
            if ( (particleSystem = o.GetComponent<ParticleSystem>()) != null)
            {
                particleObject = o;
                break;
            }
        }
        if (particleSystem == null)
        {
            Debug.Log("No particle system is selected");
            return;
        }
        foreach (GameObject o in gameObjects)
        {
            if (o != particleObject)
            {
                _CopyParticleSystem(particleSystem, o);
            }
        }
    }

    static void _CopyParticleSystem(ParticleSystem original, GameObject copyTo)
    {
		ParticleSystem ps = null;
		if( (ps = copyTo .GetComponent<ParticleSystem>()) ==null)
		{
          ps = copyTo.AddComponent<ParticleSystem>();
		}
        SerializedObject newPsSerial = new SerializedObject(ps);
        SerializedObject srcPsSerial = new SerializedObject(original);
        SerializedProperty srcProperty = srcPsSerial.FindProperty("InitialModule.startLifetime");
		 
		LegacyToShurikenConverter.SetShurikenMinMaxCurve(newPsSerial, "InitialModule.startLifetime", 0.3f, 0.4f, true);
		newPsSerial.Update();
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// GlobalDecalType - defines the decal type for global decal object.
/// </summary>
public enum GlobalDecalType
{
#region human blood 0 - 100
    HumanBlood_Splatter_Static = 0,
    HumanBlood_Splatter_Dynamic = 3,
#endregion

#region xenz blood 101 - 200
    XenzBlood1 = 101,
    #endregion
}

public enum GlobalEffectType
{
    #region human blood 0 - 100
    HumanBlood_Splatter_Small = 0,
	HumanBlood_Splatter_Middle = 1,
	HumanBlood_Splatter_Large = 2,
    #endregion

    #region xenz blood 101 - 200
    XenzBloodSplatter = 101,
    #endregion
}

/// <summary>
/// GlobalDecalData defines ground and wall decal object for global setting.
/// Differs to DecalData in UnitData.cs, GlobalDecalData ignores project direction. 
/// It creates wall and ground decal both. 
/// If you want the decal only apply to wall, or only apply to ground, you can set the GroundLayer/WallLayer to None.
/// </summary>
[System.Serializable]
public class GlobalDecalData
{
    public string Name = "Global decal name";
    public GlobalDecalType DecalType = GlobalDecalType.HumanBlood_Splatter_Static;
    /// <summary>
    /// Destory decal after life time.
    /// </summary>
    public float DecalLifetime = 10;

    /// <summary>
    /// Scale rate = 1 * Random.range(ScaleRateMin, ScaleRateMax)
    /// </summary>
    public float ScaleRateMin = 0.5f;
    public float ScaleRateMax = 1f;
    /// <summary>
    /// The decal object on ground. Randomly select one when creating
    /// </summary>
    public GameObject[] Decal_OnGround = new GameObject[] { };
    /// <summary>
    /// The decal object on wall. Randomly select one when creating
    /// </summary>
    public GameObject[] Decal_OnWall = new GameObject[] { };
    public LayerMask GroundLayer;
    public LayerMask WallLayer;
}


/// <summary>
/// define global effect data which can be used by all units.
/// </summary>
[System.Serializable]
public class GlobalEffectData
{
    public string Name = "Global effect name";
    public GlobalEffectType EffectType = GlobalEffectType.HumanBlood_Splatter_Small;
    /// <summary>
    /// Destory efect after life time.
    /// </summary>
    public float EffectLifetime = 3;
    public Object[] Effect_Object = new Object[] { };
}

/// <summary>
/// All kinds of global effect & decal handler.
/// </summary>
public class GlobalBloodEffectDecalSystem : MonoBehaviour {

    public GlobalDecalData[] GlobalDecalData = new GlobalDecalData[] { };
    public IDictionary<GlobalDecalType, GlobalDecalData> GlobalDecalDataDict = new Dictionary<GlobalDecalType, GlobalDecalData>();

    public GlobalEffectData[] globalEffectDataArray = new GlobalEffectData[] { };
    public IDictionary<GlobalEffectType, GlobalEffectData> GlobalEffectDataDict = new Dictionary<GlobalEffectType, GlobalEffectData>();

    public static GlobalBloodEffectDecalSystem Instance;

	// Use this for initialization
	void Awake () {
        Instance = this;
        foreach (GlobalDecalData globalDecalData in GlobalDecalData)
        {
            GlobalDecalDataDict.Add(globalDecalData.DecalType, globalDecalData);
        }
        foreach (GlobalEffectData globalEffectData in globalEffectDataArray)
        {
            GlobalEffectDataDict.Add(globalEffectData.EffectType, globalEffectData);
        }
	}
	
	/// <summary>
	/// Creates the effect.
	/// center: optional, used when using effect type = global type.
	/// </summary>
	/// <param name='center'>
    public static void CreateEffect(EffectData effectData)
    {
        Instance.StartCoroutine(Instance._CreateEffect(effectData));
    }
	
    IEnumerator _CreateEffect(EffectData effectData)
    {
		//Then, wait for the delay
        if(effectData.CreateDelay)
		{
			yield return new WaitForSeconds(effectData.CreateDelayTime);
		}
		for(int i=0; i < effectData.Count; i++)
		{
			//if effectData is using global setting or create mode:
            if (effectData.UseGlobalEffect || effectData.InstantionType == EffectObjectInstantiation.create)
            {
				//decide the position to create the effect object
			    Vector3 instantiationPosition = Vector3.zero;
				try{
			       instantiationPosition = effectData.instantiationData.BasicAnchor.position;
				}
				catch(System.Exception exc)
				{
					Debug.LogError("Have trouble when access base anchor position on effect data:" + effectData.Name);
					Debug.LogError(exc.StackTrace);
				}
				
				//decide the quaternion to create the effectt object:
				Quaternion instantiationQuaternion = Quaternion.identity;
			    if(effectData.instantiationData.RandomPositionInsideSphere)
			    {
			       instantiationPosition += Random.insideUnitSphere * effectData.instantiationData.RandomSphereUnit;
			    }
			    instantiationPosition += effectData.instantiationData.WorldOffset;
			    switch(effectData.instantiationData.rotationOfInstance)
			    {
				case InstantiationRotationMode.IdentityQuaternion:
			       instantiationQuaternion = Quaternion.identity;
			       break;
				case InstantiationRotationMode.RandomQuaternion:
			       instantiationQuaternion = Random.rotation;
			       break;
				case InstantiationRotationMode.AlignToAnchor:
			       instantiationQuaternion = effectData.instantiationData.BasicAnchor.rotation;
			       break;
				case InstantiationRotationMode.SpecifiedQuaternion:
				   instantiationQuaternion = effectData.instantiationData.specifiedQuaterion;
			       break;
			    }
			  Object effectObjectPrototype = effectData.UseGlobalEffect ? Util.RandomFromArray<Object>(Instance.GlobalEffectDataDict[effectData.GlobalType].Effect_Object) 
					                                                    : effectData.EffectObject;
              Object effectObject = Object.Instantiate(effectObjectPrototype,
                                                 instantiationPosition,
                                                 instantiationQuaternion);
			  float LifeTime = effectData.UseGlobalEffect ? Instance.GlobalEffectDataDict[effectData.GlobalType].EffectLifetime : effectData.DestoryTimeOut;
              //Destory the effect object after life time, if life time > 0
				if(LifeTime > 0)
				{
                   Destroy(effectObject, LifeTime);
				}
            }
			else if(effectData.InstantionType == EffectObjectInstantiation.play && effectData.EffectObject != null)
			{
				GameObject effectObject = effectData.EffectObject;
				ParticleSystem ps = effectObject.GetComponent<ParticleSystem>();
				ps.Play();
			}
		}
    }
	
    /// <summary>
    /// CreateBloodDecal is used to create blood decal. 
    /// This method should be called by receive damage behavior when receiving damage.
    /// </summary>
    /// <param name="center"></param>
    /// <param name="DecalData"></param>
    public static void CreateBloodDecal(Vector3 center, DecalData DecalData)
    {
        //create predefine global decal
        if (DecalData.UseGlobalDecal)
        {
            GlobalDecalData globalDecalData = Instance.GlobalDecalDataDict[DecalData.GlobalType];
			if(globalDecalData.Decal_OnGround.Length > 0)
			{
               CreateBloodDecalOnGround(center,
                                        Util.RandomFromArray<Object>(globalDecalData.Decal_OnGround),
                                        globalDecalData.GroundLayer,
                                        true,
                                        globalDecalData.DecalLifetime,
                                        Random.Range(globalDecalData.ScaleRateMin, globalDecalData.ScaleRateMax)
                                        );
			}
			if(globalDecalData.Decal_OnWall.Length > 0)
			{
            CreateBloodDecalOnWall(center,
                                     Util.RandomFromArray<Object>(globalDecalData.Decal_OnWall),
                                     globalDecalData.WallLayer,
                                     true,
                                     globalDecalData.DecalLifetime,
                                     Random.Range(globalDecalData.ScaleRateMin, globalDecalData.ScaleRateMax)
                                     );
			}
        }
        //Create custom decal defined by Unit
        else
        {
            switch (DecalData.ProjectDirection)
            {
                //Create decal on ground
                case HorizontalOrVertical.Vertical:
                    CreateBloodDecalOnGround(center, 
                                             Util.RandomFromArray<Object>(DecalData.DecalObjects),
                                             DecalData.ApplicableLayer,
                                             DecalData.DestoryInTimeOut,
                                             DecalData.DestoryTimeOut,
                                             DecalData.ScaleRate
                                             );
                    break;
                //Create decal on wall
                case HorizontalOrVertical.Horizontal:
                    CreateBloodDecalOnWall(center, 
                                             Util.RandomFromArray<Object>(DecalData.DecalObjects),
                                             DecalData.ApplicableLayer,
                                             DecalData.DestoryInTimeOut,
                                             DecalData.DestoryTimeOut,
                                             DecalData.ScaleRate);
                    break;
            }
        }
    }

    static void CreateBloodDecalOnGround(Vector3 center, 
                                        Object decalObject,
                                        LayerMask groundLayer,
                                        bool HasLifetime,
                                        float Lifetime,
                                        float scaleRate
                                        )
    {
        float Radius = 1;
        Vector2 randomFromCircle = Random.insideUnitCircle;
        randomFromCircle *= Radius;
        Vector3 random = new Vector3(center.x + randomFromCircle.x, center.y + 3, center.z + randomFromCircle.y);
        RaycastHit hitInfo;

        if (Physics.Raycast(random, Vector3.down, out hitInfo, 999, groundLayer))
        {
            GameObject DecalObject = (GameObject)Object.Instantiate(decalObject, hitInfo.point + hitInfo.normal * 0.1f, Quaternion.identity);
            DecalObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            DecalObject.transform.RotateAround(DecalObject.transform.up, Random.Range(0, 360));
            DecalObject.transform.localScale *= scaleRate;
            if (HasLifetime)
                Destroy(DecalObject, Lifetime);
        }
    }

    static void CreateBloodDecalOnWall(Vector3 center, Object decalObject,
                                        LayerMask walllayer,
                                        bool HasLifetime,
                                        float Lifetime,
                                        float scaleRate)
    {
        float Radius = 2;
        //check front/back/right/left direction if there's a collision:
        Collider[] wallColliders = Physics.OverlapSphere(center, Radius, walllayer);
        if (wallColliders != null && wallColliders.Length > 0)
        {
            Collider wall = wallColliders[0];
            Vector3 closestPoints = wall.ClosestPointOnBounds(center);
            closestPoints.y += Random.Range(0.1f, 0.5f);
            //Randomize the point
            closestPoints += Random.onUnitSphere*1;
            RaycastHit hitInfo;
            if (Physics.Raycast(center, closestPoints - center, out hitInfo, 20, walllayer))
            {
                GameObject DecalObject = (GameObject)Object.Instantiate(decalObject, 
                                                                  hitInfo.point + hitInfo.normal * 0.1f,
                                                                  Quaternion.identity);
                DecalObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
                DecalObject.transform.RotateAround(DecalObject.transform.up, Random.Range(0, 360));
                DecalObject.transform.localScale *= scaleRate;
                if (HasLifetime)
                    Destroy(DecalObject, Lifetime);
            }
        }
    }

}

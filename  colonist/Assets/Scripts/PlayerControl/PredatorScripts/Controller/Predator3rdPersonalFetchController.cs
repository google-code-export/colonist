using UnityEngine;
using System.Collections;

[@RequireComponent(typeof(PredatorPlayerStatus))]
public class Predator3rdPersonalFetchController : MonoBehaviour {

    public string PrePunctureAnimation = "preattack_left";
    public string PrePunctureMixedAnimation = "preattack_left_mix";
    public string PunctureAnimation = "fetch_LeftClaw_puncture";
    public string FetchLiftAnimation = "fetch_LeftClaw_Puncture_Lift";
    public string FetchHoldAnimation = "fetch_LeftClaw_Hold";
    public string PreTossAnimation = "pretoss";
    public string PreTossMixedAnimation = "pretoss_mix";
    public string TossAnimation = "toss";

    public float FetchRadius = 3f;
    public float MinTossForce = 50f;
    public float MaxTossForce = 300f;
    public float PowerAccelerationTime = 1.3333f;

    /// <summary>
    /// FetchHoldAnimationLayer will be at higher layer as it will need to mix hold animation with other
    /// moving/attacking animation.
    /// For example, when predator is holding something, the hold animation should be player with attack/moving.
    /// </summary>
    public int AnimationLayer = 3;
    
    public Transform FetchAnchor = null;
    public LayerMask[] FetchableLayer;

    [HideInInspector]
    public bool HasFetchSomething = false;

    [HideInInspector]
    public float MaxPunctureTossPower = 100;

    private float PunctureTossPower = 0;
    private WrapMode FetchAnimationWrapMode = WrapMode.Default;
    private WrapMode FetchHoldAnimationWrapMode = WrapMode.ClampForever;
    private WrapMode TossAnimationWrapMode = WrapMode.Default;
    private GameObject currentFetchedObject = null;

    void InitAnimation()
    {
        PredatorPlayerStatus predatorStatus = GetComponent<PredatorPlayerStatus>();
        //1 - preattack
        animation[PrePunctureAnimation].wrapMode = WrapMode.ClampForever;
        animation[PrePunctureAnimation].speed = animation[PrePunctureAnimation].length / PowerAccelerationTime;
        animation[PrePunctureAnimation].layer = AnimationLayer;

        //2 - preattach mixed(walking):
        animation[PrePunctureMixedAnimation].wrapMode = WrapMode.ClampForever;
        animation[PrePunctureMixedAnimation].speed = animation[PrePunctureMixedAnimation].speed;
        animation[PrePunctureMixedAnimation].layer = AnimationLayer;
        animation[PrePunctureMixedAnimation].AddMixingTransform(GetComponent<PredatorPlayerStatus>().spine);

        //3 - pretoss
        animation[PreTossAnimation].wrapMode = WrapMode.ClampForever;
        animation[PreTossAnimation].speed = animation[PrePunctureAnimation].length / PowerAccelerationTime;
        animation[PreTossAnimation].layer = AnimationLayer;

        //4 - pretoss-walking
        animation[PreTossMixedAnimation].wrapMode = WrapMode.ClampForever;
        animation[PreTossMixedAnimation].speed = animation[PrePunctureAnimation].length / PowerAccelerationTime;
        animation[PreTossMixedAnimation].layer = AnimationLayer;
        animation[PreTossMixedAnimation].AddMixingTransform(GetComponent<PredatorPlayerStatus>().spine);


        //2 - puncture toss
        animation[PunctureAnimation].layer = AnimationLayer;
        animation[PunctureAnimation].wrapMode = FetchAnimationWrapMode;

        animation[FetchLiftAnimation].layer = AnimationLayer;
        animation[FetchLiftAnimation].wrapMode = FetchAnimationWrapMode;

        animation[FetchHoldAnimation].layer = AnimationLayer;
        animation[FetchHoldAnimation].wrapMode = FetchHoldAnimationWrapMode;
        animation[FetchHoldAnimation].AddMixingTransform(GetComponent<PredatorPlayerStatus>().leftUpperClaw);

        animation[TossAnimation].layer = AnimationLayer;
        animation[TossAnimation].wrapMode = TossAnimationWrapMode;
        animation[TossAnimation].AddMixingTransform(GetComponent<PredatorPlayerStatus>().root);
        animation[TossAnimation].AddMixingTransform(GetComponent<PredatorPlayerStatus>().rightUpperClaw);
        animation[TossAnimation].RemoveMixingTransform(GetComponent<PredatorPlayerStatus>().rightUpperClaw);
    }

    void Awake()
    {
        //InitAnimation();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (HasFetchSomething == false && animation.IsPlaying(FetchHoldAnimation))
        {
            animation.Stop(FetchHoldAnimation);
        }
        if (Input.GetKeyDown("t"))
        {
            animation.CrossFade(TossAnimation);
        }
	}



    public bool isPlayingFetchAnimation()
    {
        bool ret = animation.IsPlaying(PunctureAnimation) 
            || animation.IsPlaying(TossAnimation) 
            || animation.IsPlaying(FetchLiftAnimation);
        return ret;
    }

    IEnumerator Toss()
    {
        //Debug.Break();
        //Avoid frequent invoking
        if ((PredatorPlayerStatus.IsAttacking == true) || 
            (HasFetchSomething == false || currentFetchedObject == null))
        {
            yield break;
        }
        else
        {
            float TossTimePoint = (float)2 / (float)30;
            animation.CrossFade(TossAnimation);
            yield return new WaitForSeconds(TossTimePoint);
            float tossForce = PunctureTossPower;
            TossParameter tP = new TossParameter(this.gameObject, currentFetchedObject, tossForce, transform.forward);
            currentFetchedObject.SendMessage("BeingToss", tP);
            HasFetchSomething = false;
            currentFetchedObject = null;
        }
        yield return null;
    }

    IEnumerator PunctureAndFetch()
    {
        //Avoid frequent invoking
        if (PredatorPlayerStatus.IsAttacking == true || HasFetchSomething)
        {
            yield break;
        }
        if ((currentFetchedObject = FindFetchable(FetchRadius)) != null)
        {
            Util.RotateToward(transform, currentFetchedObject.collider.bounds.center, false, 0);
            float TargetHP = currentFetchedObject.GetComponent<UnitHealth>().GetCurrentHP();
            //Puncture only,  or puncture to kill then lift
            if (TargetHP <= PunctureTossPower)
            {
                animation.CrossFade(FetchLiftAnimation);
                yield return new WaitForSeconds(0.5f);
                //Send apply damage message
                DamageParameter dP = new DamageParameter(this.gameObject, DamageForm.Punctured, PunctureTossPower);
                dP.extraParameter.Add(DamageParameter.ExtraParameterKey.PuncturedAnchor, this.FetchAnchor);
                currentFetchedObject.SendMessage("ApplyDamage", dP);
                HasFetchSomething = true;
                float liftTime = animation[FetchLiftAnimation].length - 0.5f;
                animation.CrossFade(FetchLiftAnimation);
                yield return new WaitForSeconds(liftTime);
                animation.CrossFade(FetchHoldAnimation);
            }
            else
            {
                animation.CrossFade(PunctureAnimation);
                yield return new WaitForSeconds(0.5f);
                DamageParameter dP = new DamageParameter(this.gameObject, DamageForm.Predator_Strike_Single_Claw, PunctureTossPower);
                currentFetchedObject.SendMessage("ApplyDamage", dP);
            }

        }
        //No object fetched, animating only
        else
        {
            animation.CrossFade(PunctureAnimation);
            Debug.Log("No object to fetch!");
            //yield return new WaitForSeconds(FetchAnimationLength * 1.75f);
            //animation.Stop(PunctureAnimation);
        }
        yield return null;
    }

    void PunctureOrToss()
    {
        if (HasFetchSomething == false)
        {
            StartCoroutine(PunctureAndFetch());
        }
        else
        {
            StartCoroutine(Toss());
        }
    }

    void PunctureTossPowerUp()
    {
        if (HasFetchSomething == false)
        {
            string prePunctureAnim = (PredatorPlayerStatus.IsMoving) ? PrePunctureMixedAnimation : PrePunctureAnimation;
            PunctureTossPower = Mathf.Clamp((PunctureTossPower + (MaxPunctureTossPower / PowerAccelerationTime) * Time.deltaTime), 0, MaxPunctureTossPower);
            animation.CrossFade(prePunctureAnim);
        }
        else
        {
            string preTossAnim = (PredatorPlayerStatus.IsMoving) ? PreTossMixedAnimation : PreTossAnimation;
            PunctureTossPower = Mathf.Clamp((PunctureTossPower + (MaxPunctureTossPower / PowerAccelerationTime) * Time.deltaTime), MinTossForce, MaxTossForce);
            animation.CrossFade(preTossAnim);
        }

    }

    private GameObject FindFetchable(float radius)
    {
        GameObject ret = null;
        //Get fetched object
        //Collider[] colliders = Physics.OverlapSphere(this.transform.position, radius, Util.JoinLayerMask(FetchableLayer));
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, radius, FetchableLayer[0]);
        if (colliders != null && colliders.Length > 0)
        {
            Collider c = Util.FindClosest(Util.GetCharacterCenter(this.gameObject), colliders);
            ret = c.gameObject;
        }
        return ret;
    }
}

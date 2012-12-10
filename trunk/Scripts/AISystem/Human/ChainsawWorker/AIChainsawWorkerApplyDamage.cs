using UnityEngine;
using System.Collections;

[RequireComponent(typeof (AIChainsawWorker))]
public class AIChainsawWorkerApplyDamage : MonoBehaviour {
    
    public string[] ReceiveFrontDamageAnimations = new string[] { };
    public string[] ReceiveBackDamageAnimations = new string[] { "receive_damage", "receive_damage2" };

    /// <summary>
    /// Animation to react hit down, fall backward
    /// </summary>
    public string[] ReceiveHitDownBackwardDamageAnimation = new string[] { "hitAndFallBackward" };

    /// <summary>
    /// Animation to react hit down, fall forward
    /// </summary>
    public string[] ReceiveHitDownForwardDamageAnimation = new string[] { "hitAndFallForward" };

    /// <summary>
    /// Once apply hit power more than this value, the character will be hit to fall down
    /// </summary>
    public float StrikePowerToHitFall = 35f;

    private AIChainsawWorker AI;
    private float HP
    {
        get
        {
            return AI.GetHealth();
        }
    }
    private int HitCounter = 0;


    void InitAnimation()
    {
        //Receive damage animation at layer 2
        foreach (string receiveDamageAnimation in ReceiveFrontDamageAnimations)
        {
            animation[receiveDamageAnimation].wrapMode = WrapMode.Once;
            animation[receiveDamageAnimation].layer = 2;
        }
        foreach (string receiveDamageAnimation in ReceiveBackDamageAnimations)
        {
            animation[receiveDamageAnimation].wrapMode = WrapMode.Once;
            animation[receiveDamageAnimation].layer = 2;
        }
        foreach (string receiveDamageAnimation in ReceiveHitDownBackwardDamageAnimation)
        {
            animation[receiveDamageAnimation].wrapMode = WrapMode.Once;
            animation[receiveDamageAnimation].layer = 2;
        }
        foreach (string receiveDamageAnimation in ReceiveHitDownForwardDamageAnimation)
        {
            animation[receiveDamageAnimation].wrapMode = WrapMode.Once;
            animation[receiveDamageAnimation].layer = 2;
        }
    }

    void Awake()
    {
        AI = this.GetComponent<AIChainsawWorker>();
        InitAnimation();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        bool isFallingBackward = false;
        foreach (string ani in ReceiveHitDownBackwardDamageAnimation)
        {
            isFallingBackward = animation.IsPlaying(ani);
        }
        bool isFallingForward = false;
        foreach (string ani in ReceiveHitDownForwardDamageAnimation)
        {
            isFallingForward = animation.IsPlaying(ani);
        }
        if (isFallingBackward || isFallingForward)
        {
            AI.status = AIStatus.Frozen;
        }
        else if(AI.status == AIStatus.Frozen)
        {
            AI.RestorePreviousStatus();
        }
        //Debug.Log("Status:" + AI.status);
	}

    IEnumerator ApplyDamage(DamageParameter damageParam)
    {
        Transform trans = damageParam.src.transform;
        Vector3 attackerLocalPos = transform.InverseTransformPoint(trans.position);
        bool shouldFall = damageParam.damagePoint >= StrikePowerToHitFall;
        //Attacker in front
        if (attackerLocalPos.z >= 0)
        {
            if (shouldFall)
            {
                string fallAni = Util.RandomFromArray(ReceiveHitDownBackwardDamageAnimation);
                animation.CrossFade(fallAni);
                yield return new WaitForSeconds(animation[fallAni].length);
            }
            else
                animation.CrossFade(Util.RandomFromArray(ReceiveFrontDamageAnimations));
        }
        //Attacker in back
        else
        {
            if (shouldFall)
            {
                string fallAni = Util.RandomFromArray(ReceiveHitDownForwardDamageAnimation);
                animation.CrossFade(fallAni);
                yield return new WaitForSeconds(animation[fallAni].length);
            }
            else
                animation.CrossFade(Util.RandomFromArray(ReceiveBackDamageAnimations));

        }
        yield return null;
    }
}

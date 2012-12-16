using UnityEngine;
using System.Collections;
[RequireComponent(typeof(GetHP))]
[RequireComponent(typeof(AI))]
public class AIApplyDamage : MonoBehaviour {

    public ReceiveDamageBeheavior[] receiveDamageBeheavior;
	public string[] Animations;
	public int AnimationLayer = 3;
	public WrapMode AnimationWrapMode;
    public DieBeheavior[] dieBeheavior;
    public float HP
    {
        get
        {
            return GetComponent<GetHP>().HP;
        }
        set
        {
            GetComponent<GetHP>().HP = value;
        }
    }

    public Rigidbody LeftLeg = null;
    public Rigidbody RightLeg = null;
    public Rigidbody root = null;
    public Rigidbody Chest = null;

    public string GetFetchedAnimation = "getfetch";
	
	private CharacterController characterController = null;
	private float LastGetHitTime = -99;
	private AI AI;
    void Awake()
    {
        foreach (string ani in Animations)
        {
           animation[ani].wrapMode = AnimationWrapMode;
           animation[ani].layer = AnimationLayer;
        }
        Util.SetRagdoll(this.gameObject, false);
		characterController = GetComponent<CharacterController>();
        AI = GetComponent<AI>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("p"))
        {
            DamageParameter dp= new DamageParameter(this.gameObject, DamageForm.Predator_Strike_Single_Claw, 10);
            SendMessage("ApplyDamage", dp);
        }
	}
	
	void FixedUpdate()
	{
		if((Time.time - LastGetHitTime) > 1f)
		{
			AI.Halt = false;
		}
	}

    IEnumerator ApplyDamage(DamageParameter damageParam)
    {
        //Do nothing if already die
        if (HP <= 0)
        {
            yield break;
        }

        HP -= damageParam.damagePoint;
        AI.Halt = true;
        //Find the die beheavior which match to the damageForm
        if (HP <= 0)
        {
            DieBeheavior beheavior = DieBeheavior.FindMatchRandomly(dieBeheavior, damageParam.damageForm, dieBeheavior[0]);
            beheavior.damageParameter = damageParam;
            SendMessage("Die", beheavior);
            LevelManager.GameEvent(new GameEvent(GameEventType.NPCDie, this.gameObject));
            yield break;
        }
        //Process damage
        else
        {
			
            LevelManager.GameEvent(new GameEvent(GameEventType.NPCReceiveDamage, this.gameObject));
            Transform trans = damageParam.src.transform;
            Vector3 attackerLocalPos = transform.InverseTransformPoint(trans.position);

            //if Attacker stand in front, then assign MoveDirection.Backward, else assign MoveDirection.Forward
            MoveDirection direction = (attackerLocalPos.z >= 0) ? MoveDirection.Backward : MoveDirection.Forward;

            ReceiveDamageBeheavior RDBeheavior = ReceiveDamageBeheavior.FindMatchRandomly(damageParam.damageForm, direction, receiveDamageBeheavior, receiveDamageBeheavior[0]);

            //No Independent functioning is define, use default logic
            if (RDBeheavior.IndependentFunctioning == false)
            {
                //Now that the attack from front way, then we'll seek the receive damage beheavior which towards backward
                if (RDBeheavior.animation.Length != 0)
                {
					string ani = Util.RandomFromArray(RDBeheavior.animation);
					animation.Rewind();
                    animation.CrossFade(ani);
                }
//                if (RDBeheavior.canMove)
//                {
//                    float _time = Time.time;
//                    float length = animation[RDBeheavior.animation].length;
//                    while ((Time.time - _time) < length && characterController!=null)
//                    {
//                        Vector3 localMoveDir = direction == MoveDirection.Forward ? Vector3.forward : Vector3.back;
//                        Util.MoveTowards(transform, transform.TransformDirection(localMoveDir), characterController, RDBeheavior.MoveSpeed);
//                        yield return null;
//                    }
//                }
                if (RDBeheavior.Function != string.Empty)
                {
                    SendMessage(RDBeheavior.Function, damageParam);
                }
            }
            else
            {
                SendMessage(RDBeheavior.Function, damageParam);
            }
			LastGetHitTime = Time.time;
        }
        
        yield return null;
    }

    IEnumerator GetFetched(DamageParameter dP)
    {
        //DieBeheavior beheavior = DieBeheavior.FindMatchRandomly(dieBeheavior, DamageForm.Common, dieBeheavior[0]);
        //Die(beheavior);
        Util.SetRagdoll(this.gameObject, true);
        Transform puncturedAnchor = (Transform)dP.extraParameter[DamageParameter.ExtraParameterKey.PuncturedAnchor];
        Vector3 distance = puncturedAnchor.position - Chest.transform.position;
        //Attach the victim to the puncture anchor transform
        this.transform.position += distance;
        transform.parent = puncturedAnchor;
        //Disable root rigibody
        root.useGravity = false;
        root.isKinematic = true;
        yield return null;
    }

    void BeingToss(TossParameter tP)
    {
        //Detach the current object to let it toss!
        transform.parent = null;
        root.useGravity = true;
        root.isKinematic = false;
    }

    void Die(DieBeheavior beheavior)
    {
        if (beheavior.DieReplacement != null)
        {
            GameObject replacement = (GameObject)Object.Instantiate(beheavior.DieReplacement, transform.position, transform.rotation);
            if (beheavior.CopyTransfromToReplacement)
            {
                Util.CopyTransform(this.transform, replacement.transform);
            }
            Destroy(this.gameObject);
        }

        foreach (GameObject o in beheavior.ObjectToDestory)
        {
            Destroy(o);
        }
        CommonDie(beheavior.damageParameter);
        if (beheavior.FunctionName != string.Empty)
        {
            SendMessage(beheavior.FunctionName, beheavior.damageParameter);
        }
    }

	/// <summary>
	/// Common Die routine.
	/// 1. Send message "StopAI" to AISolder.
	/// 2. Activate the ragdoll in this gameobject
	/// 3. Add a random force to push the character to falldown
	/// </summary>
    void CommonDie(DamageParameter dP)
    {
        SendMessage("StopAI", SendMessageOptions.DontRequireReceiver);
        animation.Stop();
        Util.SetRagdoll(this.gameObject, true);

        Vector3 force = (this.transform.position - dP.src.transform.position).normalized;
        Chest.AddForce(transform.TransformDirection(force * Chest.mass * dP.damagePoint), ForceMode.Impulse);
    }

    /// <summary>
    /// When the character's body being hit by any importer
    /// </summary>
    void OnImpactorHit(Collision hit)
    {
        float damagePoint = Util.CalculateCollisionHitPower(hit.rigidbody.mass , hit.rigidbody.velocity.magnitude);
        DamageParameter damageParam = new DamageParameter(hit.gameObject, DamageForm.Collision, damagePoint);
        StartCoroutine("ApplyDamage", damageParam);
    }

}

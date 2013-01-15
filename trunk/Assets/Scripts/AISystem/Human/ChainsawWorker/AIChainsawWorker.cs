using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIChainsawWorker : MonoBehaviour {

    public string SawingAnimation = "sawing";
    public string TakeABreakAnimation = "takeABreak";

    public string[] ReceiveDamageAnimations = new string[] { "receive_damage", "receive_damage2" };   
    
    
    public string StandingWithSawAnimation = "standing_with_saw";

    public string[] AttackAnimations = new string[] { "sawAttacking1", "sawAttacking2" };

    public string OffenseRunAnimation = "run_with_saw";
    public string NormalWalkAnimation = "walk";
    public string WalkAnimation = "walk_with_saw";
    public string WalkRightwardAnimation = "walk_with_saw_rightward";
    public string WalkLeftwardAnimation = "walk_with_saw_leftward";
    public string WalkBackwardAnimation = "walk_with_saw_backward";

    public float RunToOffenseSpeed = 2f;
    public float WalkToOffenseSpeed = 1f;
    public float BackwardSpeed = 2f;
    public float MoveHorizontalSpeed = 0.4f;

    public float AttackRadius = 3f;
    public float RunToOffenseDistance = 20f;
    public float CautiousOffenseDistance = 10f;


    public bool canEscape = false;

    public ParticleSystem[] SawDust;

    /// <summary>
    /// The defaultBeheavior is the coroutine name
    /// </summary>
    public string defaultBeheavior = "Attack";

    [HideInInspector]
    public Transform currentTarget = null;
    [HideInInspector]
    //public AIStatus status = AIStatus.Idle;
    public AIStatus status
    {
        set
        {
            if (statusStack.Count == 0 || statusStack.Peek() != value)
            {
                statusStack.Push(value);
            }
        }
        get
        {
            return statusStack.Peek();
        }
    }

    private Stack<AIStatus> statusStack = new Stack<AIStatus>();

    private float CurrentSpeed = 1f;
    private float LastHitTime = 0f;
    private float LastHitAnimationDuration = 0f;

    public float HP = 100;
    /// <summary>
    /// HitCounter - how many times been hit
    /// </summary>
    private int HitCounter = 0;

    private CharacterController controller = null;

    public Transform BipSpine = null;


    void InitAnimation()
    {
        //Idle animation at layer 0
        animation[SawingAnimation].wrapMode = WrapMode.Loop;
        animation[SawingAnimation].layer = 0;

        animation[TakeABreakAnimation].wrapMode = WrapMode.Once;
        animation[TakeABreakAnimation].layer = 0;

        //Movement animation at layer 1
        string[] movementAnimations = new string[] {
            NormalWalkAnimation, WalkAnimation, WalkRightwardAnimation, WalkLeftwardAnimation,
            WalkBackwardAnimation , OffenseRunAnimation
        };
        foreach (string movementAni in movementAnimations)
        {
            animation[movementAni].wrapMode = WrapMode.Loop;
            animation[movementAni].layer = 1;
        }

        //Attack animation at layer 1
        foreach (string AttackAnimation in AttackAnimations)
        {
            animation[AttackAnimation].wrapMode = WrapMode.Once;
            animation[AttackAnimation].layer = 1;
        }

        animation[StandingWithSawAnimation].AddMixingTransform(BipSpine);

    }

    void Awake()
    {
        InitAnimation();
        controller = this.GetComponent<CharacterController>();
    }

	// Use this for initialization
	void Start () {
        //StartCoroutine("Sawing");
        
        StartCoroutine(defaultBeheavior);
        
	}
	
	// Update is called once per frame
	void Update () {
        if (HP > 0)
        {
            if ((Time.time - LastHitTime) > (LastHitAnimationDuration == 0 ? 1.5f : LastHitAnimationDuration))
            {
                CurrentSpeed = RunToOffenseSpeed;
            }
            HandleSawDustEmission();
            if (currentTarget != null && CanMove())
            {
                transform.LookAt(currentTarget);
            }
        }

        //Testing
        if (Input.GetKeyDown("t"))
        {
            TestingDieBehead();
        }
	}

    void HandleSawDustEmission()
    {
        if (animation.IsPlaying(SawingAnimation))
        {
            foreach (ParticleSystem SawDustParticle in SawDust)
            {
                SawDustParticle.enableEmission = true;
            }
        }
        else
        {
            foreach (ParticleSystem SawDustParticle in SawDust)
            {
                SawDustParticle.enableEmission = false;
            }
        }
    }

    IEnumerator Sawing()
    {
        status = AIStatus.Idle;
        while (true)
        {
            animation.CrossFade(SawingAnimation);
            yield return new WaitForSeconds(Random.Range(3f, 6f));
            animation.CrossFade(TakeABreakAnimation);
            yield return new WaitForSeconds(animation[TakeABreakAnimation].length + 0.1f);
        }
    }

    IEnumerator Attack()
    {
        status = AIStatus.Attacking;
        while (true)
        {
            //Find the alien creature to attack
            Collider[] colliders = Util.GetObjectsInLayer("xenz", this.transform.position, RunToOffenseDistance);
            if (colliders != null && colliders.Length > 0)
            {
                currentTarget = colliders[0].transform;
               // currentTarget = Xenz;
                float distance = Util.DistanceOfCharactersXZ(this.controller, currentTarget.GetComponent<CharacterController>());
                //If distance not close enough to attack
                if (distance > AttackRadius)
                {
                    if (distance > CautiousOffenseDistance)
                    {
                        if (CanMove())
                        {
                            Util.MoveTowards(this.transform, currentTarget.position, this.controller, true, false, this.RunToOffenseSpeed, 0);
                            animation.CrossFade(OffenseRunAnimation);
                        }
                    }
                    else
                    {
                        if (CanMove())
                        {
                            Util.MoveTowards(this.transform, currentTarget.position, this.controller, true, false, this.WalkToOffenseSpeed, 0);
                            animation.CrossFade(WalkAnimation);
                        }
                    }
                }
                //close enough, let attack begin!
                else
                {
                    animation.Stop(NormalWalkAnimation);
                    string _AttackAnimation = Util.RandomFromArray(this.AttackAnimations);
                    animation.CrossFade(_AttackAnimation);
                    float extendWaitSeconds = 0.0f;
                    yield return new WaitForSeconds(animation[_AttackAnimation].length + extendWaitSeconds);
                    //Attack over, now let the guy slip away for a short time
                    yield return StartCoroutine(SlipAway());
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// Slip away from enemy.
    /// 1. Backward in random seconds
    /// 2. Yield return slip right or slip left
    /// </summary>
    /// <returns></returns>
    IEnumerator SlipAway()
    {
        yield return StartCoroutine(BackwardForMoment(Random.Range(1.5f, 2.5f)));
        float holdTime = Random.Range(0.5f, 1.5f);
        yield return new WaitForSeconds(holdTime);
        string coRu = Util.RandomFromArray(new string[] {"MoveLeft", "MoveRight" });
        //yield return StartCoroutine(MoveRight(Random.Range(1.5f, 2.3f)));
        yield return StartCoroutine(coRu, Random.Range(1.5f, 2.3f));
        yield return null;
    }

    IEnumerator MoveLeft(float duration)
    {
        float _time = Time.time;
        while ((Time.time - _time) <= duration)
        {
            if (!CanMove())
                yield return null;
            Vector3 directionToTarget = currentTarget.position - transform.position;
            Quaternion q = transform.rotation;
            q.SetLookRotation(currentTarget.position);
            q.eulerAngles = new Vector3(0, -90, 0);
            Vector3 newDirection = q * directionToTarget;
            Util.MoveTowards(transform, newDirection, controller, MoveHorizontalSpeed);
            animation.CrossFade(WalkRightwardAnimation);
            yield return null;
        }
        yield return null;
    }

    IEnumerator MoveRight(float duration)
    {
        float _time = Time.time;
        while ((Time.time - _time) <= duration)
        {
            if (!CanMove())
                yield return null;
            Vector3 directionToTarget = currentTarget.position - transform.position;
            Quaternion q = transform.rotation;
            q.SetLookRotation(currentTarget.position);
            // q.eulerAngles = new Vector3(q.eulerAngles.x, q.eulerAngles.y, q.eulerAngles.z);
            q.eulerAngles = new Vector3(0, 90, 0);
            Vector3 newDirection = q * directionToTarget;
            Util.MoveTowards(transform, newDirection, controller, MoveHorizontalSpeed);
            animation.CrossFade(WalkRightwardAnimation);
            yield return null;
        }
        yield return null;
    }

    IEnumerator BackwardForMoment(float duration)
    {
        transform.LookAt(currentTarget.position);
        Quaternion q = this.transform.rotation;
        q.eulerAngles= new Vector3(q.eulerAngles.x, q.eulerAngles.y + Random.Range(-30, 30), q.eulerAngles.z);
        this.transform.rotation = q;

        Vector3 direction = this.transform.TransformDirection(Vector3.back);

        float _time = Time.time;
        while ((Time.time - _time) <= duration)
        {
            if (CanMove())
               controller.SimpleMove(direction * BackwardSpeed);
            animation.CrossFade(WalkBackwardAnimation); 
            yield return null;
        }
        animation.Stop(WalkBackwardAnimation);
        animation.CrossFade(StandingWithSawAnimation);
        yield return null;
    }

    IEnumerator ApplyDamage(DamageParameter damageParam)
    {
        HitCounter++;
        HP -= damageParam.damagePoint;

        //if (HP > 0)
        //{
        //    //Play from beginning every time receives a damage
        //    CurrentSpeed = 0;
        //    string getHitAni = Util.RandomFromArray(this.ReceiveDamageAnimations);
        //    animation.Rewind(getHitAni);
        //    animation.CrossFade(getHitAni);
        //    LastHitTime = Time.time;
        //    LastHitAnimationDuration = animation[getHitAni].length;
        //    yield return new WaitForSeconds(animation[getHitAni].length);
        //    //For the first time get hit, stop sawing and escape from the monster!
        //    if (HitCounter == 1)
        //    {
        //        if (canEscape)
        //        {
        //            StopCoroutine(defaultBeheavior);
        //            animation.Stop();
        //            CurrentSpeed = RunToOffenseSpeed;
        //            Invoke("Escape", animation[getHitAni].length);
        //        }
        //        else
        //        {
        //            StopCoroutine("Sawing");
        //            animation.Stop();
        //            StartCoroutine("Attack");
        //        }
        //    }
        //}
        //else
        //{
        //    StopAllCoroutines();
        //    animation.Stop();
        //    SendMessage("Die", damageParam.damageForm);
        //    currentTarget = null;
        //}
        yield return null;
    }

    void RestoreSpeed()
    {
        CurrentSpeed = RunToOffenseSpeed;
    }

    void Escape()
    {
        StartCoroutine(Escaping());
    }

    IEnumerator Escaping()
    {
        animation.CrossFade(NormalWalkAnimation);
        while (true)
        {
            if (HP <= 0)
            {
                animation.Stop();
                yield break;
            }
            Vector3 velocity = transform.forward * CurrentSpeed * Time.deltaTime;
            if(CanMove())
               controller.SimpleMove(velocity);
            yield return null;
        }
    }

    public float GetHealth()
    {
        return HP;
    }

    public bool CanMove()
    {
        return status != AIStatus.Frozen;
    }

    public void NewStatus(AIStatus s)
    {
        this.status = s;
    }

    public void RestorePreviousStatus()
    {
        this.statusStack.Pop();
    }

    public void TestingDieBehead()
    {
        DamageParameter dp = new DamageParameter(null, DamageForm.Predator_Waving_Claw, HP);
        ApplyDamage(dp);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, RunToOffenseDistance);
    }
}

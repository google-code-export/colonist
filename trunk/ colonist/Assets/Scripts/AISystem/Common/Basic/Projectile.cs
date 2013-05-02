using UnityEngine;
using System.Collections;


/// <summary>
/// A projectile = any flying bullet, like bullet/arrow/missile/grenade.
/// Requires Collider & RigiBody component when MovementMode = StarightForward.
/// </summary>
public class Projectile : MonoBehaviour {

    public ProjectileAttackType AttackType = ProjectileAttackType.SingleTarget;

    public ProjectileMovementMode MovementMode = ProjectileMovementMode.StraightLineToPosition;

    /// <summary>
    /// Is projectile able to self-guided to target?
    /// If SelfGuide = false, TargetPosition, VerticalRate is used
    /// If SelfGuide = true, SelfGuideToTarget is used
    /// </summary>
    public bool SelfGuide;
    /// <summary>
    /// Speed.
    /// </summary>
    public float Speed = 20;

    /// <summary>
    /// Only used when ProjectileMovementMode = Parabola.
    /// When Radian = 1, Height = Length,
    /// When Radian = 0, Height = 0,
    /// When Radian = 0.5, Height = Length * 0.5
    /// </summary>
    public float Radian = 0.2f;

    /// <summary>
    /// Used only when movement mode = StarightForward.
    /// The projectile should be destory when ExpiredTime comes.
    /// </summary>
    public float LifeTime = 2;

    /// <summary>
    /// When type = Explosion, all unit in AttackableLayer, within AttackableRange, will be sent applydamage message.
    /// When MovementMode = StarightForward, hitting with attackableLayer collider with be set as %Target% variable
    /// </summary>
    public LayerMask AttackableLayer;
    public float ExplosiveRange;
    /// <summary>
    /// If IsExplosiveDamping, farer unit receives lesser damage
    /// </summary>
    public bool IsExplosiveDamping = false;
    /// <summary>
    /// When Projectile reach target/position, the HitEffect to be created. 
    /// Can be null
    /// </summary>
    public GameObject HitEffect;
    public float HitEffectTimeout;

#region vaiables to be assigned in AI script
    /// <summary>
    /// The gameObject which fires the projectile.
    /// </summary>
    [HideInInspector]
    public GameObject Src;
    /// <summary>
    /// The gameObject that the projectile target to.
    /// </summary>
    [HideInInspector]
    public GameObject Target;

    [HideInInspector]
    public DamageParameter DamageParameter;

    protected Vector3 StartPosition = Vector3.zero;
    /// <summary>
    /// Used in MovementMode = StarightForward.
    /// </summary>
    protected bool HitSomething = false;
    protected GameObject HitObject = null;
#endregion

    void Awake()
    {
        StartPosition = transform.position;
    }

    void Start()
    {
        StartCoroutine("Launch");
        Destroy(this.gameObject, LifeTime);
    }

    /// <summary>
    /// Launch the projectile.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Launch()
    {
        switch (this.MovementMode)
        {
            case ProjectileMovementMode.StraightForward:
                //For StarightForward moving object, it's mandatory to set a expiration time and destory the gameobject.
                yield return StartCoroutine(MoveStraightForward(transform.forward, Speed));
                break;
            case ProjectileMovementMode.StraightLineToPosition:
                if (SelfGuide)
                {
                    yield return StartCoroutine(SelfGuide_StraightLine(Target.transform, Speed));
                }
                else
                {
                    yield return StartCoroutine(MoveInStraightLine(transform.position, Target.transform.position, Speed));
                }
                break;
            case ProjectileMovementMode.Parabola:
                if (SelfGuide)
                {
                    yield return StartCoroutine(SelfGuide_ParabolaLine(Target.transform, Speed, Radian));
                }
                else
                {
                    yield return StartCoroutine(MoveInParabolaLine(transform.position, Target.transform.position, Speed, Radian));
                }
                break;
        }
       
        //Create hit effect:
        if (HitEffect != null)
        {
            Object effectObject = Object.Instantiate(HitEffect, transform.position, transform.rotation);
//			Debug.Log("Instantiate effectObject:" + HitEffect.name + " at pos:" + transform.position);
            if (HitEffectTimeout > 0)
            {
                Destroy(effectObject, HitEffectTimeout);
            }
        }
        //set applydamage message
        
        DoDamage();
        
        //Destory this gameobject:
        Destroy(gameObject);
    }

    /// <summary>
    /// Send ApplyDamage to target
    /// </summary>
    public virtual void DoDamage()
    {
        switch (this.AttackType)
        {
            case ProjectileAttackType.SingleTarget:
                if (HitSomething == true && HitObject != null)
                {
                    HitObject.SendMessage("ApplyDamage", DamageParameter);
                }
                break;
            case ProjectileAttackType.Explosion:
                //Pick every unit in explosive range, and send ApplyDamage message to each of them
                foreach (Collider c in Physics.OverlapSphere(transform.position, ExplosiveRange, AttackableLayer))
                {
                    float distance = Util.DistanceOfCharacters(gameObject, c.gameObject);
                    if (IsExplosiveDamping)
                    {
                        if (distance < ExplosiveRange)
                        {
                            //Farer unit receives lesser damage
                            float DampingFactor = (ExplosiveRange - distance) / ExplosiveRange;
                            DamageParameter.damagePoint *= DampingFactor;
                            c.SendMessage("ApplyDamage", DamageParameter);
                        }
                    }
                    else
                    {
                        c.SendMessage("ApplyDamage", DamageParameter);
                    }
                }
                break;
        }
    }


    /// <summary>
    /// If SelfGuide = true , selfguide to the target
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual IEnumerator SelfGuide_StraightLine(Transform target, float Speed)
    {
        while (Vector3.Distance(target.transform.position, transform.position) > 0.5f && HitSomething == false)
        {
            Vector3 velocity = (target.transform.position - transform.position).normalized * Speed;
            transform.LookAt(transform.position + velocity);
            transform.position += velocity * Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Transform move from start -> target along a straight line
    /// </summary>
    /// <param name="StartPosition"></param>
    /// <param name="TargetPosition"></param>
    /// <param name="Speed"></param>
    /// <returns></returns>
    public virtual IEnumerator MoveStraightForward(Vector3 Direction, float Speed)
    {
        Vector3 velocity = Direction.normalized * Speed;
        transform.LookAt(transform.position + velocity);
        while (HitSomething == false)
        {
            transform.position += velocity * Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Transform move from start -> target along a straight line
    /// </summary>
    /// <param name="StartPosition"></param>
    /// <param name="TargetPosition"></param>
    /// <param name="Speed"></param>
    /// <returns></returns>
    public virtual IEnumerator MoveInStraightLine(Vector3 StartPosition, Vector3 TargetPosition, float Speed)
    {
        Vector3 velocity = (TargetPosition - StartPosition).normalized * Speed;
        float totalTime = (TargetPosition - StartPosition).magnitude / Speed;
        transform.LookAt(TargetPosition);
        float starttime = Time.time;
        while ((Time.time - starttime) <= totalTime && HitSomething == false)
        {
            transform.position += velocity * Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Projectile move along parabola line.
    /// Formula 1: V = gravity * time
    /// Formula 2: Height = V_Init * time + 0.5 * gravity * time * time
    /// With the two formula, we're about to calculate the gravity and initial velocity.
    /// </summary>
    /// <param name="HorzontialForwardSpeed"></param>
    /// <param name="Distance"></param>
    /// <param name="HeightDistanceRate"></param>
    /// <returns></returns>
    public virtual IEnumerator MoveInParabolaLine(Vector3 StartPosition, Vector3 TargetPosition, float Speed, float Radian)
    {
		//calculate the distance of start to end position
        float Distance = Vector3.Distance(TargetPosition, StartPosition);
		//calculate the total time 
        float totalTime = Distance / Speed;
		//calculate the total height
        float Height = Distance * Radian;
		//calculate the rising time, which half of the total time in the air.
        float RisingTime = totalTime / 2;
        float upwardInitalSpeed, gravity = 0;//V = gravity * RisingTime
        //gravity * RisingTime * RisingTime + 0.5 * gravity * RisingTime * RisingTime = Height
        gravity =(float)(Height / (1.5 * RisingTime * RisingTime));
        upwardInitalSpeed = gravity * RisingTime;

        Vector3 forwardVelocity = (TargetPosition - StartPosition).normalized * Speed;
        Vector3 velocity = forwardVelocity;
        velocity.y = upwardInitalSpeed;
        float StartTime = Time.time;
        while ((Time.time - StartTime) <= totalTime && HitSomething == false)
        {
            transform.LookAt(transform.position + velocity);
            transform.position += velocity * Time.deltaTime;
            velocity.y -= gravity * Time.deltaTime;
            yield return null;
        }
    }

    public virtual IEnumerator SelfGuide_ParabolaLine(Transform target, float Speed, float Radian)
    {
        Vector3 TargetInitialPosition = target.transform.position;
        float Distance = Vector3.Distance(TargetInitialPosition, StartPosition);
        float totalTime = Distance / Speed;
        float Height = Distance * Radian;
        float RisingTime = totalTime / 2;
        float upwardInitalSpeed, gravity = 0;//V = gravity * RisingTime
        //gravity * RisingTime * RisingTime + 0.5 * gravity * RisingTime * RisingTime = Height
        gravity = (float)(Height / (1.5 * RisingTime * RisingTime));
        upwardInitalSpeed = gravity * RisingTime;
        Vector3 forwardVelocity = (TargetInitialPosition - StartPosition).normalized * Speed;
        Vector3 velocity = forwardVelocity;
        velocity.y = upwardInitalSpeed;
        float StartTime = Time.time;
        while (HitSomething == false)
        {
            //Different to normal ParabolaLine moving, after projectile reaching heightest point, not apply gravity, but 
            //use selfguide velocity only
            if ((Time.time - StartTime) <= RisingTime)
            {
                transform.LookAt(transform.position + velocity);
                transform.position += velocity * Time.deltaTime;
                velocity.y -= gravity * Time.deltaTime;
            }
            else
            {
                velocity = (target.position - transform.position).normalized * Speed;
                transform.LookAt(transform.position + velocity);
                transform.position += velocity * Time.deltaTime;
            }
            yield return null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
//        Debug.Log("OnTriggerEnter Hit with:" + other.gameObject.name);
        HitSomething = true;
        if (Util.CheckLayerWithinMask(other.gameObject.layer, this.AttackableLayer))
        {
            HitObject = other.gameObject;
        }
    }

    void OnTriggerStay(Collider other)
    {
//        Debug.Log("OnTriggerStay Hit with:" + other.gameObject.name);
        HitSomething = true;
        if (Util.CheckLayerWithinMask(other.gameObject.layer, this.AttackableLayer))
        {
            HitObject = other.gameObject;
        }
    }
}

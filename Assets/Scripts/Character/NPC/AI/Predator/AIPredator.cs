using UnityEngine;
using System.Collections;

[RequireComponent(typeof (CharacterController))]
public class AIPredator : MonoBehaviour {

    public bool roamer = true;
	public float OffenseRadius = 25f;
    public float OffsensiveRange = 15f;
    public float AttackRange = 10f;

    public float lowSpeed = 5f;
    public float normalSpeed = 7f;
    public float highSpeed = 10f;

    public LayerMask enemyLayer;

    [HideInInspector]
    public Vector3? nextPatrolPoint;
    [HideInInspector]
    public bool hurt;
    [HideInInspector]
    public float health;

    public static float MaxHealth = 100f;
    /// <summary>
    /// The current object Predator is attacking at
    /// </summary>
    private GameObject enemy = null;
    private int TerrainLayerMask = 1 << 10;
    private Vector3 bornPosition;
    private NavMeshAgent navMeshAgent;
    private bool HealthBelow75 = false;
    private CharacterController controller;
    private void SetupAnimation()
    {
        Transform boneRoot = transform.root.Find("CATBugParent");
        Transform pelvis = transform.root.Find("CATBugParent/BugBone-Pelvis");
        Transform frontLeftLeg = transform.root.Find("CATBugParent/BugBone-Pelvis/BugBone-Front-Left-Upper-Leg");
        Transform frontRightLeg = transform.root.Find("CATBugParent/BugBone-Pelvis/BugBone-Front-Right-Upper-Leg");
        Transform rearLeftLeg = transform.root.Find("CATBugParent/BugBone-Pelvis/BugBone-Rear-Left-ForeLimb");
        Transform rearRightLeg = transform.root.Find("CATBugParent/BugBone-Pelvis/BugBone-Rear-Right-ForeLimb");
        Transform spine = transform.root.Find("CATBugParent/BugBone-Pelvis/BugBone-Spine");
        Transform body = transform.root.Find("CATBugParent/BugBone-Pelvis/BugBone-Spine/BugBone-Body");

        animation["walk"].AddMixingTransform(pelvis, false);
        animation["walk"].AddMixingTransform(frontRightLeg, true);
        animation["walk"].AddMixingTransform(frontLeftLeg, true);
        animation["walk"].AddMixingTransform(rearLeftLeg, true);
        animation["walk"].AddMixingTransform(rearRightLeg, true);
        animation["walk"].AddMixingTransform(body, true);

        animation["threating"].wrapMode = WrapMode.Once;
        animation["threating2"].wrapMode = WrapMode.Once;
        //Debug.Log("Time:" + animation["threating2"].time + " Length:" + animation["threating2"].length);

        animation["attack"].wrapMode = WrapMode.ClampForever;
 
    }

    public void MinusHealth(float offset)
    {
        health -= offset;
        //When health below 75%, trigger a hurt animation
        if (health <= MaxHealth * 0.75f && HealthBelow75 == false)
        {
            hurt = true;
            HealthBelow75 = true;
        }
    }

    void Awake()
    {
        bornPosition = this.transform.position;
        //navMeshAgent = this.GetComponent<NavMeshAgent>();
        controller = this.GetComponent<CharacterController>();
    }

	// Use this for initialization
	void Start () {
        SetupAnimation();
        if (roamer)
        {
            StartCoroutine(Offense());
        }
        //Time.timeScale = 0.1f;
	}

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    hurt = true;
        //}
    }

    IEnumerator Offense()
    {
        bool hasActThreat = false;
        //Debug.Log("#0 frame:" + Time.frameCount);
        bool GoOn = true;
        while (GoOn)
        {
            if (enemy == null)
            {
                enemy = FindEnemy();
            }
            if (enemy == null)
            {
                //do nothing
                yield return null;
            }
            else
            {
                //Debug.Log("#1 frame:" + Time.frameCount);
                //Threat for once only
                if (hasActThreat == false)
                {
                    hasActThreat = true;
                    Util.RotateToward(this.transform, enemy.transform.position, false, 0);
                    yield return StartCoroutine(Threating());
                }
                if (hurt)
                {
                    yield return StartCoroutine(CheckHurt());
                }
                animation.CrossFade("walk2");
                Util.MoveTowards(this.transform, enemy.transform.position, controller, true, true, highSpeed, 30);

                // Yield execution of this coroutine and return to the main loop until next frame
                if (Vector3.Distance(this.transform.position, enemy.transform.position) <= AttackRange)
                {
                    yield return StartCoroutine(AttackEnemy());
                }
                yield return null;
            }
        }
    }


    private IEnumerator Threating()
    {
        //Debug.Log("In Threating:" + Time.frameCount);
        float animationStartTime = Time.time;
        string animationName = "";
        Random.seed = System.DateTime.Now.Millisecond;

        float randomValue = Random.Range(0.0f, 20.0f);
        //Debug.Log("In Threating, random value:" + randomValue);
        if (randomValue > 10)
        {
            animationName = "threating2";
        }
        else
        {
            animationName = "threating";
        }
        while (true)
        {
            //Debug.Log("Threating(): framecount-" + Time.frameCount + " time-" + Time.time);
            if ((Time.time - animationStartTime) > animation[animationName].length)
            {
                break;
            }
            animation.CrossFade(animationName);
            yield return null;
        }
        animation.Stop(animationName);
        yield return null;
    }


    private IEnumerator AttackEnemy()
    {
        while (true)
        {
            if (hurt)
            {
                yield return StartCoroutine(CheckHurt());
            }


            // Yield execution of this coroutine and return to the main loop until next frame
            if (Vector3.Distance(this.transform.position, enemy.transform.position) > AttackRange)
            {
                animation.CrossFade("walk2");
                Util.MoveTowards(this.transform, enemy.transform.position, this.GetComponent<CharacterController>(), true, true, lowSpeed, 30);
                yield return null;
            }
            else
            {
                animation.CrossFade("attack");
                //Debug.Log("Animation attack current time:" + animation["attack"].time + " total length:" + animation["attack"].length);
                if (animation["attack"].time > animation["attack"].length)
                {
                    animation["attack"].time = 0;
                    yield return StartCoroutine(AdjustPositionInAttacking());
                }
                yield return null;
            }
        }
    }

    /// <summary>
    /// 让怪物在目标面前的一个扇形区域内徘徊
    /// After one whack,  predator move along a sector edge surrond the target
    /// </summary>
    /// <returns></returns>
    private IEnumerator AdjustPositionInAttacking()
    {
        float durationTime = 1;
        float startTime = Time.time;
        Random.seed = System.DateTime.Now.Millisecond;
        float angleAround = Random.RandomRange(-1.5f,1.5f);
        while ((Time.time - startTime) <= durationTime)
        {
            if (hurt)
            {
                yield return StartCoroutine(CheckHurt());
            }
            animation.CrossFade("walkLeft");
            if (enemy == null)
            {
                Debug.Log("Enemy is null!");
            }
            Vector3 oldPosition = transform.position;

            this.transform.RotateAround(enemy.transform.position, Vector3.up, angleAround);
            
            Vector3 oldPositionLocal = this.transform.InverseTransformPoint(oldPosition);
            Debug.Log("oldPositionLocal: " + oldPositionLocal);
            yield return null;
        }
        yield return null;
    }

    public IEnumerator CheckHurt()
    {
        float _startTime = Time.time;
        string animationHurt = "hurted";
        if (hurt)
        {
            while ((Time.time - _startTime) <= animation[animationHurt].length)
            {
                animation.CrossFade(animationHurt);
                yield return null;
            }
            hurt = false;
            yield return null;
        }
        yield return null;
    }

    private GameObject FindEnemy()
    {
        Collider[] colldier = Physics.OverlapSphere(this.transform.position, OffsensiveRange, enemyLayer.value);
        if (colldier != null && colldier.Length > 0)
        {
            if (colldier.Length == 1)
            {
                return colldier[0].gameObject;
            }
            else
            {
                Random.seed = System.DateTime.Now.Millisecond;
                int index = Random.RandomRange(0, colldier.Length);
                return colldier[index].gameObject;
            }
        }
        else return null;
    }

    private Vector3? PickRandomPatrolPoint(float radius)
    {
         Vector3? nextPoint = null;
         float randomAngle = Random.Range(0, 360);
         Vector3 Pivot = bornPosition + new Vector3(Mathf.Cos(randomAngle) * radius,
                                            100,
                                           Mathf.Sin(randomAngle) * radius);
         Debug.DrawLine(Pivot, Pivot + Vector3.down * 1000, Color.white);
         RaycastHit hit;
         bool isHit = Physics.Raycast(Pivot, Vector3.down, out hit, 1000f, TerrainLayerMask);
         //Debug.Log("isHit:" + isHit + hit.point);
         if (isHit)
         {
              nextPoint = hit.point;
         }
         return nextPoint;
    }

    void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, OffenseRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, OffsensiveRange);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(this.transform.position, AttackRange);
    }
}

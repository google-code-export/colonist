using UnityEngine;
using System.Collections;

[RequireComponent(typeof (CharacterController))]
public class Citizen : MonoBehaviour {

    public Transform Biped = null;
    public Transform Chest = null;
    public WayPoint StartWaypoint = null;
    public float WalkSpeed = 0.4f;
    public float RotationSpeed = 10f;
    public float HealthPoint = 100f;

    private string WalkAnimationName = "walk2";
    private bool isBleeding = false;
    private float DistanceToReachWayPoint = 0.5f;
    private WayPoint currentWaypoint;
    private CharacterController controller = null;

    private bool isReceivingDamage = false;
    private bool isFetched = false;
    private Transform FetchAnchor = null;
    private string AnimationReceiveDamageName = "receive_damage";


    void InitAnimation()
    {
        animation["struggle"].wrapMode = WrapMode.Loop;
    }

	// Use this for initialization
	void Awake () {
        InitAnimation();
        if (currentWaypoint == null)
        {
            currentWaypoint = StartWaypoint;
        }
        controller = this.GetComponent<CharacterController>();
	}

    void Start()
    {
        StartCoroutine("StartAI");
    }
	
	// Update is called once per frame
	void Update () {
        if (isFetched)
        {
            Vector3 v_des = FetchAnchor.position - Chest.position;
            this.transform.position += v_des;
        }
	}

    IEnumerator StartAI()
    {
        bool AIGo = true;
        while (AIGo)
        {
			if(currentWaypoint != null)
			{
              //Check waypoint reaching
              if (Util.Distance_XZ(this.transform.position, currentWaypoint.transform.position) <= DistanceToReachWayPoint)
              {
                  currentWaypoint = currentWaypoint.NextWaypoint();
              }
              Util.MoveTowards(this.transform, currentWaypoint.transform.position, controller, true, (RotationSpeed>0), WalkSpeed, RotationSpeed);
              animation.CrossFade(WalkAnimationName);
			}
            yield return null;
        }
        yield return null;
    }

    void OnAttacked(float StrikePoint)
    {
        StopAllCoroutines();
        this.HealthPoint -= StrikePoint;
        if (HealthPoint <= 0)
        {
            animation.Stop();
            SendMessage("Die");
        }
        else{
           if (isReceivingDamage == false)
           {
               StopAllCoroutines();
               StartCoroutine("ReceiveDamage", StrikePoint);
           }
        }
    }

    IEnumerator ReceiveDamage(float StrikePoint)
    {
        isReceivingDamage = true;
        float _time = Time.time;
        float animationLength = animation[AnimationReceiveDamageName].length;
        while ((Time.time - _time) <= animationLength)
        {
             animation.CrossFade(AnimationReceiveDamageName);
             yield return null;
        }
        //After animating receive damage, back to AI control
        StartCoroutine("StartAI");
        isReceivingDamage = false;

        yield return null;
    }

    IEnumerator Die()
    {
        yield return null;
    }

    void OnFetched(Transform FetchAnchor)
    {
        StopAllCoroutines();
       // this.transform.parent = FetchAnchor;
        //Vector3 v_des = FetchAnchor.position - Chest.position;
        //this.transform.position += v_des;
        //this.transform.rotation = FetchAnchor.rotation;
        isFetched = true;
        this.FetchAnchor = FetchAnchor;
        this.transform.parent = FetchAnchor;
        animation.CrossFade("struggle");
        this.controller.enabled = false;
      //  Debug.Log("Help, I'm captured!");
       // Invoke("FetchDie", 3);
        isBleeding = true;
    }

    void FetchDie()
    {
        this.animation.Stop();
        Rigidbody[] rigis = this.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rigi in rigis)
        {
            if (rigi.gameObject != Chest.gameObject)
            {
                rigi.isKinematic = false;
                rigi.useGravity = true;
            }
        }
    }
}

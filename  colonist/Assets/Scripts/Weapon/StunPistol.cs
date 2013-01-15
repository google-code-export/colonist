using UnityEngine;
using System.Collections;

public class StunPistol : MonoBehaviour {
    public ParticleSystem electric;
    public Transform ParticlePivot ;

    public BoxCollider BoundToDetectHit = null;

    public float HitPower = 10f;
	public bool paralysisTaget = true;
    void Awake()
    {
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        

	}

    void Fire(Transform target)
    {
        Object.Instantiate(electric, ParticlePivot.position, ParticlePivot.rotation);
        CharacterController controller = target.GetComponent<CharacterController>();
        bool isHit = BoundToDetectHit.bounds.Intersects(controller.collider.bounds);
        if (isHit)
        {
            DamageParameter dP = new DamageParameter(this.gameObject, DamageForm.ElectricityBoltHit, HitPower);
            target.SendMessage("ApplyDamage", dP);
			if(paralysisTaget)
			{
			   target.SendMessage("Decelerate",0.8f);
			}
        }
    }

    void StopAI()
    {
        Destroy(this);
    }
}

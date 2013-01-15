using UnityEngine;
using System.Collections;

/// <summary>
/// GuidedBullet - the bullet has a target point, it will fly towards the target point
/// </summary>
public class GuidedBullet : MonoBehaviour {
    public float speed = 30f;
    private float startTime;
    public float lifeTime = 5f;
    public bool canFly = false;
    public Vector3 targetPoint;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
        Vector3 forwardDirection = targetPoint - this.transform.position;
       // this.transform.rotation.SetLookRotation(forwardDirection, Vector3.up);
        this.transform.rotation = Quaternion.LookRotation(forwardDirection);
	}
	
	// Update is called once per frame
	void Update () {
        if (canFly)
        {
            Flying();
        }
        if (Time.time - startTime >= lifeTime)
        {
            if (this.renderer != null)
            {
                this.renderer.enabled = false;
            }
             
            Destroy(this.gameObject);
        }
	}

    void Flying()
    {
        Vector3 forwardDirection = targetPoint - this.transform.position;
        forwardDirection = forwardDirection.normalized;
        this.transform.position += forwardDirection * speed;
        RaycastHit hit;
        bool isHit = Physics.Raycast(this.transform.position, forwardDirection, out hit, 10f); 

        if (isHit)
        {
            OnBulletHit(hit);
        }
    }

    void OnBulletHit(RaycastHit hit) 
    {
      //  Debug.DrawLine(hit.point, hit.point + hit.normal * 100, Color.red);
         
        GameObject gameObject = hit.collider.gameObject;
        ReceiveDamage receiveDamage = gameObject.transform.root.gameObject.GetComponent<ReceiveDamage>();
        if (receiveDamage != null)
        {
//            receiveDamage.DoDamage(new DamageParameter(10));
        }

        HitVisualEffect hitEffect = gameObject.transform.root.gameObject.GetComponent<HitVisualEffect>();
        if (hitEffect != null)
        {
            GameObject DieReplacement = hitEffect.GetBulletHitEffect();
            if (DieReplacement != null)
            {
                GameObject.Instantiate(DieReplacement, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
 
        if (this.renderer != null)
        {
            this.renderer.enabled = false;
        }

        Destroy(this.gameObject);
    }

}

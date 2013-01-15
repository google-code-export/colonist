using UnityEngine;
using System.Collections;

/// <summary>
/// A bullet which fly without a target point , it will fly along fixed direction.
/// </summary>
public class FreeBullet : MonoBehaviour {
    public float speed = 30f;
    public float lifeTime = 5f;
    public float damage = 10f;



    private Vector3 direction;

    public Vector3 Direction
    {
        get { return direction; }
        set { direction = value.normalized; }
    }

    private float startTime;
    
	// Use this for initialization
	void Start () 
    {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Time.time - startTime >= lifeTime)
        {
            if (this.renderer != null)
            {
                this.renderer.enabled = false;
            }

            Destroy(this.gameObject);
        }
	}

    void FixedUpdate()
    {
        this.transform.position += direction * speed;
        CheckHit();
    }

    void CheckHit()
    {
        RaycastHit hit;
        bool isHit = Physics.Raycast(this.transform.position, direction, out hit, 10f);
        //If the bullet hit anything
        if (isHit)
        {
            OnBulletHit(hit);
        }
    }


    void OnBulletHit(RaycastHit hit)
    {
        GameObject gameObject = hit.collider.gameObject;
        ReceiveDamage receiveDamage = gameObject.transform.root.gameObject.GetComponent<ReceiveDamage>();
        if (receiveDamage != null)
        {
            //receiveDamage.DoDamage(new DamageParameter(this.gameObject, damage));
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

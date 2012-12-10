using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BugAnimation))]
public class BugReceiveDamage : ReceiveDamage{
    public float health = 100f;
    public bool isAlive = true;
    public GameObject DieReplacement = null;
    private BugAnimation bugAnimation = null;

	// Use this for initialization
	void Start () {
        bugAnimation = this.GetComponent<BugAnimation>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public override bool IsAlive()
    {
        return isAlive;
    }


    public override IEnumerator DoDamage(DamageParameter damage)
    {
        if (isAlive == false)
        {
            yield return null;
        }
        else if (health <= 0 && isAlive)
        {
            isAlive = false;
            StartCoroutine(Die());
        }
        else
        {
            health -= damage.damagePoint;
        }
		yield return null;
    }

    private IEnumerator Die()
    {
        float animationDuration = 0.1f;
        float waitTime = 0.3f;
        StartCoroutine(PlayDieAnimation(animationDuration));
        yield return new WaitForSeconds(waitTime);
        Destroy(this.gameObject);
        Debug.Log("Bug die!");
        if (DieReplacement != null)
        {
            GameObject corpse = (GameObject)Object.Instantiate(DieReplacement, this.transform.position, this.transform.rotation);
            copyTransform(this.transform, corpse.transform);
        }
       
    }

    private IEnumerator PlayDieAnimation(float playTime)
    {
        float startTime = Time.time;
        while (Time.time <= startTime + playTime)
        {
            bugAnimation.wound();
            yield return null;
        }
    }

    private void copyTransform(Transform src, Transform dst)
    {
        dst.position = src.position; 
        dst.rotation = src.rotation;
        foreach (Transform child in dst)
        {
            Transform _src = src.Find(child.name);
            if (_src != null)
            {
                copyTransform(_src, child);
            }
        }
    }
}

using UnityEngine;
using System.Collections;

public class BloodStainOnGround : MonoBehaviour {
    public float FadeLength = 2;
    public float FadeRate = 3;


    protected float FadeRateX, FadeRateY, FadeRateZ;

    void Awake()
    {
    }

	// Use this for initialization
	void Start () {
        StartCoroutine("Fade");
	}

    IEnumerator Fade()
    {
        float StartTime = Time.time;
        while ((Time.time - StartTime) <= FadeLength)
        {
            Vector3 newScale = new Vector3(FadeRate*Time.deltaTime,1,FadeRate*Time.deltaTime);
            transform.localScale += newScale;
            yield return null;
        }
    }
}

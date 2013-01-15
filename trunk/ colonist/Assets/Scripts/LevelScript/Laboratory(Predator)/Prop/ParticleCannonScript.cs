using UnityEngine;
using System.Collections;

public class ParticleCannonScript : MonoBehaviour {

    public ParticleSystem CannonParticle;
    public ParticleSystem RaisingBubble;
    public ParticleSystem SphereSpreadBubble;
    public ParticleSystem Warmhole;
    public ParticleEmitter LightingOrb;
    public LightningBolt_Scripted[] LightingEmitter;

	// Use this for initialization
	void Start () {
        WorkAtLowestPower();

	}
	
	// Update is called once per frame
	void Update () {
        //if (Input.GetKeyDown("l"))
        //{
        //    WorkAtLowestPower();
        //}
        //if (Input.GetKeyDown("m"))
        //{
        //    WorkAtMiddlePower();
        //}
        //if (Input.GetKeyDown("t"))
        //{
        //    WorkAtTopPower();
        //}
        //if (Input.GetKeyDown("y"))
        //{
        //    WorkAtTopestPower();
        //}
        //if (Input.GetKeyDown("c"))
        //{
        //    StartCoroutine(CreateWarmHole());
        //}
	}

    void WorkAtLowestPower()
    {
        CannonParticle.enableEmission = true;
        RaisingBubble.enableEmission = false;
        SphereSpreadBubble.enableEmission = false;
        Warmhole.enableEmission = false;
        LightingOrb.enabled = false;
        LightingOrb.emit = false;
        
        foreach (LightningBolt_Scripted t in LightingEmitter)
        {
            t.enabled = false;
        }
    }

    void WorkAtMiddlePower()
    {
        CannonParticle.enableEmission = true;
        CannonParticle.Play();
        RaisingBubble.enableEmission = true; 
        RaisingBubble.Play();
        SphereSpreadBubble.enableEmission = true; 
        SphereSpreadBubble.Play();

        Warmhole.enableEmission = false;
        LightingOrb.enabled = false;
        foreach (LightningBolt_Scripted t in LightingEmitter)
        {
            t.enabled = false;
        }
    }

    void CutParticle()
    {
        CannonParticle.enableEmission = true;
        CannonParticle.Play();
        RaisingBubble.enableEmission = true;
        RaisingBubble.Play();
        SphereSpreadBubble.enableEmission = true;
        SphereSpreadBubble.Play();

        Warmhole.enableEmission = true;
        Warmhole.Play();
        LightingOrb.gameObject.active = false;
        foreach (LightningBolt_Scripted t in LightingEmitter)
        {
            t.enabled = false;
        }
    }

    public IEnumerator CreateWarmHole()
    {
        WorkAtLowestPower();
        yield return new WaitForSeconds(1);
        //Middle power level since 1 sec
        RaisingBubble.enableEmission = true;
        RaisingBubble.Play();
        SphereSpreadBubble.enableEmission = true;
        SphereSpreadBubble.Play();

        yield return new WaitForSeconds(4);
        //Top power level since 4 sec
        Warmhole.enableEmission = true;
        Warmhole.Play();
        yield return new WaitForSeconds(1);
        //Lightning line since 6 sec
        foreach (LightningBolt_Scripted t in LightingEmitter)
        {
            t.enabled = true;
        }
        float _t = Time.time;
        //Strengthen lightning line for 4 sec
        while ((Time.time - _t ) <= 4)
        {
            foreach (LightningBolt_Scripted t in LightingEmitter)
            {
                t.particleEmitter.maxSize += 0.05f;
            }
            yield return new WaitForSeconds(0.1f);
        }
        //LightningOrb since 10 sec
        LightingOrb.enabled = true;
        LightingOrb.emit = true;
        
        yield break;
    }
}

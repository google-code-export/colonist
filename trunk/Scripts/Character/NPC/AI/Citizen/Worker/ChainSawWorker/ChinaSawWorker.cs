using UnityEngine;
using System.Collections;

public class ChinaSawWorker : MonoBehaviour {

    public GameObject chainSaw = null;
    public int ChainSawMaterialIndex = 1;
    public string sawingAnimation = "sawing";
    public string breakAnimation = "TakeABreak";
    public float chainSawOffset = 0.1f;
    public ParticleSystem[] sawingParticle = null;

    private float sawingAnimationLength = 0f;
    private float breakAnimationLength = 0f;
    private GameObject chainSawParent = null;
    void Awake()
    {
        sawingAnimationLength = animation[sawingAnimation].length;
        breakAnimationLength = animation[breakAnimation].length;
        chainSawParent = chainSaw.transform.parent.gameObject;
    } 

	// Use this for initialization
	void Start () {
        SendMessage("StartAI");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    bool isWorking = true;
    IEnumerator StartAI()
    {
        while (isWorking)
        {
            yield return StartCoroutine(Sawing());
            detachChainsaw();
            yield return StartCoroutine(TakeABreak());
            attachChainsaw();
        }
        yield return null;
    }

    IEnumerator Sawing()
    {
        //Play sawing X4 times
        int animationCount = 4;
        setParticleEmission(true);
        for (int i = 0; i < animationCount; i++)
        {
            float _time = Time.time;
            while ((Time.time - _time) <= sawingAnimationLength)
            {
                animation.CrossFade(sawingAnimation);
                Material sawMaterl = chainSaw.renderer.materials[ChainSawMaterialIndex];
                Vector2 matOffset = sawMaterl.GetTextureOffset("_MainTex");
                matOffset.x += chainSawOffset;
                sawMaterl.SetTextureOffset("_MainTex", matOffset);
                yield return null;
            }
        }
        yield return null;
    }

    IEnumerator TakeABreak()
    {
        float _time = Time.time;
        setParticleEmission(false);
        while ((Time.time - _time) <= breakAnimationLength)
        {
            animation.CrossFade(breakAnimation);
            yield return null;
        }
        yield return null;
    }

    private void detachChainsaw()
    {
        chainSaw.transform.parent = null;
    }

    private void attachChainsaw()
    {
        chainSaw.transform.parent = this.chainSawParent.transform;
    }

    private void setParticleEmission(bool emission)
    {
        foreach (ParticleSystem particleSystem in this.sawingParticle)
        {
            particleSystem.enableEmission = emission;
        }
    }
}

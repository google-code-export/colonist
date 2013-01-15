using UnityEngine;
using System.Collections;

public class BugAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
        setupBlendAniation("wound");
	}
    
    

    void Update()
    {
    }

    public void idle()
    {
        this.animation.CrossFade("idle");
    }

    public void wound()
    {
        this.animation.Blend("wound");
    }

    private void setupBlendAniation(string animationState)
    {
        animation[animationState].layer = 0;
        animation[animationState].blendMode = AnimationBlendMode.Blend;
        animation[animationState].wrapMode = WrapMode.Once;
        animation[animationState].weight = 1f;
    }

    private void setupAdditiveAnimation(string animationState)
    {
        animation[animationState].layer = 10;
        animation[animationState].blendMode = AnimationBlendMode.Blend;
        animation[animationState].wrapMode = WrapMode.Once;
        animation[animationState].weight = 1f;
    }

}

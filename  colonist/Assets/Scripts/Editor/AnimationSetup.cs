using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class AnimationSetup  {

    [MenuItem("Component/Animation/SortAnimationState")]
    public static void SortAnimationState()
	{
		GameObject selectedObject = Selection.activeGameObject;
		IList<AnimationClip> animationList = new List<AnimationClip>();
		foreach(AnimationState animationState in selectedObject.animation)
		{
			if(animationState != null && animationState.clip != null)
			{
			   animationList.Add(animationState.clip);
			}
		}
		foreach(AnimationClip animationClip in animationList)
		{
			selectedObject.animation.RemoveClip(animationClip);
		}
		
		animationList = animationList.OrderBy(x=>x.name).ToList<AnimationClip>();
		for(int i =0; i <animationList.Count(); i++)
		{
			AnimationClip clip = animationList[i];
			selectedObject.animation.AddClip(clip,clip.name);
		}
	}
}

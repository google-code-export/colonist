using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
public class Predator3rdPersonalStealthController : MonoBehaviour
{
    public SkinnedMeshRenderer MeshRender;
    public Material SkinMaterial;
    public Material CloakSkinMaterial;
    public GameObject Projector;
    public float FadeTime = 0.2f;
    public string NormalLayer = "xenz-player";
    public string CloakLayer = "xenz-cloak";
    const float OutlineWidth = 0.0085f;
    private bool stealth = false;
    private float singleAnimation;

    void Awake()
    {
       // OutlineWidth = SkinMaterial.GetFloat("_Outline");
        //divide animation into three(reduce _Outline,increase _ClipValue,decrease _Alpha)
        singleAnimation = FadeTime / 3;
    }

    // Use this for initialization
    void Start()
    {
    }


    void Update()
    {

    }

    public void ToggleStealth()
    {
        if (stealth)
            SendMessage("CloakOut");
        else
            SendMessage("CloakIn");
    }

    /// <summary>
    /// on receive damage 
    /// </summary>
    /// <param name="param"></param>
    void ApplyDamage(DamageParameter param)
    {
        if (param.damagePoint > 0 && stealth)
            SendMessage("CloakOut");
    }

    /// <summary>
    /// Gradually cloaking predator to be invisible
    /// </summary>
    /// <returns></returns>
    IEnumerator CloakIn()
    {
        stealth = true;
        StopCoroutine("CloakOut");
        //Decrease outline of material
        yield return StartCoroutine(Util.ExecFunctionWithGradualParameter(OutlineWidth, 0, i => this.SkinMaterial.SetFloat("_Outline", i), singleAnimation));

        Projector.active = false;

        // Change material to cloaking material
        MeshRender.material = CloakSkinMaterial;

        CloakSkinMaterial.SetFloat("_Alpha", 1f);

        //Clip pixel and decrease alpha
        yield return StartCoroutine(Util.ExecFunctionWithGradualParameter(0f, 1f, i => CloakSkinMaterial.SetFloat("_ClipValve", i), singleAnimation));
        yield return StartCoroutine(Util.ExecFunctionWithGradualParameter(1f, 0.05f, i => CloakSkinMaterial.SetFloat("_Alpha", i), singleAnimation));

        //finally, change the object's layer
        Util.ChangeGameObjectLayer(this.gameObject, LayerMask.NameToLayer(this.CloakLayer), true);

        LevelManager.SendAIMessage("ResetTarget", LayerMask.NameToLayer(this.NormalLayer));
    }

    /// <summary>
    /// Gradually uncloaking predator to be visible
    /// </summary>
    /// <returns></returns>
    IEnumerator CloakOut()
    {
        stealth = false;
        StopCoroutine("CloakIn");
        //increase alpha & decrease clip valve
        yield return StartCoroutine(Util.ExecFunctionWithGradualParameter(0.05f, 0.3f, i => CloakSkinMaterial.SetFloat("_Alpha", i), singleAnimation));
        yield return StartCoroutine(Util.ExecFunctionWithGradualParameter(1f, 0f, i => CloakSkinMaterial.SetFloat("_ClipValve", i), singleAnimation));

        Projector.active = true;
       // BugBone.shader = OutlineShader;
        MeshRender.material = SkinMaterial;
        //increase outline width
        yield return StartCoroutine(Util.ExecFunctionWithGradualParameter(0,
                                                          OutlineWidth, i => SkinMaterial.SetFloat("_Outline", i), 
                                                          singleAnimation));

        //finally, change the object's layer
        Util.ChangeGameObjectLayer(this.gameObject, LayerMask.NameToLayer(this.NormalLayer), true);
    }

    
}

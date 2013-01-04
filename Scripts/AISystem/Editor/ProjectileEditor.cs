using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ProjectileEditor : EditorWindow
{

    [MenuItem("Component/AI/Projectile Editor")]
    public static void EditAI()
    {
        EditorWindow.GetWindow(typeof(ProjectileEditor));
    }
    float MainWindowWidth, MainWindowHeight = 0;
    Projectile Projectile;
    Vector2 ScrollPosition = Vector2.zero;
    void OnGUI()
    {
        MainWindowWidth = position.width;
        MainWindowHeight = position.height;
        GameObject selectedGameObject = Selection.activeGameObject;
        if (selectedGameObject == null)
        {
            Debug.LogWarning("No gameObject is selected.");
            return;
        }
        //Attach Projectile script button
        if (selectedGameObject.GetComponent<Projectile>() == null)
        {
            Rect newAIScriptButton = new Rect(0, 0, MainWindowWidth - 10, 30);
            if (GUI.Button(newAIScriptButton, "Attach Projectile script"))
            {
                selectedGameObject.AddComponent<Projectile>();
            }
            return;
        }
        Projectile = selectedGameObject.GetComponent<Projectile>();

        if (GUILayout.Button("Save object"))
        {
            EditorUtility.SetDirty(Projectile);
        }

        ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, false, true, null);
        EditProjectile();
        EditorGUILayout.EndScrollView();

    }

    public virtual void EditProjectile()
    {
        Projectile.AttackType = (ProjectileAttackType)EditorGUILayout.EnumPopup(new GUIContent("Attack Type:", "射弹伤害类型: 唯一目标/爆炸范围伤害"), Projectile.AttackType);
        Projectile.MovementMode = (ProjectileMovementMode)EditorGUILayout.EnumPopup(new GUIContent("Movement mode:", "射弹移动类型: 直线前进/直线向特定地点/抛物线"), Projectile.MovementMode);
        Projectile.Speed = EditorGUILayout.FloatField(new GUIContent("Speed:", ""), Projectile.Speed);
        Projectile.LifeTime = EditorGUILayout.FloatField(new GUIContent("LifeTime:", "射弹生命时间. 如果射弹一直没有击中目标,则在生命时间到达后会自动销毁对象."), Projectile.LifeTime);
        Projectile.AttackableLayer = EditorGUILayoutx.LayerMaskField("Target Unit layer:", Projectile.AttackableLayer, false);
        if (Projectile.MovementMode == ProjectileMovementMode.Parabola || Projectile.MovementMode == ProjectileMovementMode.StraightLineToPosition)
        {
            Projectile.SelfGuide = EditorGUILayout.Toggle(new GUIContent("Selfguided:", "射弹自导?"), Projectile.SelfGuide);
        }
        if (Projectile.MovementMode == ProjectileMovementMode.Parabola)
        {
            Projectile.Radian = EditorGUILayout.FloatField(new GUIContent("Radian:", "射弹弧度 - 弧度=0时射弹直线前进, 弧度=1时射弹抛物线最高点等于前进总长度."), Projectile.Radian);
        }
        if (Projectile.AttackType == ProjectileAttackType.Explosion)
        {
            Projectile.ExplosiveRange = EditorGUILayout.FloatField(new GUIContent("Explosive range:", "爆炸范围."), Projectile.ExplosiveRange);
            Projectile.IsExplosiveDamping = EditorGUILayout.Toggle(new GUIContent("Damp explosive damage:", "爆炸伤害是否会随着距离衰减,默认衰减算法是线性算法."), Projectile.IsExplosiveDamping);
        }
        Projectile.HitEffect = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Hit Effect:", "命中目标后创建的效果对象"), Projectile.HitEffect,typeof(GameObject));
        if (Projectile.HitEffect != null)
        {
            Projectile.HitEffectTimeout = EditorGUILayout.FloatField(new GUIContent("Effect timeout:", "HitEffect在Timeout后销毁"), Projectile.HitEffectTimeout);
        }
    }
}

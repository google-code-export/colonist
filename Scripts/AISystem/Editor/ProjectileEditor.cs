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
        Projectile.AttackType = (ProjectileAttackType)EditorGUILayout.EnumPopup(new GUIContent("Attack Type:", "�䵯�˺�����: ΨһĿ��/��ը��Χ�˺�"), Projectile.AttackType);
        Projectile.MovementMode = (ProjectileMovementMode)EditorGUILayout.EnumPopup(new GUIContent("Movement mode:", "�䵯�ƶ�����: ֱ��ǰ��/ֱ�����ض��ص�/������"), Projectile.MovementMode);
        Projectile.Speed = EditorGUILayout.FloatField(new GUIContent("Speed:", ""), Projectile.Speed);
        Projectile.LifeTime = EditorGUILayout.FloatField(new GUIContent("LifeTime:", "�䵯����ʱ��. ����䵯һֱû�л���Ŀ��,��������ʱ�䵽�����Զ����ٶ���."), Projectile.LifeTime);
        Projectile.AttackableLayer = EditorGUILayoutx.LayerMaskField("Target Unit layer:", Projectile.AttackableLayer, false);
        if (Projectile.MovementMode == ProjectileMovementMode.Parabola || Projectile.MovementMode == ProjectileMovementMode.StraightLineToPosition)
        {
            Projectile.SelfGuide = EditorGUILayout.Toggle(new GUIContent("Selfguided:", "�䵯�Ե�?"), Projectile.SelfGuide);
        }
        if (Projectile.MovementMode == ProjectileMovementMode.Parabola)
        {
            Projectile.Radian = EditorGUILayout.FloatField(new GUIContent("Radian:", "�䵯���� - ����=0ʱ�䵯ֱ��ǰ��, ����=1ʱ�䵯��������ߵ����ǰ���ܳ���."), Projectile.Radian);
        }
        if (Projectile.AttackType == ProjectileAttackType.Explosion)
        {
            Projectile.ExplosiveRange = EditorGUILayout.FloatField(new GUIContent("Explosive range:", "��ը��Χ."), Projectile.ExplosiveRange);
            Projectile.IsExplosiveDamping = EditorGUILayout.Toggle(new GUIContent("Damp explosive damage:", "��ը�˺��Ƿ�����ž���˥��,Ĭ��˥���㷨�������㷨."), Projectile.IsExplosiveDamping);
        }
        Projectile.HitEffect = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Hit Effect:", "����Ŀ��󴴽���Ч������"), Projectile.HitEffect,typeof(GameObject));
        if (Projectile.HitEffect != null)
        {
            Projectile.HitEffectTimeout = EditorGUILayout.FloatField(new GUIContent("Effect timeout:", "HitEffect��Timeout������"), Projectile.HitEffectTimeout);
        }
    }
}

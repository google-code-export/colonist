using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Linq;

public class EditorClone : EditorWindow
{

    [MenuItem("Window/Copy MonoBehaviors And Data")]
    public static void CreateWindow()
    {
        var win = EditorWindow.GetWindow(typeof(EditorClone));
    }


    public GameObject source = null;
    public GameObject destination = null;
    public Dictionary<Type, bool> clones = new Dictionary<Type, bool>();
    bool selectAll;

    void OnGUI()
    {
        var originalSource = source;
        source = (GameObject)EditorGUILayout.ObjectField("Source:", source, typeof(GameObject), true);
        destination = (GameObject)EditorGUILayout.ObjectField("Destination:", destination, typeof(GameObject), true);
        bool isChanged = originalSource != source;
        if (source != null)
        {
            if (isChanged)
            {
                clones.Clear();
                selectAll = false;
            }

            foreach (var item in source.GetComponents<Component>())
            {
                bool value;
                if (!clones.TryGetValue(item.GetType(), out value))
                    clones.Add(item.GetType(), value = false);

                if (item.GetType() != typeof(Transform))
                    clones[item.GetType()] = EditorGUILayout.Toggle(item.GetType().Name, value);
            }

            EditorGUILayout.Space();

            //if checkbox changed
            if (selectAll != EditorGUILayout.Toggle("Select All", selectAll))
            {
                selectAll = !selectAll;
                foreach (var item in clones.Keys.ToArray())
                    clones[item] = selectAll;
            }
        }

        if (GUILayout.Button("Copy"))
        {
            if (source != destination && source != null)
                Copy();

        }
    }

    public void Copy()
    {
        var name = destination.name;
        foreach (var kv in clones)
        {
            if (kv.Value)
            {
                var newBehavior = destination.GetComponent(kv.Key) ?? destination.AddComponent(kv.Key);
                var oldBehavior = source.GetComponent(kv.Key);

                DeepCopy(kv.Key, oldBehavior, newBehavior);
            }
        }
        //sometimes,it may change the name
        destination.name = name;
		destination.layer = source.layer;
    }

    private static bool IsUnityObjectType(Type type)
    {
        return type.IsSubclassOf(typeof(UnityEngine.Object));
    }

    private static bool IsEditableUserType(Type type)
    {
        return type.IsSerializable && type.GetConstructor(new Type[0]) != null;
    }

    private static bool IsSystemType(Type type)
    {
        return type.IsPrimitive || type == typeof(string);
    }

    private static bool IsEditable(Type type)
    {
        return IsUnityObjectType(type) || IsEditableUserType(type) || IsSystemType(type) || type.IsValueType;
    }

    private void CopyAnimation(Animation oldObj, Animation newObj)
    {
        AnimationUtility.SetAnimationClips(newObj, AnimationUtility.GetAnimationClips(oldObj));
		newObj.cullingType = oldObj.cullingType;
    }

    private object DeepCopy(Type type, object oldObj, object newObj = null)
    {
        if (IsSystemType(type) || type.IsValueType)
            return oldObj;
        else if (IsUnityObjectType(type) && newObj == null)
            return DeepCopyUnityObject(type, (UnityEngine.Object)oldObj);
        else if (type == typeof(Transform))//transform on destination GameObject        
            return newObj;
        else if (type == typeof(Animation))
        {
            CopyAnimation((Animation)oldObj, (Animation)newObj);
            return newObj;
        }


        if (newObj == null)
        {
            if (type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                Debug.LogError("cannot create a user-defined script without gameobject");
            }
            newObj = Activator.CreateInstance(type);
        }

        //unity defined object,such as CharacterController
        if (!type.IsSubclassOf(typeof(MonoBehaviour)) && type.IsSubclassOf(typeof(UnityEngine.Object)))
        {
            foreach (var prop in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                if (prop.CanRead && prop.CanWrite)
                    try
                    {
                        prop.SetValue(newObj, prop.GetValue(oldObj, null), null);
                    }
                    catch
                    {//ignore...
                    }
            }
        }

        //inspector can only edit public fields
        foreach (var field in type.GetFields())
        {
            //inspector cannot edit
            if (field.GetCustomAttributes(typeof(HideInInspector), false).Length > 0)
                continue;

            if (field.FieldType.IsArray && IsEditable(field.FieldType.GetElementType()))
            {
                var elementType = field.FieldType.GetElementType();
                var oldArray = (Array)field.GetValue(oldObj);
                if (oldArray == null)
                    continue;

                var newArray = Array.CreateInstance(elementType, oldArray.Length);

                for (int i = 0; i < oldArray.Length; i++)
                    newArray.SetValue(DeepCopy(elementType, oldArray.GetValue(i)), i);


                field.SetValue(newObj, newArray);
            }
            else if (!field.IsLiteral && IsEditable(field.FieldType))
                field.SetValue(newObj, DeepCopy(field.FieldType, field.GetValue(oldObj)));


        }

        return newObj;
    }

    private UnityEngine.Object DeepCopyUnityObject(Type type, UnityEngine.Object oldObj)
    {
        if (!oldObj)
            return oldObj;
        else if (AssetDatabase.IsMainAsset(oldObj) || AssetDatabase.IsSubAsset(oldObj))//asset objects
            return oldObj;



        GameObject copyee = type == typeof(GameObject) ? (GameObject)oldObj : ((Component)oldObj).gameObject;
        if (copyee.transform.IsChildOf(source.transform))
        {
            var clone = Duplicate(copyee);
            if (type == typeof(GameObject))
                return clone;
            else
            {
                var result=clone.GetComponent(type)??clone.AddComponent(type);
                return (UnityEngine.Object)DeepCopy(type, oldObj, result);//copy component data
            }
        }
        else
            Debug.Log(string.Format("Can't copy the scene object: {0}", copyee.name));

        return null;
    }

    private GameObject Duplicate(GameObject copyee)
    {
        if (copyee == source)
            return destination;


        var stack = new Stack<Transform>();
        var curr = copyee.transform.parent;
        while (curr != source.transform)
        {
            stack.Push(curr);
            curr = curr.parent;
        }

        Transform parent = destination.transform;
        while (stack.Count != 0)
        {
            curr = stack.Pop();
            var child = parent.FindChild(curr.name);
            if (child == null)
            {
                var clone = new GameObject(curr.name);
                clone.layer = curr.gameObject.layer;
                clone.tag = curr.gameObject.tag;
#if UNITY_4_1
                clone.SetActive(curr.gameObject.activeSelf);
#else   
                clone.active = curr.gameObject.active;

#endif

                child = clone.transform;
                child.parent = parent;
                child.localPosition = curr.localPosition;
                child.localRotation = curr.localRotation;
                child.localScale = curr.localScale;

            }
            parent = child;
        }
        if (parent.FindChild(copyee.name) != null)
        {
            return parent.FindChild(copyee.name).gameObject;
        }
        GameObject result = (GameObject)GameObject.Instantiate(copyee);
        result.name = copyee.name;
        result.layer = copyee.layer;
#if UNITY_4_1
        result.SetActive(curr.gameObject.activeSelf);
#else   
                result.active = result.gameObject.active;

#endif


        result.transform.parent = parent;
        result.transform.localPosition = copyee.transform.localPosition;
        result.transform.localRotation = copyee.transform.localRotation;
        result.transform.localScale = copyee.transform.localScale;
        return result;
    }
}

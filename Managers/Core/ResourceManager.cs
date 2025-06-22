using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        // GameObject Ÿ�� �� ��� Prefab ���� �Ʒ��� ���� ������ �Ʒ� Instantiate()���� Prefabs/{���}�� �ν��Ͻÿ���Ʈ
        if (typeof(T) == typeof(GameObject))
        {
            // Object Pool�� �����Ϸ��� Prefab�� �̸��� �ʿ�, �� ��δ� ���� �ڸ�
            string name = path;
            int index = name.LastIndexOf('/');

            if (index >= 0)
                name = name.Substring(index + 1);

            GameObject go = Managers.Pool.GetOriginal(name);

            if (go != null)
                return go as T;
        }

        // Prefab�� �ƴ� ��� ��ο� ���� Load
        return Resources.Load<T>(path); 
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");

        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;

        int index = go.name.IndexOf("(Clone)");
        if (index > 0)
            go.name = go.name.Substring(0, index);

        return go;
    }

    public GameObject Instantiate(GameObject obj, Transform parent = null)
    {
        if (obj == null)
        {
            Debug.Log($"Failed to load prefab : {obj}");
            return null;
        }

        if (obj.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(obj, parent).gameObject;

        GameObject go = Object.Instantiate(obj, parent);
        go.name = obj.name;

        int index = go.name.IndexOf("(Clone)");
        if (index > 0)
            go.name = go.name.Substring(0, index);

        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Poolable poolable = go.GetComponent<Poolable>();

        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }
}

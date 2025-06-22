using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        // GameObject 타입 일 경우 Prefab 폴더 아래에 들어가기 때문에 아래 Instantiate()에서 Prefabs/{경로}로 인스턴시에이트
        if (typeof(T) == typeof(GameObject))
        {
            // Object Pool을 적용하려면 Prefab의 이름만 필요, 앞 경로는 전부 자름
            string name = path;
            int index = name.LastIndexOf('/');

            if (index >= 0)
                name = name.Substring(index + 1);

            GameObject go = Managers.Pool.GetOriginal(name);

            if (go != null)
                return go as T;
        }

        // Prefab이 아닐 경우 경로에 따라 Load
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

using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            var name = path;
            var index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            var go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }
        return Resources.Load<T>(path);
    }
    public async UniTask<T> LoadAsync<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            var name = path;
            var index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            var go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }
        return await Resources.LoadAsync<T>(path) as T ;
    }


    public GameObject Instantiate(string path, Transform parent = null)
    {
        var original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        // 2. 혹시라도 풀에 있는지
        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;


        // 생성되는 Prefab에서 Clone 이름 삭제
        var go = Object.Instantiate(original, parent);
        go.name = original.name;

        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null) return;

        // 만약에 풀링이 필요한 아이라면 -> 풀링 매니저 한테 보냄.
        var poolAble = go.GetComponent<Poolable>();
        if (poolAble != null)
        {
            Managers.Pool.Push(poolAble);
            return;
        }

        Object.Destroy(go);
    }
}
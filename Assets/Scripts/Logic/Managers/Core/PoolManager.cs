using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;

public class PoolManager
{
  #region  Pool
  class Pool
  {
    public GameObject Original { get; private set; }
    public Transform Root { get; set; }

    Stack<Poolable> _poolStack = new Stack<Poolable>();

    public void Init(GameObject original, int count = 5)
    {
      Original = original;
      Root = new GameObject().transform;
      Root.name = $"{original.name}_Root";

      for (int i = 0; i < count; i++)
      {
        Push(Create());
      }
    }

    Poolable Create()
    {
      GameObject go = Object.Instantiate<GameObject>(Original);
      go.name = Original.name;
      return go.GetOrAddComponent<Poolable>();
    }

    public void Push(Poolable poolable)
    {
      if (poolable == null) return;

      poolable.transform.parent = Root;
      poolable.gameObject.SetActive(false);
      poolable.isUsing = false;

      _poolStack.Push(poolable);
    }

    public Poolable Pop(Transform parent)
    {
      Poolable poolable;

      if (_poolStack.Count > 0)
        poolable = _poolStack.Pop();
      else
        poolable = Create();

      poolable.gameObject.SetActive(true);

      //DontDestroyOnLoad 해체 용도 ( 꼼수 )
      if (parent == null)
        poolable.transform.parent = Managers.Scene.CurrentScene.transform;

      poolable.transform.parent = parent;
      poolable.isUsing = false;

      return poolable;
    }
  }

  #endregion
  Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();
  Transform _root;
  public void Init()
  {
    if (_root == null)
    {
      _root = new GameObject { name = "@Pool_Root" }.transform;
      Object.DontDestroyOnLoad(_root);
    }
  }

  public void CreatePool(GameObject original, int count = 5)
  {
    // 내가 만든 class 이기때문에 new로 생성
    Pool pool = new Pool();
    pool.Init(original, count);
    pool.Root.parent = _root;

    _pool.Add(original.name, pool);
  }
  public void Push(Poolable poolable)
  {
    string name = poolable.gameObject.name;
    if (_pool.ContainsKey(name) == false)
    {
      GameObject.Destroy(poolable.gameObject);
      return;
    }

    _pool[name].Push(poolable);
  }

  public Poolable Pop(GameObject original, Transform parent = null)
  {
    // 아직 준비된 풀이 없으면 풀을 만들어줌 
    if (_pool.ContainsKey(original.name) == false)
      CreatePool(original);

    return _pool[original.name].Pop(parent);
  }

  public GameObject GetOriginal(string name)
  {
    if (_pool.ContainsKey(name) == false)
      return null;
    return _pool[name].Original;
  }

  public void Clear()
  {
    foreach (Transform child in _root)
      GameObject.Destroy(child.gameObject);

    _pool.Clear();
  }
}
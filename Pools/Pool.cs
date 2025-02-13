using System;
using UnityEngine;

[Serializable]
public class Pool : BasePool<Component>
{
    [SerializeField, HideInInspector]
    private string _name;
    private Component _prefab;

    public Pool(Component prefab, int initialCount, string name) :
        base(() => preloadFunc(prefab), TakeAction, ReleaseAction, initialCount)
    {
        _prefab = prefab;
        _name = name;
    }

    public bool IsInvalid()
    {
        return _prefab == null;
    }

    private static Component preloadFunc(Component prefab)
    {
        return GameObject.Instantiate(prefab);
    }

    private static void TakeAction(Component item)
    {
        item.transform.parent = ServiceLocator.GetService<PoolsContainer>().WorldTransform;
    }

    private static void ReleaseAction(Component item)
    {
        item.transform.parent = ServiceLocator.GetService<PoolsContainer>().PoolTransform;
    }
}
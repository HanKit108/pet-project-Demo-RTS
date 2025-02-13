using UnityEngine;
using UnityEngine.UI;

public class PoolsContainer
{
    private Transform 
        _selectionCirclePrefab, 
        _pointsBarPrefab, 
        _canvasPrefab, 
        _colliderPrefab;
    private Button _buttonPrefab;
    private Entity _entityPrefab;

    private Transform _worldTransform;
    public Transform WorldTransform => _worldTransform;

    private Transform _poolTransform;
    public Transform PoolTransform => _poolTransform;

    private PoolManager PoolManager = new PoolManager();

    public PoolsContainer(
        Transform selectionCirclePrefab,
        Transform pointsBarPrefab,
        Transform canvasPrefab,
        Transform colliderPrefab,
        Button buttonPrefab,
        Entity entityPrefab,
        Transform poolTransform,
        Transform worldTransform)
    {
        _selectionCirclePrefab = selectionCirclePrefab;
        _pointsBarPrefab = pointsBarPrefab;
        _canvasPrefab = canvasPrefab;
        _colliderPrefab = colliderPrefab;
        _buttonPrefab = buttonPrefab;
        _entityPrefab = entityPrefab;
        _poolTransform = poolTransform;
        _worldTransform = worldTransform;
    }

    public Pool GetPool(Transform prefab)
    {
        return PoolManager.GetPool(prefab);
    }

    public Pool GetEntityPool()
    {
        return PoolManager.GetPool(_entityPrefab);
    }

    public Pool GetSelectionPool()
    {
        return PoolManager.GetPool(_selectionCirclePrefab);
    }

    public Pool GetBarPool()
    {
        return PoolManager.GetPool(_pointsBarPrefab);
    }

    public Pool GetColliderPool()
    {
        return PoolManager.GetPool(_colliderPrefab);
    }

    public Pool GetCanvasPool()
    {
        return PoolManager.GetPool(_canvasPrefab);
    }

    public Pool GetButtonPool()
    {
        return PoolManager.GetPool(_buttonPrefab);
    }
}

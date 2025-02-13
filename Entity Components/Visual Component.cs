using UnityEngine;

public class VisualComponent : BaseComponent, IComponent, IDisposable
{
    private Transform _transform;
    private Pool _basePool;

    public VisualComponent(Entity entity, Transform prefab)
    {
        _name = "Visual Compoment";

        _basePool = ServiceLocator.GetService<PoolsContainer>().GetPool(prefab);
        _transform = (Transform)_basePool.Take();
        _transform.position = entity.transform.position;
        _transform.transform.rotation = Quaternion.identity;
        _transform.parent = entity.transform;

        if (_transform.TryGetComponent<Animator>(out var animator))
        {
            AnimationComponent animation = new AnimationComponent(animator);
            entity.Add(animation);
        }
    }
    
    public void Dispose()
    {
        _basePool?.Release(_transform);
    }
}

public class VisualComponentCreator : BaseComponentCreator, IComponentCreator
{
    [SerializeField]
    private Transform _visualPrefab;

    public VisualComponentCreator()
    {
        _name = "Visual Component";
    }

    public void CreateComponent(Entity entity)
    {
        VisualComponent visual = new VisualComponent(entity, _visualPrefab);
        entity.Add(visual);
    }
}
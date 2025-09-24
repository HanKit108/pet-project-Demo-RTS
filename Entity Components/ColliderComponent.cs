using UnityEngine;

public class ColliderComponent : BaseComponent, IComponent, IDisposable
{
    private Pool _pool;
    private Transform _transform;
    
    public ColliderComponent(Transform owner, string name)
    {
        _name = name;

        _pool = ServiceLocator.GetService<PoolsContainer>().GetColliderPool();
        _transform = (Transform)_pool.Take();
        _transform.position = owner.position;
        _transform.parent = owner;
    }

    public void Dispose()
    {
        _pool?.Release(_transform);
    }
}

public class ColliderComponentCreator : BaseComponentCreator, IComponentCreator
{
    private const string COLLIDER_COMPONENT_NAME = "Collider Component";

    public ColliderComponentCreator()
    {
        _name = COLLIDER_COMPONENT_NAME;
    }

    public void CreateComponent(Entity entity)
    {
        ColliderComponent collider = new ColliderComponent(entity.transform, _name);
        entity.Add(collider);
    }
}
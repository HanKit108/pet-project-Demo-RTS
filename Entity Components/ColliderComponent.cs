using UnityEngine;

public class ColliderComponent : BaseComponent, IComponent, IDisposable
{
    private Pool _pool;
    private Transform _transform;
    
    public ColliderComponent(Transform owner)
    {
        _name = "Collider Component";

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
    public ColliderComponentCreator()
    {
        _name = "Collider Component";
    }

    public void CreateComponent(Entity entity)
    {
        ColliderComponent collider = new ColliderComponent(entity.transform);
        entity.Add(collider);
    }
}
using System;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class BuildingCollisionComponent : BaseComponent, 
    IComponent, ICollisiable, IDisposable, ISwitchable
{
    private Action _enableAction, _disableAction;
    private NavMeshObstacle _obstacle;

    public BuildingCollisionComponent(Transform owner)
    {
        _name = "Building Collision Component";
        if (!owner.gameObject.TryGetComponent(out _obstacle))
        {
            _obstacle = owner.gameObject.AddComponent<NavMeshObstacle>();
        }
        _obstacle.carving = true;
    }
    
    public void EnableCollision()
    {
        _obstacle.enabled = true;
    }
    public void DisableCollision()
    {
        _obstacle.enabled = false;
    }

    public void Dispose()
    {
        GameObject.Destroy(_obstacle);
        _enableAction -= EnableCollision;
        _disableAction -= DisableCollision;
    }

    public void SetSwitchEvents(ref Action enableAction, ref Action disableAction)
    {
        enableAction += EnableCollision;
        disableAction += DisableCollision;
        _enableAction = enableAction;
        _disableAction = disableAction;
    }
}

public class BuildingCollisionComponentCreator : BaseCollisionComponentCreator, IComponentCreator
{
    public BuildingCollisionComponentCreator()
    {
        _name = "Building Collision Component";
    }

    public void CreateComponent(Entity entity)
    {
        BuildingCollisionComponent building = new BuildingCollisionComponent(entity.transform);
        TrySetLifeEvents(entity, building);
        entity.Add(building);
    }
}
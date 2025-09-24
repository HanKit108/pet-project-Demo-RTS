using System;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class BuildingCollisionComponent : BaseComponent, 
    IComponent, ICollisiable, IDisposable, ISwitchable
{
    private Action _enableAction, _disableAction;
    private NavMeshObstacle _obstacle;

    public BuildingCollisionComponent(Transform owner, string name)
    {
        _name = name;
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
    private const string BUILDING_COLLISION_COMPONENT_NAME = "Building Collision Component";

    public BuildingCollisionComponentCreator()
    {
        _name = BUILDING_COLLISION_COMPONENT_NAME;
    }

    public void CreateComponent(Entity entity)
    {
        BuildingCollisionComponent building = new BuildingCollisionComponent(entity.transform, _name);
        TrySetLifeEvents(entity, building);
        entity.Add(building);
    }
}
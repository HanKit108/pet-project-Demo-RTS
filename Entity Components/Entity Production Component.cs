using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EntityProductionComponent : BaseConditionComponent, 
    IComponent, ISwitchable, IEnable, IDisable, IDisposable
{
    [SerializeField]
    private readonly List<EntityConfigSO> _producedEntities = new();
    private Action _onEnabled, _onDisabled;
    private Entity _owner;
    private Timer _timer;
    private Queue<EntityConfigSO> _productionQueue = new();
    private bool _isProduction, _enabled = true;

    public Action OnDequeued;
    public Action<Sprite> OnEnqueued;

    public EntityProductionComponent(Entity owner, string name)
    {
        _name = name;
        _owner = owner;
    }

    public void SetSwitchEvents(ref Action onEnabled, ref Action onDisabled)
    {
        onEnabled += Enable;
        onDisabled += Disable;

        _onEnabled = onEnabled;
        _onDisabled = onDisabled;
    }

    public void Enable()
    {
        _enabled = true;
        _timer?.Resume();
    }

    public void Disable()
    {
        _enabled = false;
        _timer?.Pause();
    }

    public List<EntityConfigSO> GetProducedEntities()
    {
        return _producedEntities;
    }

    public Queue<EntityConfigSO> GetProductionQueue()
    {
        return _productionQueue;
    }

    public void AddProducedEntity(EntityConfigSO entity)
    {
        _producedEntities.Add(entity);
    }

    public void RemoveProducedEntity(EntityConfigSO entity)
    {
        _producedEntities.Remove(entity);
    }

    public void Dispose()
    {
        _timer?.Abort();
        _onEnabled -= Enable;
        _onDisabled -= Disable;
    }

    public void TryEnqueueProduction(EntityConfigSO entityConfigSO)
    {
        if (ServiceLocator.GetService<PlayerManager>().ResourseBank.TrySpendResourse(entityConfigSO.ProductionCost))
        {
            _productionQueue.Enqueue(entityConfigSO);
            OnEnqueued?.Invoke(entityConfigSO.Icon);
            TryStartProduction();
        }
    }

    private void TryStartProduction()
    {
        if (CanProduce())
        {
            if(_productionQueue.TryPeek(out var result))
            {
                _isProduction = true;
                _timer = ServiceLocator.GetService<TimerSystem>().CreateTimer(result.ProductionDelay, CompleteProduction);
            }
        }
    }

    private void CompleteProduction()
    {
        var entity = ServiceLocator.GetService<EntitySpawner>().SpawnEntity(
            _productionQueue.Dequeue(),
            _owner.transform.position, Quaternion.identity);
        ServiceLocator.GetService<EntityTeamInstaller>().TryReplicateTeam(_owner, entity);

        OnDequeued?.Invoke();
        _isProduction = false;
        TryStartProduction();
    }

    private bool CanProduce()
    {
        return _compositeConditions.Invoke() &&
                !_isProduction &&
                _enabled;
    }
}

public class ProductionComponentCreator : BaseComponentCreator, IComponentCreator
{
    private const string ENTITY_PRODUCTION_COMPONENT_NAME = "Entity Production Component";

    [SerializeField]
    private List<EntityConfigSO> _producedEntities = new();

    public ProductionComponentCreator()
    {
        _name = ENTITY_PRODUCTION_COMPONENT_NAME;
    }

    public void CreateComponent(Entity entity)
    {
        EntityProductionComponent production = new EntityProductionComponent(entity, _name);

        foreach (EntityConfigSO entityConfig in _producedEntities)
        {
            production.AddProducedEntity(entityConfig);
        }
        TrySetLifeEvents(entity, production);
        entity.Add(production);
    }
}
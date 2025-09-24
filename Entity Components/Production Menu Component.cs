using System;
using UnityEngine;

[Serializable]
public class ProductionMenuComponent: BaseComponent, 
    IComponent, ISelectable, IDisposable
{
    private EntityProductionComponent _production;
    private Action _onSelected, _onDeselected, _onDequeued;
    private Action<Sprite> _onEnqueued;
    private bool _isSelected;

    public ProductionMenuComponent(EntityProductionComponent production, string name)
    {
        _name = name;
        _production = production;

        production.OnEnqueued  += OnEnqueued;
        production.OnDequeued += OnDequeued;

        _onEnqueued = production.OnEnqueued;
        _onDequeued = production.OnDequeued;
    }

    public void SetSelectionEvents(ref Action onSelected, ref Action onDeselected)
    {
        onSelected += OnSelected;
        onDeselected += OnDeselected;

        _onSelected = onSelected;
        _onDeselected = onDeselected;
    }

    public void Dispose()
    {
        _onSelected -= OnSelected;
        _onDeselected -= OnDeselected;
        _onDequeued -= OnDequeued;
        _onEnqueued -= OnEnqueued;
    }

    private void OnSelected()
    {
        if(!_isSelected)
        {
            _isSelected = true;
            foreach(EntityConfigSO entity in _production.GetProducedEntities())
            {
                ServiceLocator.GetService<PlayerPanel>().AddButton(() => _production.TryEnqueueProduction(entity), entity.Icon);
            }
            var queue = _production.GetProductionQueue();
            for (int i = 0; i < queue.Count; i++)
            {
                ServiceLocator.GetService<PlayerPanel>().AddToQueue(queue.ToArray()[i].Icon);
            }
        }
    }

    private void OnDeselected()
    {
        _isSelected = false;
        ServiceLocator.GetService<PlayerPanel>().ClearPanel();
    }

    private void OnEnqueued(Sprite icon)
    {
        if (_isSelected)
        {
            ServiceLocator.GetService<PlayerPanel>().AddToQueue(icon);
        }
    }

    private void OnDequeued()
    {
        if (_isSelected)
        {
            ServiceLocator.GetService<PlayerPanel>().RemoveFromQueue();
        }
    }
}

public class ProductionMenuComponentCreator : BaseComponentCreator, IComponentCreator
{
    private const string PRODUCTION_MENU_COMPONENT_NAME = "Production Menu Component";

    public ProductionMenuComponentCreator()
    {
        _name = PRODUCTION_MENU_COMPONENT_NAME;
    }

    public void CreateComponent(Entity entity)
    {
        if (entity.TryGetComponent<EntityProductionComponent>(out var production))
        {
            ProductionMenuComponent menu = new ProductionMenuComponent(production, _name);
            TrySetSelectionEvents(entity, menu);
            entity.Add(menu);
        }
    }
}
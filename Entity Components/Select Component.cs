using System;
using UnityEngine;

[Serializable]
public class SelectComponent : BaseComponent, 
    IComponent, IDisposable, ISwitchable
{
    private Transform _transform, _owner;
    private Action _enableAction, _disableAction;
    private Pool _pool;

    public Action OnSelected, OnDeselected;

    public SelectComponent(Transform owner, string name)
    {
        _name = name;

        _owner = owner;
        _pool = ServiceLocator.GetService<PoolsContainer>().GetSelectionPool();
        _transform = (Transform)_pool.Take();
        _transform.position = _owner.position + Vector3.up * Constants.SELECTION_VERTICAL_OFFSET;
        _transform.parent = _owner;
        Deselect();
    }

    public void SetSwitchEvents(ref Action enableAction, ref Action disableAction)
    {
        enableAction += EnableSelection;
        disableAction += DisableSelection;

        _enableAction = enableAction;
        _disableAction = disableAction;
    }

    public void Select()
    {
        _transform.gameObject.SetActive(true);
        OnSelected?.Invoke();
    }

    public void Deselect()
    {
        _transform.gameObject.SetActive(false);
        OnDeselected?.Invoke();
    }

    public void Dispose()
    {
        Deselect();
        EnableSelection();
        _enableAction -= EnableSelection;
        _disableAction -= DisableSelection;
        _pool?.Release(_transform);
    }


    private void EnableSelection()
    {
        for(int i = 0; i < _owner.transform.childCount; i++)
        {
            var child = _owner.transform.GetChild(i).gameObject;
            child.layer = ServiceLocator.GetService<GameManager>().UnitLayer;
        }
    }

    private void DisableSelection()
    {
        for (int i = 0; i < _owner.transform.childCount; i++)
        {
            var child = _owner.transform.GetChild(i).gameObject;
            child.layer = ServiceLocator.GetService<GameManager>().UnselectableLayer;
        }
        Deselect();
    }
}

public class SelectComponentCreator : BaseComponentCreator, IComponentCreator
{
    private const string ROTATE_COMPONENT_NAME = "Select Component";

    public SelectComponentCreator()
    {
        _name = ROTATE_COMPONENT_NAME;
    }

    public void CreateComponent(Entity entity)
    {
        SelectComponent selection = new SelectComponent(entity.transform, _name);
        TrySetLifeEvents(entity, selection);
        entity.Add(selection);
    }
}
using System.Collections.Generic;
using UnityEngine;
using System;

public class SquadControl: IUpdatable, IDisable
{
    private List<Entity> _entityList = new List<Entity>();
    private float _distanceBetweenUnits, _rayDistance = 100f;
    private int _minSelectionSize = 1;

    private Vector3 _startMousePosition, _endWorldSelectionPosition, _startWorldSelectionPosition;
    private Vector2 _p1, _p2;
    private List<Entity> _selectedEntityList = new List<Entity>();
    private PlayerControls _playerControls;
    private int _unitLayer = (int)Mathf.Pow(2, (float)ServiceLocator.GetService<GameManager>().UnitLayer);

    public static Action<Vector3> OnSpawned;

    public SquadControl(PlayerControls playerControls, float distanceBetweenUnits)
    {
        _distanceBetweenUnits = distanceBetweenUnits;
        _playerControls = playerControls;

        _playerControls.Units.Select.started += context => StartSelect();
        _playerControls.Units.Select.canceled += context => CancelSelect();
        _playerControls.Units.Interact.performed += context => OrderToPoint();
        _playerControls.Units.Autoattack.performed += context => OrderAutoattackToPoint();
        _playerControls.Units.Stop.performed += context => SquadOrderToStop();
        _playerControls.Units.Spawn.performed += context => OnSpawned?.Invoke(Utils.GetMousePositionFromScreen());

        ServiceLocator.GetService<UpdateManager>().Add(this);
        ServiceLocator.GetService<DisableManager>().Add(this);
    }
    public void Disable()
    {
        _playerControls.Units.Select.started -= context => StartSelect();
        _playerControls.Units.Select.canceled -= context => CancelSelect();
        _playerControls.Units.Interact.performed -= context => OrderToPoint();
        _playerControls.Units.Autoattack.performed -= context => OrderAutoattackToPoint();
        _playerControls.Units.Stop.performed -= context => SquadOrderToStop();
        _playerControls.Units.Spawn.performed -= context => OnSpawned?.Invoke(Utils.GetMousePositionFromScreen());
        
        ServiceLocator.GetService<UpdateManager>().Remove(this);
    }

    public void OnUpdate(float deltaTime)
    {
        if (_playerControls.Units.Select.IsPressed())
        {
            PerformSelect();
        }
    }

    private void StartSelect()
    {
        RemoveDiedEntities();
        _startMousePosition = Input.mousePosition;
        _startWorldSelectionPosition = Utils.GetMousePositionFromScreen();

        TrySelectEntityFromScreen();
    }

    private void PerformSelect()
    {
        _endWorldSelectionPosition = Utils.GetMousePositionFromScreen();

        if (Mathf.Abs((_startMousePosition - Input.mousePosition).magnitude) > _minSelectionSize)
        {
            Collider[] hitColliders = Physics.OverlapSphere(
                (_startWorldSelectionPosition + _endWorldSelectionPosition) / 2,
                Vector3.Distance(_startWorldSelectionPosition, _endWorldSelectionPosition) / 2,
                _unitLayer);
            foreach (var hitCollider in hitColliders)
            {
                var selected = hitCollider.GetComponentInParent<Entity>();
                if (IsAvailableSelect(selected) && IsPlayer(selected))
                {
                    var selectable = selected.GetComponent<SelectComponent>();
                    if (Utils.IsPointInside(hitCollider.transform.position, _startWorldSelectionPosition, _endWorldSelectionPosition))
                    {
                        _selectedEntityList.Add(selected);
                        selectable.Select();
                    }
                    else
                    {
                        selectable.Deselect();
                    }
                }
            }
        }
    }

    private void CancelSelect()
    {
        if (Mathf.Abs((_startMousePosition - Input.mousePosition).magnitude) > _minSelectionSize)
            DeselectEntites();

        foreach (Entity entity in _selectedEntityList)
        {
            if (Utils.IsPointInside(entity.transform.position, _startWorldSelectionPosition, _endWorldSelectionPosition))
                SelectEntity(entity);
        }
        _selectedEntityList.Clear();
    }

    private void OrderToPoint()
    {
        RemoveDiedEntities();
        if (TryGetSelectableEntityFromScreen(out var entity))
        {
            SquadOrderToEntity(entity);
        }
        else
        {
            SquadOrderToMove(Utils.GetMousePositionFromScreen());
        }
    }

    private void OrderAutoattackToPoint()
    {
        RemoveDiedEntities();
        if (TryGetSelectableEntityFromScreen(out var entity))
        {
            SquadOrderToAttackEntity(entity);
        }
        else
        {
            SquadOrderToAutoattackMove(Utils.GetMousePositionFromScreen());
        }
    }
    
    private void TrySelectEntityFromScreen()
    {
        if (TryGetSelectableEntityFromScreen(out var entity) && IsPlayer(entity))
        {
            DeselectEntites();
            SelectEntity(entity);
        }
    }

    private bool TryGetSelectableEntityFromScreen(out Entity entity)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _rayDistance, _unitLayer))
        {
            var selected = hit.collider.gameObject.GetComponentInParent<Entity>();
            if (IsAvailableSelect(selected))
            {
                entity = selected;
                return true;
            }
        }
        entity = null;
        return false;
    }

    private bool IsAvailableSelect(Entity entity)
    {
        return entity != null && 
               entity.TryGetComponent<SelectComponent>(out var selectable);
    }

    private bool IsPlayer(Entity entity)
    {
        return entity.TryGetComponent<TeamComponent>(out var teamable) &&
               teamable.Team == ServiceLocator.GetService<PlayerManager>().Team;
    }

    private void SelectEntity(Entity selected)
    {
        if (IsAvailableSelect(selected) && !_entityList.Contains(selected))
        {
            _entityList.Add(selected);
            selected.GetComponent<SelectComponent>().Select();
        }
    }

    private void DeselectEntity(Entity selected)
    {
        if (_entityList.Contains(selected) && selected.TryGetComponent<SelectComponent>(out var selectable))
        {
            _entityList.Remove(selected);
            selectable.Deselect();
        }
    }

    private void DeselectEntites()
    {
        foreach (Entity entity in _entityList)
        {
            if (entity != null && entity.TryGetComponent<SelectComponent>(out var selectable))
            {
                selectable.Deselect();
            }
        }
        _entityList.Clear();
    }

    private void SquadOrderToMove(Vector3 targetPosition)
    {
        List<Vector3> movePositionList = Utils.GetPositionListAround(targetPosition, _distanceBetweenUnits, _entityList.Count);

        int index = 0;

        foreach (Entity entity in _entityList)
        {
            if (entity.TryGetComponent<IUnitControl>(out var control))
            {
                control.OrderToMove(movePositionList[index]);
                index++;
            }
        }
    }

    private void SquadOrderToAutoattackMove(Vector3 targetPosition)
    {
        List<Vector3> movePositionList = Utils.GetPositionListAround(targetPosition, _distanceBetweenUnits, _entityList.Count);

        int index = 0;

        foreach (Entity entity in _entityList)
        {
            if (entity.TryGetComponent<IUnitControl>(out var control))
            {
                control.OrderToAutoattackMove(movePositionList[index]);
                index++;
            }
        }
    }

    private void SquadOrderToEntity(Entity targetEntity)
    {
        foreach (Entity entity in _entityList)
        {
            if (entity.TryGetComponent<IUnitControl>(out var control))
            {
                control.OrderToEntity(targetEntity);
            }
        }
    }

    private void SquadOrderToAttackEntity(Entity targetEntity)
    {
        foreach (Entity entity in _entityList)
        {
            if (entity.TryGetComponent<IUnitControl>(out var control))
            {
                control.OrderToAttackEntity(targetEntity);
            }
        }
    }

    private void SquadOrderToStop()
    {
        foreach (Entity entity in _entityList)
        {
            if (entity.TryGetComponent<IUnitControl>(out var control))
            {
                control.OrderToStop();
            }
        }
    }

    private void RemoveDiedEntities()
    {
        for(int i = 0; i <  _entityList.Count; i++)
        {
            if (_entityList[i].TryGetComponent<IDamagable>(out var damagable) && !damagable.IsAlive())
            {
                _entityList.Remove(_entityList[i]);
                i--;
            }
        }
    }
}

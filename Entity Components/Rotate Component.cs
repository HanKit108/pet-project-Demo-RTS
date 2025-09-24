using System;
using UnityEngine;

[Serializable]
public class RotateComponent: BaseConditionComponent, IComponent, IRotatable
{
    [SerializeField, HideInInspector]
    private Transform _transform, _targetTransform;
    [SerializeField]
    private float _rotateSpeed;

    public RotateComponent(string name)
    {
        _name = name;
    }

    public void Initialize(Transform transform, float rotateSpeed)
    {
        _transform = transform;
        _rotateSpeed = rotateSpeed;
    }

    public void RotateToward(Transform targetTransform, float deltaTime)
    {
        _targetTransform = targetTransform;
        if (_compositeConditions.Invoke())
        {
            Quaternion target = Quaternion.LookRotation(targetTransform.position - _transform.position, _transform.up);
            float step = deltaTime * _rotateSpeed * Constants.ROTATE_SPEED_MULTIPLUER;
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation, target, step);
        }
    }

    public bool IsAimed()
    {
        return Vector3.Angle(_transform.forward, _targetTransform.position - _transform.position) < Constants.MAX_ATTACK_ANGLE;
    }
}

public class RotateComponentCreator : BaseComponentCreator, IComponentCreator
{
    private const string ROTATE_COMPONENT_NAME = "Rotate component";

    [SerializeField, Min(0f)]
    protected float _rotationSpeed;

    public RotateComponentCreator()
    {
        _name = ROTATE_COMPONENT_NAME;
    }

    public void CreateComponent(Entity entity)
    {
        RotateComponent rotation = new RotateComponent(_name);
        rotation.Initialize(entity.transform, _rotationSpeed);
        TryAddDeathCondition(entity, rotation);
        entity.Add(rotation);
    }
}
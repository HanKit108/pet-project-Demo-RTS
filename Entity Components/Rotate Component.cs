using System;
using UnityEngine;

[Serializable]
public class RotateComponent: BaseConditionComponent, IComponent, IRotatable
{
    private const float ROTATE_SPEED_MULTIPLUER = 100f;
    private const float MAX_ATTACK_ANGLE = 5f;

    [SerializeField, HideInInspector]
    private Transform _transform, _targetTransform;
    [SerializeField]
    private float _rotateSpeed;

    public RotateComponent()
    {
        _name = "Rotate Component";
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
            float step = deltaTime * _rotateSpeed * ROTATE_SPEED_MULTIPLUER;
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation, target, step);
        }
    }

    public bool IsAimed()
    {
        return Vector3.Angle(_transform.forward, _targetTransform.position - _transform.position) < MAX_ATTACK_ANGLE;
    }
}

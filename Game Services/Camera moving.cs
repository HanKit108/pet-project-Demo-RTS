using UnityEngine;

public class CameraMoving: IUpdatable, IDisable
{
    private const float ANGLE_MULTIPLUER = 10f;
    private const float MAP_SIZE = 100f;

    private float _moveSpeed, _minYPosition, _minXRotation, _maxXRotation, _smooth;
    private PlayerControls _playerControls;

    private Transform _cameraTransform;
    public Transform CameraTransform => _cameraTransform;


    public CameraMoving(Transform cameraTransform, 
        float moveSpeed, 
        float minYPosition, 
        float minXRotation,
        float maxXRotation,
        float smooth,
        PlayerControls playerControls)
    {
        _cameraTransform = cameraTransform;
        _moveSpeed = moveSpeed;
        _minYPosition = minYPosition;
        _minXRotation = minXRotation;
        _maxXRotation = maxXRotation;
        _smooth = smooth;
        _playerControls = playerControls;

        ServiceLocator.GetService<UpdateManager>().Add(this);
        ServiceLocator.GetService<DisableManager>().Add(this);
    }

    public void Disable()
    {
        ServiceLocator.GetService<UpdateManager>().Remove(this);
    }

    public void OnUpdate(float deltaTime)
    {
        Vector3 desiredDirection = _playerControls.Camera.Move.ReadValue<Vector3>();
        Vector3 moveDirection = new Vector3(desiredDirection.x, desiredDirection.z, desiredDirection.y);

        
        float XAngle = Mathf.Clamp(_cameraTransform.position.y * ANGLE_MULTIPLUER, _minXRotation, _maxXRotation);
        Quaternion target = Quaternion.Euler(XAngle, 0, 0);
        _cameraTransform.rotation = Quaternion.Lerp(_cameraTransform.rotation, target, deltaTime * _smooth);

        var position = _cameraTransform.position + moveDirection * _moveSpeed * _cameraTransform.position.y * deltaTime;
        float XPosition = Mathf.Clamp(position.x, -MAP_SIZE, MAP_SIZE);
        float YPosition = Mathf.Clamp(position.y, _minYPosition, MAP_SIZE / 2);//Mathf.Max(_minYPosition, position.y);
        float ZPosition = Mathf.Clamp(position.z, -MAP_SIZE, MAP_SIZE);
        _cameraTransform.position = new Vector3(XPosition, YPosition, ZPosition);
    }
}
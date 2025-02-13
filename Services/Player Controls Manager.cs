public class PlayerControlsManager: IDisable
{
    private PlayerControls _playerControls;
    public PlayerControls PlayerControls => _playerControls;

    public PlayerControlsManager()
    {
        _playerControls = new PlayerControls();
        _playerControls.Enable();
        ServiceLocator.GetService<DisableManager>().Add(this);
    }

    public void Disable()
    {
        _playerControls.Disable();
    }
}

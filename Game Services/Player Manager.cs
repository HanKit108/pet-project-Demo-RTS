public class PlayerManager
{
    private TeamSO _team;
    public TeamSO Team => _team;
    private ResourseBank _resourseBank = new ResourseBank();
    public ResourseBank ResourseBank => _resourseBank;
    private int _startResourse;

    public PlayerManager(TeamSO team, int startResourse)
    {
        _team = team;
        _resourseBank.AddResourse(startResourse);
    }
}

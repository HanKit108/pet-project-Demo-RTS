using System;
using UnityEngine;

public class TeamComponent: BaseComponent, IComponent
{
    [SerializeField]
    private TeamSO _team;

    public TeamSO Team => _team;

    public Action<Color> OnTeamChanged;

    public TeamComponent(string name)
    {
        _name = name;
    }

    public void SetTeam(TeamSO team)
    {
        _team = team;
        OnTeamChanged?.Invoke(_team.GetColor());
    }
}

public class TeamComponentCreator : BaseComponentCreator, IComponentCreator
{
    private const string TEAM_COMPONENT_NAME = "Team Component";

    public TeamComponentCreator()
    {
        _name = TEAM_COMPONENT_NAME;
    }

    public void CreateComponent(Entity entity)
    {
        TeamComponent team = new TeamComponent(_name);
        entity.Add(team);
    }
}
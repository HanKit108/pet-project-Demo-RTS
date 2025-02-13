using System;
using UnityEngine;

public class TeamComponent: BaseComponent, IComponent
{
    [SerializeField]
    private TeamSO _team;

    public TeamSO Team => _team;

    public Action<Color> OnTeamChanged;

    public TeamComponent()
    {
        _name = "Team Component";
    }

    public void SetTeam(TeamSO team)
    {
        _team = team;
        OnTeamChanged?.Invoke(_team.GetColor());
    }
}

public class TeamComponentCreator : BaseComponentCreator, IComponentCreator
{
    public TeamComponentCreator()
    {
        _name = "Team Component";
    }

    public void CreateComponent(Entity entity)
    {
        TeamComponent team = new TeamComponent();
        entity.Add(team);
    }
}
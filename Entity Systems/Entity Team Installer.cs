using System.Collections.Generic;

public class EntityTeamInstaller
{
    public void TrySetTeam(Entity entity, TeamSO team)
    {
        if (entity.TryGetComponent<TeamComponent>(out var entityTeam))
        {
            entityTeam.SetTeam(team);
        }
    }

    public void TrySetTeam(List<Entity> entities, TeamSO team)
    {
        foreach (Entity entity in entities)
        {
            TrySetTeam(entity, team);
        }
    }

    public void TryReplicateTeam(Entity source, Entity recipient)
    {
        if (source.TryGetComponent<TeamComponent>(out var sourceTeam) &&
            recipient.TryGetComponent<TeamComponent>(out var recipientTeam))
        {
            recipientTeam.SetTeam(sourceTeam.Team);
        }
    }

    public void TryReplicateTeam(Entity source, List<Entity> recipients)
    {
        if (source.TryGetComponent<TeamComponent>(out var sourceTeam))
        {
            foreach (var recipient in recipients)
            {
                if (recipient.TryGetComponent<TeamComponent>(out var recipientTeam))
                {
                    recipientTeam.SetTeam(sourceTeam.Team);
                }
            }
        }
    }
}
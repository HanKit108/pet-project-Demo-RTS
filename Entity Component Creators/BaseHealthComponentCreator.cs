using System;

public class BaseHealthComponentCreator : BaseComponentCreator
{
    protected const string DEATH = "Death Animation";

    protected void TryAddAnimation(Entity entity, ref Action onRevived, ref Action onDied)
    {
        if (entity.TryGetComponent<AnimationComponent>(out var animation))
        {
            animation.Add(DEATH, ref onDied);
            animation.Add(IDLE, ref onRevived);
        }
    }
}
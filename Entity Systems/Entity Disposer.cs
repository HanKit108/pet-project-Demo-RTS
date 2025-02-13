public class EntityDisposer
{
    private float _delay;

    public EntityDisposer(float delay)
    {
        _delay = delay;
    }

    public void DelayedDispose(Entity entity)
    {
        ServiceLocator.GetService<TimerSystem>().CreateTimer(_delay, () => Dispose(entity));
    }

    public void Dispose(Entity entity)
    {
        entity.Dispose();
        ServiceLocator.GetService<PoolsContainer>().GetEntityPool().Release(entity);
    }
}

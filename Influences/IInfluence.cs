public interface IInfluence
{

}

public interface IRemoveableInfluence: IInfluence
{
    public void Apply(Entity entity);
    public void Dispel(Entity entity);
}

public interface IOnceInfluence: IInfluence
{
    public void Apply(Entity entity);
}
using System.Collections.Generic;

public class DisableManager
{
    private readonly List<IDisable> _disables = new();

    public void Add(IDisable disable)
    {
        _disables.Add(disable);
    }

    public void DisableAll()
    {
        for (int i = 0; i < _disables.Count; i++)
        {
            _disables[i].Disable();
        }
    }
}

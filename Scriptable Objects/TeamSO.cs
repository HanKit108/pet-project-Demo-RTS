using UnityEngine;

[CreateAssetMenu(fileName = "Team", menuName = "Team", order = 51)]
public class TeamSO : ScriptableObject
{
    [SerializeField]
    private string _name;

    [SerializeField]
    private Color _color;

    public void SetName(string name)
    {
        _name = name;
    }

    public void SetColor(Color color)
    {
        _color = color;
    }

    public string GetName()
    {
        return _name;
    }

    public Color GetColor()
    {
        return _color;
    }
}

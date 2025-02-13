using TMPro;
using UnityEngine;

public class ResourseDisplay
{
    [SerializeField]
    private TextMeshProUGUI _text;

    public ResourseDisplay(TextMeshProUGUI text)
    {
        _text = text;
        var player = ServiceLocator.GetService<PlayerManager>();
        player.ResourseBank.OnAmountChanged += OnAmountChanged;
        OnAmountChanged(player.ResourseBank.ResourseAmount);
    }

    private void OnAmountChanged(int amount)
    {
        _text.text = amount.ToString();
    }
}

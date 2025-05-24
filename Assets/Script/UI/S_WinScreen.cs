using UnityEngine;

/// <summary>
/// Manage the win screen
/// </summary>
public class S_WinScreen : S_EndScreen
{
    [SerializeField] private GameObject _victoryCam = null;

    public override void Show()
    {
        base.Show();
        _victoryCam.SetActive(true);
    }

    public override void Hide()
    {
        base.Hide();
        _victoryCam.SetActive(false);
    }
}

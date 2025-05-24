using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Manage the main menu of the game
/// </summary>
public class S_MainMenu : S_Screen
{
    [SerializeField] private Button _playBtn = null;

    public event UnityAction onPlay;

    void Start()
    {
        _playBtn.onClick?.AddListener(() =>
        {
            onPlay?.Invoke();
            Hide();
        });    
    }
}

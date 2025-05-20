using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_UIGameManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private S_MainMenu _mainMenu = null;

    public event UnityAction onUIGameStarted;

    private static S_UIGameManager instance;
    public static S_UIGameManager Instance => instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);

        instance = this;
    }

    private void Start()
    {
        _mainMenu.onPlay += MainMenu_onPlay;
    }

    private void MainMenu_onPlay()
    {
        StartGame();
    }

    private void StartGame()
    {
        onUIGameStarted?.Invoke();
    }
}

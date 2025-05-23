using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_EndScreen : S_Screen
{
    [SerializeField] protected Button _restartBtn = null;

    protected override void Awake()
    {
        base.Awake();

        _restartBtn.onClick.AddListener(() => S_LoadManager.Load(S_LoadManager.Scene.MainScene));
    }
}

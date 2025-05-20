using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveSettings", menuName = "ScriptableObjects/Wave Settings")]
public class SO_WaveSettings : ScriptableObject
{
    [SerializeField] private List<Wave> _waves = new List<Wave>();

    public List<Wave> Waves => _waves;
}

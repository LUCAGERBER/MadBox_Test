using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveSettings", menuName = "ScriptableObjects/Wave Settings")]
public class SO_WaveSettings : ScriptableObject
{
    [SerializeField] private List<Wave> _waves = new List<Wave>();
    [SerializeField] private float _timeBetweenWaves = 2.5f;

    public List<Wave> Waves => _waves;
    public float TimeBetweenWaves => _timeBetweenWaves;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon")]
public class SO_Weapon : ScriptableObject
{
    [SerializeField] private GameObject _weaponObject = null;
    [SerializeField] private int _damages = 2;

    public GameObject WeaponObject => _weaponObject;
    public int Damages => _damages;
}

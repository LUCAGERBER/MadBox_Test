using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_AutoCameraLookAt : MonoBehaviour
{
    private void Update()
    {
        transform.LookAt(Camera.main.transform);   
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_AutoCameraLookAt : MonoBehaviour
{
    private void Update()
    {
        if(Camera.main != null) transform.LookAt(Camera.main.transform);   
    }
}

using UnityEngine;

/// <summary>
/// Helper to make an object look at the camera
/// </summary>
public class S_AutoCameraLookAt : MonoBehaviour
{
    private void Update()
    {
        if(Camera.main != null) transform.LookAt(Camera.main.transform);   
    }
}

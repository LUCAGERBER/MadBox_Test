using System.Collections;
using UnityEngine;

/// <summary>
/// Manage the loose screen
/// </summary>
public class S_LooseScreen : S_EndScreen
{
    [SerializeField] private Camera _mainCamera = null;
    [SerializeField] private float _zoomInStrenght = 10f;
    [SerializeField] private float _zoomInDuration = 5f;

    public override void Show()
    {
        base.Show();

        //Update the main camera to have her only render the Player and the Enemies.
        _mainCamera.backgroundColor = Color.black;
        _mainCamera.clearFlags = CameraClearFlags.SolidColor;

        _mainCamera.cullingMask = (1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("Character"));

        StartCoroutine(CameraZoom());
    }

    /// <summary>
    /// Coroutine zooming the camera slowly. Needed since editing the Camera settings cause Cinemachine to loose control of the camera
    /// </summary>
    /// <returns></returns>
    private IEnumerator CameraZoom()
    {
        float elapsed = 0f;
        Vector3 startPos = _mainCamera.transform.position;
        Vector3 endPos = startPos + _mainCamera.transform.forward;

        Debug.DrawLine(startPos, endPos, Color.red, 15f);

        while (elapsed < _zoomInDuration) 
        {
            elapsed += Time.unscaledDeltaTime;

            _mainCamera.transform.position += _mainCamera.transform.forward * (_zoomInStrenght * Time.unscaledDeltaTime);
            _mainCamera.transform.position += -_mainCamera.transform.up * (_zoomInStrenght/2 * Time.unscaledDeltaTime);

            yield return null;
        }
    }
}

using UnityEngine;
using System.Collections;

public class CameraBounds : MonoBehaviour
{
    /// <summary>
    /// Calculates the current bounds of the camera.
    /// </summary>
    /// <returns>Bounds of the current camera object.</returns>
    public Bounds OrthographicBounds()
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = Camera.main.orthographicSize * 2;
        Bounds bounds = new Bounds(
                            GetComponent<Camera>().transform.position,
                            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }
}

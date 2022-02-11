using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Parallax : MonoBehaviour
{
    private float length, startpos;
    public float parallaxFactor;
    public GameObject cam;
    public CinemachineVirtualCameraBase cinemachineVirtualCamera;
    public CinemachineConfiner camConfiner;
    public float PixelsPerUnit;
    public Vector3 newPosition;

    private void Awake()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = cam.transform.position.x * parallaxFactor;

        newPosition = new Vector3(startpos + distance, transform.position.y, transform.position.z);

        if (camConfiner.CameraWasDisplaced(cinemachineVirtualCamera))   if(camConfiner.GetCameraDisplacementDistance(cinemachineVirtualCamera) > 10)
            {
                Debug.Log("Camera Displacement = " + camConfiner.GetCameraDisplacementDistance(cinemachineVirtualCamera));
            }

        transform.position = PixelPerfectClamp(newPosition, PixelsPerUnit);
    }

    private Vector3 PixelPerfectClamp(Vector3 locationVector, float pixelsPerUnit)
    {
        Vector3 vectorInPixels = new Vector3(Mathf.CeilToInt(locationVector.x * pixelsPerUnit), Mathf.CeilToInt(locationVector.y * pixelsPerUnit), Mathf.CeilToInt(locationVector.z * pixelsPerUnit));
        return vectorInPixels / pixelsPerUnit;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Parallax : MonoBehaviour
{
    private float length, startpos;
    public float parallaxFactor;
    public GameObject cam;
    public float PixelsPerUnit;
    public Vector3 newPosition;
    public float leftBounds;
    public float rightBounds;

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

        Vector3 updatedPos = PixelPerfectClamp(newPosition, PixelsPerUnit);
        //updatedPos.x = Mathf.Clamp(updatedPos.x, leftBounds, rightBounds);
        transform.position = updatedPos;
    }

    private Vector3 PixelPerfectClamp(Vector3 locationVector, float pixelsPerUnit)
    {
        Vector3 vectorInPixels = new Vector3(Mathf.CeilToInt(locationVector.x * pixelsPerUnit), Mathf.CeilToInt(locationVector.y * pixelsPerUnit), Mathf.CeilToInt(locationVector.z * pixelsPerUnit));
        return vectorInPixels / pixelsPerUnit;
    }
}

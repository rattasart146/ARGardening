using UnityEngine;
using System.Collections;

public class PlaneScaleCamera : MonoBehaviour
{
    public Camera resultCam;
    public GameObject areaMesh;
    private Bounds objectSize;
    int i = 0;

    void Start()
    {
    }
    void Update()
    {
        if (areaMesh != null)
        {
            objectSize = areaMesh.GetComponent<MeshFilter>().mesh.bounds;

            float screenRatio = (float)Screen.width / (float)Screen.height;
            float targetRatio = objectSize.size.x / objectSize.size.z;

            if (screenRatio >= targetRatio)
           {
                resultCam.orthographicSize = objectSize.size.z / 2;
            }
            else
            {
                float differenceInSize = targetRatio / screenRatio;
                resultCam.orthographicSize = objectSize.size.z / 2 * differenceInSize;
            }

            resultCam.transform.position = new Vector3(objectSize.center.x, resultCam.transform.position.y, objectSize.center.z);
        }
    }
}
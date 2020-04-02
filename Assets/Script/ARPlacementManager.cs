using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPlacementManager : MonoBehaviour
{
    public GameObject placementObject;
    public GameObject placementIndicator;
    public Camera arCamera;
    private Vector3 PlacementPose;
    private Quaternion PlacementRotaion;
    private bool placementPoseIsValid = false;
    private List<Vector3> markedPosition = new List<Vector3>();


    Ray myRay;
    RaycastHit hit;

    void Update()
    {
        //UpdatePlacementPose();
        UpdatePlacementIndicator();

        markedPosition.Add(PlacementPose);
        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            calculateRealPosition();
            ContinuePlaceObject();
        }

    }

    public void ContinuePlaceObject()
    {
        //Create Clone Object ----- try  to  change -----
        GameObject placeObject = Instantiate(placementObject, PlacementPose, PlacementRotaion);
    }

    private void UpdatePlacementIndicator()
    {

        myRay = arCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        var cameraForward = arCamera.transform.forward;
        var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;


        if (Physics.Raycast(myRay, out hit, 100))
        {
            if ((hit.transform.name == "ShapeMesh") || (hit.transform == placementObject))
            {
                PlacementPose = hit.point;
                PlacementRotaion = Quaternion.LookRotation(cameraBearing);

                placementIndicator.SetActive(true);
                placementIndicator.transform.SetPositionAndRotation(PlacementPose, PlacementRotaion);

                placementPoseIsValid = true;
            }
        }
        else
        {
            placementPoseIsValid = false;
        }
    }

    private void calculateRealPosition()
    {
        var halfObjectSizePosition = PlacementPose.y + (placementObject.GetComponent<Renderer>().bounds.size.y / 2);
        PlacementPose = new Vector3(PlacementPose.x, halfObjectSizePosition, PlacementPose.z);
    }
}

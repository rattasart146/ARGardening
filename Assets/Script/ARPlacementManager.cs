using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class ARPlacementManager : MonoBehaviour
{

    private ARRaycastManager _arRaycastManager;
    private ARPlaneManager arPlaneManager;
    private AREventManager arEventManager;

    public GameObject placementPrefab;
    public GameObject placementIndicator;
    public Camera arCamera;
    public GameObject baseIndicator;

    private Quaternion PlacementRotaion;
    private GameObject[] placedPrefabs;
    private Ray indicatorRay, placePrefabRay, selectPrefabRay;
    private RaycastHit indicatorHit, placePrefabHit, selectPrefabHit;
    private Vector3 PlacementPose;
    private GameObject loadedGameObject;
    private string doneStateCheck = "default";


    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
        arPlaneManager = GetComponent<ARPlaneManager>();
        arEventManager = GetComponent<AREventManager>();

        baseIndicator.gameObject.SetActive(false);
    }

    void Update()
    {
        SetAllPlanesActive(false);
        //UpdatePlacementPose();
        placedPrefabs = GameObject.FindGameObjectsWithTag("Decoration");

        if (doneStateCheck == "default")
        {
            arEventManager.placingPanel.SetActive(true);
            placementIndicator.SetActive(true);
            UpdatePlacementIndicator();
        }
        else
        {
            placementIndicator.SetActive(false);
        }

    }

    public void ChangePrefabSelection(string name)
    {

        loadedGameObject = Resources.Load<GameObject>($"Prefabs/{name}");
        if (loadedGameObject != null)
        {
            placementPrefab = loadedGameObject;
        }
        else
        {
            Debug.Log($"Unable to find a game object with name {name}");
        }
    }

    public void PlaceObject()
    {
        //Create Clone Object ----- try  to  change -----
        var Index = placedPrefabs.Length;
        GameObject placeObject = Instantiate(placementPrefab, PlacementPose, PlacementRotaion);
        placeObject.transform.name = $"{placementPrefab.name} {Index}";
    }

    private void UpdatePlacementIndicator()
    {

        indicatorRay = arCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        var cameraForward = arCamera.transform.forward;
        var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;


        if (Physics.Raycast(indicatorRay, out indicatorHit, 100))
        {
            if ((indicatorHit.transform.name == "AreaMesh") || (indicatorHit.transform == placementPrefab))
            {
                PlacementPose = indicatorHit.point;
                PlacementRotaion = Quaternion.LookRotation(cameraBearing);

                placementIndicator.SetActive(true);
                placementIndicator.transform.SetPositionAndRotation(PlacementPose, PlacementRotaion);

            }
        }
    }

    private void calculateRealPosition()
    {
        var halfObjectSizePosition = PlacementPose.y + (placementPrefab.GetComponent<Renderer>().bounds.size.y / 2);
        PlacementPose = new Vector3(PlacementPose.x, halfObjectSizePosition, PlacementPose.z);
    }

    void SetAllPlanesActive(bool value)
    {
        foreach (var plane in arPlaneManager.trackables)
            plane.gameObject.SetActive(value);
    }
}

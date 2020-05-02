using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class ARPlacementManager : MonoBehaviour
{

    private ARRaycastManager _arRaycastManager;

    public GameObject placementPrefab;
    public GameObject placementIndicator;
    public Camera arCamera;

    private Vector3 PlacementPose;
    private Quaternion PlacementRotaion;
    private List<GameObject> placedPrefabs = new List<GameObject>();
    private Vector2 touchPosition = default;
    private GameObject lastSelectedPrefab;
    private bool objectSelection;
    private Ray indicatorRay, placePrefabRay, selectPrefabRay;
    private RaycastHit indicatorHit, placePrefabHit, selectPrefabHit;
    private GameObject loadedGameObject;

    Text debugText;


    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
        debugText = GameObject.Find("DebugText").GetComponent<Text>();
    }

    void Update()
    {
        //UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            touchPosition = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                placePrefabRay = arCamera.ScreenPointToRay(touch.position);
                if (Physics.Raycast(placePrefabRay, out placePrefabHit))
                {
                    if (placePrefabHit.collider.transform.parent.tag == "Decoration")
                    {
                        lastSelectedPrefab = placePrefabHit.transform.parent.gameObject;

                        //debugText.text = lastSelectedPrefab.transform.name + " was Selected";
                        if (lastSelectedPrefab != null)
                        {
                            foreach (GameObject placementObject in placedPrefabs)
                            {
                                objectSelection = placementObject == lastSelectedPrefab;
                                if (objectSelection)
                                {
                                    debugText.text = lastSelectedPrefab.name + " Selected";
                                }
                            }
                        }
                    }
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                objectSelection = false;
            }

        }

        selectPrefabRay = arCamera.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(selectPrefabRay, out selectPrefabHit))
        {
            if (selectPrefabHit.transform.name == "AreaMesh")
            {
                if (objectSelection)
                {
                    lastSelectedPrefab.transform.position = selectPrefabHit.point;
                    lastSelectedPrefab.transform.rotation = Quaternion.identity;
                }
            }
        }

    }

    public void ChangePrefabSelection(string name)
    {
        loadedGameObject = Resources.Load<GameObject>($"Prefabs/{name}");
        if (loadedGameObject != null)
        {
            placementPrefab = loadedGameObject;
            debugText.text = placementPrefab.name + " was Active";
        }
        else
        {
            Debug.Log($"Unable to find a game object with name {name}");
        }
    }

    public void PlaceObject()
    {
        debugText.text = placementPrefab.name + " was placed";
        //Create Clone Object ----- try  to  change -----
        var Index = placedPrefabs.Count;
        GameObject placeObject = Instantiate(placementPrefab, PlacementPose, PlacementRotaion);
        placeObject.transform.name = $"{placementPrefab.name} {Index}";
        placedPrefabs.Add(placeObject);
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
}

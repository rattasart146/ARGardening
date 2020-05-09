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
    public GameObject baseIndicator;

    private Quaternion PlacementRotaion;
    private GameObject[] placedPrefabs;
    private GameObject lastSelectedPrefab;
    private bool objectSelection = false;
    private Ray indicatorRay, placePrefabRay, selectPrefabRay;
    private RaycastHit indicatorHit, placePrefabHit, selectPrefabHit;
    private Vector2 touchPosition = default;
    private Vector3 PlacementPose;
    private Vector3 selectingPos;
    private Vector3 selectTarget;
    private Vector3 placeTarget;
    private GameObject loadedGameObject;
    private string doneStateCheck = "default";

    Text debugText;
    Button placeButton, doneButton;


    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
        debugText = GameObject.Find("DebugText").GetComponent<Text>();
        placeButton = GameObject.Find("PlaceButton").GetComponent<Button>();
        doneButton = GameObject.Find("DoneButton").GetComponent<Button>();
        doneButton.onClick.AddListener(doneState);
        doneButton.gameObject.SetActive(false);
        baseIndicator.gameObject.SetActive(false);
    }

    void Update()
    {
        //UpdatePlacementPose();
        if(objectSelection == false)
        {

            UpdatePlacementIndicator();
        }

        placedPrefabs = GameObject.FindGameObjectsWithTag("Decoration");

        if (Input.touchCount == 1)
        {
            Touch fisrtTouch = Input.GetTouch(0);
            touchPosition = fisrtTouch.position;
            Debug.Log("Touched");
            if (fisrtTouch.phase == TouchPhase.Began)
            {
                if (objectSelection == false)
                {
                    placePrefabRay = arCamera.ScreenPointToRay(fisrtTouch.position);
                    if (Physics.Raycast(placePrefabRay, out placePrefabHit))
                    {
                        if (placePrefabHit.transform.parent != null)
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
                                            //do anything when object was selected
                                            selectTarget = new Vector3 (lastSelectedPrefab.transform.position.x, lastSelectedPrefab.transform.position.y + 0.1f, lastSelectedPrefab.transform.position.z);
                                            placeTarget = lastSelectedPrefab.transform.position;
                                            baseIndicator.transform.position = lastSelectedPrefab.transform.position;
                                            doneStateCheck = "Start";
                                            debugText.text = "pass1";
                                            debugText.text = doneStateCheck;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            selectPrefabRay = arCamera.ScreenPointToRay(touchPosition);
            if (Physics.Raycast(selectPrefabRay, out selectPrefabHit))
            {
                if (selectPrefabHit.transform.name == "AreaMesh")
                {
                    if (objectSelection)
                    {
                        baseIndicator.transform.position = selectPrefabHit.point;
                        lastSelectedPrefab.transform.position = new Vector3(selectPrefabHit.point.x, selectPrefabHit.point.y + 0.1f, selectPrefabHit.point.z);
                        selectTarget = lastSelectedPrefab.transform.position;
                        placeTarget = lastSelectedPrefab.transform.position - selectingPos;
                        Debug.Log(selectTarget);
                        Debug.Log(placeTarget);
                        debugText.text = "pass2";
                    }
                }
            }
            
        }

        if (doneStateCheck == "Start")
        {
            debugText.text = doneStateCheck;
            doneButton.gameObject.SetActive(true);
            placeButton.gameObject.SetActive(false);
            debugText.text = "pass3";
            lastSelectedPrefab.transform.position = Vector3.MoveTowards(lastSelectedPrefab.transform.position, selectTarget, Time.deltaTime * 7);
            if (lastSelectedPrefab.transform.position == selectTarget)
            {
                baseIndicator.gameObject.SetActive(true);
                doneStateCheck = "Active";
            }
        }
        if (doneStateCheck == "Active")
        {
            if (Input.touchCount == 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);

                if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
                {
                    var v2 = touch1.position - touch0.position;
                    var newAngle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
                    var realAngle = new Vector3(0, lastSelectedPrefab.transform.rotation.y - newAngle, 0);
                    Debug.Log("newAngle : " + newAngle);
                    Debug.Log("lastPrefab : " + lastSelectedPrefab.transform.rotation.y);
                    Debug.Log("realAngle : " + realAngle.y);

                    lastSelectedPrefab.transform.localEulerAngles = realAngle;
                    baseIndicator.transform.localEulerAngles = realAngle;
                }
            }
        }
        if (doneStateCheck == "End")
        {
            lastSelectedPrefab.transform.position = Vector3.MoveTowards(lastSelectedPrefab.transform.position, placeTarget, Time.deltaTime * 7);
            if (lastSelectedPrefab.transform.position == placeTarget)
            {
                doneStateCheck = "default";
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

    private void doneState()
    {
        doneButton.gameObject.SetActive(false);
        baseIndicator.gameObject.SetActive(false);
        placeButton.gameObject.SetActive(true);
        objectSelection = false;
        Debug.Log(objectSelection);
        doneStateCheck = "End";
    }
}

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
    private Vector3 selectingPos = new Vector3(0, 0.1f, 0);
    private Vector3 selectTarget = new Vector3(0, 0, 0);
    private Vector3 placeTarget = new Vector3(0, 0, 0);
    private GameObject loadedGameObject;
    private string doneStateCheck = "default";
    private float speed = 1;

    //Ui
    Text debugText;
    Button placeButton, doneButton;

    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
        arPlaneManager = GetComponent<ARPlaneManager>();

        debugText = GameObject.Find("DebugText").GetComponent<Text>();
        placeButton = GameObject.Find("PlaceButton").GetComponent<Button>();
        doneButton = GameObject.Find("DoneButton").GetComponent<Button>();

        placeButton.gameObject.SetActive(false);
        doneButton.gameObject.SetActive(false);
        baseIndicator.gameObject.SetActive(false);

        doneButton.onClick.AddListener(doneState);
    }
    private void Start()
    {
        placeButton.gameObject.SetActive(true);
    }
    void Update()
    {
        SetAllPlanesActive(false);
        //UpdatePlacementPose();
        placedPrefabs = GameObject.FindGameObjectsWithTag("Decoration");

        if (doneStateCheck == "default")
        {
            placementIndicator.SetActive(true);
            UpdatePlacementIndicator();
        }
        else
        {
            placementIndicator.SetActive(false);
        }

        if (Input.touchCount == 1)
        {
            Touch singleTouch = Input.GetTouch(0);

            touchPosition = singleTouch.position;
            Debug.Log("Touched");
            if (singleTouch.phase == TouchPhase.Began)
            {
                placePrefabRay = arCamera.ScreenPointToRay(singleTouch.position);

                if (doneStateCheck == "default")
                {
                    if (Physics.Raycast(placePrefabRay, out placePrefabHit))
                    {
                        if (placePrefabHit.collider.transform.parent.tag == "Decoration")
                        {
                            lastSelectedPrefab = placePrefabHit.transform.parent.gameObject;
                            debugText.text = lastSelectedPrefab.transform.name + " was Selected";
                            if (lastSelectedPrefab != null)
                            {
                                foreach (GameObject placementObject in placedPrefabs)
                                {
                                    objectSelection = placementObject == lastSelectedPrefab; //check object was hit
                                    Debug.Log("object selection : " + objectSelection);
                                    if (objectSelection)
                                    {
                                        //do anything when object was selected
                                        selectTarget = lastSelectedPrefab.transform.position + selectingPos;
                                        placeTarget = lastSelectedPrefab.transform.position;
                                        debugText.text = "selectTarget : " + selectTarget;
                                        baseIndicator.transform.position = lastSelectedPrefab.transform.position;
                                        doneStateCheck = "Active";
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (doneStateCheck == "Active")
                {
                    if (Physics.Raycast(placePrefabRay, out placePrefabHit))
                    {
                        if (placePrefabHit.collider.transform.parent.tag == "Decoration")
                        {
                            var activeSelectedPrefab = placePrefabHit.transform.parent.gameObject;
                            //debugText.text = lastSelectedPrefab.transform.name + " was Selected";
                            if (lastSelectedPrefab != null)
                            {
                                objectSelection = activeSelectedPrefab == lastSelectedPrefab; //check object was hit
                            }
                        }
                    }
                }
            }

            if (singleTouch.phase == TouchPhase.Ended)
            {
                objectSelection = false;
            }


            if (doneStateCheck == "Active")
            {
                if (objectSelection)
                {
                    selectPrefabRay = arCamera.ScreenPointToRay(singleTouch.position);
                    if (Physics.Raycast(selectPrefabRay, out selectPrefabHit))
                    {
                        if (selectPrefabHit.collider.name == "AreaMesh")
                        {
                            debugText.text = "happen1";
                            baseIndicator.transform.position = selectPrefabHit.point;
                            lastSelectedPrefab.transform.position = selectPrefabHit.point + selectingPos; //auto increase y pose
                            selectTarget = lastSelectedPrefab.transform.position;
                            placeTarget = lastSelectedPrefab.transform.position - selectingPos;
                            //Debug.Log(selectPrefabHit.point + new Vector3(0, selectTarget.y, 0));
                            debugText.text = "happen2";
                        }
                    }
                }
            }
        }

        if (doneStateCheck == "Start")
        {
            doneButton.gameObject.SetActive(true);
            placeButton.gameObject.SetActive(false);
            lastSelectedPrefab.transform.position = Vector3.Lerp(lastSelectedPrefab.transform.position, selectTarget, Time.deltaTime * speed);
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
                    //Debug.Log("newAngle : " + newAngle);
                    //Debug.Log("lastPrefab : " + lastSelectedPrefab.transform.rotation.y);
                    //Debug.Log("realAngle : " + realAngle.y);

                    lastSelectedPrefab.transform.localEulerAngles = realAngle;
                    baseIndicator.transform.localEulerAngles = realAngle;
                }
            }
        }
        if (doneStateCheck == "End")
        {
            lastSelectedPrefab.transform.position = Vector3.Lerp(lastSelectedPrefab.transform.position, placeTarget, Time.deltaTime * speed);
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

    void SetAllPlanesActive(bool value)
    {
        foreach (var plane in arPlaneManager.trackables)
            plane.gameObject.SetActive(value);
    }
}

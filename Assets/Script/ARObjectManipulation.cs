using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class ARObjectManipulation : MonoBehaviour
{

    private ARRaycastManager _arRaycastManager;
    private ARPlaneManager arPlaneManager;
    private AREventManager arEventManager;

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
    private float speed = 15;


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

        if (Input.touchCount == 1)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return;
            }

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
                        if (placePrefabHit.collider.transform.root.tag == "Decoration")
                        {
                            lastSelectedPrefab = placePrefabHit.transform.root.gameObject;
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
                                        baseIndicator.transform.position = lastSelectedPrefab.transform.position;
                                        doneStateCheck = "Start";
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
                        if (placePrefabHit.collider.transform.root.tag == "Decoration")
                        {
                            var activeSelectedPrefab = placePrefabHit.transform.root.gameObject;
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
                            baseIndicator.transform.position = selectPrefabHit.point;
                            lastSelectedPrefab.transform.position = selectPrefabHit.point + selectingPos; //auto increase y pose
                            selectTarget = lastSelectedPrefab.transform.position;
                            placeTarget = lastSelectedPrefab.transform.position - selectingPos;
                            //Debug.Log(selectPrefabHit.point + new Vector3(0, selectTarget.y, 0));
                        }
                    }
                }
            }
        }

        if (doneStateCheck == "Start")
        {
            lastSelectedPrefab.transform.position = Vector3.Lerp(lastSelectedPrefab.transform.position, selectTarget, Time.deltaTime * speed);
            if (lastSelectedPrefab.transform.position == selectTarget)
            {
                doneStateCheck = "Active";
            }
        }
        if (doneStateCheck == "Active")
        {
            baseIndicator.gameObject.SetActive(true);
            arEventManager.manipulationPanel.SetActive(true);
            arEventManager.placingPanel.SetActive(false);
            arEventManager.selectingPanel.SetActive(false);
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
            if (lastSelectedPrefab != null)
            {
                lastSelectedPrefab.transform.position = Vector3.Lerp(lastSelectedPrefab.transform.position, placeTarget, Time.deltaTime * speed);
                if (lastSelectedPrefab.transform.position == placeTarget)
                {
                    doneStateCheck = "default";
                    arEventManager.manipulationPanel.SetActive(false);
                    baseIndicator.gameObject.SetActive(false);
                }
            }
            else
            {
                    doneStateCheck = "default";
                    arEventManager.manipulationPanel.SetActive(false);
                    baseIndicator.gameObject.SetActive(false);
            }
        }
    }

    public void doneState()
    {
        objectSelection = false;
        Debug.Log(objectSelection);
        doneStateCheck = "End";
    }

    public void destroyObject()
    {
        objectSelection = false;
        Destroy(lastSelectedPrefab);
        doneStateCheck = "End";
    }

    void SetAllPlanesActive(bool value)
    {
        foreach (var plane in arPlaneManager.trackables)
            plane.gameObject.SetActive(value);
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}

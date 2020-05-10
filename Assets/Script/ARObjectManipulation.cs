using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class ARObjectManipulation : MonoBehaviour
{
    private ARRaycastManager _arRaycastManager;

    public GameObject placementPrefab;
    public GameObject baseIndicator;
    public Camera arCamera;

    private GameObject[] placedPrefabs;
    private Vector2 touchPosition = default;
    private GameObject lastSelectedPrefab;
    private bool objectSelection;
    private Ray placePrefabRay, selectPrefabRay;
    private RaycastHit placePrefabHit, selectPrefabHit;

    Button placeButton, doneButton;
    // Start is called before the first frame update
    void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
        placeButton = GameObject.Find("PlaceButton").GetComponent<Button>();
        doneButton = GameObject.Find("DoneButton").GetComponent<Button>();
        //doneButton.onClick.AddListener(doneState);
    }

    // Update is called once per frame
    void Update()
    {
        placedPrefabs = GameObject.FindGameObjectsWithTag("Decoration");

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            touchPosition = touch.position;
            placePrefabRay = arCamera.ScreenPointToRay(touch.position);

            if (objectSelection)
            {
                placeButton.gameObject.SetActive(false);
                doneButton.gameObject.SetActive(true);
                if (touch.phase == TouchPhase.Began)
                {
                    selectPrefabRay = arCamera.ScreenPointToRay(touchPosition);
                    if (Physics.Raycast(selectPrefabRay, out selectPrefabHit))
                    {
                        lastSelectedPrefab.transform.position = selectPrefabHit.point;
                        lastSelectedPrefab.transform.rotation = Quaternion.identity;
                    }
                }
            }
            else
            {
                if (Physics.Raycast(placePrefabRay, out placePrefabHit))
                {
                    if (placePrefabHit.collider.tag == "Decoration")
                    {
                        lastSelectedPrefab = placePrefabHit.transform.gameObject;

                        //debugText.text = lastSelectedPrefab.transform.name + " was Selected";
                        if (lastSelectedPrefab != null)
                        {
                            foreach (GameObject placementObject in placedPrefabs)
                            {
                                objectSelection = placementObject == lastSelectedPrefab;
                            }
                        }
                    }
                }
            }
        }

    }

    private void doneState()
    {
        placeButton.gameObject.SetActive(true);
        doneButton.gameObject.SetActive(false);
        objectSelection = false;
    }
}

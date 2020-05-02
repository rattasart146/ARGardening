using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testHit : MonoBehaviour
{
    public GameObject placementPrefab;
    public GameObject baseIndicator;

    private GameObject[] placedPrefabs;
    private Vector2 touchPosition = default;
    private GameObject lastSelectedPrefab;
    private bool objectSelection = false;
    private Ray placePrefabRay, selectPrefabRay;
    private RaycastHit placePrefabHit, selectPrefabHit;

    Button placeButton, doneButton;
    // Start is called before the first frame update
    void Awake()
    {
        doneButton = GameObject.Find("DoneButton").GetComponent<Button>();
        doneButton.onClick.AddListener(doneState);
        doneButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        placedPrefabs = GameObject.FindGameObjectsWithTag("Decoration");

        if (Input.GetMouseButtonDown(0))
        {
            if (objectSelection)
            {
                selectPrefabRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(selectPrefabRay, out selectPrefabHit))
                {
                    if (selectPrefabHit.transform.name == "Plane")
                    {
                        lastSelectedPrefab.transform.position = selectPrefabHit.point;
                        lastSelectedPrefab.transform.rotation = Quaternion.identity;
                    }
                }
            }
            else
            {
                placePrefabRay = Camera.main.ScreenPointToRay(Input.mousePosition);
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
                                    Debug.Log(lastSelectedPrefab.name);
                                    doneButton.gameObject.SetActive(true);
                                }
                            }
                        }
                    }
                }
            }
        }

    }

    private void doneState()
    {
        doneButton.gameObject.SetActive(false);
        objectSelection = false;
    }
}

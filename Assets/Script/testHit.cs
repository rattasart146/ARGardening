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
    private Vector3 selectingPos = new Vector3(0, 1, 0);
    private Vector3 selectTarget = new Vector3(0, 0, 0);
    private Vector3 placeTarget =  new Vector3(0, 0, 0);
    private string doneStateCheck = "default";

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

        if (Input.touchCount >= 1)
        {
            Touch firstTouch = Input.GetTouch(0);
            Debug.Log("Touched");
            if (objectSelection)
            {
                selectPrefabRay = Camera.main.ScreenPointToRay(firstTouch.position);
                if (Physics.Raycast(selectPrefabRay, out selectPrefabHit))
                {
                    if (selectPrefabHit.transform.name == "Plane")
                    {
                        lastSelectedPrefab.transform.position = selectPrefabHit.point + new Vector3(0, selectTarget.y, 0);
                        lastSelectedPrefab.transform.rotation = Quaternion.identity;
                        selectTarget = lastSelectedPrefab.transform.position;
                        placeTarget = lastSelectedPrefab.transform.position - selectingPos;
                        Debug.Log(selectTarget);
                        Debug.Log(placeTarget);
                    }
                }
            }
            else
            {
                placePrefabRay = Camera.main.ScreenPointToRay(firstTouch.position);
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
                                        selectTarget = lastSelectedPrefab.transform.position + selectingPos;
                                        doneButton.gameObject.SetActive(true);
                                        doneStateCheck = "Active";
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (Input.touchCount >= 2)
        {

        }

        if (doneStateCheck == "Active")
        {
            lastSelectedPrefab.transform.position = Vector3.MoveTowards(lastSelectedPrefab.transform.position, selectTarget, Time.deltaTime * 7);
        }
        if (doneStateCheck == "End")
        {
            lastSelectedPrefab.transform.position = Vector3.MoveTowards(lastSelectedPrefab.transform.position, placeTarget, Time.deltaTime * 7);
            if(lastSelectedPrefab.transform.position == placeTarget)
            {
                doneStateCheck = "default";
            }
        }
    }

    private void doneState()
    {
        doneButton.gameObject.SetActive(false);
        objectSelection = false;
        Debug.Log(objectSelection);
        doneStateCheck = "End";
    }
}

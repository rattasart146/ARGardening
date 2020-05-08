using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testHit : MonoBehaviour
{
    public GameObject placementPrefab;
    public GameObject baseIndicator;

    private GameObject[] placedPrefabs;
    private Vector3 baseIndicatorPos;
    private Vector2 touchPosition = default;
    private GameObject lastSelectedPrefab;
    private bool objectSelection = false;
    private Ray placePrefabRay, selectPrefabRay;
    private RaycastHit placePrefabHit, selectPrefabHit;
    private Vector3 selectingPos = new Vector3(0, 0.5f, 0);
    private Vector3 selectTarget = new Vector3(0, 0, 0);
    private Vector3 placeTarget =  new Vector3(0, 0, 0);
    private string doneStateCheck = "default";
    private float previousValue;

    public bool Rotate;
    bool rotating;
    Vector2 startVector;
    float rotGestureWidth;
    float rotAngleMinimum;
 
    Button placeButton, doneButton;
    Slider rotateSlider;
    // Start is called before the first frame update
    void Awake()
    {
        doneButton = GameObject.Find("DoneButton").GetComponent<Button>();
        doneButton.onClick.AddListener(doneState);
        doneButton.gameObject.SetActive(false);
        baseIndicator.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        placedPrefabs = GameObject.FindGameObjectsWithTag("Decoration");

        if (Input.touchCount == 1)
        {
            Touch fisrtTouch = Input.GetTouch(0);
            Debug.Log("Touched");
            if (objectSelection)
            {
                selectPrefabRay = Camera.main.ScreenPointToRay(fisrtTouch.position);
                if (Physics.Raycast(selectPrefabRay, out selectPrefabHit))
                {
                    if (selectPrefabHit.transform.name == "Plane")
                    {
                        baseIndicator.transform.position = selectPrefabHit.point;
                        lastSelectedPrefab.transform.position = selectPrefabHit.point + new Vector3(0, selectTarget.y, 0);
                        selectTarget = lastSelectedPrefab.transform.position;
                        placeTarget = lastSelectedPrefab.transform.position - selectingPos;
                        Debug.Log(selectTarget);
                        Debug.Log(placeTarget);
                    }
                }
            }
            else
            {
                placePrefabRay = Camera.main.ScreenPointToRay(fisrtTouch.position);
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
            }
        }

        if (doneStateCheck == "Start")
        {
            doneButton.gameObject.SetActive(true);

            lastSelectedPrefab.transform.position = Vector3.MoveTowards(lastSelectedPrefab.transform.position, selectTarget, Time.deltaTime * 7);
            if (lastSelectedPrefab.transform.position == selectTarget)
            {
                baseIndicator.gameObject.SetActive(true);
                doneStateCheck = "Active";
            }
        }
        if (doneStateCheck == "Active")
        {
            if(Input.touchCount == 2)
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
            if(lastSelectedPrefab.transform.position == placeTarget)
            {
                doneStateCheck = "default";
            }
        }
    }

    private void doneState()
    {
        doneButton.gameObject.SetActive(false);
        baseIndicator.gameObject.SetActive(false);
        objectSelection = false;
        Debug.Log(objectSelection);
        doneStateCheck = "End";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testHit : MonoBehaviour
{
    public GameObject placementPrefab;
    public GameObject baseIndicator;

    private GameObject[] placedPrefabs;
    private GameObject lastSelectedPrefab;
    private bool objectSelection = false;
    private Ray placePrefabRay, selectPrefabRay;
    private RaycastHit placePrefabHit, selectPrefabHit;
    private Vector3 selectingPos = new Vector3(0, 1f, 0);
    private Vector3 selectTarget = new Vector3(0, 0, 0);
    private Vector3 placeTarget =  new Vector3(0, 0, 0);
    private string doneStateCheck = "default";
    private float speed = 7;

    Button doneButton;

    RaycastHit hit;
    // Start is called before the first frame update
    void Awake()
    {
        doneButton = GameObject.Find("DoneButton").GetComponent<Button>();
        doneButton.onClick.AddListener(doneState);
        doneButton.gameObject.SetActive(false);
        baseIndicator.gameObject.SetActive(false);
    }
    private void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

        placedPrefabs = GameObject.FindGameObjectsWithTag("Decoration");

        if (Input.touchCount == 1)
        {
            Touch singleTouch = Input.GetTouch(0);
            Debug.Log("Touched");
            if (singleTouch.phase == TouchPhase.Began)
            {
                placePrefabRay = Camera.main.ScreenPointToRay(singleTouch.position);

                if (doneStateCheck == "default")
                {
                    if (Physics.Raycast(placePrefabRay, out placePrefabHit))
                    {
                        Debug.Log(placePrefabHit.collider.transform.root.name);
                        if (placePrefabHit.collider.transform.root.tag == "Decoration")
                        {
                            lastSelectedPrefab = placePrefabHit.transform.root.gameObject;
                            //debugText.text = lastSelectedPrefab.transform.name + " was Selected";
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
                if(doneStateCheck == "Active")
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
                    selectPrefabRay = Camera.main.ScreenPointToRay(singleTouch.position);
                    if (Physics.Raycast(selectPrefabRay, out selectPrefabHit))
                    {
                        if (selectPrefabHit.collider.name == "Plane")
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
            doneButton.gameObject.SetActive(true);
            Debug.Log("<");
            lastSelectedPrefab.transform.position = Vector3.Lerp(lastSelectedPrefab.transform.position, selectTarget, Time.deltaTime * speed);
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
            Debug.Log(">");
            lastSelectedPrefab.transform.position = Vector3.Lerp(lastSelectedPrefab.transform.position, placeTarget, Time.deltaTime * speed);
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

    public void destroyObject2()
    {
        doneButton.gameObject.SetActive(false);
        baseIndicator.gameObject.SetActive(false);
        Destroy(lastSelectedPrefab);
        doneStateCheck = "End";
    }
}

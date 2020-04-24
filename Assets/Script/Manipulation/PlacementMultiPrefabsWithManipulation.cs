using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementMultiPrefabsWithManipulation : MonoBehaviour
{
    public List<GameObject> placementObjectList= new List<GameObject>();
    public GameObject placedObject;
    public GameObject parentObject;

    GameObject objectInfo;

    private bool placementPoseIsValid = false;


    Ray myRay;
    RaycastHit hit;

    void Update()
    {
        myRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(myRay, out hit, 100))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if(hit.transform.name == "Plane")
                {
                    var Index = placementObjectList.Count;
                    objectInfo = Instantiate(placedObject, hit.point, Quaternion.identity);
                    objectInfo.transform.parent = parentObject.transform;
                    objectInfo.transform.name = "prefabe " + Index;
                    placementObjectList.Add(objectInfo);
                }

                foreach (GameObject objectInPlace in placementObjectList)
                {
                    if (hit.transform.IsChildOf(objectInfo.transform.parent))
                    {
                       var objectName = hit.transform.parent.name;
                       if (objectName == objectInPlace.name)
                       {
                           Debug.Log(objectName);
                       }
                    }
                }
            }

        }   
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class AREventManager : MonoBehaviour
{
    private ARPlaneManager arPlaneManager;
    private ARShapeBuilder arShapeBuilder;
    private ARPlacementManager arPlacementManager;
    private string stateChecker = "Start";
    Button completeMarkButton, placeButton, doneButton;
    Text startButtonText, debugText;
    GameObject markerPanel, planeDetectPanel, placingPanel;
    // Start is called before the first frame update
    void Awake()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
        arShapeBuilder = GetComponent<ARShapeBuilder>();
        arPlacementManager = GetComponent<ARPlacementManager>();
        //startButtonText = GameObject.Find("StartButtonText").GetComponent<Text>();
        completeMarkButton = GameObject.Find("CompleteMarkButton").GetComponent<Button>();


        markerPanel = GameObject.Find("MarkerPanel");
        placingPanel = GameObject.Find("PlacingPanel");
        planeDetectPanel = GameObject.Find("PlaneDetectPanel");
        markerPanel.SetActive(false);
        placingPanel.SetActive(false);
    }

    private void Update()
    {
        if (stateChecker == "Start")
        {
            planeDetectPanel.SetActive(true);
            if (arPlaneManager.trackables.count > 0)
            {
                stateChecker = "Marking";
            }
        }
        else
        {
            foreach (var plane in arPlaneManager.trackables)
                plane.gameObject.SetActive(false);
        }

        if (stateChecker == "Marking")
        {
            planeDetectPanel.SetActive(false);
            placingPanel.SetActive(false);
            markerPanel.SetActive(true);

            arShapeBuilder.enabled = true;
            arPlacementManager.enabled = false;
        }
        if (stateChecker == "Placing")
        {
            planeDetectPanel.SetActive(false);
            markerPanel.SetActive(false);
            placingPanel.SetActive(true);

            arShapeBuilder.enabled = false;
            arPlacementManager.enabled = true;
        }
        
    }

    public void processChecker(string text)
    {
        stateChecker = text;
    }
}

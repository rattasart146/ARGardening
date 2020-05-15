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
    public GameObject markerPanel, planeDetectPanel, placingPanel, manipulationPanel;
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
        manipulationPanel = GameObject.Find("ManipulationPanel");
        planeDetectPanel = GameObject.Find("PlaneDetectPanel");
        markerPanel.SetActive(false);
        placingPanel.SetActive(false);
        manipulationPanel.SetActive(false);
        arShapeBuilder.markerPointIndicator.SetActive(false);
        arPlacementManager.placementIndicator.SetActive(false);
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
            arShapeBuilder.markerPointIndicator.SetActive(true);
            arPlacementManager.placementIndicator.SetActive(false);
            arShapeBuilder.enabled = true;
            arPlacementManager.enabled = false;
        }
        if (stateChecker == "Placing")
        {
            arShapeBuilder.markerPointIndicator.SetActive(false);
            arPlacementManager.placementIndicator.SetActive(true);
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

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
    private ARObjectManipulation arObjectManipulation;
    private string stateChecker = "Start";
    private Image selectionImg;
    public GameObject markerPanel, planeDetectPanel, selectingPanel, placingPanel, manipulationPanel;
    public GameObject saveButton, captureButton;
    // Start is called before the first frame update
    void Awake()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
        arShapeBuilder = GetComponent<ARShapeBuilder>();
        arPlacementManager = GetComponent<ARPlacementManager>();
        arObjectManipulation = GetComponent<ARObjectManipulation>();
        //startButtonText = GameObject.Find("StartButtonText").GetComponent<Text>();
        

        markerPanel = GameObject.Find("MarkerPanel");
        placingPanel = GameObject.Find("PlacingPanel");
        selectingPanel = GameObject.Find("SelectingPanel");
        manipulationPanel = GameObject.Find("ManipulationPanel");
        planeDetectPanel = GameObject.Find("PlaneDetectPanel");
        saveButton = GameObject.Find("SaveResultButton");
        captureButton = GameObject.Find("CaptureButton");
        selectionImg = GameObject.Find("SelectionImageBar").GetComponent<Image>();

        markerPanel.SetActive(false);
        placingPanel.SetActive(false);
        manipulationPanel.SetActive(false);
        selectingPanel.SetActive(false);
        saveButton.SetActive(false);
        captureButton.SetActive(false);

        arShapeBuilder.markerPointIndicator.SetActive(false);
        arPlacementManager.placementIndicator.SetActive(false);
    }

    private void Update()
    {
        var decorationObj = GameObject.FindGameObjectsWithTag("Decoration");

        selectionImg.sprite = Resources.Load<Sprite>($"trees/{arPlacementManager.getSelectionData()}");
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
            captureButton.SetActive(false);
            planeDetectPanel.SetActive(false);
            arShapeBuilder.markerPointIndicator.SetActive(true);
            arShapeBuilder.enabled = true;
            placingPanel.SetActive(false);
            markerPanel.SetActive(true);
            selectingPanel.SetActive(false);
            arPlacementManager.placementIndicator.SetActive(false);

            arPlacementManager.enabled = false;
            arObjectManipulation.enabled = false;
        }
        if (stateChecker == "Selecting")
        {
            captureButton.SetActive(true);
            selectingPanel.SetActive(true);
            arShapeBuilder.markerPointIndicator.SetActive(false);
            arPlacementManager.placementIndicator.SetActive(false);
            planeDetectPanel.SetActive(false);
            markerPanel.SetActive(false);
            placingPanel.SetActive(false);

            if (decorationObj != null)
            {
                saveButton.SetActive(true);
            }
            else
            {
                saveButton.SetActive(false);
            }

            arShapeBuilder.enabled = false;
            arPlacementManager.enabled = false;
            arObjectManipulation.enabled = true;
        }
        if (stateChecker == "Placing")
        {
            captureButton.SetActive(false);
            captureButton.SetActive(false);
            selectingPanel.SetActive(false);
            arShapeBuilder.markerPointIndicator.SetActive(false);
            arPlacementManager.placementIndicator.SetActive(true);
            planeDetectPanel.SetActive(false);
            markerPanel.SetActive(false);
            placingPanel.SetActive(true);

            arShapeBuilder.enabled = false;
            arPlacementManager.enabled = true;
            arObjectManipulation.enabled = true;
        }
        
    }

    public void processChecker(string text)
    {
        stateChecker = text;
    }
}

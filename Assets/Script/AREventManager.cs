using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class AREventManager : MonoBehaviour
{
    private ARShapeBuilder arShapeBuilder;
    private ARPlacementManager arPlacementManager;
    private ARPlaneManager arPlaneManager;
    Button startButton, placeButton;
    GameObject treePanel;
    Text startButtonText, debugText;
    // Start is called before the first frame update
    void Awake()
    {
        arShapeBuilder = GetComponent<ARShapeBuilder>();
        arPlacementManager = GetComponent<ARPlacementManager>();
        arPlaneManager = GetComponent<ARPlaneManager>();

        startButtonText = GameObject.Find("StartButtonText").GetComponent<Text>();
        startButton = GameObject.Find("StartButton").GetComponent<Button>();
        placeButton = GameObject.Find("PlaceButton").GetComponent<Button>();
        treePanel = GameObject.Find("TreePanel");
        placeButton.gameObject.SetActive(false);
        treePanel.gameObject.SetActive(false);
    }

    public void StartButton(string text)
    {
        if (startButtonText.text == "Finish")
        {
            arShapeBuilder.enabled = false;
            arPlacementManager.enabled = true;
            startButton.gameObject.SetActive(false);
            placeButton.gameObject.SetActive(true);
            treePanel.gameObject.SetActive(true);
            SetAllPlanesActive(false);
        }
        else
        {
            startButtonText.text = text;
            arShapeBuilder.enabled = true;
        }
    }

    void SetAllPlanesActive(bool value)
    {
        foreach (var plane in arPlaneManager.trackables)
            plane.gameObject.SetActive(value);
    }
}

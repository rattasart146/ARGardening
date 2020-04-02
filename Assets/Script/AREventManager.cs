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
    Button startButton;
    Text startButtonText;
    // Start is called before the first frame update
    void Start()
    {
        arShapeBuilder = GetComponent<ARShapeBuilder>();
        arPlaneManager = GetComponent<ARPlaneManager>();

        startButtonText = GameObject.Find("StartButtonText").GetComponent<Text>();
        startButtonText = GameObject.Find("TogglePlaceButtonText").GetComponent<Text>();

        startButton = GameObject.Find("StartButton").GetComponent<Button>();
    }

    public void StartButton(string text)
    {
        if (startButtonText.text == "Finish")
        {
            arShapeBuilder.enabled = false;
            arPlacementManager.enabled = true;
            SetAllPlanesActive(false);
            startButton.gameObject.SetActive(false);
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

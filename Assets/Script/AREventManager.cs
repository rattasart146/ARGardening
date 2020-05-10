using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class AREventManager : MonoBehaviour
{
    private ARShapeBuilder arShapeBuilder;
    private ARPlacementManager arPlacementManager;
    Button startButton, placeButton, doneButton;
    Text startButtonText, debugText;
    // Start is called before the first frame update
    void Awake()
    {
        arShapeBuilder = GetComponent<ARShapeBuilder>();
        arPlacementManager = GetComponent<ARPlacementManager>();
        startButtonText = GameObject.Find("StartButtonText").GetComponent<Text>();
        startButton = GameObject.Find("StartButton").GetComponent<Button>();
   
    }

    public void StartButton(string text)
    {
        if (startButtonText.text == "Finish")
        {
            arShapeBuilder.enabled = false;
            arPlacementManager.enabled = true;
            startButton.gameObject.SetActive(false);
            
        }
        else
        {
            startButtonText.text = text;
            arShapeBuilder.enabled = true;
        }
    }
}

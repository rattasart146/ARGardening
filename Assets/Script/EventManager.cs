using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    private ShapeManager shapeBuilder;
    // Start is called before the first frame update
    void Start()
    {
        shapeBuilder = GetComponent<ShapeManager>();
    }

    public void StartController(string text)
    {
        Text txt = GameObject.Find("Text").GetComponent<Text>();
        txt.text = text;

        shapeBuilder.enabled = true;
    }
}

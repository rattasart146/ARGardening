using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        Vector2 v2 = new Vector2(1, 2);
        Debug.Log("Vector2 is: " + v2);

        // convert v2 to v3
        Vector3 v3 = v2;
        Debug.Log("Vector3 is: " + v3);

        // convert v3 to new Vector3
        Debug.Log("Set v3 to (3, 4, 5)");
        v3 = new Vector3(3, 4, 5);

        // convert v3 to v2
        v2 = v3;
        Debug.Log("Vector2 is: " + v2);
    }
}

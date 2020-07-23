using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateLayout : MonoBehaviour
{
    public VerticalLayoutGroup verticalLayout;
    // Start is called before the first frame update
    void Start()
    {
        verticalLayout = GetComponent<VerticalLayoutGroup>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {

        verticalLayout.childForceExpandWidth = false;
        verticalLayout.childForceExpandWidth = true;
    }
}

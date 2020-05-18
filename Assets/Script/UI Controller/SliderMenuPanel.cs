using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderMenuPanel : MonoBehaviour
{
    public GameObject cardParentPanel;
    // Start is called before the first frame update
    public void ShowHideMenu()
    {
        if(cardParentPanel != null)
        {
            Animator animator = cardParentPanel.GetComponent<Animator>();
            if (animator != null)
            {
                bool isOpen = animator.GetBool("show");
                animator.SetBool("show", !isOpen);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class CardItemManager : MonoBehaviour
{
    public GameObject cardPanel;
    public Transform cardParent;
    private Text cardTag, cardName, cardPrice;
    private Button IDButton;
    private Image cardImage;
    private ARPlacementManager m_arPlacementManager;

    // Detail page ui
    private Image detailImage;
    private Text detailTag, detailName, detailContent, detailPrice;
    private Button selectIdButton;
    private SliderMenuPanel sliderController;

    // Start is called before the first frame update
    public DecorationComponent decorationComponent = new DecorationComponent();

    private void Awake()
    {
        sliderController = GameObject.Find($"DetailPanel").GetComponent<SliderMenuPanel>();
        m_arPlacementManager = GetComponent<ARPlacementManager>();
    }

    void Start()
    {
        TextAsset asset = Resources.Load("Decoration") as TextAsset;
        decorationComponent = JsonUtility.FromJson<DecorationComponent>(asset.text);
        if (asset != null)
        {
            foreach (Decoration decoration in decorationComponent.Decoration)
            {
                cardPanel = Instantiate(cardPanel, cardParent);
                cardPanel.transform.name = decoration.title;
                cardTag = GameObject.Find($"CardParentPanel/{decoration.title}/TagName").GetComponent<Text>();
                cardName = GameObject.Find($"CardParentPanel/{decoration.title}/CardName").GetComponent<Text>();
                cardPrice = GameObject.Find($"CardParentPanel/{decoration.title}/Price").GetComponent<Text>();
                IDButton = GameObject.Find($"CardParentPanel/{decoration.title}/IDButton").GetComponent<Button>();
                cardTag.text = decoration.tag;
                cardName.text = decoration.title;
                cardPrice.text = $"{decoration.price}฿";
                IDButton.onClick.AddListener(() => ShowDataFromClick(decoration.ID));
            }
        }
        else
        {
            Debug.Log(asset);
        }
    }

    private void ShowDataFromClick(string text)
    {
        Debug.Log(text);
        sliderController.ShowHideMenu();
        foreach (Decoration decoration in decorationComponent.Decoration)
        {
            if (decoration.ID == text)
            {
                detailTag = GameObject.Find($"DetailPanel/TagText").GetComponent<Text>();
                detailName = GameObject.Find($"DetailPanel/NameText").GetComponent<Text>();
                detailContent = GameObject.Find($"DetailPanel/ContentText").GetComponent<Text>();
                detailPrice = GameObject.Find($"DetailPanel/PriceText").GetComponent<Text>();
                selectIdButton = GameObject.Find($"DetailPanel/SelectoPlaceButton").GetComponent<Button>();

                detailTag.text = decoration.tag;
                detailName.text = decoration.title;
                detailContent.text = decoration.content;
                detailPrice.text = $"{decoration.price}฿";
                selectIdButton.onClick.AddListener(() => m_arPlacementManager.ChangePrefabSelection(decoration.ojectName));
            }
        }
    }

    private void testObject(string iD)
    {
        Debug.Log(iD);
    }
}

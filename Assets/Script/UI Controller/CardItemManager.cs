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
    private AREventManager m_arEventManager;
    // Detail page ui
    private Image detailImage;
    private Text detailTag, detailName, detailContent, detailPrice;
    private Button selectIdButton;
    private SliderMenuPanel sliderController, sliderControllerDetail;
    Text text;
    // Start is called before the first frame update
    public DecorationComponent decorationComponent = new DecorationComponent();

    private void Awake()
    {
        sliderController = GameObject.Find($"DetailPanel").GetComponent<SliderMenuPanel>();
        sliderControllerDetail = GameObject.Find($"MenuPanel").GetComponent<SliderMenuPanel>();
        m_arPlacementManager = GetComponent<ARPlacementManager>();
        m_arEventManager = GetComponent<AREventManager>();
        //text = GameObject.Find("Canvas/Text").GetComponent<Text>();
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
                Debug.Log(decoration.ojectName);
                detailPrice.text = $"{decoration.price}฿";
                selectIdButton.onClick.AddListener(() => SelectDataFromClick(decoration.ojectName));
            }
        }
    }

    private void SelectDataFromClick(string objectName)
    {
        //text.text = iD;
        sliderController.ShowHideMenu();
        sliderControllerDetail.ShowHideMenu();
        m_arEventManager.processChecker("Placing");
        m_arPlacementManager.ChangePrefabSelection(objectName);
    }
}

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
    // Result page ui
    public GameObject resultPanel;
    public Transform resultParent;
    private Text item, amount, price, totalPrice;
    private List<string> resultList = new List<string>();

    private GameObject[] decorationPrefabs;

    // Start is called before the first frame update
    public DecorationComponent decorationComponent = new DecorationComponent();

    private void Awake()
    {
        sliderController = GameObject.Find($"DetailPanel").GetComponent<SliderMenuPanel>();
        sliderControllerDetail = GameObject.Find($"MenuPanel").GetComponent<SliderMenuPanel>();
        totalPrice = GameObject.Find($"TotalPrice/PriceTotal").GetComponent<Text>();
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
                cardImage = GameObject.Find($"CardParentPanel/{decoration.title}/Image").GetComponent<Image>();

                cardImage.sprite = Resources.Load<Sprite>($"trees/{decoration.ojectName}");
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
                detailImage = GameObject.Find($"DetailPanel/ObjectImage").GetComponent<Image>();

                detailImage.sprite = Resources.Load<Sprite>($"trees/{decoration.ojectName}");
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

    public void ShowResult()
    {
        var totalPricing = 0;
        var totalPricingByOrder = 0;
        var amountCounting = 0;
        var amountIndex = 0;
        string resultString;
        decorationPrefabs = GameObject.FindGameObjectsWithTag("Decoration"); //Pull data from "Decoration" Tag

        foreach (Decoration decoration in decorationComponent.Decoration)
        {
            foreach (GameObject placedDecoration in decorationPrefabs)
            {
                if (placedDecoration.transform.name.Contains(decoration.ojectName))
                {
                    if (!resultList.Contains(decoration.ojectName))
                    {
                        resultPanel = Instantiate(resultPanel, resultParent);
                        resultPanel.transform.name = decoration.ojectName;
                        resultList.Add(resultPanel.name);

                        item = GameObject.Find($"ItemListParent/{decoration.ojectName}/Item").GetComponent<Text>();
                        amount = GameObject.Find($"ItemListParent/{decoration.ojectName}/Amount").GetComponent<Text>();
                        price = GameObject.Find($"ItemListParent/{decoration.ojectName}/Price").GetComponent<Text>();
                    }

                    amountCounting++;
                    item.text = decoration.title;
                    totalPricingByOrder = amountCounting * decoration.price;
                    totalPricing += decoration.price;
                    amount.text = amountCounting.ToString();
                    price.text = totalPricingByOrder.ToString();
                    totalPrice.text = totalPricing.ToString();
                }
            }
            amountCounting = 0; 
        }
    }
    //public void ShowResult()
    //{
    //    var totalPricing = 0;
    //    var totalPricingByOrder = 0;
    //    var amountCounting = 0;
    //    var resultIndex = 0;
    //    decorationPrefabs = GameObject.FindGameObjectsWithTag("Decoration"); //Pull data from "Decoration" Tag

    //    foreach (Decoration decoration in decorationComponent.Decoration)
    //    {
    //        foreach (GameObject placedDecoration in decorationPrefabs)
    //        {
    //            if (placedDecoration.transform.name.Contains(decoration.ojectName))
    //            {
    //                foreach (Transform child in resultParent)
    //                {
    //                    //child is your child transform
    //                    if (child.transform.name != decoration.ojectName)
    //                    {
    //                        resultPanel = Instantiate(resultPanel, resultParent);
    //                        resultPanel.transform.name = decoration.ojectName;

    //                        item = GameObject.Find($"ItemListParent/{decoration.ojectName}/Item").GetComponent<Text>();
    //                        amount = GameObject.Find($"ItemListParent/{decoration.ojectName}/Amount").GetComponent<Text>();
    //                        price = GameObject.Find($"ItemListParent/{decoration.ojectName}/Price").GetComponent<Text>();
    //                    }
    //                }

    //                amountCounting++;
    //                item.text = $"{placedDecoration.transform.name} : {decoration.ojectName} : {resultPanel.transform.name}";//decoration.title;
    //                totalPricingByOrder = amountCounting * decoration.price;
    //                totalPricing += decoration.price;
    //                amount.text = amountCounting.ToString();
    //                price.text = totalPricingByOrder.ToString();
    //                totalPrice.text = totalPricing.ToString();

    //            }
    //        }
    //        amountCounting = 0;
    //    }
    //}
}

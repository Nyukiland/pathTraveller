using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIControl : MonoBehaviour
{
    [Header("Script needed")]
    [Space(2)]

    [Tooltip("the manager is used ton control the recreation of the playground")]
    [SerializeField]
    Manager manager;

    [Header("Diverse UI element used")]
    [Space(2)]

    [Tooltip("the button used to restart the scene")]
    [SerializeField]
    Button regenButton;

    //deck
    [Tooltip("the text for the tile deck number")]
    [SerializeField]
    TextMeshProUGUI numberDeck;

    [Tooltip("the slider for the tile deck number")]
    [SerializeField]
    Slider sliderDeck;

    //height of tile
    [Tooltip("the text for the tile deck number")]
    [SerializeField]
    TextMeshProUGUI numberHeight;

    [Tooltip("the slider for the tile deck number")]
    [SerializeField]
    Slider sliderHeight;

    //width of tile
    [Tooltip("the text for the tile deck number")]
    [SerializeField]
    TextMeshProUGUI numberWidth;

    [Tooltip("the slider for the tile deck number")]
    [SerializeField]
    Slider sliderWidth;

    void Start()
    {
        regenButton.onClick.AddListener(Regenerate);
    }

    // Update is called once per frame
    void Update()
    {
        numberDeck.text = sliderDeck.value.ToString();
        numberHeight.text = sliderHeight.value.ToString();
        numberWidth.text = sliderWidth.value.ToString();
    }

    void Regenerate()
    {
        //first reset the playground
        manager.ResetScene();

        //then give the script the proper variable
        manager.matrixSize = new Vector2(sliderWidth.value, sliderHeight.value);
        manager.inHandSize = (int)sliderDeck.value;

        //finally call the generation of the playground
        manager.PreSet();
    }
}

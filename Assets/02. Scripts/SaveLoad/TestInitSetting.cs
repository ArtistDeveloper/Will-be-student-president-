using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestInitSetting : MonoBehaviour
{
    public Button SaveButton, LoadButton;
    void Start()
    {
        SaveButton.onClick.AddListener(SlotManager.instance.OpenSaveSlotWindow);
        LoadButton.onClick.AddListener(SlotManager.instance.OpenLoadSlotWindow);
    }

    
}

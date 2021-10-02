using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelActiveControl : MonoBehaviour
{
    public GameObject contentsPanels;
    public bool option;
    public void ButtonClick(){
        contentsPanels.SetActive(option);
    }
}

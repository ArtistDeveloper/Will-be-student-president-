using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelActiveControl : MonoBehaviour
{
    public List<GameObject> contentsPanels;
    public List<bool> activeOption;

    public void ButtonClick()
    {
        for (int i = 0; i < contentsPanels.Count; i++)
        {
            contentsPanels[i].SetActive(activeOption[i]);
        }
    }
}

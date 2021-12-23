using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelToggleControl : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> contentsPanels;
    public void ButtonClick(){
        for(int i=0;i<contentsPanels.Count;i++){
            contentsPanels[i].SetActive(!contentsPanels[i].activeInHierarchy);
        }
    }
}

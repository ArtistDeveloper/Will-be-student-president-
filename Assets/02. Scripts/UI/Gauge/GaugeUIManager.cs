using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeUIManager : MonoBehaviour
{
    public Image gaugeImg;
    float changedApprovalRating;
    
    public void ChangeGaugeUI(float nowApprovalRating)
    {
        changedApprovalRating = gaugeImg.fillAmount - (nowApprovalRating);
        if(changedApprovalRating == 0) return;
        else if(0 < changedApprovalRating)
        {
            UpGaugeUI();
        } else 
        {
            DownGaugeUI();
        }
    }

    public void UpGaugeUI() 
    {
        Mathf.Lerp(gaugeImg.fillAmount, gaugeImg.fillAmount += changedApprovalRating, 0.5f);
    }

    public void DownGaugeUI() 
    {
        Mathf.Lerp(gaugeImg.fillAmount, gaugeImg.fillAmount -= changedApprovalRating, 0.5f);
    }
}

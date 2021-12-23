using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;

public class GetNextHappening : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void GetNext(){
        HappeningUtils.happeningUtils.DebugPrintHappening__(
            HappeningUtils.happeningUtils.GetNextHappening__());
    }
}

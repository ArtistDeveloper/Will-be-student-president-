using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestSceneChange : MonoBehaviour
{
    
    void Awake()
    {
        SceneManager.LoadScene("GamePlayScene(SaveLoadTest)");
    }
}

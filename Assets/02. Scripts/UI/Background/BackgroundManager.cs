using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace CustomDict
{
    [System.Serializable]
    public class SerializableDictionary<Tkey, TValue> : Dictionary<Tkey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<Tkey> keys = new List<Tkey>();
        [SerializeField] private List<TValue> values = new List<TValue>();


        // List를 Dictionary에 저장
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<Tkey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }


        // 리스트로부터 딕셔너리를 불러온다
        public void OnAfterDeserialize()
        {
            this.Clear();

            if (keys.Count != values.Count)
                throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

            for (int i = 0; i < keys.Count; i++)
                this.Add(keys[i], values[i]);
        }
    }
}

[System.Serializable]
public class ImageDictionary : CustomDict.SerializableDictionary<string, Sprite> { }


public class BackgroundManager : MonoBehaviour
{
    [Header("Dictionary 확인")]
    [SerializeField] private ImageDictionary imageDict = new ImageDictionary();

    [Header("Dictinary에 들어갈 Key, value")]
    public List<string> bgName;
    public List<Sprite> bgSource;
    
    private Image bg;

    private static BackgroundManager _instance;

    public static BackgroundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // _instance = FindObjectOfType<BackgroundManager>();
                Debug.LogError("instance가 비어있습니다");
            }

            return _instance;
        }
    }

    /// <summary>
    /// 이 함수를 호출하여 Background의 Sprite를 받아주세요.
    /// </summary>
    /// <param name="bgName"></param>
    /// <returns></returns>
    public Sprite GetBackground(string bgName)
    {
        return imageDict[bgName];
    }


    public void ChangeBackground(string bgName)
    {
        Sprite changeSource = GetBackground(bgName);

        bg.sprite = changeSource;
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }

        _instance = this;
        Initialize();
        SubstituteImage();
        bg = GetComponent<Image>();
    }


    private void Start()
    {
        ChangeBackground("Test");
    }

    // TODO : 이미지 효과 제작하기 shake, fadein, fadeout, 


    private void Initialize()
    {
        imageDict.Clear();
    }


    private void SubstituteImage()
    {
        foreach (var item in bgName.Zip(bgSource, (name, sprite) => new { Name = name, Sprite = sprite }))
        {
            imageDict.Add(item.Name, item.Sprite);
        }
    }
}

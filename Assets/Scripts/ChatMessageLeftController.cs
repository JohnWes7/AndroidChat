using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;

public class ChatMessageLeftController : MonoBehaviour
{
    [SerializeField] private string senderID;
    [SerializeField] private Text nameText;
    [SerializeField] private Text contentText;
    [SerializeField] private Image headImage;
    [SerializeField] private Image textBackground;

    private void Start()
    {
        ChangeSize();
    }

    public void init(string name, Sprite headSprit, DataSnapshot oneMessage)
    {
        senderID = oneMessage.Child("Sender").Value.ToString();

        if (nameText)
        {
            SetSenderName(name);
        }

        if (headSprit != null && headImage)
        {
            headImage.sprite = headSprit;
        }

        if (contentText)
        {
            SetContentText(oneMessage.Child("Content").Value.ToString());
        }
    }

    public void SetSenderName(string name)
    {
        nameText.text = name;
    }

    public void UpdateSpriteDependDict(Dictionary<string, Sprite> spDict)
    {
        try
        {
            headImage.sprite = spDict[senderID];
        }
        catch (System.Exception e)
        {

            Debug.LogWarning(e);
        }
       
    }

    public void UpdateNameDependNameDict(Dictionary<string, Dictionary<string, string>> dict)
    {
        try
        {
            nameText.text = dict[senderID]["name"];
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
        }
        
    }

    public void SetContentText(string content)
    {
        contentText.text = content;
        ChangeSize();
    }

    public void ChangeSize()
    {
        float needspan = Mathf.Clamp(contentText.preferredHeight - 41f, 0f, contentText.preferredHeight);
        textBackground.rectTransform.sizeDelta = new Vector2(430, needspan + 80);
        (this.transform as RectTransform).sizeDelta = new Vector2(560, 180 + needspan);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using UnityEngine.Events;

public class ChatPanelController : MonoBehaviour
{
    [SerializeField] private InputField userinput;
    [SerializeField] private Text inputText;
    [SerializeField] private string chatID;
    [SerializeField] private Dictionary<string, string> userNameDict = new Dictionary<string, string>();

    

    public void Init(string chatID, string curAuthID)
    {
        
    }


    public void OnSendButtonClick()
    {
        
    }

    public static IEnumerator GetChatContent(string chatID, string curAuthID, UnityEvent failEvent = null)
    {
        var refe = FirebaseDatabase.DefaultInstance.GetReference($"Chats/{chatID}/Content");
        var task = refe.GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.Log("网络连接错误");
            failEvent.Invoke();
        }
        else
        {
            if (task.Result == null)
            {
                Debug.Log($"{chatID} 没有数据");
                failEvent.Invoke();
            }
        }
    }

    //public delegate void ChatInfoEvent(string chatID, string curAuthID, )
}

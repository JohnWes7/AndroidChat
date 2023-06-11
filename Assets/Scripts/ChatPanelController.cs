using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using UnityEngine.Events;
using Firebase.Auth;

public class ChatPanelController : MonoBehaviour
{
    [SerializeField] private InputField userinput;
    [SerializeField] private Text inputText;
    [SerializeField] private Text friendName;
    [SerializeField] private string chatID;
    [SerializeReference] private Dictionary<string, Dictionary<string, string>> userNameDict = new Dictionary<string, Dictionary<string, string>>();
    public delegate void ChatInfoHandler(string chatID, string curAuthID, DataSnapshot content);
    public delegate void UserNameHandler(string userID, string name);
    private event UserNameHandler CurUserNameEvent;
    private event UserNameHandler OtherUserNameEvent;
    private event ChatInfoHandler ChatInfoGetEvent;

    [SerializeField] private GameObject messageLeftPrefab;
    [SerializeField] private GameObject viewPortVontent;
    [SerializeField] private List<ChatMessageLeftController> chatMessageList = new List<ChatMessageLeftController>();

    private void Start()
    {
        Init("6AxgcfVMjMY8Mt3sdvkFKHv7oYC2lHutIeAly4QKNPASN5HxotT2CL23", FirebaseAuth.DefaultInstance.CurrentUser.UserId);
        //Init("6AxgcfVMjMY8Mt3sdvkF", FirebaseAuth.DefaultInstance.CurrentUser.UserId);
    }

    public void Init(string chatID, string curAuthID)
    {
        Debug.Log($"{chatID}  {curAuthID}");
        this.chatID = chatID;
        
        string otherID = (chatID.Clone() as string).Replace(curAuthID, "");
        userNameDict.Clear();
        userNameDict.Add(curAuthID, new Dictionary<string, string>());
        userNameDict[curAuthID].Add("name", curAuthID);
        userNameDict[curAuthID].Add("image", "");
        
        userNameDict.Add(otherID, new Dictionary<string, string>());
        userNameDict[otherID].Add("name", otherID);
        userNameDict[otherID].Add("image", "");

        CurUserNameEvent += AddIDtoDitc;
        ChatInfoGetEvent += CreatAllMessageContent;
        OtherUserNameEvent += AddIDtoDitc;
        OtherUserNameEvent += AddName; 

        StartCoroutine(GetUserName(curAuthID, CurUserNameEvent, CurUserNameEvent));
        StartCoroutine(GetUserName(otherID, OtherUserNameEvent, OtherUserNameEvent));
        StartCoroutine(GetChatContent(chatID, curAuthID, null, ChatInfoGetEvent));
        
    }

    public void AddName(string userID, string name)
    {
        friendName.text = name;
    }

    public void AddIDtoDitc(string userID, string name)
    {
        userNameDict[userID]["name"] = name;
        foreach (var item in chatMessageList)
        {
            if (item != null)
            {
                item.UpdateNameDependNameDict(userNameDict);
            }
        }
    }


    public void OnSendButtonClick()
    {
        
    }

    // 更新面板
    private void CreatAllMessageContent(string chatID, string curAuthID, DataSnapshot content)
    {
        foreach (var item in chatMessageList)
        {
            Destroy(item.gameObject);
        }
        chatMessageList.Clear();

        foreach (var item in content.Children)
        {
            ChatMessageLeftController temp = Instantiate<GameObject>(messageLeftPrefab, viewPortVontent.transform).GetComponent<ChatMessageLeftController>();
            string sanderid = item.Child("Sender").Value.ToString();
            temp.name = sanderid;
            string tempname = sanderid;
            //string image = "";

            Dictionary<string, string> tempdict;
            if (userNameDict.TryGetValue(sanderid, out tempdict))
            {
                tempname = tempdict.GetValueOrDefault<string, string>("name", sanderid);
            }

            temp.init(tempname, null, item);
            chatMessageList.Add(temp);
        }
    }

    
    // 获取改用户id下的名字
    public static IEnumerator GetUserName(string userid, UserNameHandler fail = null, UserNameHandler success = null)
    {
        var task = FirebaseDatabase.DefaultInstance.GetReference($"Users/{userid}/Name").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null || task.Result.Value == null)
        {
            Debug.Log($"获取用户名字失败 : {userid}");
            fail.Invoke(userid, userid);
        }
        else
        {
            Debug.Log($"获取用户名字成功: {userid} : {task.Result.Value.ToString()}");
            success.Invoke(userid, task.Result.Value.ToString());
        }
    }

    // 获取全部聊天
    public static IEnumerator GetChatContent(string chatID, string curAuthID, UnityEvent failEvent = null, ChatInfoHandler sucessEvent = null)
    {
        Debug.Log(chatID.Equals("6AxgcfVMjMY8Mt3sdvkFKHv7oYC2lHutIeAly4QKNPASN5HxotT2CL23"));
        var refe = FirebaseDatabase.DefaultInstance.GetReference("Chats/" + chatID + "/Content/");
        var task = refe.GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.Log("网络连接错误");
            if (failEvent != null)
            {
                failEvent.Invoke();
            }
        }
        else
        {
            if (!task.Result.HasChildren)
            {
                Debug.Log(task.Result.ChildrenCount);
                Debug.Log(task.Result);
                Debug.Log($"{chatID} 没有数据");
                if (failEvent != null)
                {
                    failEvent.Invoke();
                }
            }
            else
            {
                Debug.Log(task.Result);
                Debug.Log(task.Result.ChildrenCount);
                Debug.Log("获取数据成功");
                sucessEvent.Invoke(chatID, curAuthID, task.Result);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }
}

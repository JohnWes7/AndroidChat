using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using UnityEngine.Events;
using Firebase.Auth;
using System;

public class ChatPanelController : MonoBehaviour
{
    [SerializeField] private InputField userinput;
    [SerializeField] private Text inputText;
    [SerializeField] private Text friendName;
    [SerializeField] private string chatID;
    [SerializeField] private Sprite defaultHeadIcon;

    [SerializeReference] private Dictionary<string, Dictionary<string, string>> userNameDict = new Dictionary<string, Dictionary<string, string>>();
    [SerializeField] private Dictionary<string, Sprite> userHeadSpriteDict = new Dictionary<string, Sprite>();
    public delegate void ChatInfoHandler(string chatID, string curAuthID, DataSnapshot content);
    public delegate void UserNameHandler(string userID, string name);
    private event UserNameHandler CurUserNameEvent;
    private event UserNameHandler OtherUserNameEvent;
    private event ChatInfoHandler ChatInfoGetEvent;

    [SerializeField] private GameObject messageLeftPrefab;
    [SerializeField] private GameObject viewPortVontent;
    [SerializeField] private List<ChatMessageLeftController> chatMessageList = new List<ChatMessageLeftController>();

    //private void Start()
    //{
    //    //Init("6AxgcfVMjMY8Mt3sdvkFKHv7oYC2lHutIeAly4QKNPASN5HxotT2CL23", FirebaseAuth.DefaultInstance.CurrentUser.UserId);
    //    //Init("6AxgcfVMjMY8Mt3sdvkF", FirebaseAuth.DefaultInstance.CurrentUser.UserId);
    //}

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

        userHeadSpriteDict.Clear();
        userHeadSpriteDict.Add(curAuthID, defaultHeadIcon);
        userHeadSpriteDict.Add(otherID, defaultHeadIcon);

        CurUserNameEvent += AddIDtoDitc;
        ChatInfoGetEvent += CreatAllMessageContent;
        OtherUserNameEvent += AddIDtoDitc;
        OtherUserNameEvent += AddName;

        UnityEvent<string> getCurHeadImage = new UnityEvent<string>();
        UnityEvent<string> getOtherHeadImage = new UnityEvent<string>();

        getCurHeadImage.AddListener((imageName) => {
            UnityEvent<byte[]> suc = new UnityEvent<byte[]>();

            suc.AddListener((bytes) =>
            {
                AddSpriteID(curAuthID, Tool_LYJ.Bytes2Sprite(bytes));
            });

            StartCoroutine(Tool_ZW.GetImage(imageName, suc));
        });

        getOtherHeadImage.AddListener((imageName) => {
            UnityEvent<byte[]> suc = new UnityEvent<byte[]>();

            suc.AddListener((bytes) =>
            {
                AddSpriteID(otherID, Tool_LYJ.Bytes2Sprite(bytes));
            });

            StartCoroutine(Tool_ZW.GetImage(imageName, suc));
        });

        // 获取头像
        StartCoroutine(Tool_LYJ.GetUserHeadImageName(curAuthID, getCurHeadImage));
        StartCoroutine(Tool_LYJ.GetUserHeadImageName(otherID, getOtherHeadImage));
        // 获取名字
        StartCoroutine(GetUserName(curAuthID, CurUserNameEvent, CurUserNameEvent));
        StartCoroutine(GetUserName(otherID, OtherUserNameEvent, OtherUserNameEvent));
        // 获取聊天
        StartCoroutine(GetChatContent(chatID, curAuthID, null, ChatInfoGetEvent));

        //添加监听数据
        FirebaseDatabase.DefaultInstance.GetReference($"Chats/{chatID}/Content").LimitToLast(1).ChildAdded += HandleChildAdded;


    }

    private void OnDisable()
    {
        //取消监听
        FirebaseDatabase.DefaultInstance.GetReference($"Chats/{chatID}/Content").LimitToLast(1).ChildAdded -= HandleChildAdded;
    }

    public void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot

        Debug.Log($"检测到添加数据 抓取最后添加的数据 位置{args.Snapshot.Reference}");
        Debug.Log(args.Snapshot.Child("Content").Value.ToString());

        ChatMessageLeftController temp = Instantiate<GameObject>(messageLeftPrefab, viewPortVontent.transform).GetComponent<ChatMessageLeftController>();
        string sanderid = args.Snapshot.Child("Sender").Value.ToString();
        temp.name = sanderid;
        string tempname = sanderid;
        //string image = "";

        Dictionary<string, string> tempdict;
        if (userNameDict.TryGetValue(sanderid, out tempdict))
        {
            tempname = tempdict.GetValueOrDefault<string, string>("name", sanderid);
        }

        temp.init(tempname, userHeadSpriteDict.GetValueOrDefault("sanderid", defaultHeadIcon), args.Snapshot);
        chatMessageList.Add(temp);
    }

    public void AddName(string userID, string name)
    {
        friendName.text = name;
    }

    public void AddSpriteID(string userID, Sprite sp)
    {
        userHeadSpriteDict[userID] = sp;

        foreach (var item in chatMessageList)
        {
            if (item != null)
            {
                item.UpdateSpriteDependDict(userHeadSpriteDict);
            }
        }
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
        Dictionary<string, string> messageData = new Dictionary<string, string>();
        string needSendText = userinput.text;
        userinput.text = "";
        string other = ChatIDtoOtherID(chatID);

        // 生成发送的数据字典
        messageData["Acceptor"] = other;
        messageData["Content"] = needSendText;
        messageData["Time"] = DateTime.Now.ToUniversalTime().Ticks.ToString();
        messageData["Sender"] = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        var refe = FirebaseDatabase.DefaultInstance.GetReference($"Chats/{chatID}/Content").Push();
        refe.SetValueAsync(messageData).ContinueWith((task) => {
            if (task.Exception != null)
            {
                Debug.Log($"写入数据失败 chatid: {chatID} content: {needSendText} sender: {FirebaseAuth.DefaultInstance.CurrentUser.UserId}");
            }
            else
            {
                Debug.Log($"写入数据成功 chatid: {chatID} content: {needSendText} sender: {FirebaseAuth.DefaultInstance.CurrentUser.UserId}");
            }
        });

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
            string senderid = item.Child("Sender").Value.ToString();
            temp.name = senderid;
            string tempname = senderid;
            //string image = "";

            Dictionary<string, string> tempdict;
            if (userNameDict.TryGetValue(senderid, out tempdict))
            {
                tempname = tempdict.GetValueOrDefault<string, string>("name", senderid);
            }

            temp.init(tempname, userHeadSpriteDict.GetValueOrDefault(senderid, defaultHeadIcon), item);
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
                //Debug.Log(task.Result);
                //Debug.Log(task.Result.ChildrenCount);
                Debug.Log($"获取数据成功 {task.Result}");
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

    public static string ChatIDtoOtherID(string chatID)
    {
        string curID = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        try
        {
            return chatID.Replace(curID, "");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"chatID 分割失败: chatid: {chatID} cur userid: {curID}\n{e}");
            return "";
        }
    }
}

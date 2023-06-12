using Firebase.Database;
using Firebase.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendIconController : MonoBehaviour
{
    
    [SerializeField] private Text friendName;
    [SerializeField] private Text lastMessage;
    [SerializeField] private Image friendImage;
    [SerializeField] private string friendID;
    [SerializeField] private GameObject chatPrefab;
    [SerializeField] private ChatPanelController chatpanelController;
    [SerializeField] private FriendListController parent;

    // Start is called before the first frame update
    public void init(string friendid, FriendListController parent)
    {
        this.friendID = friendid;
        this.parent = parent;
        StartCoroutine(ShowFriendDetail(friendid));
        Debug.Log(Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId);
    }

    public IEnumerator ShowFriendDetail(string friendid)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        var task = FirebaseDatabase.DefaultInstance.GetReference("Users/" + friendid ).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.LogWarning("网络错误");
        }
        else 
        {
            DataSnapshot snapshot = task.Result;
            Debug.Log("好友数据获取成功");
            friendName.text = snapshot.Child("Name").Value.ToString();
            string imageid = snapshot.Child("Image").Value.ToString();
            string userid = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            string combineid;
            if(CombineId(userid, friendid, out combineid))
            {
                Debug.Log($"id组合成功{combineid}");
                StartCoroutine(ShowChatDetail(combineid));
            }
            else
            {
                Debug.LogWarning("id组合失败！");
            }
            FirebaseStorage storage = FirebaseStorage.DefaultInstance;
            //Debug.Log($"gs://chat-softw.appspot.com/userimage/{snapshot.Child("Image").Value.ToString()}");
            var task1 = storage.GetReferenceFromUrl($"gs://chat-softw.appspot.com/userimage/{snapshot.Child("Image").Value.ToString()}").GetBytesAsync(10485760);
            yield return new WaitUntil(() => task1.IsCompleted);
            if (task1.Exception != null)
            {
                Debug.Log("加载默认头像");
                friendImage.sprite = Resources.Load<Sprite>("UI/Image/defaultHead");
            }
            else
            {
                Debug.Log("头像获取成功");
                Texture2D texture = new Texture2D(300, 300);
                texture.LoadImage(task1.Result);
                friendImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }

        }
    }
    public IEnumerator ShowChatDetail(string chatid)
    {
        //DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        //var task1 = FirebaseDatabase.DefaultInstance.GetReference("Chats").GetValueAsync();
        //yield return new WaitUntil(() => task1.IsCompleted);
        //if (task1.Exception != null)
        //{
        //    Debug.LogWarning("网络错误");
        //}
        //else
        //{
        //    DataSnapshot snapshot1 = task1.Result;
        //    if (snapshot1.HasChild(chatid))
        //    {
        //        var task = FirebaseDatabase.DefaultInstance.GetReference("Chats/" + chatid + "/Amount").GetValueAsync();
        //        yield return new WaitUntil(() => task.IsCompleted);
        //        if (task.Exception != null)
        //        {
        //            Debug.LogWarning("网络错误");
        //        }
        //        else
        //        {
        //            DataSnapshot snapshot = task.Result;
        //            Debug.Log($"聊天长度获取成功{snapshot.Value.ToString()}");
        //            Debug.Log(snapshot == null);
        //            Debug.Log($"聊天·序号为{(int.Parse(snapshot.Value.ToString()) - 1).ToString()}");
        //            StartCoroutine(GetLaetMessage("Chats/" + chatid + "/Content/" + (int.Parse(snapshot.Value.ToString()) - 1).ToString() + "/Content"));
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogWarning("聊天id不存在");
        //    }
        //}

        var task = FirebaseDatabase.DefaultInstance.GetReference($"Chats/{chatid}/Content").LimitToLast(1).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogWarning("网络错误");
        }
        else if (task.Result.Value == null)
        {
            //Debug.Log(task.Result);
            Debug.Log("没有上一条消息");
        }
        else
        {
            foreach (var item in task.Result.Children)
            {
                lastMessage.text = item.Child("Content").Value.ToString();
                Debug.Log($"最后消息获取成功 {task.Result}");
                break;
            }
            
        }
    }


    //public IEnumerator GetLaetMessage(string messageid)
    //{
    //    DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
    //    var task = FirebaseDatabase.DefaultInstance.GetReference(messageid).GetValueAsync();
    //    yield return new WaitUntil(() => task.IsCompleted);
    //    if (task.Exception != null)
    //    {
    //        Debug.LogWarning("网络错误");
    //    }
    //    else
    //    {
    //        DataSnapshot snapshot = task.Result;
    //        Debug.Log($"最后消息获取成功{snapshot.Value.ToString()}");
    //        lastMessage.text = snapshot.Value.ToString();
    //    }
    //}

    public static bool CombineId(string userid,string friendid,out string combineid)
    {
        int result = string.Compare(userid, friendid);
        if (result < 0)
        {
            combineid = userid + friendid;
            return true;
            // string1 在 string2 前面
        }
        else if (result == 0)
        {
            Debug.LogWarning($"和自己聊天？ 你的：{userid}  对方的： {friendid}");
            combineid = "";
            return false;
            // string1 和 string2 相同
        }
        else
        {
            combineid = friendid + userid;
            return true;
            // string1 在 string2 后面
        }
    }

    public void OpenChatPanel()
    {
        string chatID;
        if (CombineId(Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId, friendID, out chatID))
        {
            if (chatpanelController)
            {
                chatpanelController.gameObject.SetActive(true);
            }
            else
            {
                chatpanelController = Instantiate<GameObject>(chatPrefab, parent.transform).GetComponent<ChatPanelController>();
            }
            chatpanelController.Init(chatID, Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId);
        }
    }
}

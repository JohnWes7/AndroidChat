using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendIconController : MonoBehaviour
{
    [SerializeField] private Text friendName;
    [SerializeField] private Text lastMessage;
    [SerializeField] private Image friendImage;
    // Start is called before the first frame update
    public void init(string friendid)
    {
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
            Debug.LogWarning("缃戠粶閿欒");
        }
        else 
        {
            DataSnapshot snapshot = task.Result;
            Debug.Log("濂藉弸鏁版嵁鑾峰彇鎴愬姛");
            friendName.text = snapshot.Child("Name").Value.ToString();
            string imageid = snapshot.Child("Image").Value.ToString();
            string userid = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId;
           // string userid = "lHutIeAly4QKNPASN5HxotT2CL23";
            if (string.IsNullOrWhiteSpace(imageid))
            {
                
            }
            string combineid;
            if(CombineId(userid, friendid, out combineid))
            {
                Debug.Log($"id组合成功{combineid}");
                Debug.Log("id缁勫悎鎴愬姛");
                StartCoroutine(ShowChatDetail(combineid));
            }
            else
            {
                Debug.LogWarning("id缁勫悎澶辫触锛?");
            }
            

        }
    }
    public IEnumerator ShowChatDetail(string chatid)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        var task1 = FirebaseDatabase.DefaultInstance.GetReference("Chats").GetValueAsync();
        yield return new WaitUntil(() => task1.IsCompleted);
        if (task1.Exception != null)
        {
            Debug.LogWarning("缃戠粶閿欒");
        }
        else
        {
<<<<<<< Updated upstream
            DataSnapshot snapshot1 = task1.Result;
            if (snapshot1.HasChild(chatid))
            {
                var task = FirebaseDatabase.DefaultInstance.GetReference("Chats/" + chatid + "/Amount").GetValueAsync();
                yield return new WaitUntil(() => task.IsCompleted);
                if (task.Exception != null)
                {
                    Debug.LogWarning("网络错误");
                }
                else
                {
                    DataSnapshot snapshot = task.Result;
                    Debug.Log($"聊天长度获取成功{snapshot.Value.ToString()}");
                    Debug.Log(snapshot == null);
                    Debug.Log($"聊天·序号为{(int.Parse(snapshot.Value.ToString()) - 1).ToString()}");
                    StartCoroutine(GetLaetMessage("Chats/" + chatid + "/" + (int.Parse(snapshot.Value.ToString()) - 1).ToString() + "/Content"));
                }
            }
            else
            {
                Debug.LogWarning("聊天id不存在");
            }
=======
            Debug.Log("鑱婂ぉ闀垮害鑾峰彇鎴愬姛");
            DataSnapshot snapshot = task.Result;
            Debug.Log("鑱婂ぉ路搴忓彿涓?+(int.Parse(snapshot.Value.ToString()) - 1).ToString());
            StartCoroutine(GetLaetMessage("Chats/" + chatid + "/" + (int.Parse(snapshot.Value.ToString()) - 1).ToString() + "/Content"));
>>>>>>> Stashed changes
        }
        
    }


    public IEnumerator GetLaetMessage(string messageid)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        var task = FirebaseDatabase.DefaultInstance.GetReference(messageid).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.LogWarning("缃戠粶閿欒");
        }
        else
        {
            Debug.Log("鏈€鍚庢秷鎭幏鍙栨垚鍔?");
            DataSnapshot snapshot = task.Result;
            Debug.Log($"最后消息获取成功{snapshot.Value.ToString()}");
            lastMessage.text = snapshot.Value.ToString();
        }
    }

    public static bool CombineId(string userid,string friendid,out string combineid)
    {
        int result = string.Compare(userid, friendid);
        if (result < 0)
        {
            combineid = userid + friendid;
            return true;
            // string1 鍦?string2 鍓嶉潰
        }
        else if (result == 0)
        {
            Debug.LogWarning($"鍜岃嚜宸辫亰澶╋紵 浣犵殑锛歿userid}  瀵规柟鐨勶細 {friendid}");
            combineid = "";
            return false;
            // string1 鍜?string2 鐩稿悓
        }
        else
        {
            combineid = friendid + userid;
            return true;
            // string1 鍦?string2 鍚庨潰
        }
    }
}

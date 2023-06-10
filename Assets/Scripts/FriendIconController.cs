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
            Debug.LogWarning("�������");
        }
        else 
        {
            DataSnapshot snapshot = task.Result;
            Debug.Log("�������ݻ�ȡ�ɹ�");
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
                Debug.Log($"id��ϳɹ�{combineid}");
                StartCoroutine(ShowChatDetail(combineid));
            }
            else
            {
                Debug.LogWarning("id���ʧ�ܣ�");
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
            Debug.LogWarning("�������");
        }
        else
        {
            DataSnapshot snapshot1 = task1.Result;
            if (snapshot1.HasChild(chatid))
            {
                var task = FirebaseDatabase.DefaultInstance.GetReference("Chats/" + chatid + "/Amount").GetValueAsync();
                yield return new WaitUntil(() => task.IsCompleted);
                if (task.Exception != null)
                {
                    Debug.LogWarning("�������");
                }
                else
                {
                    DataSnapshot snapshot = task.Result;
                    Debug.Log($"���쳤�Ȼ�ȡ�ɹ�{snapshot.Value.ToString()}");
                    Debug.Log(snapshot == null);
                    Debug.Log($"���졤���Ϊ{(int.Parse(snapshot.Value.ToString()) - 1).ToString()}");
                    StartCoroutine(GetLaetMessage("Chats/" + chatid + "/" + (int.Parse(snapshot.Value.ToString()) - 1).ToString() + "/Content"));
                }
            }
            else
            {
                Debug.LogWarning("����id������");
            }
        }
        
    }


    public IEnumerator GetLaetMessage(string messageid)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        var task = FirebaseDatabase.DefaultInstance.GetReference(messageid).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.LogWarning("�������");
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            Debug.Log($"�����Ϣ��ȡ�ɹ�{snapshot.Value.ToString()}");
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
            // string1 �� string2 ǰ��
        }
        else if (result == 0)
        {
            Debug.LogWarning($"���Լ����죿 ��ģ�{userid}  �Է��ģ� {friendid}");
            combineid = "";
            return false;
            // string1 �� string2 ��ͬ
        }
        else
        {
            combineid = friendid + userid;
            return true;
            // string1 �� string2 ����
        }
    }
}

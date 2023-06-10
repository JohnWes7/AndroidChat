using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddFriendController : MonoBehaviour
{
    [SerializeField] private GameObject friendPrefab;
    [SerializeField] private GameObject content;
    [SerializeField] List<AddFriendIconController> friendIconList = new List<AddFriendIconController>();
    [SerializeField] private InputField friendName;
    public void OnSearchButtonClick ()
    {
        string friendId = friendName.text;
        Debug.Log($"输入框文本获取成功{friendId}");
        StartCoroutine(GetFriendList(friendId));


    }
    public IEnumerator GetFriendList(string friendId)
    {
        //List<string> friendlist = new List<string>();
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        var task = FirebaseDatabase.DefaultInstance.GetReference("Users").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.LogWarning("网络错误");
        }
        else
        {
            Debug.Log("用户数据获取成功");
            DataSnapshot snapshot = task.Result;
            if (snapshot.HasChild(friendId))
            {
                Debug.Log($"用户存在{friendId}");
                var temp = Instantiate<GameObject>(friendPrefab, content.transform).GetComponent<AddFriendIconController>();
                friendIconList.Add(temp);
                temp.init(friendId);
            }
            else
            {
                Debug.LogWarning("该用户不存在");
            }
            /*foreach (DataSnapshot childSnapshot in snapshot.Children)//保存所有好友的uid
            {
                string name = childSnapshot.Value.ToString();
                friendlist.Add(name);
            }
            Debug.Log("好友列表获取完成");
        }
        foreach (AddFriendIconController item in friendIconList)
        {
            Destroy(item.gameObject);
        }
        friendIconList.Clear();
        foreach (string friend in friendlist)
        {
            Debug.Log("好友名： " + friend);
            var temp = Instantiate<GameObject>(friendPrefab, content.transform).GetComponent<FriendIconController>();
            friendIconList.Add(temp);
            temp.init(friend);
        }*/
            
        }
    }
}

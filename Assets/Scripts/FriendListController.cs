using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendListController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject friendPrefab;
    [SerializeField] private GameObject content;
    [SerializeField] List<FriendIconController> friendIconList = new List<FriendIconController>(); 
    void Start()
    {
        //init("lHutIeAly4QKNPASN5HxotT2CL23");
        //StartCoroutine(Tool_ZW.CheckUserData("lHutIeAly4QKNPASN5HxotT2CL23"));
    }

    public void init(string Userid)
    {

        StartCoroutine(GetFriendList(Userid));


    }
    public IEnumerator GetFriendList(string Userid)
    {
        List<string> friendlist = new List<string>();
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        var task = FirebaseDatabase.DefaultInstance.GetReference("Users/" + Userid + "/Friend").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.LogWarning("网络错误");
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            foreach (DataSnapshot childSnapshot in snapshot.Children)//保存所有好友的uid
            {
                string name = childSnapshot.Value.ToString();
                friendlist.Add(name);
            }
            Debug.Log("好友列表获取完成");
            /*foreach(string friend in friendlist)
            {
                FirebaseDatabase.DefaultInstance.GetReference("Users/" + friend + "/Image").GetValueAsync().ContinueWithOnMainThread(task =>//获取对应好友的图片url
                {
                    if (task.IsFaulted)
                    {
                        // Handle the error...
                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;
                        string image = snapshot.Value.ToString();
                        imagelist.Add(image);//保存好友头像链接到列表中
                    }
                });
            }*/
        }
        foreach(FriendIconController item in friendIconList)
        {
            Destroy(item.gameObject);
        }
        friendIconList.Clear();
        Debug.Log("长度： " + friendlist.Count);
        foreach (string friend in friendlist)
        {
            Debug.Log("好友名： " + friend);
            var temp = Instantiate<GameObject>(friendPrefab, content.transform).GetComponent<FriendIconController>();
            friendIconList.Add(temp);
            temp.init(friend);
        }
    }
}

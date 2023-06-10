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
        // init("lHutIeAly4QKNPASN5HxotT2CL23");
        StartCoroutine(Tool_ZW.CheckUserData("lHutIeAly4QKNPASN5HxotT2CL23"));
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
            Debug.LogWarning("�������");
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            foreach (DataSnapshot childSnapshot in snapshot.Children)//�������к��ѵ�uid
            {
                string name = childSnapshot.Value.ToString();
                friendlist.Add(name);
            }
            Debug.Log("�����б��ȡ���");
            /*foreach(string friend in friendlist)
            {
                FirebaseDatabase.DefaultInstance.GetReference("Users/" + friend + "/Image").GetValueAsync().ContinueWithOnMainThread(task =>//��ȡ��Ӧ���ѵ�ͼƬurl
                {
                    if (task.IsFaulted)
                    {
                        // Handle the error...
                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;
                        string image = snapshot.Value.ToString();
                        imagelist.Add(image);//�������ͷ�����ӵ��б���
                    }
                });
            }*/
        }
        foreach(FriendIconController item in friendIconList)
        {
            Destroy(item.gameObject);
        }
        friendIconList.Clear();
        Debug.Log("���ȣ� " + friendlist.Count);
        foreach (string friend in friendlist)
        {
            Debug.Log("�������� " + friend);
            var temp = Instantiate<GameObject>(friendPrefab, content.transform).GetComponent<FriendIconController>();
            friendIconList.Add(temp);
            temp.init(friend);
        }
    }
}

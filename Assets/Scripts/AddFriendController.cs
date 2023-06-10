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
        Debug.Log("������ı���ȡ�ɹ�");
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
            Debug.LogWarning("�������");
        }
        else
        {
            Debug.Log("�û����ݻ�ȡ�ɹ�");
            DataSnapshot snapshot = task.Result;
            if (snapshot.HasChild(friendId))
            {
                Debug.Log("�û�����");
                var temp = Instantiate<GameObject>(friendPrefab, content.transform).GetComponent<AddFriendIconController>();
                friendIconList.Add(temp);
                temp.init(friendId);
            }
            else
            {
                Debug.LogWarning("���û�������");
            }
            /*foreach (DataSnapshot childSnapshot in snapshot.Children)//�������к��ѵ�uid
            {
                string name = childSnapshot.Value.ToString();
                friendlist.Add(name);
            }
            Debug.Log("�����б��ȡ���");
        }
        foreach (AddFriendIconController item in friendIconList)
        {
            Destroy(item.gameObject);
        }
        friendIconList.Clear();
        foreach (string friend in friendlist)
        {
            Debug.Log("�������� " + friend);
            var temp = Instantiate<GameObject>(friendPrefab, content.transform).GetComponent<FriendIconController>();
            friendIconList.Add(temp);
            temp.init(friend);
        }*/
            
        }
    }
}

using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendListController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject friendPrefab;
    [SerializeField] private GameObject content;
    [SerializeField] private Text upperUserNameText;
    [SerializeField] private Image upperUserHead;
    [SerializeField] private ToggleGroup bottomToggleGroup;
    [SerializeField] List<FriendIconController> friendIconList = new List<FriendIconController>();

    [SerializeField] private GameObject AddFriendPanelPrefab;
    [SerializeField] private AddFriendController addFriendPanel;
    [SerializeField] private GameObject UserDataPanelPrefab;
    [SerializeField] private UserDataController userDataPanel;
    
    
    void Start()
    {
        // init("lHutIeAly4QKNPASN5HxotT2CL23");
        // StartCoroutine(Tool_ZW.CheckUserData("lHutIeAly4QKNPASN5HxotT2CL23"));
    }

    public void init(string Userid)
    {

        StartCoroutine(GetFriendList(Userid));
    }
    public IEnumerator GetFriendList(string Userid)
    {
        List<string> friendlist = new List<string>();
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        var task = FirebaseDatabase.DefaultInstance.GetReference("Users/" + Userid).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.LogWarning("网络错误");
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            var firendShot = snapshot.Child("Friend");
            foreach (DataSnapshot childSnapshot in firendShot.Children) //保存所有好友的uid
            {
                string name = childSnapshot.Value.ToString();
                friendlist.Add(name);
            }
            Debug.Log("用户信息获取成功");

            //更改名字
            if (snapshot.Child("Name").Value != null)
            {
                upperUserNameText.text = snapshot.Child("Name").Value.ToString();
            }

            //清楚之前的生成
            foreach (FriendIconController item in friendIconList)
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
                temp.init(friend, this);
            }
        }

        
    }

    
}

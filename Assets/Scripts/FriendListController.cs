using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FriendListController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private string Userid;
    [SerializeField] private GameObject friendPrefab;
    [SerializeField] private GameObject content;
    [SerializeField] private Text upperUserNameText;
    [SerializeField] private Image upperUserHead;
    [SerializeField] private Sprite defaultHeadIcon;
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

    private void OnDisable()
    {
        upperUserHead.sprite = defaultHeadIcon;
        upperUserNameText.text = "UserName";
        FirebaseDatabase.DefaultInstance.GetReference($"Users/{Userid}/Friend").LimitToLast(1).ChildAdded -= HandleFirendAdded;
    }


    public void init(string Userid)
    {
        this.Userid = Userid;
        StartCoroutine(GetUserData(Userid));
        FirebaseDatabase.DefaultInstance.GetReference($"Users/{Userid}/Friend").LimitToLast(1).ChildAdded += HandleFirendAdded;
    }

    public void HandleFirendAdded(object sender, ChildChangedEventArgs args)
    {
        Debug.Log($"HandleFirendAdded 检测到好友添加 位置 {args.Snapshot.Reference.ToString()}");
        var temp = Instantiate<GameObject>(friendPrefab, content.transform).GetComponent<FriendIconController>();
        friendIconList.Add(temp);
        temp.init(args.Snapshot.Value.ToString(), this);
    }

    public IEnumerator GetUserData(string Userid)
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
                //Debug.LogWarning("用户名称" + snapshot.Child("Name").Value);
                upperUserNameText.text = snapshot.Child("Name").Value.ToString();
            }

            //更改头像
            if (snapshot.Child("Image") != null && !string.IsNullOrWhiteSpace(snapshot.Child("Image").Value.ToString()))
            {
                UnityEvent fail = new UnityEvent();
                UnityEvent<byte[]> suc = new UnityEvent<byte[]>();

                fail.AddListener(()=> {
                    upperUserHead.sprite = defaultHeadIcon;
                });

                suc.AddListener((bytes) => {
                    Texture2D texture = new Texture2D(300, 300);
                    texture.LoadImage(bytes);
                    upperUserHead.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                });

                StartCoroutine(Tool_ZW.GetImage(snapshot.Child("Image").Value.ToString(), suc, fail));
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
    
    public void ChatToggleClick(bool value)
    {
        //Debug.Log($"ChatToggleClick {value}");
        
    }

    public void AddToggleClick(bool value)
    {
        Debug.Log($"AddToggleClick {value}");
        if (value)
        {
            if (addFriendPanel == null)
            {
                addFriendPanel = Instantiate<GameObject>(AddFriendPanelPrefab, transform).GetComponent<AddFriendController>();
            }

            if (!addFriendPanel.isActiveAndEnabled)
            {
                addFriendPanel.gameObject.SetActive(true);
            }
        }
        else
        {
            if (addFriendPanel != null)
            {
                addFriendPanel.gameObject.SetActive(false);
            }
        }

        
    }

    public void GearToggleClick(bool value)
    {
        Debug.Log($"GearToggleClick {value}");
        if (value)
        {
            if (userDataPanel == null)
            {
                userDataPanel = Instantiate<GameObject>(UserDataPanelPrefab, transform).GetComponent<UserDataController>();
                userDataPanel.init(Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId);
                return;
            }

            if (!userDataPanel.isActiveAndEnabled)
            {
                userDataPanel.gameObject.SetActive(true);
                userDataPanel.init(Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId);
            }
            
        }
        else
        {
            if (userDataPanel != null)
            {
                userDataPanel.gameObject.SetActive(false);
            }
        }

    }

    
}

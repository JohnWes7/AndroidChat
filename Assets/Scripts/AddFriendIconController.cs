using Firebase.Database;
using Firebase.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddFriendIconController : MonoBehaviour
{
    [SerializeField] private Text friendName;
    [SerializeField] private Image friendImage;
    [SerializeField] private Button addFriend;
    string id = new string("");
    public void init(string friendid)
    {
        StartCoroutine(ShowFriendDetail(friendid));
        id = friendid;
    }
    public void OnAddButtonClick()
    {
       // string userId = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        string userId = "6AxgcfVMjMY8Mt3sdvkFKHv7oYC2";
        Debug.Log($"当前用户：{userId},目标用户：{id}");
        var refe = FirebaseDatabase.DefaultInstance.GetReference($"Users/{userId}/Friend").Push();
        refe.SetValueAsync(id).ContinueWith((task) => {
            if (task.Exception != null)
            {
                Debug.Log("添加好友失败");
            }
            else
            {
                Debug.Log("添加好友成功");
                addFriend.gameObject.SetActive(false);
            }
        });


    }



    public IEnumerator ShowFriendDetail(string friendid)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        var task = FirebaseDatabase.DefaultInstance.GetReference("Users/" + friendid).GetValueAsync();
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
            FirebaseStorage storage = FirebaseStorage.DefaultInstance;
            //Debug.Log($"gs://chat-softw.appspot.com/userimage/{snapshot.Child("Image").Value.ToString()}");
            var task1 = storage.GetReferenceFromUrl($"gs://chat-softw.appspot.com/userimage/{snapshot.Child("Image").Value.ToString()}").GetBytesAsync(10485760);
            yield return new WaitUntil(() => task1.IsCompleted);
            if (task1.Exception != null)
            {
                Debug.LogWarning("加载默认头像");
                friendImage.sprite = Resources.Load<Sprite>("UI/Image/defaultHead");
            }
            else
            {
                Debug.LogWarning("头像获取成功");
                Texture2D texture = new Texture2D(300, 300);
                texture.LoadImage(task1.Result);
                friendImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }


        }
    }
}

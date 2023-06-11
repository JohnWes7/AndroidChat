using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Storage;

public class UserDataController : MonoBehaviour
{
    [SerializeField] private Text userName;
    [SerializeField] private Image userImage;
    void Start()
    {
        init("6AxgcfVMjMY8Mt3sdvkFKHv7oYC2");
        // StartCoroutine(Tool_ZW.CheckUserData("lHutIeAly4QKNPASN5HxotT2CL23"));
    }
    public void init(string Userid)
    {

        StartCoroutine(GetUserData(Userid));


    }
    public IEnumerator GetUserData(string Userid)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        var task = FirebaseDatabase.DefaultInstance.GetReference($"Users/{Userid}").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.LogWarning("网络错误");
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            userName.text = snapshot.Child("Name").Value.ToString();
            Debug.Log($"{snapshot.Child("Name").Value.ToString()}用户id获取成功： {Userid}");
            FirebaseStorage storage = FirebaseStorage.DefaultInstance;
            Debug.Log($"gs://chat-softw.appspot.com/userimage/{snapshot.Child("Image").Value.ToString()}");
            var task1 = storage.GetReferenceFromUrl($"gs://chat-softw.appspot.com/userimage/{snapshot.Child("Image").Value.ToString()}").GetBytesAsync(10485760);
            yield return new WaitUntil(() => task1.IsCompleted);
            if (task1.Exception != null)
            {
                Debug.LogWarning("加载默认头像");
                userImage.sprite = Resources.Load<Sprite>("UI/Image/defaultHead.png");
            }
            else
            {
                Debug.LogWarning("头像获取成功");
                Texture2D texture = new Texture2D(300, 300);
                texture.LoadImage(task1.Result);
                userImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),new Vector2(0.5f, 0.5f));
            }
        }
    }
}

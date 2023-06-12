using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Storage;
using System.IO;
using Firebase.Auth;

public class UserDataController : MonoBehaviour
{
    [SerializeField] private Text userName;
    [SerializeField] private Image userImage;
    [SerializeField] public Button btn;
    [SerializeField] public Button LogoutButton;

    //AndroidJavaObject jo;

    private void Awake()
    {
        //AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        //btn.onClick.AddListener(CallAndroid);
    }

    void Start()
    {
        // init("6AxgcfVMjMY8Mt3sdvkFKHv7oYC2");
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

            UnityEngine.Events.UnityEvent<byte[]> success = new UnityEngine.Events.UnityEvent<byte[]>();
            success.AddListener((imagebyte) =>
            {
                Texture2D texture = new Texture2D(300, 300);
                texture.LoadImage(imagebyte);
                userImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            });

            UnityEngine.Events.UnityEvent fail = new UnityEngine.Events.UnityEvent();
            fail.AddListener(() =>
            {
                //Debug.Log("UI/Image/defaultHead.png");
                userImage.sprite = Resources.Load<Sprite>("UI/Image/defaultHead");
            });

            StartCoroutine(Tool_ZW.GetImage(snapshot.Child("Image").Value.ToString(), success, fail));


            //string imageid = snapshot.Child("Image").Value.ToString();
            //FirebaseStorage storage = FirebaseStorage.DefaultInstance;
            ////Debug.Log($"gs://chat-softw.appspot.com/userimage/{snapshot.Child("Image").Value.ToString()}");
            //var task1 = storage.GetReferenceFromUrl($"gs://chat-softw.appspot.com/userimage/{snapshot.Child("Image").Value.ToString()}").GetBytesAsync(10485760);
            //yield return new WaitUntil(() => task1.IsCompleted);
            //if (task1.Exception != null)
            //{
            //    Debug.LogWarning("加载默认头像");
            //    userImage.sprite = Resources.Load<Sprite>("UI/Image/defaultHead.png");
            //}
            //else
            //{
            //    Debug.LogWarning("头像获取成功");
            //    Texture2D texture = new Texture2D(300, 300);
            //    texture.LoadImage(task1.Result);
            //    userImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            //}
        }
    }

    public void Logout()
    {
        FirebaseAuth.DefaultInstance.SignOut();
    }
    
    //void CallAndroid()
    //{
    //    jo.Call("startPhoto");
    //}

    //public void CallUnity(string str)
    //{
    //    ShowImage(str);
    //    jo.Call("CallAndroid", string.Format("图片Address>>>>" + str));

    //    //string path = "file://"  + str;
    //    //StartCoroutine(LoadTexturePreview(path));
    //}

    //private void ShowImage(string path)
    //{
    //    string userid = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    //    FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
    //    fileStream.Seek(0, SeekOrigin.Begin);
    //    byte[] bye = new byte[fileStream.Length];
    //    fileStream.Read(bye, 0, (int)bye.Length);
    //    fileStream.Close();
    //    fileStream.Dispose();
    //    fileStream = null;
    //    UpdateImage(bye, userid);
    //}

    //public IEnumerator UpdateImage(byte[] bye, string userid)
    //{
    //    FirebaseStorage storage = FirebaseStorage.DefaultInstance;
    //    StorageReference storageRef = storage.GetReferenceFromUrl("gs://chat-softw.appspot.com");
    //    StorageReference riversRef = storageRef.Child($"userimage/{userid}.png");
    //    var task = riversRef.PutBytesAsync(bye);
    //    yield return new WaitUntil(() => task.IsCompleted);
    //    if (task.Exception != null)
    //    {
    //        Debug.LogWarning("网络错误");
    //    }
    //    else
    //    {
    //        StorageMetadata metadata = task.Result;
    //        string md5Hash = metadata.Md5Hash;
    //        Debug.Log("Finished uploading...");
    //        Debug.Log("md5 hash = " + md5Hash);
    //    }
    //}

}

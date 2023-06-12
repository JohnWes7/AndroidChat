using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Storage;
using System.IO;

public class UserDataController : MonoBehaviour
{
    [SerializeField] private Text userName;
    [SerializeField] private Image userImage;
    public Button btn;
    AndroidJavaObject jo;
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
            UnityEngine.Events.UnityEvent <byte[]>success = new UnityEngine.Events.UnityEvent<byte[]>();
            success.AddListener((imagebyte) =>
            {
                Texture2D texture = new Texture2D(300, 300);
                texture.LoadImage(imagebyte);
                userImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            });
            UnityEngine.Events.UnityEvent fail = new UnityEngine.Events.UnityEvent();
            fail.AddListener(() =>
            {
                userImage.sprite = Resources.Load<Sprite>("UI/Image/defaultHead.png");
            });
            StartCoroutine(Tool_ZW.GetImage(snapshot.Child("Image").ToString(), success,fail));
        }
    }
    private void Awake()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        btn.onClick.AddListener(CallAndroid);
    }
    void CallAndroid()
    {
        jo.Call("startPhoto");
    }
    public void CallUnity(string str)
    {
        ShowImage(str);
        jo.Call("CallAndroid", string.Format("图片Address>>>>" + str));

        //string path = "file://"  + str;
        //StartCoroutine(LoadTexturePreview(path));
    }
    private void ShowImage(string path)
    {
        string userid = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        byte[] bye = new byte[fileStream.Length];
        fileStream.Read(bye, 0, (int)bye.Length);
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;
        UpdateImage(bye, userid);
    }
    public IEnumerator UpdateImage(byte[] bye, string userid)
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference storageRef = storage.GetReferenceFromUrl("gs://chat-softw.appspot.com");
        StorageReference riversRef = storageRef.Child($"userimage/{userid}.png");
        var task = riversRef.PutBytesAsync(bye);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.LogWarning("网络错误");
        }
        else
        {
            StorageMetadata metadata = task.Result;
            string md5Hash = metadata.Md5Hash;
            Debug.Log("Finished uploading...");
            Debug.Log("md5 hash = " + md5Hash);
        }
    }

    }

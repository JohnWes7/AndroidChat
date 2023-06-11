using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDataController : MonoBehaviour
{
    [SerializeField] private Text userName;
    [SerializeField] private Image userImage;
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
            FirebaseStorage storage = FirebaseStorage.DefaultInstance;

        }
    }
}

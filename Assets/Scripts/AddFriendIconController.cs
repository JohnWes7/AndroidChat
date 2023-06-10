using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddFriendIconController : MonoBehaviour
{
    [SerializeField] private Text friendName;
    [SerializeField] private Image friendImage;
    public void init(string friendid)
    {
        StartCoroutine(ShowFriendDetail(friendid));
    }
    public IEnumerator ShowFriendDetail(string friendid)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        var task = FirebaseDatabase.DefaultInstance.GetReference("Users/" + friendid).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.LogWarning("�������");
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            Debug.Log("�������ݻ�ȡ�ɹ�");
            friendName.text = snapshot.Child("Name").Value.ToString();
            string imageid = snapshot.Child("Image").Value.ToString();
            


        }
    }
}

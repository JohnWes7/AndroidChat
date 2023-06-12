using Firebase.Database;
using Firebase.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class Tool_ZW
{
    public static IEnumerator CheckUserData(string UserId, UnityEvent dataExistEvent = null, UnityEvent failEvent = null,UnityEvent dataNotExistEvent = null)
    {
        //初始值字典

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        var task = FirebaseDatabase.DefaultInstance.GetReference($"Users/{UserId}" ).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogWarning($"Tool:CheckUserData 用户数据查询失败 : {UserId}");
            if (failEvent != null)
            {
                failEvent.Invoke();
            }
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            if (snapshot.HasChild("Friend") && snapshot.HasChild("Image") && snapshot.HasChild("Name")){
                Debug.Log($"Tool:CheckUserData 该用户的数据已存在 : {UserId} ");
                if (dataExistEvent != null)
                {
                    dataExistEvent.Invoke();
                }
            }
            else
            {
                Debug.Log($"Tool:CheckUserData 该用户的数据不存在 : {UserId} ");
                if (dataNotExistEvent != null)
                {
                    dataNotExistEvent.Invoke();
                }
            }
            
        }
    }
    public static IEnumerator GetImage(string imageid, UnityEvent <byte[]>succesEvent = null, UnityEvent failEvent = null)
    {
        Debug.Log("开始获取图片");
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        var task1 = storage.GetReferenceFromUrl($"gs://chat-softw.appspot.com/userimage/{imageid}").GetBytesAsync(10485760);
        yield return new WaitUntil(() => task1.IsCompleted);
        if (task1.Exception != null)
        {
            if (failEvent != null)
            {
                Debug.Log($"头像获取失败 {imageid}");

                failEvent.Invoke();
            }
        }
        else
        {
            if (succesEvent != null)
            {
                Debug.Log("头像获取成功");
                succesEvent.Invoke(task1.Result);
            }
        }
    }
    }

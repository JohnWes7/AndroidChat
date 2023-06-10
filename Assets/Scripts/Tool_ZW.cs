using Firebase.Database;
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
}

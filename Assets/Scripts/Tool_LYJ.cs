using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.Events;
using Firebase.Auth;

public static class Tool_LYJ
{
    public static IEnumerator CreateNewUserData(FirebaseUser user, string name = "", UnityEvent succesfulEvent = null, UnityEvent failEvent = null)
    {
        //初始值字典
        Dictionary<string, string> defaultInfo = new Dictionary<string, string>();
        string UserId = user.UserId;
        defaultInfo.Add("Friend", "");
        defaultInfo.Add("Image", "");
        if (string.IsNullOrWhiteSpace(name))
        {
            name = user.Email;
        }
        defaultInfo.Add("Name", name);

        var refence = FirebaseDatabase.DefaultInstance.GetReference($"Users/{UserId}");
        var task = refence.SetValueAsync(defaultInfo);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogWarning($"Tool:CreateNewUserData 用户初始值创建失败 : {UserId} {name}");
            if (failEvent != null)
            {
                failEvent.Invoke();
            }
        }
        else
        {
            Debug.Log($"Tool:CreateNewUserData 用户初始值创建成功 : {UserId} {name}");
            if (succesfulEvent!=null)
            {
                succesfulEvent.Invoke();
            }
        }
    }
    
}

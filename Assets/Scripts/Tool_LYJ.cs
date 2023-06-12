using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.Events;
using Firebase.Auth;
using Firebase.Storage;

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
            if (succesfulEvent != null)
            {
                succesfulEvent.Invoke();
            }
        }
    }

    public static IEnumerator GetUserHeadImageName(string userID, UnityEvent<string> succesEvent = null, UnityEvent failEvent = null)
    {
        var imageURLtask = FirebaseDatabase.DefaultInstance.GetReference($"Users/{userID}/Image").GetValueAsync();
        yield return new WaitUntil(() => imageURLtask.IsCompleted);

        if (imageURLtask.Exception != null)
        {
            if (failEvent!=null)
            {
                Debug.Log("Tool:GetUserHeadImage 获取用户头像url失败 网络链接失败");
                failEvent.Invoke();
            }
        }
        else
        {
            
            if (imageURLtask.Result.Value != null)
            {
                Debug.Log("Tool:GetUserHeadImage 获取用户头像url成功");
                if (succesEvent != null)
                {
                    succesEvent.Invoke(imageURLtask.Result.Value.ToString());
                }
            }
            else
            {
                Debug.Log("Tool:GetUserHeadImage 获取用户头像url失败 返回value为null");
                failEvent.Invoke();
            }
        }
    }

    public static Sprite Bytes2Sprite(byte[] data)
    {
        Texture2D texture = new Texture2D(300, 300);
        texture.LoadImage(data);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}

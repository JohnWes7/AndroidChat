using Firebase;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBase : MonoBehaviour
{
    private bool firebaseInitialized;

    public void Awake()
    {
        
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        InitializeFirebaseAndStart();
        while (!firebaseInitialized)
        {
            yield return null;
        }

    }

    // 初始化firebase
    void InitializeFirebaseAndStart()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                firebaseInitialized = true;
                Debug.Log("firebase Initialized");
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
                // Application.Quit();
            }
        });
    }


    private IEnumerator RegisterByEmail(string email, string password)
    {
        Debug.Log("开始注册");
        var task = Firebase.Auth.FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogWarning($"注册错误: {task.Exception}");
            Debug.LogWarning($"注册错误 inner: {task.Exception.InnerExceptions}");
        }
        else
        {
            Debug.Log($"注册成功: {task.Result.User.Email}");
        }
    }
}


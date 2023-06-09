using Firebase;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBase : MonoBehaviour
{
    [SerializeReference] Firebase.FirebaseApp app;
    [SerializeReference] Firebase.Auth.FirebaseAuth auth;
    [SerializeField] string email;
    [SerializeField] string password;

    private bool firebaseInitialized;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        InitializeFirebaseAndStart();
        while (!firebaseInitialized)
        {
            yield return null;
        }
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Debug.Log("开始注册: " + email + " " + password);
            //auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            //    Debug.Log("回调函数");

            //    if (task.IsCanceled)
            //    {
            //        Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
            //        return;
            //    }
            //    if (task.IsFaulted)
            //    {
            //        Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
            //        return;
            //    }

            //    // Firebase user has been created.
            //    Firebase.Auth.AuthResult result = task.Result;
            //    Debug.LogFormat("Firebase user created successfully: {0} ({1})",
            //        result.User.DisplayName, result.User.UserId);

            //});

            StartCoroutine(RegisterByEmail(email, password));
        }
    }

    private IEnumerator RegisterByEmail(string email, string password)
    {
        Debug.Log("开始注册");
        var task = auth.CreateUserWithEmailAndPasswordAsync(email, password);
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


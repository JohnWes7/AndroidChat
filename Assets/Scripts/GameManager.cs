using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;

public class GameManager : MonoBehaviour
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
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
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


    
}

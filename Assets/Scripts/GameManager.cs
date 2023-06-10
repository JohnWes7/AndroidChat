using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;

public class GameManager : MonoBehaviour
{
    

    private bool firebaseInitialized;
    [SerializeField] Firebase.Auth.FirebaseAuth auth;
    [SerializeField] Firebase.Auth.FirebaseUser user;
    

    public void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        InitializeFirebaseAndStart();
        while (!firebaseInitialized)
        {
            yield return null;
        }

        // 增加用户活跃
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

        // 用户改变回调
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);

        Debug.Log(auth.CurrentUser);
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


    // Handle initialization of the necessary firebase modules:
    //void InitializeFirebase()
    //{
    //    Debug.Log("Setting up Firebase Auth");
    //    auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    //    auth.StateChanged += AuthStateChanged;
    //    AuthStateChanged(this, null);
    //}

    // Track state changes of the auth object.
    // 跟踪
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("GameManager: Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("GameManager: Signed in " + user.UserId);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            auth.SignOut();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log(auth.CurrentUser);
        }
    }

    // Handle removing subscription and reference to the Auth instance.
    // Automatically called by a Monobehaviour after Destroy is called on it.
    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

}

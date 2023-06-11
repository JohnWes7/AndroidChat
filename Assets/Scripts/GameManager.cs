using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;

public class GameManager : MonoBehaviour
{
    // 单例
    private static GameManager instance;
    public static GameManager Instance { get => instance; }

    public enum GameState
    {
        loginSignup,
        checkCurUserData,
        friendList
    }

    private bool firebaseInitialized;
    [SerializeField] private Firebase.Auth.FirebaseAuth auth;
    [SerializeField] private Firebase.Auth.FirebaseUser user;
    [SerializeField] private GameState state;

    // 界面实例
    [SerializeField] private SignInUpPanelController signInUpPanelController;
    [SerializeField] private CheckUserDataPanelController checkUserDataPanelController;
    [SerializeField] private FriendListController friendListPanelController;

    //[SerializeField] private  userdata;

    public void Awake()
    {
        // 如果有其他初始单例直接销毁
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            SwitchState(GameState.loginSignup);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
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
            Debug.Log($"{signedIn} {user} {auth.CurrentUser}");
            if (!signedIn && user != null)
            {
                Debug.Log("GameManager: Signed out " + user.UserId);
                SwitchState(GameState.loginSignup);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("GameManager: Signed in " + user.UserId);
                SwitchState(GameState.checkCurUserData);
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

    public void SwitchState(GameState state)
    {
        // 如果是自己转自己则不转换状态
        if (this.state == state)
        {
            Debug.Log($"{this.state} -> {state} do nothing");
            return;
        }

        Debug.Log($"{this.state} -> {state}");
        switch (state)
        {
            // 注册登录
            case GameState.loginSignup:
                SetActivePanel(friendListPanelController, false);
                SetActivePanel(checkUserDataPanelController, false);
                SetActivePanel(signInUpPanelController, true);
                break;
            // 检查数据
            case GameState.checkCurUserData:
                SetActivePanel(friendListPanelController, false);
                SetActivePanel(signInUpPanelController, false);
                SetActivePanel(checkUserDataPanelController, true);
                if (checkUserDataPanelController)
                {
                    checkUserDataPanelController.Init();
                }
                break;
            // 好友列表
            case GameState.friendList:
                SetActivePanel(friendListPanelController, true);
                SetActivePanel(signInUpPanelController, false);
                SetActivePanel(checkUserDataPanelController, false);
                if (friendListPanelController != null)
                {
                    friendListPanelController.init(auth.CurrentUser.UserId);
                }
                break;
            default:
                break;
        }
        this.state = state;
    }

    private void SetActivePanel(MonoBehaviour panelCompent, bool value)
    {
        if (panelCompent != null)
        {
            panelCompent.gameObject.SetActive(value);
            return;
        }

        Debug.LogWarning($"panelCompent 不存在 不能设置Active");
    }

    public string GetUserNameFromSignPanel()
    {
        if (signInUpPanelController != null)
        {
            return signInUpPanelController.GetSignupName();
        }

        return "";
    }
}

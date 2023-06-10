using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CheckUserDataPanelController : MonoBehaviour
{
    [SerializeField] private UnityEvent dataExistEvent;
    [SerializeField] private UnityEvent failEvent;
    [SerializeField] private UnityEvent dataNotExistEvent;

    [SerializeField] private Text logText;
    [SerializeField] private Button retryButton;

    [SerializeField] private Coroutine checkCoro;
    [SerializeField] private Coroutine createCoro;


    public void Init()
    {
        //关闭retry按钮显示
        retryButton.gameObject.SetActive(false);

        // 没有用户
        if (Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser == null)
        {
            GameManager.Instance.SwitchState(GameManager.GameState.loginSignup);
            return;
        }

        // 有登录的用户
        var user = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser;

        // 检查是否有正在check 或者正在创建
        if (checkCoro != null || createCoro != null)
        {
            Debug.Log($"CheckUserDataPanelController:Init find user data : {user.UserId}");
            return;
        }

        // 必须要没有check 或者正在创建数据才能init 因为也只有这里再创建和检查用户数据
        dataExistEvent = new UnityEvent();
        failEvent = new UnityEvent();
        dataNotExistEvent = new UnityEvent();
        // 数据存在事件
        dataExistEvent.AddListener(() =>
        {
            Debug.Log($"CheckUserDataPanelController:Init find user data : {user.UserId}");
            logText.text = "found data!";
            checkCoro = null;
            createCoro = null;

            GameManager.Instance.SwitchState(GameManager.GameState.friendList);
        });

        // 无法连接数据库 获取不到数据
        failEvent.AddListener(() =>
        {
            Debug.Log($"CheckUserDataPanelController:Init network error pleace try again : {user.UserId}");
            logText.text = $"no network connect, pleace try agan";
            checkCoro = null;
            createCoro = null;
        });

        // 数据不存在
        dataNotExistEvent.AddListener(() =>
        {
            Debug.Log($"CheckUserDataPanelController:Init can not find user data : {user.UserId}, start create");
            logText.text = $"can not find any data of user: {user.UserId}\nstart to create";

            UnityEvent failCreateEvent = new UnityEvent();
            UnityEvent sucCreateEvent = new UnityEvent();

            failCreateEvent.AddListener(()=> {
                Debug.Log("创建数据失败");
                logText.text = "fail to create defuault data";
                checkCoro = null;
                createCoro = null;
            });

            sucCreateEvent.AddListener(() =>
            {
                Debug.Log("创建数据成功");
                logText.text = "create data succesfully, jump to main panel";
                // 跳出界面
                GameManager.Instance.SwitchState(GameManager.GameState.friendList);
                checkCoro = null;
                createCoro = null;
            });

            // 数据不存在创建数据
            createCoro = StartCoroutine(Tool_LYJ.CreateNewUserData(user, GameManager.Instance.GetUserNameFromSignPanel(), sucCreateEvent, failCreateEvent));
        });

        
        checkCoro = StartCoroutine(Tool_ZW.CheckUserData(user.UserId, dataExistEvent, failEvent, dataNotExistEvent));
    }

    private void OnDisable()
    {
        
    }
}

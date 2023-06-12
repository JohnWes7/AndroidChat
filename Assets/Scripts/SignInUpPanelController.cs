using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SignInUpPanelController : MonoBehaviour
{
    public enum SUPstate
    {
        login,
        signup
    }

    [SerializeField] private Text showText;
    [SerializeField] private GameObject login;
    [SerializeField] private GameObject signup;
    [SerializeField] private SUPstate state;
    // 已经开始的协程
    [SerializeField] private Coroutine startedCorotine;

    [SerializeField] private UnityEvent signupSuccessfulEvent = new UnityEvent();
    [SerializeField] private UnityEvent signupFailEvent = new UnityEvent();
    [SerializeField] private UnityEvent loginSuccessfulEvent = new UnityEvent();
    [SerializeField] private UnityEvent loginFailEvent = new UnityEvent();

    // log input 登录输入框
    [SerializeField] private InputField logEmail;
    [SerializeField] private InputField logPsw;
    // signup input 注册输入框
    [SerializeField] private InputField signupName;
    [SerializeField] private InputField signupEmail;
    [SerializeField] private InputField signupPsw;
    [SerializeField] private InputField signupConfirmPsw;



    private void Start()
    {
        Init();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        startedCorotine = null;
    }

    private void Init()
    {
        Debug.Log("初始化登录界面");
        // 初始是在登录界面
        state = SUPstate.login;
        SwitchPanel(state);

        // 注册成功回调
        signupSuccessfulEvent.AddListener(() =>
        {
            //成功创建账号 打印
            if (showText)
            {
                showText.text = "创建账号成功";
            }

        });

        // 注册失败回调
        signupFailEvent.AddListener(() =>
        {
            if (showText)
            {
                showText.text = "fail to sign up, check your network or email has been registed";
                showText.text = "注册失败, 检查网络连接或者邮箱已经被注册";
            }
        });

        // 登录成功回调
        loginSuccessfulEvent.AddListener(LoginSuccessfulCallBack);

        // 登陆失败回调
        loginFailEvent.AddListener(() => {
            if (showText)
            {
                showText.text = "fail to log in, please check your email and password";
                showText.text = "登录失败, 检查网络链接或者密码不正确";
            }
        });
    }

    public void LogIn()
    {
        if (state == SUPstate.login)
        {
            //执行登录
            if (startedCorotine == null)
            {
                string email;
                string psw;

                GetLoginInfo(out email, out psw);
                Debug.Log($"{email} {psw}");
                if (CheckInfo(email, psw))
                {
                    startedCorotine = StartCoroutine(LoginByEmail(email, psw));
                }
                else
                {
                    showText.text = "some in input is empty or password not corect";
                    showText.text = "账号密码不正确或者输入为空";
                }
            }
            else
            {
                showText.text = "already doing login";
                showText.text = "正在与服务器链接";
            }
        }
        else
        {
            //切换界面
            SwitchPanel(SUPstate.login);
        }

    }

    public void SignUp()
    {
        if (state == SUPstate.signup)
        {
            if (startedCorotine == null)
            {
                string email;
                string psw;
                string confirm;
                string sname;

                GetSignUpInfo(out email, out psw, out confirm, out sname);
                Debug.Log($"{email} {psw} {confirm} {name}");
                if (CheckInfo(email, psw, confirm, name))
                {
                    startedCorotine = StartCoroutine(RegisterByEmail(email, psw, name));
                }
                else
                {
                    showText.text = "some in input is empty or confirm password is not same";
                    showText.text = "输入不正确,两个密码不一致或者为空";
                }
            }
            else
            {
                showText.text = "already doing sign up";
                showText.text = "正在与服务器链接";
            }
        }
        else
        {
            //切换界面
            SwitchPanel(SUPstate.signup);
        }
    }

    public void SwitchPanel(SUPstate state)
    {
        // 切换面板
        switch (state)
        {
            case SUPstate.login:
                login.SetActive(true);
                signup.SetActive(false);
                break;
            case SUPstate.signup:
                login.SetActive(false);
                signup.SetActive(true);
                break;
            default:
                break;
        }
        this.state = state;
    }

    public void GetSignUpInfo(out string email, out string psw, out string confirmPsw, out string name)
    {
        // 获取注册界面用户信息
        email = signupEmail.text;
        psw = signupPsw.text;
        confirmPsw = signupConfirmPsw.text;
        name = signupName.text;
    }

    public void GetLoginInfo(out string email, out string psw)
    {
        // 获取登录界面用户信息
        email = logEmail.text;
        psw = logPsw.text;
    }

    public bool CheckInfo(string email, string psw)
    {
        return !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(psw);
    }

    public bool CheckInfo(string email, string psw, string confirm, string name)
    {
        return CheckInfo(email, psw) && !string.IsNullOrEmpty(psw) && confirm.Equals(psw) && !string.IsNullOrWhiteSpace(name);
    }

    private IEnumerator RegisterByEmail(string email, string password, string name)
    {
        Debug.Log($"开始注册 {email} {password}");
        var task = Firebase.Auth.FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogWarning($"注册错误: {task.Exception}");
            signupFailEvent.Invoke();
        }
        else
        {
            Debug.Log($"注册成功: {task.Result.User.Email}");
            // 如果注册成功开始 则要创建用户数据
            signupSuccessfulEvent.Invoke();
        }

        startedCorotine = null;
    }

    private IEnumerator LoginByEmail(string email, string password)
    {
        Debug.Log($"开始登录 {email} {password}");
        var task = Firebase.Auth.FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsCanceled)
        {
            Debug.LogWarning("SignInWithEmailAndPasswordAsync was canceled.");
            loginFailEvent.Invoke();
        }
        else if (task.IsFaulted)
        {
            Debug.LogWarning("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
            loginFailEvent.Invoke();
        }
        else
        {
            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);
            Debug.Log($"登录成功: {Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId}");
            //Debug.Log($"display name: {Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.DisplayName}");
            loginSuccessfulEvent.Invoke();
        }

        startedCorotine = null;
    }

    private void LoginSuccessfulCallBack()
    {
        Debug.Log("SignInUpPanelController:LoginSuccessfulCallBack 等待用户状态跟踪进行跳转");
        // 如果没有跳转则需这边强制跳转 前提是真的用户登录了

    }

    public string GetSignupName()
    {
        if (signupName)
        {
            return signupName.text;
        }
        return "";
    }
}

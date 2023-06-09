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

    [SerializeField] private UnityEvent signupSuccessful;
    [SerializeField] private UnityEvent signupFail;
    [SerializeField] private UnityEvent loginSuccessful;
    [SerializeField] private UnityEvent loginFail;

    // log input
    [SerializeField] private InputField logEmail;
    [SerializeField] private InputField logPsw;
    // signup input
    [SerializeField] private InputField signupEmail;
    [SerializeField] private InputField signupPsw;
    [SerializeField] private InputField signupConfirmPsw;


    private void Start()
    {
        Init();
    }

    private void Init()
    {
        state = SUPstate.login;
        signupSuccessful.AddListener(() =>
        {
            //成功创建账号 打印
            if (showText)
            {
                showText.text = "create account successfully";
            }
        });

        signupFail.AddListener(() =>
        {
            if (showText)
            {
                showText.text = "fail to sign up, check your network or email has been registed";
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

                GetSignUpInfo(out email, out psw, out confirm);
                Debug.Log($"{email} {psw} {confirm}");
                if (CheckInfo(email, psw, confirm))
                {
                    startedCorotine = StartCoroutine(RegisterByEmail(email, psw));
                }
                else
                {
                    showText.text = "some in input is empty or confirm password is not same";
                }
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

    public void GetSignUpInfo(out string email, out string psw, out string confirmPsw)
    {
        // 获取注册界面用户信息
        email = signupEmail.text;
        psw = signupPsw.text;
        confirmPsw = signupConfirmPsw.text;
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

    public bool CheckInfo(string email, string psw, string confirm)
    {
        return CheckInfo(email, psw) && !string.IsNullOrEmpty(psw) && confirm.Equals(psw);
    }

    private IEnumerator RegisterByEmail(string email, string password)
    {
        Debug.Log($"开始注册 {email} {password}");
        var task = Firebase.Auth.FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogWarning($"注册错误: {task.Exception}");
            signupFail.Invoke();
        }
        else
        {
            Debug.Log($"注册成功: {task.Result.User.Email}");
            signupSuccessful.Invoke();
        }
        startedCorotine = null;
    }

    private IEnumerator LoginByEmail(string email, string password)
    {
        Debug.Log($"开始登录 {email} {password}");
        var task = Firebase.Auth.FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => task.IsCompleted);

        if (true)
        {

        }
    }
}

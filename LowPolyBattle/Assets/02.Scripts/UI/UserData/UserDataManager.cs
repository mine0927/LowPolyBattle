using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public partial class UserDataManager : MonoBehaviour
{
    public TMP_InputField loginId;
    public TMP_InputField loginPw;
    public TMP_InputField registerId;
    public TMP_InputField registerPw;
    public TMP_InputField registerPwCheck;
    public TMP_InputField registerName;
    public TextMeshProUGUI errorLoginText;
    public TextMeshProUGUI errorRegisterText;
    public GameObject window;

    [HideInInspector] public string userId;
    private static UserDataManager _instance;

    private void Awake()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        _instance = this;
    }

    public static UserDataManager GetInstance()
    {
        if (_instance == null)
            _instance = new UserDataManager();

        return _instance;
    }

    public void Login()
    {
        var request = new LoginWithPlayFabRequest { Username = loginId.text, Password = loginPw.text};
        PlayFabClientAPI.LoginWithPlayFab(request, ResultLogin, ErrorMsg);
    }
    
    private void ResultLogin(LoginResult result)
    {
        userId = result.PlayFabId;
        var request = new GetUserDataRequest { PlayFabId = result.PlayFabId };
        PlayFabClientAPI.GetUserData(request, resultData, ErrorMsg);
    }

    private void resultData(GetUserDataResult result)
    {
        foreach(var currdata in result.Data)
        {
            if (currdata.Key == "hairNum")
            {
                SceneManager.LoadScene("03Lobby");
                return;
            }
        }
        SceneManager.LoadScene("02Character Custom");
    }


    public void ErrorMsg(PlayFabError error)
    {
        switch (error.ErrorMessage)
        {
            case "Username not available":
                errorRegisterText.text = "���� ���̵�� �ߺ��Դϴ�.";
                break;
            case "The display name entered is not available.":
                errorRegisterText.text = "���� �г����� �ߺ��Դϴ�.";
                break;

            case "Invalid input parameters":
                if(loginId.text.Length < 6)
                    errorLoginText.text = "���̵� 6�� �̸����� �Է��Ͽ����ϴ�.";
                else
                    errorLoginText.text = "��й�ȣ�� 6�� �̸����� �Է��Ͽ����ϴ�.";
                break;
            case "Invalid username or password":
                errorLoginText.text = "���̵�/��й�ȣ�� �ǹٸ��� �ʽ��ϴ�.";
                break;
        }
        Debug.Log(error.ErrorMessage);
        
    }

    public void Register()
    {
        if(registerId.text.Length < 6)
        {
            errorRegisterText.text = "���̵�� �ּ� 6�� �̻� �Է��ϼž� �մϴ�.";
            return;
        }
        else if(registerPw.text.Length < 6)
        {
            errorRegisterText.text = "��й�ȣ�� �ּ� 6�� �̻� �Է��ϼž� �մϴ�";
            return;
        }
        else if(registerPw.text != registerPwCheck.text)
        {
            errorRegisterText.text = "�Է��� ��й�ȣ�� ���� �ʽ��ϴ�.";
            return;
        }
        var request = new RegisterPlayFabUserRequest { Username = registerId.text, Password = registerPw.text, DisplayName = registerName.text, RequireBothUsernameAndEmail = false };
        PlayFabClientAPI.RegisterPlayFabUser(request, ResultRegister, ErrorMsg);
    }

    private void ResultRegister(RegisterPlayFabUserResult result)
    {
        Debug.Log("ȸ������ ����");
        window.SetActive(false);
    }

}

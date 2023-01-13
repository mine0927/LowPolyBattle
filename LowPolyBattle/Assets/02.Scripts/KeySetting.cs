using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public interface enumSetting
{
    public enum skinPart { belt, cloth, crown, glove, hat, helm, shoe, shoulderPad, hair, half, face };
    public enum KeyPart { forward, backward, left, right, dash, jump, attack, pick, inventory, character };
}

public class KeySetting:MonoBehaviour, enumSetting
{
    public static Dictionary<enumSetting.KeyPart, KeyCode> initKey = new Dictionary<enumSetting.KeyPart, KeyCode>();
    private Dictionary<enumSetting.KeyPart, KeyCode> currKey = new Dictionary<enumSetting.KeyPart, KeyCode>();
    private UserDataManager manager;
    [SerializeField] private int keyCount = -1;
    [SerializeField] private TextMeshProUGUI[] keyName;
    private bool isMouse = false;
    
    private void Start()
    {
        manager = UserDataManager.GetInstance();
        var getUserData = new GetUserDataRequest { PlayFabId = manager.userId };
        PlayFabClientAPI.GetUserData(getUserData, GetUserData, manager.ErrorMsg);
        DontDestroyOnLoad(this);
    }

    private void GetUserData(GetUserDataResult result)
    {
        bool isGetKey = result.Data.ContainsKey("forward");
        
        if(isGetKey)
        {
            for(int i= 0; i < Enum.GetValues(typeof(enumSetting.KeyPart)).Length; i++)
            {
                string name = Enum.GetName(typeof(enumSetting.KeyPart), i);
                initKey[(enumSetting.KeyPart)i] = (KeyCode)Enum.Parse(typeof(KeyCode), result.Data[name].Value, true);
            }
        }
        else
        {
            InitKey(initKey);
            var data = initKey.ToDictionary(num => num.Key.ToString(), num => num.Value.ToString());
            var updateUserData = new UpdateUserDataRequest { Data = data, Permission = UserDataPermission.Public };
            PlayFabClientAPI.UpdateUserData(updateUserData, null, manager.ErrorMsg);
        }

        for (int i = 0; i < 10; i++)
        {
            keyName[i].text = initKey[(enumSetting.KeyPart)i].ToString();
            currKey.Add((enumSetting.KeyPart)i, initKey[(enumSetting.KeyPart)i]);
        }
    }

    private void InitKey(Dictionary<enumSetting.KeyPart, KeyCode> key)
    {
        key.Add(enumSetting.KeyPart.forward, KeyCode.W);
        key.Add(enumSetting.KeyPart.backward, KeyCode.S);
        key.Add(enumSetting.KeyPart.left, KeyCode.A);
        key.Add(enumSetting.KeyPart.right, KeyCode.D);
        key.Add(enumSetting.KeyPart.dash, KeyCode.LeftShift);
        key.Add(enumSetting.KeyPart.jump, KeyCode.Space);
        key.Add(enumSetting.KeyPart.inventory, KeyCode.I);
        key.Add(enumSetting.KeyPart.character, KeyCode.C);
        key.Add(enumSetting.KeyPart.attack, KeyCode.Mouse0);
        key.Add(enumSetting.KeyPart.pick, KeyCode.Z);
    }

    private void OnGUI()
    {
        Event keyEvent = Event.current;
        if((keyEvent.isKey || keyEvent.isMouse) && keyCount != -1)
        {
            KeyCode keyCode = KeyCode.None;
            if (keyEvent.isMouse) 
            {
                switch (keyEvent.button)
                {
                    case 0: keyCode = KeyCode.Mouse0; break;
                    case 1: keyCode = KeyCode.Mouse1; break;
                    case 2: keyCode = KeyCode.Mouse2; break;
                }
            }
            else
                keyCode = keyEvent.keyCode;

            if (currKey.ContainsValue(keyCode))
            {
                foreach(var keyNum in currKey.Keys)
                {
                    if (currKey[keyNum] == keyCode)
                    {
                        currKey[keyNum] = KeyCode.None;
                        keyName[(int)keyNum].text = KeyCode.None.ToString();
                        break;
                    }
                }
            }
            currKey[(enumSetting.KeyPart)keyCount] = keyCode;
            keyName[keyCount].text = keyCode.ToString();
            keyCount = -1;
            if(keyEvent.isMouse) isMouse = true;
        }
    }

    public void ChangeKey(int idx)
    {
        if (isMouse) isMouse = false;
        else keyCount = idx;
    }

    public void SaveOrCancel(bool isSave)
    {
        if (isSave)
        {
            var data = currKey.ToDictionary(num => num.Key.ToString(), num => num.Value.ToString());
            var updateUserData = new UpdateUserDataRequest { Data = data, Permission = UserDataPermission.Public };
            PlayFabClientAPI.UpdateUserData(updateUserData, SaveKey, manager.ErrorMsg);
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                keyName[i].text = initKey[(enumSetting.KeyPart)i].ToString();
                currKey[(enumSetting.KeyPart)i] = initKey[(enumSetting.KeyPart)i];
            }
        }
    }

    private void SaveKey(UpdateUserDataResult result)
    {
        for (int i = 0; i < 10; i++)
        {
            keyName[i].text = currKey[(enumSetting.KeyPart)i].ToString();
            initKey[(enumSetting.KeyPart)i] = currKey[(enumSetting.KeyPart)i];
        }
    }
}

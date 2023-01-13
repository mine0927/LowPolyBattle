using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Linq;
using UnityEngine.SceneManagement;

public class CreatePlayer: MonoBehaviour
{
    private Dictionary<string, string> outputData;
    
    public void Create()
    { 
        var inputData = new Dictionary<string, int>();
        inputData.Add("hairNum", ModelShow.hairNum);
        inputData.Add("skinNum", ModelShow.skinNum);
        outputData = inputData.ToDictionary(num => num.Key, num => num.Value.ToString());
        var request = new UpdateUserDataRequest { Data = outputData, Permission = UserDataPermission.Public };
        PlayFabClientAPI.UpdateUserData(request, ResultCreate, ErrorCallback);
    }

    private void ResultCreate(UpdateUserDataResult result)
    {
        SceneManager.LoadScene("03Lobby");
    }

    private void ErrorCallback(PlayFabError error)
    {

    }
}

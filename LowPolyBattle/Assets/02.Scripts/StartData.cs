using Photon.Pun;
using UnityEngine;

public class StartData : MonoBehaviour
{
    public void Ready()
    {
        foreach (Team team in FindObjectsOfType<Team>())
        {
            team.ReadySetting();
        }
    }
}

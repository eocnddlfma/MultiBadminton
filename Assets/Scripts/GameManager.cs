using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public GameObject LeftPlayer;
    public GameObject RightPlayer;
    public Shuttlecock shuttlecock;
    public static GameManager instance;
    public int LeftScore = 0;
    public int RightScore = 0;
    public int EndScore; 
    public string sceneName;

    public TextMeshProUGUI ScoreBoard;
    public TextMeshProUGUI Winner;
    public GameObject back;

    private void Awake()
    {
        instance = this;
        print(NetworkManager.Singleton.LocalClientId);
        RelayManager.instance.spawnObjectServerRpc(NetworkManager.Singleton.LocalClientId);
        //shuttlecock.setpositionstartClientRpc();
        shuttlecock.SetPositionWithNoRigidbody(0);
    }
    
    public void OnFloorTouch(Vector3 pos)
    {
        if ((0 < pos.x && pos.x < 15) || pos.x <= -15)
        {
            LeftScoreClientRpc();
        }
        else if ((0 > pos.x && pos.x > -15) || pos.x >= 15)
        {
            RightScoreClientRpc();
        }
    }

    public void CheckScore()
    {
        print($"ScoreBoard.text = {LeftScore} : {RightScore}");;
        print("checkscore");
        if (LeftScore > EndScore)
        {
            print("leftwin");
            Winner.gameObject.SetActive(true);
            back.SetActive(true);
            Winner.text = "Left Player Wins";
        }
        else if (RightScore > EndScore){
            print("rightwin");
            Winner.gameObject.SetActive(true);
            back.SetActive(true);
            Winner.text = "Right Player Wins";
        }
        else
        {
            print("else");
            Winner.gameObject.SetActive(false);
            back.SetActive(false);
            SetScoreClientRpc();
        }
        DebugLogExporter._logExporter.SetLog(ScoreBoard.text);
    }

    [ClientRpc]
    public void LeftScoreClientRpc()
    {
        LeftScore++;
        Shuttlecock.instance.SetPositionWithNoRigidbody(0);
        ScoreBoard.text = $"{LeftScore} : {RightScore}";
        CheckScore();
    }

    [ClientRpc]
    public void RightScoreClientRpc()
    {
        RightScore++;
        Shuttlecock.instance.SetPositionWithNoRigidbody(1);
        ScoreBoard.text = $"{LeftScore} : {RightScore}";
        CheckScore();
    }
    [ClientRpc]
    public void SetScoreClientRpc()
    {
        Shuttlecock.instance.SetPositionWithNoRigidbody(0);
        ScoreBoard.text = $"{LeftScore} : {RightScore}";
    }
    [ClientRpc]
    public void NextSceneClientRpc()
    {
        LeftScore = 0;
        RightScore = 0;
        CheckScore();
        Shuttlecock.instance.SetPositionWithNoRigidbody(0);
    }

    [ServerRpc(RequireOwnership = false)]
    public void NextSceneServerRpc()
    {
        NextSceneClientRpc();
    }
}

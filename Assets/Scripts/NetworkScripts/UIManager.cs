using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    public string sceneName;
    public static UIManager instance;

    private bool hasServerStarted;
    private void Awake()
    {
        instance = this;
    }

    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;
    [SerializeField] private Button startGameHostClientButton;
    [SerializeField] private TMP_InputField joinCodeInput;

    private void Start()
    {
        startHostButton?.onClick.AddListener(async () =>
        {
            if (RelayManager.instance.IsRelayEnabled)
                await RelayManager.instance.SetupRelay();
            
            if (NetworkManager.Singleton.StartHost())
                print("start host");
            else
                print("failhost");
        });
        startClientButton?.onClick.AddListener(async () =>
        {
            if (RelayManager.instance.IsRelayEnabled && !string.IsNullOrEmpty(joinCodeInput.text))
            {
                await RelayManager.instance.JoinRelay(joinCodeInput.text); 
            }

            if (NetworkManager.Singleton.StartClient())
            {
                print("start client");
                //SceneManager.LoadScene(1);
            }
            else
                print("failclient");
        });
        startGameHostClientButton?.onClick.AddListener(async () =>
        {
            if(NetworkManager.Singleton.IsServer)
                NextSceneServerRpc();
        });
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            hasServerStarted = true;
        };

        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            print($"{id} has connected");
            //SceneManager.LoadScene(1);
        };
    }

    [ClientRpc]
    private void NextSceneClientRpc()
    {
        NetworkLog.LogInfo("sceneclient");
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    [ServerRpc(RequireOwnership = false)]
    private void NextSceneServerRpc()
    {
        NetworkLog.LogInfo("serverscene");
        NextSceneClientRpc();
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    
    
}

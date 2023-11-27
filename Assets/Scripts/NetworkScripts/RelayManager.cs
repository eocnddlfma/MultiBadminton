using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Serialization;

public class RelayManager : NetworkBehaviour
{
    public static RelayManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            print("relaymanagerdes");
        }
        DontDestroyOnLoad(gameObject);
    }

 
    [SerializeField] private string environment = "production";
    [SerializeField] private int maxConnections = 4;

    public bool IsRelayEnabled => Transport != null &&
        Transport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport;
    
    public UnityTransport Transport => NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

    public async Task SetupRelay()
    {
        try
        {
            print("setting relay");
            InitializationOptions options = new InitializationOptions().SetEnvironmentName(environment);

            await UnityServices.InitializeAsync(options);

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            Allocation allocation = await Relay.Instance.CreateAllocationAsync(maxConnections);

            string joincode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            
            print($"{joincode} is the join code");
            MessageManager.messageManager.UpdateChat($"{joincode} is the join code");
            GameObject.Find("InputField").GetComponentInChildren<TextMeshProUGUI>().text = joincode;
            
            Transport.SetHostRelayData(allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes,
                allocation.Key, allocation.ConnectionData);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task JoinRelay(string joinCode)
    {
        try
        {
            print("joining relay");
            InitializationOptions options = new InitializationOptions().SetEnvironmentName(environment);

            await UnityServices.InitializeAsync(options);

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
            Transport.SetClientRelayData(allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes,
                allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);
    /*        Transport.SetHostRelayData(allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes,
                allocation.Key, allocation.HostConnectionData);*/


            MessageManager.messageManager.UpdateChat($"{OwnerClientId} has joined");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public GameObject rightPlayer;
    public GameObject leftPlayer;

    [ServerRpc(RequireOwnership = false)]
    public void spawnObjectServerRpc(ulong id)
    {
        // print("rpcspawnleft rpcrpcrpc");
        GameObject gameobject = Instantiate(leftPlayer);

        //gameobject.GetComponent<NetworkObject>().SpawnAsPlayerObject(id);
        gameobject.GetComponent<NetworkObject>().SpawnWithOwnership(id);
    }
}

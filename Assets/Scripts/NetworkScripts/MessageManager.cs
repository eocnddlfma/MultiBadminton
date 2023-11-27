using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class MessageManager : NetworkBehaviour
{
    public static MessageManager messageManager;
    public TMP_InputField inputField;
    public GameObject textChatPrefab;
    public Transform parentContent;

    
    private void Awake()
    {
        messageManager = this;
    }

    public void OnEndEditEventMethod()
    {
        SendChatMessageServerRpc(inputField.text);
        inputField.text = "";
    }

    public void UpdateChat(string text)
    {
        if(text.Equals("")) return;
        
        GameObject clone = Instantiate(textChatPrefab, parentContent);
        NetworkLog.LogInfo("the text is"+text);
        clone.GetComponent<TextMeshProUGUI>().text = $"{text}";
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && inputField.isFocused == false)
        {
            inputField.ActivateInputField();
        }
    }

    [ClientRpc]
    private void ReceiveChatMessageClientRpc(string text)
    {
        UpdateChat(text);
        NetworkLog.LogInfo("server");
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(string message)
    {
        ReceiveChatMessageClientRpc(message);
    }
}

using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region
    [Header("Photon")]
    public string gameVersion = "1.0";
    public PhotonView PV;

    [Header("Start Panel")]
    public Text StatusText;
    public InputField NickNameInput;
    public Button startButton;
    public GameObject startPanel;
    public GameObject rulePanel;

    [Header("Lobby Panel")]
    public GameObject lobbyPanel, lobbyChatView;
    public GameObject[] playerList;
    public InputField lobbyChatInput;
    public Text currentPlayerCountText, roomMasterText;
    public Button gameStartButton, backButton;

    private Text[] lobbyChatList;

    [Header("InGame Panel")]
    public InputField ChatInput;
    public GameObject chatPanel, chatView, gameOverPanel;
    public List<string> nicknameList = new List<string>();
    public bool isGameStart;
    private List<Transform> positionsList = new List<Transform>();
    private Text[] chatList;
    public Text victoryUserText;
    private int idx;
    private Button exitBtn;
    
    #endregion

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Screen.SetResolution(1280, 800, false);
    }

    private void Start()
    {
        Transform[] positions = GameObject.Find("SpawnPosition").GetComponentsInChildren<Transform>();
        foreach (Transform pos in positions)
            positionsList.Add(pos);

        chatList = chatView.GetComponentsInChildren<Text>();
        lobbyChatList = lobbyChatView.GetComponentsInChildren<Text>();
        foreach (GameObject player in playerList)
            player.SetActive(false);

        victoryUserText = gameOverPanel.GetComponent<Text>();
        isGameStart = false;
        exitBtn = GameObject.Find("Canvas").transform.Find("GameOverPanel").transform.Find("GameExitButton").gameObject.GetComponent<Button>();

        exitBtn.onClick.AddListener(GameExit);
        startButton.onClick.AddListener(JoinRoom);

        OnLogin();
    }

    void OnLogin()
    {
        PhotonNetwork.GameVersion = this.gameVersion;
        PhotonNetwork.ConnectUsingSettings();
        startButton.interactable = false;
        StatusText.text = "������ ������ ������...";
    }

    void JoinRoom()
    {
        if (NickNameInput.text.Equals(""))
            PhotonNetwork.LocalPlayer.NickName = "unknown";
        else
            PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        PhotonNetwork.JoinRandomRoom();
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        StatusText.text = "Online: ������ ������ ���� ��";
        startButton.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        

        startPanel.SetActive(false);
        lobbyPanel.SetActive(true);

        ChatClear();
        RoomRenewal();
        nicknameList.Add(PhotonNetwork.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC("SetMasterText", RpcTarget.AllBufferedViaServer, PhotonNetwork.NickName + "���� ��");
            PV.RPC("PlayerConnect", RpcTarget.AllBufferedViaServer, 0, PhotonNetwork.NickName);
            PV.RPC("ChatRPC", RpcTarget.All, "<color=blue>[����] " + PhotonNetwork.NickName + "���� �����ϼ̽��ϴ�</color>", 1);
            gameStartButton.interactable = true;
            exitBtn.interactable = true;
        }
        else
        {
            gameStartButton.interactable = false;
            exitBtn.interactable = false;
        }

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        GameManager.instance.SetCount(PhotonNetwork.CurrentRoom.PlayerCount, true);
        PV.RPC("PlayerConnect", RpcTarget.AllBufferedViaServer, 0, newPlayer.NickName);
        PV.RPC("ChatRPC", RpcTarget.All, "<color=blue>" + newPlayer.NickName + "���� �����ϼ̽��ϴ�</color>", 1);
    }
    // �÷��̾� ���� �� ä�� â ���
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        nicknameList.Remove(otherPlayer.NickName);
        RoomRenewal();
        GameManager.instance.SetCount(PhotonNetwork.CurrentRoom.PlayerCount, false);
        PV.RPC("PlayerConnect", RpcTarget.AllBufferedViaServer, 1, otherPlayer.NickName);
        if (!isGameStart)
            PV.RPC("ChatRPC", RpcTarget.All, "<color=blue>" + otherPlayer.NickName + "���� �������ϴ�</color>", 1);
        else
            PV.RPC("ChatRPC", RpcTarget.All, "<color=blue>" + otherPlayer.NickName + "���� �������ϴ�</color>", 0);

    }

    // ������ ���� ���� ��, �� ����
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        this.CreateRoom();
    }

    // �� ���� �Լ�
    void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }


    public void GameExit() => PV.RPC("GameExitRPC", RpcTarget.All);

    public int GetPlayerCount()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public void ChatClear()
    {
        ChatInput.text = "";
        lobbyChatInput.text = "";
        foreach (Text chat in chatList)
            chat.text = "";
        foreach (Text chat in lobbyChatList)
            chat.text = "";
    }

    public void RoomRenewal()
    {
        currentPlayerCountText.text = "";
        currentPlayerCountText.text = PhotonNetwork.PlayerList.Length + " / 4";
    }

    public void GameStart()
    {
        PV.RPC("CreateComputerPlayer", RpcTarget.All);
    }

    public void LeftRoom()
    {
        if (PhotonNetwork.IsMasterClient)
            PV.RPC("GameExitRPC", RpcTarget.All);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        Application.Quit();
    }

    // ä�� ���� �Լ�
    public void Send()
    {
        if (ChatInput.text.Equals(""))
            return;
        string msg = "[" + PhotonNetwork.NickName + "] " + ChatInput.text;
        PV.RPC("ChatRPC", RpcTarget.All, msg, 0);
        ChatInput.text = "";
    }

    // ä�� ���� �Լ�
    public void LobbySend()
    {
        if (lobbyChatInput.text.Equals(""))
            return;
        string msg = "[" + PhotonNetwork.NickName + "] " + lobbyChatInput.text;
        PV.RPC("ChatRPC", RpcTarget.All, msg, 1);
        lobbyChatInput.text = "";
    }
    public void DeadSend(string msg) => PV.RPC("ChatRPC", RpcTarget.All, msg, 0);

    [PunRPC]
    void SetMasterText(string masterPlayer)
    {
        roomMasterText.text = masterPlayer;
    }

    [PunRPC]
    void ChatRPC(string msg, int state)
    {
        bool isInput = false;
        if (state == 0)
        {
            for (int i = 0; i < chatList.Length; i++)
                if (chatList[i].text == "")
                {
                    isInput = true;
                    chatList[i].text = msg;
                    break;
                }
            if (!isInput)
            {
                for (int i = 1; i < chatList.Length; i++) chatList[i - 1].text = chatList[i].text;
                chatList[chatList.Length - 1].text = msg;
            }
        }
        else
        {
            for (int i = 0; i < lobbyChatList.Length; i++)
                if (lobbyChatList[i].text == "")
                {
                    isInput = true;
                    lobbyChatList[i].text = msg;
                    break;
                }
            if (!isInput)
            {
                for (int i = 1; i < lobbyChatList.Length; i++) lobbyChatList[i - 1].text = lobbyChatList[i].text;
                lobbyChatList[lobbyChatList.Length - 1].text = msg;
            }
        }
    }

    [PunRPC]
    void PlayerConnect(int state, string NickName)
    {
        if (state == 0)
        {
            foreach (GameObject player in playerList)
            {
                if (player.activeSelf == false)
                {
                    player.SetActive(true);
                    player.GetComponentInChildren<Text>().text = NickName;
                    break;
                }
            }
        }
        else
        {
            foreach (GameObject player in playerList)
            {
                if (player.GetComponentInChildren<Text>().text == NickName)
                {
                    player.GetComponentInChildren<Text>().text = "";
                    player.SetActive(false);
                }
            }
        }
    }

    [PunRPC]
    void CreateComputerPlayer()
    {
        lobbyPanel.SetActive(false);
        chatPanel.SetActive(true);
        isGameStart = true;
        idx = Random.Range(1, positionsList.Count);
        PhotonNetwork.Instantiate("Player", positionsList[idx].position, Quaternion.identity);
        positionsList.RemoveAt(idx);
        for (int i = 0; i < 10; i++)
        {
            idx = Random.Range(1, positionsList.Count);
            PhotonNetwork.Instantiate("Computer", positionsList[idx].position, Quaternion.identity);
            positionsList.RemoveAt(idx);
        }
    }

    [PunRPC]
    public void EndGame()
    {
        isGameStart = false;
        //victoryUserText.text = nicknameList[0];
        gameOverPanel.SetActive(true);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    [PunRPC]
    public void GameExitRPC()
    {
        Application.Quit();
    }
    public void Rules()
    {
        startPanel.SetActive(false);
        rulePanel.SetActive(true);
    }
    public void RulesExit()
    {
        rulePanel.SetActive(false);
        startPanel.SetActive(true);
    }
}

using Photon.Pun; //����Ƽ�� ���� ������Ʈ
using Photon.Realtime; //���� ���� ���� ���̺귯��
using UnityEngine;
using UnityEngine.UI;

//������(��ġ����ŷ) ������ �� ���� ���
public class LobbyManager : MonoBehaviourPunCallbacks {
    private string gameVersion = "1"; //���ӹ���
    
    public Text connectionInfoText; //��Ʈ��ũ ���� ǥ�� �ؽ�Ʈ
    public Button joinButton; // �� ���� ��ư
    public InputField NickNameInput;
    //public GameObject StartPanel;

  

    //���� ����� ���ÿ� ������ ���� ���� �õ�
    private void Start()
    {
        //���ӿ� �ʿ��� ����(���� ����) ����
        PhotonNetwork.GameVersion = gameVersion;
        //������ ������ ������ ���� ���� �õ�
        PhotonNetwork.ConnectUsingSettings();
        //�� ���� ��ư ��� ��Ȱ��ȭ
        joinButton.interactable = false;
        //���� �õ� ������ �ؽ�Ʈ�� ǥ��
        connectionInfoText.text = "������ ������ ���� ��...";

        
    }
    //������ ���� ���� ���� �� �ڵ� ����
    public override void OnConnectedToMaster()
    {
        //�� ���� ��ư Ȱ��ȭ
        joinButton.interactable = true;
        //���� ���� ǥ��
        connectionInfoText.text = "�¶��� : ������ ������ �����";
    }
    //������ ���� ���� ���� �� �ڵ� ����
    public override void OnDisconnected(DisconnectCause cause)
    {
        //�� ���� ��ư ��Ȱ��ȭ
        joinButton.interactable = false;
        //���� ���� ǥ��
        connectionInfoText.text = "�������� : ������ ������ ������� ����\n���� ��õ� ��...";
        //������ �������� ������ �õ�
        PhotonNetwork.ConnectUsingSettings();
    }
    //�� ���� �õ�
    public void Connect()
    {
        //�ߺ� ���� �õ��� ���� ���� ���� ��ư ��� ��Ȱ��ȭ
        joinButton.interactable = false;
        //������ ������ ���� ���̶��
        if (PhotonNetwork.IsConnected)
        {
            //�� ���� ����
            connectionInfoText.text = "�뿡 ����...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            //������ ������ ���� ���� �ƴ϶�� ������ ������ ���� �õ�
            connectionInfoText.text = "�������� : ������ ������ ������� ����\n���� ��õ� ��...";
            //������ �������� ������ �õ�
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    //(�� ���� ����) ���� �� ������ ������ ��� �ڵ� ����
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //���� ���� ǥ��
        connectionInfoText.text = "�� ���� ����, ���ο� �� ����..."; 
        //�ִ� 4���� ���� ������ �� �� ����
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }
    //�뿡 ���� �Ϸ�� ��� �ڵ� ����
    public override void OnJoinedRoom()
    {
        if (NickNameInput.text.Equals(""))
            PhotonNetwork.LocalPlayer.NickName = "unknown";
        else
            PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
       
        //���� ���� ǥ��
        connectionInfoText.text = "�� ���� ����";
        //��� �� �����ڰ� ���� ���� �ε��ϰ� ��
        PhotonNetwork.LoadLevel("Main");
        
    }


}

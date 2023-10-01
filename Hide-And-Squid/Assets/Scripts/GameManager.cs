using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ������ ���� ���� ����, ���� UI�� �����ϴ� ���� �Ŵ���
public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    /*public GameObject player;*/
    
    public AudioClip Gameover;
    // �ܺο��� �̱��� ������Ʈ�� �����ö� ����� ������Ƽ
    public static GameManager instance
    {
        get
        {
            // ���� �̱��� ������ ���� ������Ʈ�� �Ҵ���� �ʾҴٸ�
            if (m_instance == null)
            {
                // ������ GameManager ������Ʈ�� ã�� �Ҵ�
                m_instance = FindObjectOfType<GameManager>();
            }

            // �̱��� ������Ʈ�� ��ȯ
            return m_instance;
        }
    }

    private static GameManager m_instance; // �̱����� �Ҵ�� static ����

    public GameObject gameOverPanel;
    public ParticleSystem deathEffect;
  
    private GameObject countdownText, victoryUserText, networkManager;
    private Button exitBtn;

    private void Awake()
    {
        // ���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ�
        if (instance != this)
        {
            // �ڽ��� �ı�
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        networkManager = GameObject.Find("NetworkManager");
        victoryUserText = GameObject.Find("Canvas").transform.Find("GameOverPanel").transform.Find("VictoryText").gameObject;

        totalComputerCount = 20;
        totalPlayerCount = 1;
        PlayerCount = totalPlayerCount;
        ComputerCount = totalComputerCount;
    }

    public void Dead(GameObject obj)
    {
        AddScore(obj);

        Vector3 hitPoint = obj.transform.position;
        hitPoint.y += 1.0f;
        Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitPoint)), deathEffect.main.startLifetimeMultiplier);
        obj.SetActive(false);
    }

    // ������ �߰��ϰ� UI ����
    public void AddScore(GameObject who)
    {
        if (who.tag.Equals("Computer"))
            ComputerCount -= 1;
        else if (who.tag.Equals("Player"))
        {
            string deadNickname = who.GetComponent<Control>().nickname;
            string msg = "<color=red>" + deadNickname + "���� �ƿ��ƽ��ϴ�.</color>";

            networkManager.GetComponent<NetworkManager>().nicknameList.Remove(deadNickname);
            networkManager.GetComponent<NetworkManager>().DeadSend(msg);

            PlayerCount -= 1;
            if (PlayerCount == 1)
            {
                networkManager.GetComponent<NetworkManager>().PV.RPC("EndGame", RpcTarget.AllViaServer);
                GetComponent<AudioSource>().PlayOneShot(Gameover);
                /*gameOverPanel.SetActive(true);
                /*player.SetActive(false);*/
            }
        }
        // ���� UI �ؽ�Ʈ ����
        UIManager.instance.UpdateScoreText(ComputerCount, PlayerCount, totalComputerCount, totalPlayerCount);
    }

    public void SetCount(int totalPlayer, bool isEnter)
    {
        if (isEnter)
            PlayerCount += 1;
        else
            PlayerCount -= 1;
        totalPlayerCount = totalPlayer;
        UIManager.instance.UpdateScoreText(ComputerCount, PlayerCount, totalComputerCount, totalPlayerCount);
    }

    private int ComputerCount, PlayerCount, totalComputerCount, totalPlayerCount;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // ���� ������Ʈ��� ���� �κ��� �����
        if (stream.IsWriting)
        {
            // ��Ʈ��ũ�� ���� score ���� ������
            stream.SendNext(ComputerCount);
            stream.SendNext(PlayerCount);
            stream.SendNext(totalComputerCount);
            stream.SendNext(totalPlayerCount);
        }
        else
        {
            // ����Ʈ ������Ʈ��� �б� �κ��� �����
            // ��Ʈ��ũ�� ���� score �� �ޱ�
            ComputerCount = (int)stream.ReceiveNext();
            PlayerCount = (int)stream.ReceiveNext();
            totalComputerCount = (int)stream.ReceiveNext();
            totalPlayerCount = (int)stream.ReceiveNext();
            // ����ȭ�Ͽ� ���� ������ UI�� ǥ��
            UIManager.instance.UpdateScoreText(ComputerCount, PlayerCount, totalComputerCount, totalPlayerCount);
        }
    }
}
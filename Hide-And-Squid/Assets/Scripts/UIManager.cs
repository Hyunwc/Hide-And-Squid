using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //�̱��� ���ٿ� ������Ƽ
    public static UIManager instance
    {
        get
        {
            if(m_instance == null)
            {
                m_instance = FindObjectOfType<UIManager>();
            }
            return m_instance;
        }
    }
    private static UIManager m_instance; //�̱����� �Ҵ�� ����
    public Text ComputerCountText, PlayerCountText;

    private void Awake()
    {
        if (instance != this)
            Destroy(gameObject);
    }
    void Start()
    {
        
    }
    public void UpdateScoreText(int com, int play, int tcom, int tplay)
    {
        ComputerCountText.text = "Computer: " + com + " / " + tcom;
        PlayerCountText.text = "Player: " + play + " / " + tplay;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTeleport : MonoBehaviour
{
    public GameObject targetObj;

    public GameObject toObj;

    public ParticleSystem Particle;


    private void OnTriggerEnter(Collider collision) //OnTriggerEnter�Լ��� �̿��ؼ� player�� �����ϴ� ����
    {
        if (collision.CompareTag("Player"))
        {
            targetObj = collision.gameObject;

        }
    }

    private void OnTriggerStay(Collider collision) // ������������ TeleportRoutine����
    {
        if (collision.CompareTag("Player") && Input.GetKeyDown(KeyCode.UpArrow))
        {
            StartCoroutine(TeleportRoutine());
        }
    }

    IEnumerator TeleportRoutine()
    {
        yield return null;
        targetObj.GetComponent<Control>().isControl = false;
        GameObject go = Instantiate(Particle).gameObject;
        go.transform.position = targetObj.transform.position;
        yield return new WaitForSeconds(0.5f); //�÷��̾� �̵���0.5�������� ��������

        targetObj.transform.position = toObj.transform.position;

        yield return new WaitForSeconds(0.5f);

        targetObj.GetComponent<Control>().isControl = true;
    }
}

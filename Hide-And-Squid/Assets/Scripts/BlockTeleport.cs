using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTeleport : MonoBehaviour
{
    public GameObject targetObj;

    public GameObject toObj;

    public ParticleSystem Particle;


    private void OnTriggerEnter(Collider collision) //OnTriggerEnter함수를 이용해서 player가 접촉하는 순간
    {
        if (collision.CompareTag("Player"))
        {
            targetObj = collision.gameObject;

        }
    }

    private void OnTriggerStay(Collider collision) // 접촉해있으면 TeleportRoutine실행
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
        yield return new WaitForSeconds(0.5f); //플레이어 이동시0.5초정도의 딜레이줌

        targetObj.transform.position = toObj.transform.position;

        yield return new WaitForSeconds(0.5f);

        targetObj.GetComponent<Control>().isControl = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickRandomPage : MonoBehaviour
{
    public Animation anim;
    public Transform coin;
    public Transform canvas;
    public Transform[] cups;

    int m_choise;
    System.Action<bool> m_done;
    public void Init(System.Action<bool> done)
    {
        m_done = done;
        gameObject.SetActive(true);
        coin.localPosition = new Vector3(0.0f, coin.localPosition.y, coin.localPosition.z);
        canvas.gameObject.SetActive(false);
        coin.gameObject.SetActive(true);
        anim.Stop();
        anim.Play("quickrandom");

        cups[0].gameObject.SetActive(true);
        cups[1].gameObject.SetActive(true);
        cups[2].gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void OnDone()
    {
        StartCoroutine(IEOnDone());
    }
    IEnumerator IEOnDone()
    {
        m_choise = 3.Random();
        coin.position = new Vector3(cups[m_choise].position.x, coin.position.y, coin.position.z);
        yield return new WaitForSeconds(0.15f);
        canvas.gameObject.SetActive(true);
    }


    public void Selete(int index)
    {
        StartCoroutine(IESelete(index));
    }
    IEnumerator IESelete(int index)
    {
        coin.gameObject.SetActive(m_choise == index);
        iTween.ShakePosition(cups[index].gameObject, Vector3.one * 0.15f , 0.25f);
        iTween.ShakePosition(coin.gameObject, Vector3.one * 0.15f, 0.25f);

        cups[0].gameObject.SetActive(index == 0);
        cups[1].gameObject.SetActive(index == 1);
        cups[2].gameObject.SetActive(index == 2);

        anim.Stop();
        anim.Play("quickopen");
        yield return new WaitForSeconds(1.25f);

        m_done?.Invoke(m_choise == index);
        Close();

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickRandomPage : MonoBehaviour
{
    public Animation anim;
    public Transform coin;
    public Awake coinEffect;
    public Transform canvas;
    public SpriteRenderer[] cups;
    public AnimCallback animcallback;
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
        animcallback.AddAction("start", () =>
        {
            coin.gameObject.SetActive(false);
        });
        animcallback.AddAction("done", () =>
        {
            OnDone();
        });
        animcallback.AddAction("open", () =>
        {
            if(isMatch)
                Sound.Play(Sound.playlist.yeahh);
        });
        foreach (var cup in cups)
        {
            cup.gameObject.SetActive(true);
            cup.gameObject.transform.localScale = Vector3.one;
            cup.color = Color.white;
        }
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
        coin.position = new Vector3(cups[m_choise].transform.position.x, coin.position.y, coin.position.z);
        yield return new WaitForSeconds(0.15f);
        canvas.gameObject.SetActive(true);
    }


    public void Selete(int index)
    {
        StartCoroutine(IESelete(index));
    }
    bool isMatch = false;
    IEnumerator IESelete(int index)
    {
        this.isMatch = m_choise == index;
        coinEffect.gameObject.SetActive(isMatch);
        coin.gameObject.SetActive(true);
        iTween.ShakePosition(cups[index].gameObject, Vector3.one * 0.15f , 0.25f);
        iTween.ShakePosition(coin.gameObject, Vector3.one * 0.15f, 0.25f);


        cups.Length.Loop(i => {
            cups[i].gameObject.transform.localScale = Vector3.one * ((index == i) ? 1.15f : 1.0f);
            cups[i].color =  ((index == i) ? Color.white : Color.gray);
        });

        anim.Stop();
        anim.Play("quickopen");
        yield return new WaitForSeconds(1.65f);
        PetObj.Current.talking.bubble.OnEmo(isMatch? Talking.Bubble.EmoType.FeelingHappy : Talking.Bubble.EmoType.FeelingBad, 1.5f);
        if (isMatch)
        {
            PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.GoodJob);
        }

        m_done?.Invoke(isMatch);
        Close();

    }
}

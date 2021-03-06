using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MemoryPage : MonoBehaviour
{


    public SpriteRenderer box;
    public UnityEngine.UI.Button[] Btns;
    public Transform block;
    public Transform fail;
    public Transform t_next;
    System.Action<bool> m_done;



    public void Init(System.Action<bool> done)
    {
        m_done = done;
        round = 0;
        gameObject.SetActive(true);
        StartCoroutine(StartWave());
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }





    int enterIndex = 0;
    public void OnEnter(GameObject btn)
    {
        var value = btn.name.ToInt();

        if (value == indexs[enterIndex])
        {
            Debug.Log("Yes!");
            Sound.Play(Sound.playlist.journey_bitz[enterIndex]);

            Btns[value - 1].interactable = false;
            enterIndex++;
            if (enterIndex >= indexs.Count)
            {
                PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.GoodJob);
                Sound.Play(Sound.playlist.match);
                Debug.Log("Done");
                Talking.instance.petTalk.ShowHeader(Talking.PetTalk.HeaderType.goodjob);
                iTween.ShakePosition(gameObject, Vector3.one * 0.15f, 0.25f);

                if (round == maxRound)
                {
                    StartCoroutine(End(true));
                }
                else
                {
                    StartCoroutine(Wave());
                }

            }
            else 
            {
            
            }
        }
        else 
        {
            Debug.Log("No!");
            PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.Bad);
            Sound.Play(Sound.playlist.fail);

            fail.gameObject.SetActive(true);
            fail.position = Btns[value - 1].transform.position;
            //StartCoroutine(Wave());
            StartCoroutine(End(false));
        }



    }

    IEnumerator StartWave( )
    {

        OnDeactive();
        BtnActive(false);
        yield return new WaitForSeconds(0.75f);
        //foreach (var b in Btns)
        //{
        //    var colors = b.colors;
        //    colors.highlightedColor = ((Information.instance.IsMobile) ? b.colors.normalColor : Color.white);
        //    b.colors = colors;
        //}


        BtnActive(true);
        StartCoroutine(Wave());

    }

    void BtnActive(bool active)
    {
        foreach (var b in Btns)
        {
            b.gameObject.SetActive(active);
        }
    }
    void OnDeactive() 
    {
        t_next.gameObject.SetActive(false);
        fail.gameObject.SetActive(false);
        foreach (var b in Btns)
        {
            b.interactable = true;
        }
        block.gameObject.SetActive(true);
        box.color = Color.gray;
        isready = false;
    }

    public int maxRound = 4;
    bool isready = false;
    int round = 0;
    int point;
    List<int> indexs = new List<int>();
    IEnumerator Wave() 
    {
        EventSystem.current.SetSelectedGameObject(null);
        block.gameObject.SetActive(true);
        box.color = Color.gray;
        enterIndex = 0;
        isready = false;
        




        round++;
        int count = 1 + round;
        indexs = new List<int>();
        var choise = new List<int>() { 1,2,3,4,5,6,7,8 };
        count.Loop(x=> {
            var c = choise[choise.Count.Random()];
            choise.Remove(c);
            indexs.Add(c);
        });
        yield return new WaitForSeconds((round>1) ?1.5f:0.5f);

        //Next
        t_next.gameObject.SetActive(true);
        while (!isready)
        {
            yield return new WaitForEndOfFrame();
        }


        fail.gameObject.SetActive(false);
        foreach (var b in Btns)
            b.interactable = true;


        yield return new WaitForSeconds(0.5f);

        int index = 0;
        while (index < count) {

            var i = indexs[index];
            var btn = Btns[i-1];
            btn.interactable = false;
            Sound.Play(Sound.playlist.journey_bitz[index]);
            yield return new WaitForSeconds(0.5f);
            btn.interactable = true;
            yield return new WaitForSeconds(0.25f);
            index++;
        }
        EventSystem.current.SetSelectedGameObject(null);

        box.color = Color.white;
        block.gameObject.SetActive(false);

    }






    public void Next() 
    {
        t_next.gameObject.SetActive(false);
        isready = true;
    }



    IEnumerator End(bool win)
    {
        //yield return new WaitForSeconds(0.5f);
        //foreach (var b in Btns)
        //    b.interactable = true;

        block.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.25f);
        OnDeactive();
        BtnActive(false);
      

        if (win)
        {
            PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.GoodJob);
            Sound.Play(Sound.playlist.yeahh);
        }
        PetObj.Current.talking.bubble.OnEmo(win ? Talking.Bubble.EmoType.FeelingHappy : Talking.Bubble.EmoType.FeelingBad, 1.5f);

        m_done?.Invoke(win);
        Close();

    }

}

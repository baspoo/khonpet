using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPage : MonoBehaviour
{


    public SpriteRenderer box;
    public UnityEngine.UI.Button[] Btns;
    public Transform block;
    public Transform fail;
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
            Btns[value - 1].interactable = false;
            enterIndex++;
            if (enterIndex >= indexs.Count)
            {
                PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.GoodJob);
                Debug.Log("Done");
                Talking.instance.message.Show(Talking.Message.MessageType.goodjob);
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
        }
        else 
        {
            Debug.Log("No!");
            PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.Bad);

            fail.gameObject.SetActive(true);
            fail.position = Btns[value - 1].transform.position;
            //StartCoroutine(Wave());
            StartCoroutine(End(false));
        }



    }

    IEnumerator StartWave( )
    {
        OnDeactive();
        yield return new WaitForSeconds(0.75f);
        StartCoroutine(Wave());

    }


    void OnDeactive() 
    {
        foreach (var b in Btns)
            b.interactable = true;
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
        fail.gameObject.SetActive(false);
        foreach (var b in Btns)
            b.interactable = true;
        yield return new WaitForSeconds(0.5f);

        int index = 0;
        while (index < count) {

            var i = indexs[index];
            var btn = Btns[i-1];
            btn.interactable = false;
            yield return new WaitForSeconds(0.5f);
            btn.interactable = true;
            yield return new WaitForSeconds(0.25f);
            index++;
        }

        isready = true;
        box.color = Color.white;
        block.gameObject.SetActive(false);
    }

    IEnumerator End(bool win)
    {
        yield return new WaitForSeconds(0.75f);
        OnDeactive();
        m_done?.Invoke(win);
        Close();

    }

}

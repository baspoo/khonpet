using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceObj : MonoBehaviour
{

    public bool IsRuning;
    public int Index;
    public UnityEngine.UI.Image Image;
    public UnityEngine.UI.Image Arrow;




    DancePage m_page;
    DancePage.DanceData m_data;
    public void Init( DancePage page , DancePage.DanceData data )
    {
        m_data = data;
        m_page = page;
        Index = m_data.Index;
        Image.color = m_data.Color;
        Arrow.color = m_data.ArrowColor;
        Arrow.transform.localRotation = Quaternion.Euler(m_data.Rotate);
        transform.localPosition = new Vector3(m_data.EndPoint.localPosition.x, m_page.Born.localPosition.y, m_page.Born.localPosition.z);
        gameObject.SetActive(true);

        gameObject.transform.localScale = Vector3.zero;
        iTween.ScaleTo(gameObject,Vector3.one,0.3f);
        IsRuning = true;
    }

    
    void Update()
    {
        if (IsRuning) 
        { 
            transform.Translate( Vector3.down* m_page.setting.Speed * Time.deltaTime );
            // (N == 95) < (100 -  20) = 80
            if (transform.localPosition.y < (m_data.EndPoint.localPosition.y - m_page.setting.PaddingRange)) 
            {
                OnMissing();
            }
        }
    }

    public void OnMissing() 
    {
        Debug.Log("OnMissing!");
        //PetObj.Current?.anim.OnAnimForce(PetAnim.AnimState.Bad);
        Talking.instance?.message.Show(Talking.Message.MessageType.bad);
        Dispose();
    }
    public bool OnCheck() 
    {


        // (N == 95) < (100 +  20) = 120
        // (N == 95) > (100 + -20) = 80


        if (
            transform.localPosition.y < (m_data.EndPoint.localPosition.y + m_page.setting.MatchRanges[0]) &&
            transform.localPosition.y > (m_data.EndPoint.localPosition.y + m_page.setting.MatchRanges[1]))
        {
            Debug.Log("Match!");

            //PetObj.Current?.anim.OnAnimForce(PetAnim.AnimState.GoodJob);
            Talking.instance?.message.Show(Talking.Message.MessageType.goodjob);
            m_data.Eff.OnAwake();
            m_page.OnMatched();
            Dispose();
            return true;
        }
        else 
        {

            Debug.Log($"{transform.localPosition.y}  <  {m_data.EndPoint.localPosition.y + m_page.setting.MatchRanges[0]}  &&  >  {m_data.EndPoint.localPosition.y + m_page.setting.MatchRanges[1]}");
            return false;
        }
    }

    public void Dispose()
    {
        IsRuning = false;
        gameObject.SetActive(false);
    }

}

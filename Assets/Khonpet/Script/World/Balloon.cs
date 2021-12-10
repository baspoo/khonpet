using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    public bool IsTest;
    public List<NFTService.CollectionData> BalloonTests;

    public static Balloon instance { get { if (m_instance == null) m_instance = FindObjectOfType<Balloon>(); return m_instance; } }
    static Balloon m_instance;





    public Transform RootBalloon;
    public Transform PositionBalloon;
    public UnityEngine.UI.RawImage Image;
    public Awake tEffect;
    public float[] Times;
    public float[] PcRanges;
    public float[] MobileRanges;
    List<NFTService.CollectionData> m_balloons;
    bool isReady;
    public void Init()
    {
        Close();
        gameObject.SetActive(Config.Data.Balloon.Active);
        Times = Config.Data.Balloon.Duration_Sec;
        m_balloons = new List<NFTService.CollectionData>();
        foreach (var data in NFTService.instance.OtherOwner) 
        {
            if (data.ImageUrl != null) 
            {
                m_balloons.Add(data);
            }
        }
        if (IsTest)
            m_balloons = BalloonTests;
        isReady = m_balloons.Count > 0;
        Debug.Log($"Balloon : {isReady}");

        Next();
    }




    public void Open() 
    {
        tEffect.transform.position = PositionBalloon.position;
        tEffect.OnAwake();
        PopupPage.instance.balloon.Open(m_data);
        Close();
    }

    public bool IsRuning => RootBalloon.gameObject.activeSelf;
    NFTService.CollectionData m_data;
    void Avtive()
    {
        m_data = m_balloons[m_balloons.Count.Random()];
        var postions = Information.instance.IsMobile ? MobileRanges : PcRanges;

        Vector3 vec = RootBalloon.transform.localPosition;
        if (Random.Range(0, 100) > 50)
        {
            vec.x = postions[0];
        }
        else 
        {
            vec.x = postions[1];
        }
        if (!Information.instance.IsMobile) 
        {
            vec.x += Random.RandomRange(-1.0f, 1.0f);
        }

        LoaderService.instance.OnLoadImage(m_data.ImageUrl, (img) => { Image.texture = img; });
        RootBalloon.transform.localPosition = vec;
        RootBalloon.gameObject.SetActive(true);
    }
    public float runTime;
    public float nextTime;
    void Next() 
    {
        runTime = 0.0f;
        nextTime = Random.RandomRange(Times[0], Times[1]);
    }
    public void Close() 
    {
        runTime = 0.0f;
        RootBalloon.gameObject.SetActive(false);
    }
    public void AnimDone( )
    {
        Close();
    }





    void Update()
    {
        if (isReady && !IsRuning && !ConsoleActivity.IsActing) 
        {
            if (runTime < nextTime) runTime += Time.deltaTime;
            else 
            {
                Next();
                Avtive();
            }
        }
    }
}

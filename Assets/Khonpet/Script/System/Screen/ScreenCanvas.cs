using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCanvas : MonoBehaviour
{

    public static ScreenCanvas instance { get{ if (m_instance == null) m_instance = FindObjectOfType<ScreenCanvas>(); return m_instance; } }
    static ScreenCanvas m_instance;


    public Canvas Canvas;
    public UnityEngine.UI.CanvasScaler CanvasScaler;
    public float RefreshTime = 0.5f;
    List<ScreenHandle> ScreenHandles = new List<ScreenHandle>();



    private void Awake()
    {
        Refresh();
    }
    float m_time = 0.0f;
    void Update()
    {
        if (m_time > RefreshTime)
        {
            m_time = 0.0f;
            Refresh();
        }
        else m_time += Time.deltaTime;
    }



    public void AddScreenHandle(ScreenHandle sh)
    {
        if (!ScreenHandles.Contains(sh))
            ScreenHandles.Add(sh);
    }
    void Refresh() 
    {
        bool isWide = Information.instance.IsWidescreen;
        CanvasScaler.referenceResolution = (isWide) ? new Vector2(1920, 720) : new Vector2(880, 720);

        ScreenHandles.ForEach(sh => {
            if (sh!=null && sh.enabled) 
            {
                sh.Refresh(isWide);
            }
        });
    }


}

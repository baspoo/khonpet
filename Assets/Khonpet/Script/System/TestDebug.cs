using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class TestDebug : MonoBehaviour
{

    public List<GameObject> objectsShow;
    public List<GameObject> objectsHide;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.D))
            //InterfaceRoot.instance.mainmenu.starZone.OnUpdate();
        //if (Input.GetKeyDown(KeyCode.A))
           // InterfaceRoot.instance.mainmenu.starZone.OnAddStar(6.Random());

        //if (Input.GetKeyDown(KeyCode.S))
            //Playing.instance.Save();

    }
}













#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(TestDebug))]
[System.Serializable]
public class TestDebugUI : Editor
{


    TestDebug m_page => (TestDebug)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Adjuest Scene"))
        {
            m_page.objectsHide = m_page.objectsHide.FindAll(x => x != null);
            m_page.objectsShow = m_page.objectsShow.FindAll(x => x != null);

            m_page.objectsHide.ForEach(x => x.SetActive(false));
            m_page.objectsShow.ForEach(x => x.SetActive(true));
        }
        if (GUILayout.Button("Test"))
        {


            FirebaseService.instance.TopScoreVerify(Random.RandomRange(0,1000));

            //for (int d = 0; d < 100; d++)
            //    Debug.Log(Random.RandomRange(1, 4));


            //Debug.Log(100/20);
            //Debug.Log(100 / 20);
            //Debug.Log(87 / 20);
            //Debug.Log(52 / 20);
            //Debug.Log(23 / 20);
            //Debug.Log(19 / 20);
            //Debug.Log(9 / 20);
            //StatUtility.CalStat(0,0);
        }
    }
}
#endif
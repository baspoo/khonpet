using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class TestDebug : MonoBehaviour
{



    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            InterfaceRoot.instance.mainmenu.starZone.OnUpdate();
        if (Input.GetKeyDown(KeyCode.A))
            InterfaceRoot.instance.mainmenu.starZone.OnAddStar(6.Random());

        if (Input.GetKeyDown(KeyCode.S))
            Playing.instance.Save();

    }
}

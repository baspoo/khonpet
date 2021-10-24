using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

    public static World instance { get { if (m_instance == null) m_instance = FindObjectOfType<World>(); return m_instance; } }
    static World m_instance;









    public void Init()
    {
       
    }



}

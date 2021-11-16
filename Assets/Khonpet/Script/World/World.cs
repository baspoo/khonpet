using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

    public static World instance { get { if (m_instance == null) m_instance = FindObjectOfType<World>(); return m_instance; } }
    static World m_instance;





    public Transform Root;
    public Transform Background;
    public Transform PetPosition;





        public void Awake()
    {
        Root.gameObject.SetActive(false);
    }
    public void Init()
    {
        Root.gameObject.SetActive(true);
    }



}

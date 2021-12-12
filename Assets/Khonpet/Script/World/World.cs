using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

    public static World instance { get { if (m_instance == null) m_instance = FindObjectOfType<World>(); return m_instance; } }
    static World m_instance;





    public Transform Root;
    public Camera Camera;
    public Transform Background;
    public Transform PetPosition;



    public void HideWorld(bool isHide) 
    {
        //World.instance.Camera.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        Camera.transform.localRotation = Quaternion.Euler(0.0f, isHide? 180.0f : 0.0f , 0.0f);
    }



    public void Awake()
    {
        Root.gameObject.SetActive(false);
    }
    public void Init()
    {
        Root.gameObject.SetActive(true);
        Balloon.OnAwake();
    }



}

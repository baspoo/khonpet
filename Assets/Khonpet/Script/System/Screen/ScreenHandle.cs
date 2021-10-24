using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenHandle : MonoBehaviour
{

    public Handle Pc;
    public Handle Mobile;

    [System.Serializable]
    public class Handle 
    {
        public bool Enable = true;
        public Vector3 Position;
        public Vector3 Size = Vector3.one;
    }


    void Awake()
    {
        Refresh(Information.instance.IsWidescreen);
        ScreenCanvas.instance.AddScreenHandle(this);
    }
    public void Refresh(bool isWide) 
    {
        var handle = (isWide) ? Pc : Mobile;
        transform.localPosition = handle.Position;
        transform.localScale = handle.Size;
        transform.gameObject.SetActive(handle.Enable);
    }



}

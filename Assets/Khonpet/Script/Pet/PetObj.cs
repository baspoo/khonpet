using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetObj : MonoBehaviour
{



    public PetAnim anim { get { if (m_anim == null) m_anim = GetComponent<PetAnim>(); return m_anim; } }
    PetAnim m_anim;




    public PetInfo info;
    [System.Serializable]
    public class PetInfo
    {
        public string ID;
    }


    public PetBody body;
    [System.Serializable]
    public class PetBody 
    {
        public Transform root;
        public Transform head;
        public Transform glasses;
        public Transform[] eyes = new Transform[2];
        public Transform mouth;
        public Transform body;
        public Transform food;
    }



    public List<PetJoint> joints;
    [System.Serializable]
    public class PetJoint
    {
        public Transform root;
        public Vector3 position;
        public Quaternion rotate;
    }




    public PetVoice voice;
    [System.Serializable]
    public class PetVoice
    {

    }






}

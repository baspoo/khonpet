using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetObj : MonoBehaviour
{

    public static PetObj Current => m_Current;
    static PetObj m_Current;

    public PetAnim anim { get { if (m_anim == null) m_anim = GetComponent<PetAnim>(); return m_anim; } }
    PetAnim m_anim;

    public PetBehaviour behaviour => PetBehaviour.instance;

    public Talking talking => Talking.instance;

    public PetData petData => m_petData;
    PetData m_petData;

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
        public Transform talk;
        public Transform glasses;
        public Transform[] eyes = new Transform[2];
        public Transform mouth;
        public Transform bodycenter;
        public Transform foot;
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

    public List<Achievement.AchievementData> achievements;


  


    public void Init(PetData petData)
    {
        m_Current = this;
        m_petData = petData;
        behaviour.Init(this);
        OnUpdatePetObj();
    }
    public void OnUpdatePetObj()
    {
        m_Current = this;
        this.AdjuestAnimation();
        this.AdjuestCostume();
    }



}

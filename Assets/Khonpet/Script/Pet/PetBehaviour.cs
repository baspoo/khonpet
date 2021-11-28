using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetBehaviour : MonoBehaviour
{
    public static PetBehaviour instance { get { if (m_instance == null) m_instance = FindObjectOfType<PetBehaviour>(); return m_instance; } }
    static PetBehaviour m_instance;

    public bool IsReady => m_pet != null;
    PetObj m_pet;
    PetActivity.PetInspector m_inspector;
    PetBehaviourEngine m_engine;
    public void Init( PetObj pet )
    {
        m_pet = pet;
        m_inspector = pet.petData.petInspector;
        m_engine = new PetBehaviourEngine(this);
        AwakeBehaviour();
    }
    private void Update()
    {
        if (IsReady)
        {
            m_engine.Update();
            AnimState = m_pet.anim.animState;
        }
    }



    public PetAnim.AnimState AnimState;












    void AwakeBehaviour()
    {
        m_engine.AddEvent("UpdateBehaviour", 1.0f, UpdateBehaviour, false);
        m_engine.AddEvent("UpdateBoring", 1.0f, UpdateBoring, false);
    }




    void UpdateBehaviour()
    {


    }
    void UpdateBoring(  ) 
    {
        if (m_inspector.IsBoring)
        {
            if (!m_pet.talking.bubble.IsTalking || m_pet.anim.animState == PetAnim.AnimState.Idle)
            {
                m_pet.talking.bubble.OnEmo(Talking.Bubble.EmoType.Boring);
                m_pet.anim.OnAnimForce(PetAnim.AnimState.Need);
            }
        }
        else 
        {
            if (m_pet.talking.bubble.emoType == Talking.Bubble.EmoType.Boring) 
            {
                m_pet.talking.bubble.Hide();
                m_pet.anim.OnReset();
            }
        }
    }








}
























public class PetBehaviourEngine 
{
    PetBehaviour behaviour;
    public PetBehaviourEngine(PetBehaviour behaviour)  
    {
        this.behaviour = behaviour;
    }

    float time = 0.0f;
    public void Update()
    {
        if (time < 1.0f)
        {
            time += Time.deltaTime;
        }
        else
        {
            ExcuteEvents(time);
            time = 0.0f;
        }
    }
    class PetEvent
    {
        public string EventName;
        public float Remaining;
        public float Duration;
        public System.Action Action;
        public bool isOnceTime;
        public bool isTimeout => Remaining <= 0.0f;
    }
    List<PetEvent> petEvents = new List<PetEvent>();
    public void AddEvent(string EventName, float Duration, System.Action Action, bool isOnceTime = true)
    {
        petEvents.Add(new PetEvent()
        {
            EventName = EventName,
            Remaining = Duration,
            Duration = Duration,
            Action = Action,
            isOnceTime = isOnceTime,
        });
    }
    public void RemoveEvent(string EventName)
    {
        petEvents.RemoveAll(x => x.EventName == EventName);
    }
    void ExcuteEvents(float time)
    {
        List<PetEvent> remove = new List<PetEvent>();
        foreach (var petEvent in petEvents)
        {
            petEvent.Remaining -= time;
            if (petEvent.isTimeout)
            {
                if (!petEvent.isOnceTime)
                {
                    petEvent.Remaining = petEvent.Duration;
                }
                else
                {
                    remove.Add(petEvent);
                }
                petEvent.Action?.Invoke();
            }
        }
        if (remove.Count != 0)
            foreach (var rev in remove)
            {
                RemoveEvent(rev.EventName);
            }
    }






}
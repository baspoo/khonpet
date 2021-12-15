using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetBehaviour : MonoBehaviour
{
    public static PetBehaviour instance { get { if (m_instance == null) m_instance = FindObjectOfType<PetBehaviour>(); return m_instance; } }
    static PetBehaviour m_instance;
    public bool IsReady => m_pet != null;
    public PetObj pet => m_pet;
    PetObj m_pet;

    public PetActivity.PetInspector inspector => m_inspector;
    PetActivity.PetInspector m_inspector;

    public Behaviour.PetBehaviourEngine engine => m_engine;
    Behaviour.PetBehaviourEngine m_engine;
    public void Init( PetObj pet )
    {
        m_pet = pet;
        m_inspector = pet.petData.petInspector;
        m_engine = new Behaviour.PetBehaviourEngine(this);
        AwakeBehaviour();
    }
    private void Update()
    {
        if (IsReady)
        {
            m_engine.Update();
        }
    }
    void AwakeBehaviour()
    {
        m_engine.AddEvent("StayBehaviour", 1.0f, new Behaviour.StayBehaviour(this).Update, false);
        m_engine.AddEvent("UpdateBoring", 1.0f, new Behaviour.PetBoring(this).UpdateBoring, false);
    }
}

























namespace Behaviour
{

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
        public class PetEvent
        {
            public string EventName;
            public float Remaining;
            public float Duration;
            public System.Action Action;
            public bool isOnceTime;
            public bool isTimeout => Remaining <= 0.0f;


            public void Reset() {
                Remaining = Duration;
            }

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
        public PetEvent GetEvent(string EventName)
        {
            return petEvents.Find(x=>x.EventName == EventName);
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














    public class PetBoring 
    {

        PetBehaviour m_behaviour;
        public PetBoring(PetBehaviour behaviour)
        {
            m_behaviour = behaviour;
        }

        bool haveNeed = false;
        public void UpdateBoring()
        {
            haveNeed = false;


            if (m_behaviour.inspector.IsBoring && !m_behaviour.inspector.IsActing)
            {
                if (!m_behaviour.pet.talking.bubble.IsTalking || m_behaviour.pet.anim.animState == PetAnim.AnimState.Idle)
                {
                    m_behaviour.pet.talking.bubble.OnEmo(Talking.Bubble.EmoType.Boring);
                    m_behaviour.pet.anim.OnAnimForce(PetAnim.AnimState.Need);
                    haveNeed = true;
                }
            }
            else
            {
                if (m_behaviour.pet.talking.bubble.emoType == Talking.Bubble.EmoType.Boring)
                {
                    m_behaviour.pet.talking.bubble.Hide();
                    m_behaviour.pet.anim.OnReset();
                }
            }


            if (haveNeed)
                return;

            if (m_behaviour.inspector.IsHighDemandData(Pet.StatType.Hungry) && !m_behaviour.inspector.IsActing)
            {
                if (!m_behaviour.pet.talking.bubble.IsTalking || m_behaviour.pet.anim.animState == PetAnim.AnimState.Idle)
                {
                    m_behaviour.pet.talking.bubble.OnEmo(Talking.Bubble.EmoType.Food);
                    m_behaviour.pet.anim.OnAnimForce(PetAnim.AnimState.Need);
                    haveNeed = true;
                }
            }
            else
            {
                if (m_behaviour.pet.talking.bubble.emoType == Talking.Bubble.EmoType.Food)
                {
                    m_behaviour.pet.talking.bubble.Hide();
                    m_behaviour.pet.anim.OnReset();
                }
            }


            if (haveNeed)
                return;




            if (m_behaviour.inspector.IsHighDemandData(Pet.StatType.Energy) && !m_behaviour.inspector.IsActing)
            {
                if (!m_behaviour.pet.talking.bubble.IsTalking || m_behaviour.pet.anim.animState == PetAnim.AnimState.Idle)
                {
                    m_behaviour.pet.talking.bubble.OnEmo(Talking.Bubble.EmoType.LowBattery);
                    m_behaviour.pet.anim.OnAnimForce(PetAnim.AnimState.Need);
                    haveNeed = true;
                }
            }
            else
            {
                if (m_behaviour.pet.talking.bubble.emoType == Talking.Bubble.EmoType.LowBattery)
                {
                    m_behaviour.pet.talking.bubble.Hide();
                    m_behaviour.pet.anim.OnReset();
                }
            }




        }
    }





    public class StayBehaviour
    {
       
        PetBehaviour m_behaviour;
        PetBehaviourEngine.PetEvent m_stay = null;
        public StayBehaviour(PetBehaviour behaviour)
        {
            m_behaviour = behaviour;
        }

        int m_Relation;
        bool Next() 
        {
            m_Relation = PetData.PetInspector.Relationship.Relation;
            var RateData = Config.Data.Behaviours[m_Relation];
            m_stay.Duration = Random.RandomRange(RateData.TalkStayTime_Sec[0], RateData.TalkStayTime_Sec[1]);
            return RateData.Percent.IsPercent();
        }
        public void Update()
        {
            if (m_stay == null)
            {
                m_stay = m_behaviour.engine.GetEvent("StayBehaviour");
                Next();
                return;
            }
            else 
            {
                if (Talking.instance.petTalk.IsTalking) 
                {
                    //reset
                    m_stay.Reset();
                }

                if (Next()) 
                {
                    Conversation.PetBehaviour(m_Relation);
                }
            }
        }
    }














}
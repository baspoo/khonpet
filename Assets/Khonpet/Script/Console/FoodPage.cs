using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPage : MonoBehaviour
{
    public static FoodPage instance { get { if (m_instance == null) m_instance = FindObjectOfType<FoodPage>(); return m_instance; } }
    static FoodPage m_instance;



    [System.Serializable]
    public class FoodSpin 
    {
        public Food.FoodType Food;
        public Vector3 Rotate;
        public UnityEngine.UI.Image Icon;
    }
    public List<FoodSpin> FoodSpins;
    public Transform Root;
    public Transform tLock;
    public Animation animSpin;
    public AnimCallback tFoodAnim;
    public SpriteRenderer sprFoodAnim;
    public Transform[] tDiraction;

    void OnEnable()
    {
        //OnPlay((r)=> {
        //    Debug.Log(r);
        //});
    }

    public void Init()
    {
        m_instance = this;
        OnClose();
    }



    bool isDone = false;
    Coroutine corotine;
    public void OnPlay(System.Action<Food.FoodType> callback)
    {
        isDone = false;
        Root.gameObject.SetActive(true);
        if (corotine != null)
            StopCoroutine(corotine);
        corotine = StartCoroutine(Play(callback));
    }
    IEnumerator Play(System.Action<Food.FoodType> callback) 
    {

        //** Pet Face Diraction
        foreach (var t in tDiraction) 
        {
            PetObj.Current.ChangeDiraction(t);
        }

        //**Clean
        FoodSpins.ForEach(x => { x.Icon.transform.localScale = Vector3.one; });

        //** Random Food
        var index = 8.Random();
        Food.FoodType Food = (Food.FoodType)index;
        var Spin = FoodSpins.Find(x=>x.Food == Food);
        tLock.localRotation = Quaternion.Euler(Spin.Rotate);
        tFoodAnim.gameObject.SetActive(false);
        while (!isDone)
        yield return new WaitForEndOfFrame();

        //** Done Spin
        yield return new WaitForSeconds(0.25f);
        FoodSpins.ForEach(x => {
            iTween.ScaleTo(x.Icon.gameObject, Vector3.zero, 0.25f);
        });
        iTween.ShakeScale(Spin.Icon.gameObject, Vector3.one * 0.15f, 0.25f);

        //** Close Spin
        yield return new WaitForSeconds(0.5f);
        OnClose();

        bool isAnimDone = false;

        //** Food Sprite Start
        tFoodAnim.ClearAction();
        tFoodAnim.AddAction("start",()=> {
    
        });
        tFoodAnim.AddAction("eat", () => {
            PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.Eat);
            Talking.instance.bubble.OnEmo(Talking.Bubble.EmoType.Eating);
        });
        tFoodAnim.AddAction("end", () => {
            PetObj.Current.talking.bubble.Hide();
            tFoodAnim.gameObject.SetActive(false);
            isAnimDone = true;
        });

        //** Pet Eatting
        tFoodAnim.gameObject.SetActive(true);
        sprFoodAnim.sprite = Spin.Icon.sprite;

        //** wait
        while(!isAnimDone) yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.35f);

        //** done
        if (PetData.PetInspector.IsNeedFood)
        {
            var feeling = PetData.Current.Foods[Food];
            if (feeling == Feeling.FeelingType.Super) PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.LikeLove);
            else if (feeling == Feeling.FeelingType.Bad) PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.Bad);
            else PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.GoodJob);
            //PetObj.Current.talking.bubble.OnEmo(Store.instance.FindFeeling(feeling).Icon, 1.5f, 2.0f);
            PetObj.Current.talking.bubble.OnEmo( (Talking.Bubble.EmoType)feeling, 2.0f);
        }
        else 
        {
            //** full
            PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.Bad);
        }

       


        //** End
        callback?.Invoke(Food);
        OnClose();
    }
    public void OnDone( )
    {
        isDone = true;
    }
    public void OnClose()
    {
        Root.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MainmenuPage : MonoBehaviour
{
    public static MainmenuPage instance { get { if (m_instance == null) m_instance = FindObjectOfType<MainmenuPage>(); return m_instance; } }
    static MainmenuPage m_instance;

    public Main main;
    [System.Serializable]
    public class Main
    {
        public Transform tRoot;
        public Animation animation;
        public void OnVisible(bool visible)
        {
            tRoot.gameObject.SetActive(visible);
        }
        bool active = true;
        public void OnActive(bool active)
        {
            if(this.active != active) 
            { 
                this.active = active;
                animation.Play(active? "mainmenu awake": "mainmenu out");
            }
        }
    }


    public OwnerZone ownerZone;
    [System.Serializable]
    public class OwnerZone 
    {
        public Transform tRoot;
        public RawImage imgProfile;
        public Text txtName;
        public Text txtAddress;
        public Btn btnGoto;
        public void OnUpdate()
        {
            LoaderService.instance.OnLoadImage(NFTService.instance.Preset.OwnerProfileImgUrl, (img) => { imgProfile.texture = img; });
            txtName.text = $"OWNER : {NFTService.instance.Preset.OwnerName}";
            txtAddress.text = $"{NFTService.instance.Preset.OwnerAddress.Substring(0,20)}....";
            btnGoto.name = $"https://opensea.io/{NFTService.instance.Preset.OwnerAddress}";
        }
    }


    public AirZone airZone;
    [System.Serializable]
    public class AirZone
    {
        public Transform tRoot;
        public Image imgAirIcon;
        public Text txtAirType;
        public Text txtAirValue;
        public void OnUpdate()
        {
            var air = AirActivity.GetAirData();
            imgAirIcon.sprite = air.icon;
            txtAirType.text = air.airName;
            txtAirValue.text = air.getTemperature.ToString();
        }
    }

    public LikeZone likeZone;
    [System.Serializable]
    public class LikeZone
    {
        public Transform tRoot;
        public Btn btnLike;
        public Btn btnHome;
        public Text txtLikeCount;
        public void OnUpdate()
        {
            txtLikeCount.text = PetData.Current.Like.KiloFormat();
        }
    }

    public MessageZone messageZone;
    [System.Serializable]
    public class MessageZone
    {
        public Transform tRoot;
    }

    public SocialZone socialZone;
    [System.Serializable]
    public class SocialZone
    {
        public Transform tRoot;
        public Btn btnFacebook;
        public Btn btnOpenSea;
        public void OnUpdate()
        {
            btnFacebook.name = Setting.instance.link.facebook;
            btnOpenSea.name = Setting.instance.link.opensea;
        }
    }


    public SubmenuZone submenuZone;
    [System.Serializable]
    public class SubmenuZone
    {
        public Transform tRoot;
        public Btn btnPetInfo;
        public Btn btnAchievement;
        public Btn btnSetting;
    }



    public ConsoleZone consoleZone;
    [System.Serializable]
    public class ConsoleZone
    {
        public Transform tRoot;
        public Btn btnFood;
        public Btn btnPlay;
        public Btn btnClean;
        public Btn btnSleep;
    }


    public StarZone starZone;
    [System.Serializable]
    public class StarZone
    {
        public Transform tRoot;
        public Transform tPath;
        public Transform tStarIcon;
        public Transform tFocus;
        public SplineWalker[] walkStars;
        public Text txtExp;
        public Text txtLv;
        public Image imgExpPilot;
        public Image imgExpRun;
        public Awake effect;
        public float SpeedBar;
        BezierCurve[] paths;
        Vector2 sizeDelta = Vector2.zero;
        public void Init()
        {
            sizeDelta = imgExpPilot.rectTransform.sizeDelta;
            OnUpdate();
        }
        void Resetposition() 
        {
            tFocus.position = tStarIcon.position;
            paths = tPath.GetComponents<BezierCurve>();
            foreach (var p in paths)
                p.points[p.points.Length - 1] = tFocus.localPosition;
        }
        public void OnUpdate()
        {
            if (corotine!=null) InterfaceRoot.instance.mainmenu.StopCoroutine(corotine);
            corotine = InterfaceRoot.instance.mainmenu.StartCoroutine(update());
        }
        bool first = true;
        float current = 0.0f;
        Coroutine corotine;
        IEnumerator update() 
        {
            var level = PetData.Current.Lv;
            var width = sizeDelta.x * level.Percent;
            txtLv.text = $"Lv.{level.CurrentLevel}";
            txtExp.text = $"{level.XP} / {level.xpNextlevel}";
            imgExpPilot.rectTransform.sizeDelta = new Vector2(width, sizeDelta.y);
            if (!first)
            {
                yield return new WaitForEndOfFrame();
                while (current < width)
                {
                    current += Time.deltaTime * SpeedBar;
                    imgExpRun.rectTransform.sizeDelta = new Vector2(current, sizeDelta.y);
                    yield return new WaitForEndOfFrame();
                }
            }
            current = width;
            imgExpRun.rectTransform.sizeDelta = new Vector2(width, sizeDelta.y);
            first = true;
        }
        public void OnAddStar(int star)
        {
            if (corotine != null) InterfaceRoot.instance.mainmenu.StopCoroutine(corotine);
            corotine = InterfaceRoot.instance.mainmenu.StartCoroutine(addStar(star));
        }
        IEnumerator addStar(int star)
        {
            Resetposition();
            foreach (var w in walkStars)
            {
                w.gameObject.SetActive(false);
            }
            int index = 0;
            InterfaceRoot.instance.mainmenu.StartCoroutine(update());
            foreach (var w in walkStars) 
            {
                yield return new WaitForEndOfFrame();
                if (index < star) 
                {
                    w.curve = paths[paths.Length.Random()];
                    w.progress = 0.0f;
                    w.gameObject.SetActive(true);
                    w.ondone = (walk) => { 
                        effect.OnAwake();
                        tStarIcon.gameObject.transform.localScale = Vector3.one;
                        iTween.ShakeScale(tStarIcon.gameObject, Vector3.one * 0.65f, 0.35f);
                    };
                    index++;
                    yield return new WaitForSeconds(0.15f);
                }
            }
            yield return new WaitForSeconds(2);
            foreach (var w in walkStars)
            {
                w.gameObject.SetActive(false);
            }
        }
    }
















    private void Update()
    {

    }







    private void Awake()
    {
        main.OnVisible(false);
    }
    public void Init()
    {
        m_instance = this;
        main.OnVisible(true);

        likeZone.OnUpdate();
        ownerZone.OnUpdate();
        socialZone.OnUpdate();
        airZone.OnUpdate();
        ownerZone.OnUpdate();
        starZone.Init();
    }

    public void OnPetting()
    {
        //ConsoleActivity.OnBegin(ConsoleActivity.Activity.Food);
        if(!ConsoleActivity.IsActing)
            PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.Petting);
    }

    public void OnConsoleFood()
    {
        ConsoleActivity.OnBegin(ConsoleActivity.Activity.Food);
    }
    public void OnConsolePlay()
    {
        ConsoleActivity.OnBegin(ConsoleActivity.Activity.Play);
    }
    public void OnConsoleClean()
    {
        ConsoleActivity.OnBegin(ConsoleActivity.Activity.Clean);
    }
    public void OnConsoleSleep()
    {
        ConsoleActivity.OnBegin(ConsoleActivity.Activity.Sleep);
    }




    public void OnPetInfo( )
    {
        PopupPage.instance.petInfo.Open();
    }
    public void OnAchievement()
    {

    }
    public void OnSetting()
    {

    }
    public void OnLike()
    {

    }
    public void OnHome()
    {

    }
    public void OnMesaage()
    {

    }



    public void OnQuest()
    {

    }
    public void OnStatus()
    {

    }






}

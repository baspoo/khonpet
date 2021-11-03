using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MainmenuPage : MonoBehaviour
{

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
        public SplineWalker[] walkStars;
        public Text txtExp;
        public Text txtLv;
        public Image imgExpPilot;
        public Image imgExpRun;
        public Awake effect;
        public float SpeedBar;
        public void OnUpdate()
        {
            if(corotine!=null) InterfaceRoot.instance.mainmenu.StopCoroutine(corotine);
            corotine = InterfaceRoot.instance.mainmenu.StartCoroutine(update());
        }
        bool first = true;
        float current = 0.0f;
        Coroutine corotine;
        IEnumerator update() 
        {
            var level = new Utility.Level(PetData.Current.Star);
            txtLv.text = $"Lv.{level.CurrentLevel}";
            txtExp.text = $"{level.XP} / {level.xpNextlevel}";
            imgExpPilot.fillAmount = level.Percent;
            if (!first)
            {
                yield return new WaitForEndOfFrame();
                while (current < imgExpPilot.fillAmount)
                {
                    current += Time.deltaTime * SpeedBar;
                    imgExpRun.fillAmount = current;
                    yield return new WaitForEndOfFrame();
                }
            }
            current = imgExpPilot.fillAmount;
            imgExpRun.fillAmount = current;
            first = true;
        }
        public void OnAddStar(int star)
        {
            if (corotine != null) InterfaceRoot.instance.mainmenu.StopCoroutine(corotine);
            corotine = InterfaceRoot.instance.mainmenu.StartCoroutine(addStar(star));
        }
        IEnumerator addStar(int star)
        {
            foreach (var w in walkStars)
            {
                w.gameObject.SetActive(false);
            }
            int index = 0;
            var paths = tPath.GetComponents<BezierCurve>();
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
        main.OnVisible(true);

        likeZone.OnUpdate();
        ownerZone.OnUpdate();
        socialZone.OnUpdate();
        airZone.OnUpdate();
        ownerZone.OnUpdate();
        starZone.OnUpdate();
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











}

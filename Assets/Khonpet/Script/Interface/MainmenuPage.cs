using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

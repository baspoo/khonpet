using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class PopupPage : MonoBehaviour
{

    [Header("Master")]
    public Transform root;
    public Transform popup;
    [Header("Background")]
    public Transform bg_center;
    public Transform bg_side;
    public static PopupPage instance { get { if (m_instance == null) m_instance = FindObjectOfType<PopupPage>(); return m_instance; } }
    static PopupPage m_instance;



    enum BgStyle { center,side }

    public void Init()
    {
        m_instance = this;
        OnClose();
    }


    bool ismarkclose = true;
    void Init(Transform root , System.Action<string> btn,bool ismarkclose = true , Transform position = null)
    {
        this.ismarkclose = ismarkclose;
        this.btns = btn;
        this.root.gameObject.SetActive(true);
        root.gameObject.SetActive(true);
        if (position != null)
            this.popup.gameObject.transform.position = position.position;
    }
    void Bg(BgStyle style)
    {
        bg_center.gameObject.SetActive(style == BgStyle.center);
        bg_side.gameObject.SetActive(style == BgStyle.side);
    }
    public void OnMarkClose()
    {
       if(ismarkclose)
            OnClose();
    }
    public void OnClose()
    {
        root.gameObject.SetActive(false);
        petInfo.root.gameObject.SetActive(false);
    }

    System.Action<string> btns;
    public void OnBtn(string state)
    {
        btns?.Invoke(state);
    }



















    [Header("Contents")]
    public MessagePage message;
    [System.Serializable]
    public class MessagePage
    {
        public Transform root;
        public Text txtHeader;
        public Text txtDescription;
        public Transform tClose;
        public MessagePage Open(string header , string message , System.Action onclose = null)
        {
            instance.Bg( BgStyle.center );
            txtHeader.text = header;
            txtDescription.text = message;
            tClose.gameObject.SetActive(true);
            instance.Init(root, (state) => {
                if (state == "close") 
                {
                    instance.OnClose(); onclose?.Invoke();
                }
            }, false);
            return this;
        }
        public void HideBtnClose( ) => tClose.gameObject.SetActive(false);
    }






    public PetInfo petInfo;
    [System.Serializable]
    public class PetInfo 
    {
        public Transform root;
        public Transform position;
        public RawImage imgProfile;
        public Text txtName;
        public Text txtAddress;
        public Text txtDescription;

        public List<Food> foods;
        [System.Serializable]
        public class Food {
            public Image imgFood;
            public Image imgEmo;
        }
        public void Open() 
        {
            instance.Bg(BgStyle.center);
            LoaderService.instance.OnLoadImage(NFTService.instance.Preset.PetImageUrl, (img) => { imgProfile.texture = img; });
            txtName.text = $"{PetData.Current.Name}";
            txtDescription.text = $"{PetData.Current.Description}";
            txtAddress.text = $"{NFTService.instance.Preset.ContractAddress.Substring(0, 20)}....";
            var gotoUrl = $"{"https://"}opensea.io/assets/{NFTService.instance.Preset.ContractAddress}/{NFTService.instance.Preset.Token}";

            //Food Feeling
            int indexFood = 0;
            foreach (var food in PetData.Current.Foods.OrderBy(x=>x.Value)) 
            {
                var foodData = Store.instance.FindFood(food.Key);
                var feelData = Store.instance.FindFeeling(food.Value);
                foods[indexFood].imgFood.sprite = foodData.Icon;
                foods[indexFood].imgEmo.sprite = feelData.Icon;
                indexFood++;
            }

            instance.Init(root,(state)=> {
                if (state == "gotourl") 
                {
                    Debug.Log(gotoUrl);
                    Application.OpenURL(gotoUrl);
                } 
            },true, position );
        }
    }








}

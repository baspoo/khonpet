using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class PopupPage : MonoBehaviour
{
    public Transform root;
    public Transform popup;
    public static PopupPage instance { get { if (m_instance == null) m_instance = FindObjectOfType<PopupPage>(); return m_instance; } }
    static PopupPage m_instance;

    bool ismarkclose = true;
    public void Init(Transform root , System.Action<string> btn,bool ismarkclose = true , Transform position = null)
    {
        this.ismarkclose = ismarkclose;
        this.btns = btn;
        this.root.gameObject.SetActive(true);
        root.gameObject.SetActive(true);
        if (position != null)
            this.popup.gameObject.transform.position = position.position;
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
            LoaderService.instance.OnLoadImage(NFTService.instance.Preset.PetImageUrl, (img) => { imgProfile.texture = img; });
            txtName.text = $"{PetData.Current.Name}";
            txtDescription.text = $"{PetData.Current.Description}";
            txtAddress.text = $"{NFTService.instance.Preset.ContractAddress.Substring(0, 20)}....";
            var gotoUrl = $"https://opensea.io/assets/{NFTService.instance.Preset.ContractAddress}/{NFTService.instance.Preset.Token}";

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
                if (state == "gotourl") Application.OpenURL(gotoUrl);
            },true, position );
        }
    }








}

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
    public Transform bg_hight;
    public static PopupPage instance { get { if (m_instance == null) m_instance = FindObjectOfType<PopupPage>(); return m_instance; } }
    static PopupPage m_instance;



    enum BgStyle { center,side,hight }

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
        else
            this.popup.gameObject.transform.position = Vector3.zero;
    }
    void Bg(BgStyle style)
    {
        bg_center.gameObject.SetActive(style == BgStyle.center);
        bg_side.gameObject.SetActive(style == BgStyle.side);
        bg_hight.gameObject.SetActive(style == BgStyle.hight);
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
        achievement.root.gameObject.SetActive(false);
        message.root.gameObject.SetActive(false);
        questPage.root.gameObject.SetActive(false);
        statusPage.root.gameObject.SetActive(false);
        displayName.root.gameObject.SetActive(false);
        airPage.root.gameObject.SetActive(false);
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
        public Transform position;
        public Text txtHeader;
        public Text txtDescription;
        public Transform tClose;
        public MessagePage Open(string header , string message , bool ismarkclose = true, System.Action onclose = null)
        {
            instance.Bg( BgStyle.center );
            txtHeader.text = header;
            txtDescription.text = message;
            tClose.gameObject.SetActive(true);
            instance.Init(root, (state) => {
                if (state == "close") 
                {
                    instance.OnClose(); 
                    onclose?.Invoke();
                }
            }, ismarkclose, position);
            return this;
        }
        public void HideBtnClose( ) => tClose.gameObject.SetActive(false);
    }



    public DisplayName displayName;
    [System.Serializable]
    public class DisplayName
    {
        public Transform root;
        public InputField input;
        public void Open(System.Action<string> onDone = null)
        {
            instance.Bg(BgStyle.center);
            input.text = string.Empty;
            instance.Init(root, (state) => {
                if (state == "enter")
                {
                    var diaplyName = input.text;
                    if (!string.IsNullOrEmpty(diaplyName)) 
                    {
                        instance.OnClose();
                        onDone?.Invoke(diaplyName);
                    }
                }
            }, false);
        }
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
                    Utility.Web.GotoUrl(gotoUrl);
                } 
            },true, position );
        }
    }





    
    public Achievement achievement;
    [System.Serializable]
    public class Achievement
    {
        public Transform root;
        public Transform position;
        public List<BarObject> barObjects;
        public Sprite icon_anim;
        public Sprite icon_sound;
        public Color cc_disableText;
        public Color cc_disableStar;
        public void Open()
        {
            instance.Bg(BgStyle.hight);
            instance.Init(root, (state) => { }, true, position);


            int index = 0;
            int lv = PetData.PetInspector.Lv.CurrentLevel;
            barObjects.ForEach(b => {
                b.gameObject.SetActive(false);

                if (index < PetObj.Current.achievements.Count) 
                {
                    b.gameObject.SetActive(true);
                    var ach = PetObj.Current.achievements[index];
                    b.Name.text = ach.Name;
                    b.Count.text = $"Level {ach.Level}";
                    b.Other.gameObject.SetActive(false);
                    b.Img.gameObject.SetActive(false);



                    if (ach.RewardType == global::Achievement.RewardType.Animation)
                    {
                        b.Icon.sprite = icon_anim;
                        if (ach.IsUnlock(lv))
                        {
                            b.onselete = (str) => {
                                PetObj.Current.anim.OnAnimForce(ach.Clip);
                            };
                        }
                    }
                    else 
                    {
                        b.Icon.sprite = ach.Icon;
                        if (ach.IsUnlock(lv)) 
                        { 
                            b.Other.gameObject.SetActive(true);
                            b.Other.InitToggle(null, PetData.PetInspector.IsEquip(ach.Index));
                            b.onselete = (str) => {
                                var toggle = b.Other.OnToggle();
                                PetData.PetInspector.OnEquip(ach.Index, toggle);
                            };
                        }
                    }


    


                    if (ach.IsUnlock(lv))
                    {
                        b.Btn.interactable = true;
                        b.Name.color = Color.white;
                        b.Count.color = Color.white;
                        b.Bar.color = Color.white;
                    }
                    else 
                    {
                        b.Btn.interactable = false;
                        b.Name.color = cc_disableText;
                        b.Count.color = cc_disableText;
                        b.Bar.color = cc_disableStar;
                        b.Img.gameObject.SetActive(true);
                    }




                }
                index++;
            });
        }
    }



    public AirPage airPage;
    [System.Serializable]
    public class AirPage
    {
        public Transform root;
        public Transform position;
        public List<BarObject> barObjects;

        public void Open()
        {
            instance.Bg(BgStyle.center);
            instance.Init(root, (state) => {   }, true, position);

            int index = 0;
            barObjects.ForEach(b => {


                var air = Store.instance.AirDatas[index];
                b.Icon.sprite = air.icon;
                b.Name.text = air.airName;
                b.Count.text = $"{air.temperature[0]}-{air.temperature[1]}c";


                bool isCurrent = AirActivity.GetAirData().airType == air.airType;
                if (isCurrent)
                {
                    b.Icon.color = Color.white;
                    b.Name.color = Color.white;
                    b.Count.color = Color.white;
                    b.Bar.color = b.Colors[0];
                }
                else 
                {
                    b.Icon.color = b.Colors[2];
                    b.Name.color = b.Colors[2];
                    b.Count.color = b.Colors[2];
                    b.Bar.color = b.Colors[1];
                }


                index++;
            });
        }
    }







    public QuestPage questPage;
    [System.Serializable]
    public class QuestPage
    {
        public Transform root;
        public Transform position;
        public List<BarObject> barObjects;
        public void Open()
        {
            instance.Bg(BgStyle.side);
            instance.Init(root, (state) => { }, true, position);

           
            barObjects.ForEach(b => {

                var quest = PetData.PetInspector.Quests[b.Value];
                var questData = Store.instance.FindQuest(quest.QuestID);
                var values = quest.GetProgress();
                var done = quest.IsDone();

                b.Text = $"done{done}  / IsClaimed{quest.IsClaimed}";

                b.Name.text = questData.Description;
                b.Icon.sprite = questData.Icon;
                b.Count.text = $"{values[0]}/{values[1]}";
                b.Bar.fillAmount = quest.GetPercent();
                b.Img.enabled = quest.IsClaimed;
                b.Btn.interactable = done && !quest.IsClaimed;
                b.onselete = (str) => {
                    quest.OnClaim();
                    b.Btn.interactable = false;
                    b.Img.enabled = true;
                };

            });
        }
    }

    public StatusPage statusPage;
    [System.Serializable]
    public class StatusPage
    {
        public Transform root;
        public Transform position;
        public List<BarObject> barObjects;
        public Transform statTab;
        public Transform descriptionTab;
        public UnityEngine.UI.Text textHeader;
        public UnityEngine.UI.Text textDiscription;
        public void Open()
        {


            void main() 
            {
                textHeader.text = "Status";
                statTab.gameObject.SetActive(true);
                descriptionTab.gameObject.SetActive(false);
            }
            void description(Pet.StatType stat) 
            {

                var description = Store.instance.FindStat(stat).description;

                textHeader.text = stat.ToString();
                textDiscription.text = description;
                statTab.gameObject.SetActive(false);
                descriptionTab.gameObject.SetActive(true);
            }


            main();
            instance.Bg(BgStyle.side);
            instance.Init(root, (state) => {
                if (state == "back") 
                {
                    main();
                }
            }, true, position);
            barObjects.ForEach(b => {

                var stat = (Pet.StatType)b.Value;
                var val = PetData.PetInspector.GetStat(stat);
                b.Bar.fillAmount = val / 100.0f;
                b.Bar.color = b.Colors[PetData.PetInspector.IsStatFull(stat)?1:0];
                b.onselete = (str) => {
                    description(stat);
                };
            });
        }
    }



}

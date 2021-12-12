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
    public Transform bg_hight;
    public static PopupPage instance { get { if (m_instance == null) m_instance = FindObjectOfType<PopupPage>(); return m_instance; } }
    static PopupPage m_instance;



    enum BgStyle { none,center,hight }

    public void Init()
    {
        m_instance = this;
        OnClose();
    }


    bool ismarkclose = true;
    void Init(Transform root , System.Action<string> btn,bool ismarkclose = true , Transform position = null)
    {
        Sound.Play(Sound.playlist.openpage);
        OnClose();
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
        settingPage.root.gameObject.SetActive(false);
        message.root.gameObject.SetActive(false);
        questPage.root.gameObject.SetActive(false);
        statusPage.root.gameObject.SetActive(false);
        journey.root.gameObject.SetActive(false);
        displayName.root.gameObject.SetActive(false);
        airPage.root.gameObject.SetActive(false);
        balloon.root.gameObject.SetActive(false);
        map.root.gameObject.SetActive(false);
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
        public MessagePage Open(string header , string message , bool ismarkclose = true, bool forcetoCenter = false,System.Action onclose = null)
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
            }, ismarkclose, (forcetoCenter) ? null : position);
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

            if (Information.instance.IsMobile) 
            {
                HtmlCallback.PopupInputMessage("Input Display Name", (name) => {
                    if (!string.IsNullOrEmpty(name))
                    {
                        Playing.instance.UpdateDisplayName(name);
                        instance.OnClose();
                        onDone?.Invoke(name);
                    }
                    else 
                    {
                        Open(onDone);
                    }
                });
                instance.OnClose();
                return;
            }


            instance.Bg(BgStyle.center);
            input.text = string.Empty;
            instance.Init(root, (state) => {
                if (state == "enter")
                {
                    var diaplayName = input.text;
                    if (!string.IsNullOrEmpty(diaplayName)) 
                    {
                        Playing.instance.UpdateDisplayName(diaplayName);
                        instance.OnClose();
                        onDone?.Invoke(diaplayName);
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
            imgProfile.texture = PetObj.Current.info.Thumbnail;
            //LoaderService.instance.OnLoadImage(NFTService.instance.Preset.PetImageUrl, (img) => { imgProfile.texture = img; });
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




    public SettingPage settingPage;
    [System.Serializable]
    public class SettingPage
    {
        public Transform root;
        public Transform position;
        public Text txt_name;
        public Text txt_star;
        public Text txt_user;
        public Text txt_pin;
        public Btn toggle_bgm;
        public Btn toggle_sfx;

        public UnityEngine.UI.Button btn_en;
        public UnityEngine.UI.Button btn_th;


        public void Open()
        {
            instance.Bg(BgStyle.center);


            txt_name.text = Playing.instance.playingData.NickName;
            txt_user.text = $"UserID : {Playing.instance.playingData.UserID}";
            txt_pin.text = $"PIN : {Playing.instance.playingData.PIN}";
            txt_star.text = $"Give {Playing.instance.playingData.StarPoint} Star";

            void updatelanguage()
            {
                var language = Playing.instance.playingData.Language;
                btn_en.interactable = language == 1;
                btn_th.interactable = language == 0;
            }
            updatelanguage();

            toggle_bgm.InitToggle((t) => {
                Playing.instance.Sound(t, null);
                Sound.Init();
            }, Playing.instance.playingData.IsBgm);

            toggle_sfx.InitToggle((t) => {
                Playing.instance.Sound(null, t);
            }, Playing.instance.playingData.IsSfx);


            instance.Init(root, (state) => {
                if (state == "rename")
                {
                    instance.OnClose();
                    instance.displayName.Open();
                }
                if (state == "copy")
                {
                    Playing.instance.playingData.UserID.Copy();
                }
                if (state == "en")
                {
                    Playing.instance.ChangeLanguage((Language.LanguageType)0);
                    updatelanguage();
                }
                if (state == "th")
                {
                    Playing.instance.ChangeLanguage((Language.LanguageType)1);
                    updatelanguage();
                }
            }, true, position);
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

    public BalloonPage balloon;
    [System.Serializable]
    public class BalloonPage
    {
        public Transform root;
        public Transform position;
        public RawImage image;
        public Image bar;
        public Text name;
        public Text address;
        public Button btn;
        public Button btngoto;

        public void Open(NFTService.CollectionData data)
        {
            Playing.instance.AddBalloon(1);


            //var postions = Information.instance.IsMobile ? MobileRanges : PcRanges;
            var pos =  position.position;
            pos.x = Information.instance.IsMobile ? 0.0f : Balloon.instance.PositionBalloon.transform.position.x;
            position.position = pos;

            bar.fillAmount = (float)Playing.instance.playingData.BalloonPoint / (float)Pet.Static.MaxBalloon;
            name.text = data.Name;
            address.text = $"{data.ContractAddress.Substring(0, 20)}....";
            btn.interactable = bar.fillAmount >= 1.0f;
            LoaderService.instance.OnLoadImage(data.ImageUrl, (img) => { image.texture = img; });

            instance.Bg(BgStyle.center);
            instance.Init(root, (state) => {
                if (state == "claim") 
                {
                    Playing.instance.ResetBalloon();
                    PetData.PetInspector.AddStar(3);
                    instance.OnClose();
                }
                if (state == "goto")
                {
                    var gotoUrl = $"{"https://"}opensea.io/assets/{data.ContractAddress}/{data.Token}";
                    Debug.Log(gotoUrl);
                    Utility.Web.GotoUrl(gotoUrl);

                }
            }, true, position);
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
            instance.Bg(BgStyle.center);
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
            instance.Bg(BgStyle.center);
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


                if (stat == Pet.StatType.Relationship) 
                {
                    var reletion = PetData.PetInspector.Relationship;
                    b.Name.text = reletion.Name;
                    b.Icon.sprite = reletion.Icon;
                }



            });
        }
    }




    public JourneyPopup journey;
    [System.Serializable]
    public class JourneyPopup
    {
        public Transform root;
        public Transform position;
        public List<BarObject> bars;
        public Transform scoreTab;
        public Transform descriptionTab;
        public Text highscore;
        public Text dailyscore;
        public UnityEngine.UI.Text textDiscription;
        public Scrollbar scrollbar;
        public void Open()
        {
            instance.Bg(BgStyle.hight);


            highscore.text = $"High Score : {PetData.PetInspector.Journey.HighScore}";
            dailyscore.text = $"Daily Score : {PetData.PetInspector.Journey.DailyScore}";

            void main()
            {
                scoreTab.gameObject.SetActive(true);
                descriptionTab.gameObject.SetActive(false);
            }
            void description( )
            {
                textDiscription.text = Language.Get("journey_description");
                scoreTab.gameObject.SetActive(false);
                descriptionTab.gameObject.SetActive(true);
            }
            main();

            instance.Init(root, (state) => {
                if (state == "letgo")
                {
                    ConsoleActivity.OnBegin(ConsoleActivity.Activity.Journey);
                    instance.OnClose();
                }
                if (state == "what")
                {
                    description();
                }
                if (state == "back")
                {
                    main();
                }
            }, true, position);


            bars.ForEach(x => {
                x.Name.text = "------------";
                x.Count.text = "";
            });

            int index = 0;
            PetData.PetInspector.JourneyScore.OrderByDescending(x=>x.score).ToList().ForEach(x => {

                if (index < bars.Count)
                {
                    bars[index].enabled = true;
                    bars[index].Name.text = $"#{index + 1} {x.name}";
                    bars[index].Count.text = x.score.ToString("#,##0");
                    index++;
                }
            });

        }
    }



    public Map map;
    [System.Serializable]
    public class Map
    {
        public Transform root;
        public List<BarObject> barObjects;
        public UnityEngine.UI.RawImage map;
        System.DateTime datetime = System.DateTime.Now;
        bool init = false;
        public void Open()
        {
            instance.Bg(BgStyle.none);
            instance.Init(root, (state) => {
                if (state == "close")
                {
                    instance.OnClose();
                }
                if (state == "ethcoin")
                {
                    Utility.Web.GotoUrl(Setting.instance.link.opensea);
                }
            }, false, null);





            if (!init || datetime.IsTimeout(5))
            {

                // Map
                map.gameObject.SetActive(false);
                LoaderService.instance.OnLoadMapImage("map", (texture) =>
                {
                    map.gameObject.SetActive(true);
                    map.texture = texture;
                });

                // Pet
                int count = Random.RandomRange(2, barObjects.Count);
                count = count.Max(PetData.PetDatas.Count);
                List<PetData> pets = new List<PetData>();
                while (count != 0)
                {
                    var pet = PetData.PetDatas[Random.RandomRange(0, PetData.PetDatas.Count - 1)];
                    if (!pets.Contains(pet))
                    {
                        pets.Add(pet);
                        count--;
                    }
                }
                foreach (var bar in barObjects)
                {
                    bar.Value = Random.RandomRange(0, 100);
                    bar.gameObject.SetActive(false);
                }
                int index = 0;
                foreach (var bar in barObjects.OrderBy(x => x.Value))
                {
                    if (index < pets.Count)
                    {
                        var pet = pets[index];
                        LoaderService.instance.OnLoadMapImage(pet.ID, (texture) => { bar.Raw.texture = texture; });
                        bar.gameObject.SetActive(true);
                        bar.Text = pets[index].ID;
                        bar.onselete = (str) =>
                        {
#if UNITY_EDITOR
                            Information.instance.ContractAddress = pet.ContractAddress;
                            Information.instance.TokenId = pet.TokenId;
                            Application.LoadLevel(0);
#else
                        HtmlCallback.GotoPet($"{pet.ContractAddress}/{pet.TokenId}");
#endif
                        };
                        index++;
                    }
                }
                datetime = System.DateTime.Now;
                init = true;
            }




        }
    }

}

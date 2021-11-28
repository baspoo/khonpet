using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigData 
{
    public static bool Done = false;
    public static Dictionary<string, string> Configs = new Dictionary<string, string>();
    public static void Init(System.Action done)
    {
        if (Done)
        {
            done();
            return;
        }


        if (Setting.instance.tsv.getTsv == Setting.Tsv.GetTsv.bySetting)
        {
            //GetBy Setting Editor
            setup(Setting.instance.tsv.ConfigTsv);
        }
        else
        {
            //GetBy Internet
            LoaderService.instance.OnLoadTsv(LoaderService.GoogleSpreadsheetsID.config, (data) =>
            {
                setup(data);
            });
        }
        void setup(string data)
        {
            var table = GameDataTable.ReadData(data);
            foreach (var d in table.GetTable())
            {
                Configs.Add(d.GetIndex(0), d.GetIndex(1));
            }
            Debug.Log("ContentDatas:" + Configs.Count);
            Done = true;
            done();
        }
    }
}
public class Language
{



    public static bool Done = false;
    public static Dictionary<string, string[]> Languages = new Dictionary<string, string[]>();
    public static void Init(System.Action done)
    {
        if (Done) 
        {
            done();
            return;
        }
           

        if (Setting.instance.tsv.getTsv == Setting.Tsv.GetTsv.bySetting)
        {
            //GetBy Setting Editor
            setup(Setting.instance.tsv.Language);
        }
        else
        {
            //GetBy Internet
            LoaderService.instance.OnLoadTsv(LoaderService.GoogleSpreadsheetsID.language, (data) =>
            {
                setup(data);
            });
        }
        void setup(string data) 
        {
            Debug.Log("data: " + data);
            var table = GameDataTable.ReadData(data);
            Debug.Log("table: " + table.GetTable().Count);
            foreach (var d in table.GetTable())
            {
                Debug.Log("d: " + d.DataLists.Count);
                Languages.Add(d.GetIndex(0), new string[2] { d.GetIndex(1), d.GetIndex(2) });
            }
            Debug.Log("Languages: " + Languages.Count);
            Done = true;
            done();
        }
    }


    public enum LanguageType { En=0,Th=1 }

    static int languageIndex => Playing.instance.playingData.Language;
    public static LanguageType languageType => (LanguageType)languageIndex;

    public static string Get(string key) 
    {
        if (Languages.ContainsKey(key)) 
        {
            return Languages[key][languageIndex];
        }
        else return key;
    } 

}
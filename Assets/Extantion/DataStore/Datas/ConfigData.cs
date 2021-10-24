using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigData 
{



    public static bool Done = false;
    public static Dictionary<string, string> Configs = new Dictionary<string, string>();
    public static void Init(System.Action done)
    {
        //GetBy Internet
        LoaderService.instance.OnLoadTsv(LoaderService.GoogleSpreadsheetsID.config, (data) =>
        {
            var table = GameDataTable.ReadData(data);
            foreach (var d in table.GetTable()) {
                Configs.Add(d.GetIndex(0),d.GetIndex(1));
            }
            Debug.Log("ContentDatas:" + Configs.Count);
            done();
        });

    }


}

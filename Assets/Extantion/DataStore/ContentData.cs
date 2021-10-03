using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentData
{




    public bool Enable;
    public string ContentID;
    public string Name;
    public string Description;
    public string Type;
    public class ContentType
    {
        public const string Img = "Img";
        public const string Vdo = "Vdo";
        public const string Audio = "Audio";
        public const string Gif = "Gif";
        public const string Gltf = "Gltf";
    }
    public string Url;




    public ContentData(GameData raw)
    {
        Enable = System.Convert.ToBoolean(raw.GetValue("Enable"));
        ContentID = raw.GetValue("Enable");
        Name = raw.GetValue("Name");
        Description = raw.GetValue("Description");
        Type = raw.GetValue("Type");
        Url = raw.GetValue("Url");
    }







    public static List<ContentData> ContentDatas = new List<ContentData>();
    public static void Init(System.Action done)
    {
        TsvLoaderService.instance.OnLoadTsv((data) =>
        {
            var table = GameDataTable.ReadData(data);
            foreach (var d in table.GetTable())
            {
                ContentDatas.Add(new ContentData(d));
            }
            Debug.Log("ContentDatas:" + ContentDatas.Count);
            done();
        });
    }


}

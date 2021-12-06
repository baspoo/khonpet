using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirActivity
{
    public enum AirType
    {
         Sun  , Cloud , Rain, Snow , Night
    }



    [System.Serializable]
    public class AirData
    {
        public string airName;
        [TextArea]
        public string airDescription;
        public AirType airType;
        public int[] temperature;
        public Sprite icon;
        public GameObject prefab;
        public Color colorBg;
        public int getTemperature => Random.RandomRange(temperature[0], temperature[1]);
    }




    static AirData m_AirData;
    public static void Init() 
    {


        if (PetData.PetInspector.AirName != null && !PetData.PetInspector.AirTimeOut)
            m_AirData = Store.instance.FindAirData(PetData.PetInspector.AirName);

        if (m_AirData == null)
        {
            m_AirData = Store.instance.AirDatas[Store.instance.AirDatas.Count.Random()];
            PetData.PetInspector.UpdateAir(m_AirData.airName);
        }


        if (Setting.instance.debug.isDebugAir) 
        {
            m_AirData = Store.instance.FindAirData(Setting.instance.debug.AirType);
        }


        if(m_AirData.prefab!=null)
            m_AirData.prefab.Create(World.instance.transform);

        if(m_AirData.colorBg != Color.black)
            World.instance.Camera.backgroundColor = m_AirData.colorBg;

    }

    public static AirData GetAirData()
    {
        return m_AirData;
    }
}

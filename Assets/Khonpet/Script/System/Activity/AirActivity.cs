using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirActivity
{
    public enum AirType
    {
         Sun , Hot , Cloud , Rain, Snow , Night
    }



    [System.Serializable]
    public class AirData
    {
        public string airName;
        public AirType airType;
        public int[] temperature;
        public Sprite icon;
        public GameObject prefab;
        public int getTemperature => Random.RandomRange(temperature[0], temperature[1]);
    }




    static AirData m_AirData;
    public static void Init() 
    {
        m_AirData = Store.instance.AirDatas[Store.instance.AirDatas.Count.Random()];
        if (m_AirData == null)
            Debug.LogError("m_AirData == null");
        else
            Debug.Log("Air:"+ m_AirData.airName);
    }

    public static AirData GetAirData()
    {
        return m_AirData;
    }
}

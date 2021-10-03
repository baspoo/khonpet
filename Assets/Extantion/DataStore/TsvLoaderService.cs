using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsvLoaderService : MonoBehaviour
{

	
	public static TsvLoaderService instance { 
		get 
		{
			if (m_instance == null) 
				m_instance = FindObjectOfType<TsvLoaderService>(); 
			return m_instance; 
		} 
	}
	static TsvLoaderService m_instance;












	public void OnLoadTsv(System.Action<string> onfinish) => StartCoroutine(LoadTsv(onfinish));
	IEnumerator LoadTsv(System.Action<string> onfinish = null)
	{
		//https://docs.google.com/spreadsheets/d/1p7NaMLRUIa4eX-35CdOxTKpAHlpTnqEqhynQVPnX5Do/edit#gid=0
		var GoogleSheetID = "13P0feMixvxMn41-pQQrtgpYb-__SE3RnJbeHtCheBgs";
		var gid = "0";


		Debug.Log("Loading Data from Google Sheet id : " + GoogleSheetID);
		string URL = "https://docs.google.com/spreadsheets/d/" + GoogleSheetID + "/export?gid=" + gid + "&exportFormat=tsv";
		WWW www = new WWW(URL);
		yield return www;
		if (www.isDone)
		{
			if (www.error == null)
			{
				Debug.Log("<color=green> load tsv done => </color>" + www.text);
				onfinish(www.text);
			}
			else
			{
				Debug.Log("<color=red> Fail => </color>" + www.error);
			}
		}
		else
		{
			Debug.Log("<color=red> Fail </color>");
		}
	}





}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderService : MonoBehaviour
{
	public static LoaderService instance { 
		get 
		{
			if (m_instance == null) 
				m_instance = FindObjectOfType<LoaderService>(); 
			return m_instance; 
		} 
	}
	static LoaderService m_instance;










	#region TSV Google Spreadsheets
	Dictionary<string, string> m_tsvstock = new Dictionary<string, string>();
	public void OnLoadTsv(string gid, System.Action<string> onfinish) => StartCoroutine(LoadTsv(gid, onfinish));
	IEnumerator LoadTsv(string gid, System.Action<string> onfinish = null)
	{

		if (m_tsvstock.ContainsKey(gid)) 
		{
			onfinish(m_tsvstock[gid]);
			yield break;
		}
			

		//gid google spreadsheets
		//"0" = pet
		//"1934218596" = config
		var GoogleSheetID = "13P0feMixvxMn41-pQQrtgpYb-__SE3RnJbeHtCheBgs";
		string URL = "https://docs.google.com/spreadsheets/d/" + GoogleSheetID + "/export?gid=" + gid + "&exportFormat=tsv";
		WWW www = new WWW(URL);
		yield return www;
		if (www.isDone)
		{
			if (www.error == null)
			{
				Debug.Log("<color=green> Load TSV Done => </color>" + www.text);
				m_tsvstock.Add(gid,www.text);
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
	#endregion







	#region Image
	Dictionary<string, Texture> m_imgstock = new Dictionary<string, Texture>();
	public void OnLoadImage(string url, System.Action<Texture> onfinish) => StartCoroutine(LoadImage(url, onfinish));
	IEnumerator LoadImage(string url, System.Action<Texture> onfinish = null)
	{
		if (m_imgstock.ContainsKey(url))
		{
			onfinish(m_imgstock[url]);
			yield break;
		}
		WWW www = new WWW(url);
		yield return www;
		if (www.isDone)
		{
			if (www.error == null)
			{
				Debug.Log("<color=green> Load Image Done => </color>" + www.text);
				m_imgstock.Add(url, www.texture);
				onfinish(www.texture);
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
	#endregion











	#region Bundle
	Dictionary<string, AssetBundle> m_bundlestock = new Dictionary<string, AssetBundle>();
	public void OnLoadBundle(string url, System.Action<AssetBundle> onfinish) => StartCoroutine(LoadBundle(url, onfinish));
	IEnumerator LoadBundle(string url, System.Action<AssetBundle> onfinish = null)
	{
		if (m_bundlestock.ContainsKey(url))
		{
			onfinish(m_bundlestock[url]);
			yield break;
		}
		WWW www = new WWW(url);
		yield return www;
		if (www.isDone)
		{
			if (www.error == null)
			{
				Debug.Log("<color=green> Load Bundle Done => </color>" + www.text);
				m_bundlestock.Add(url, www.assetBundle);
				onfinish(www.assetBundle);
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
	#endregion




}

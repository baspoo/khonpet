using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData {
	private string Key;
	private string Value;
	List<GameData> DataLists = new List<GameData>();
	public void SetValue(string key,string value){
		GameData GameData = new GameData ();
		GameData.Key = key;
		GameData.Value = value;
		DataLists.Add (GameData);
	}
	public string GetValue(string key){
		foreach (GameData GD in DataLists)
			if (GD.Key == key)
				return GD.Value;
		Debug.LogError ("Find null value : " + key );
		return string.Empty;
	}
	public bool isHave(string key){
		foreach (GameData GD in DataLists)
			if (GD.Key == key)
				return true;
		return false;
	}
}

public class GameDataTable {
	public void SetTable(List<GameData> Lists){
		GameDatas = Lists;
	}
	public 	List<GameData> GetTable(){
		return GameDatas;
	}
	List<GameData> GameDatas = new List<GameData>();
	public static GameDataTable ReadData(string Text)
	{
		GameDataTable GameDatasMaster= new GameDataTable();

		List<GameData> ThisGameDatas = new List<GameData>();
		string Data = Text;
		string[] Row = Data.Split ('\n');
		string[] Header = Row[0].Split ('\t');

		//Remove Character Special.
		for(int H = 0; H < Header.Length; H++){
			string HeaderText = Header[H];
			HeaderText = HeaderText.Replace("\n","");
			HeaderText = HeaderText.Replace("\r","");
			HeaderText = HeaderText.Replace("\t","");
			HeaderText = HeaderText.Replace("\v","");
			HeaderText = HeaderText.Replace("\b","");
			HeaderText = HeaderText.Replace(" ","");
			HeaderText = HeaderText.Trim ();
			Header[H] = HeaderText;
			//Debug.Log ("HeaderText:"+HeaderText);
		}

		for (int N = 1; N < Row.Length; N++) {
			int InfID = 0;
			if (!string.IsNullOrEmpty (Row [N])) {
				string[] Informations = Row [N].Split ('\t');
				GameData GameData = new GameData ();
				foreach (string keyHeader in Header) {
					string val = Informations [InfID];
					val = val.Replace ("\t", "");
					val = val.Replace ("\n", "");
					val = val.Replace ("\r", "");
					GameData.SetValue (keyHeader, val);
					InfID++;
				}
				ThisGameDatas.Add (GameData);
			}
		}
		GameDatasMaster.SetTable (ThisGameDatas);
		return GameDatasMaster;
	}
}

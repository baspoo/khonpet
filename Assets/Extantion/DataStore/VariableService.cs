using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

public static class VariableService 
{


	#region String
	public static bool notnull(this string value)
	{
		return !string.IsNullOrEmpty(value);
	}
	public static int ToInt(this string str)
	{
		var output = 0;
		int.TryParse(str, out output);
		return output;
	}
	public static long ToLong(this string str)
	{
		var output = 0l;
		long.TryParse(str, out output);
		return output;
	}
	public static float ToFloat(this string str)
	{
		var output = 0f;
		float.TryParse(str, out output);
		return output;
	}
	public static double ToDouble(this string str)
	{
		var output = 0d;
		double.TryParse(str, out output);
		return output;
	}
	public static bool ToBool(this string str)
	{
		var output = false;
		bool.TryParse(str, out output);
		return output;
	}
	public static System.Enum ToEnum(this string str,object defaultenum)
	{
		return (System.Enum)Enum.Parse(defaultenum.GetType(), str); 
	}
	public static T DeserializeObject<T>(this string value) 
	{
		return JsonConvert.DeserializeObject<T>(value);
	}
	public static string ToStringComma(this string[] values)
	{
		return ToStringComma(values.ToList());
	}
	public static string ToStringComma(this List<string> values)
	{
		string str = string.Empty;
		values.ForEach(x=> {
			if (string.IsNullOrEmpty(str)) str = x;
			else str += "," + x;
		});
		return str;
	}
	#endregion





	#region Number
	public static void Loop(this int max , System.Action<int> round)
	{
		for (int i = 0; i < max; i++) 
		{
			round?.Invoke(i);
		}
	}
	public static int Max(this int i, int max) => (i > max) ? max : i;
	public static double Max(this double i, double max) => (i > max) ? max : i;
	public static float Max(this float i, float max) => (i > max) ? max : i;
	#endregion















	#region Object
	public static string SerializeToJson(this object obj)
	{
		return JsonConvert.SerializeObject(obj);
		//return ServiceJson.Json.SerializeObject(obj);
	}
	public static T DeserializeObject<T>(this object obj)
	{
		return JsonConvert.DeserializeObject<T>(SerializeToJson(obj));
	}
	#endregion


	#region GameObject
	public static GameObject Create(this GameObject gameobject , Transform tranform = null)
	{
		var g = UnityEngine.GameObject.Instantiate(gameobject, tranform);
		if (tranform != null) 
		{
			ResetTransform(g);
		}
		return g;
	}
	public static void ResetTransform(this GameObject gameobject)
	{
		gameobject.transform.localPosition = Vector3.zero;
		gameobject.transform.localScale = Vector3.one;
		gameobject.transform.localRotation = Quaternion.identity;
	}
	#endregion


}

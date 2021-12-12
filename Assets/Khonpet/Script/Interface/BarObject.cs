using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BarObject : MonoBehaviour
{
    public Text Name;
    public Text Count;
    public Text Content;
    public Image Icon;
    public Image Bar;
    public Image Img;
    public RawImage Raw;
    public Button Btn;
    public Btn Other;
    public int Value;
    public string Text;
    public Color[] Colors;

    public System.Action<string> onselete;
    public void OnSelete( string clickname ) 
    {
        onselete?.Invoke(clickname);
    }
}

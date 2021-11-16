using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatAnim : MonoBehaviour
{

    public float Speed;
    public Vector2 Offset;
    Vector2 m_Offset;
    public Material mat;
    float runtime = 0.0f;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Change()
    {

        m_Offset.x += Offset.x;
        m_Offset.y += Offset.y;
        mat.SetTextureOffset("_MainTex", m_Offset);
    }

    void Update()
    {
        if (runtime < 1.0f)
        {
            runtime += Time.deltaTime * Speed;
        }
        else
        {
            runtime = 0.0f;
            Change();
        }
    }







}

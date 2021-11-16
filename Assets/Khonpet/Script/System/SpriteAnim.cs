using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnim : MonoBehaviour
{

    public float Speed;
    public List<Sprite> Sprites;
    int index=0;
    float runtime=0.0f;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Change()
    {
        if (Sprites.Count != 0)
        {
            index++;
            if (index >= Sprites.Count)
                index = 0;

            var spr = Sprites[index];

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = spr;
            }
        }

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

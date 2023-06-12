using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LYJ_Testdatabase : MonoBehaviour
{
    //[SerializeField] private Dictionary<string, Sprite> spDict;
    //[SerializeField] private Sprite spriteAdr;
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private bool temp;

    [SerializeField] private Image testimage;

    //private void Start()
    //{
    //    spriteAdr = sprite1;
    //    testimage.sprite = spriteAdr;
    //    spriteAdr = sprite2;
    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            //spriteAdr = sprite2;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TEST();
        }
    }

    public void TEST()
    {
        if (temp)
        {
            testimage.sprite = sprite1;
        }
        else
        {
            testimage.sprite = sprite2;
        }

        temp = !temp;
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateSize : MonoBehaviour
{
    Slider w;
    Slider h;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("gWidth", 50);
        PlayerPrefs.SetInt("gHeight", 50);
        w = GameObject.Find("Width Slider").GetComponent<Slider>();
        h = GameObject.Find("Height Slider").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("gWidth") != (int)w.value)
            PlayerPrefs.SetInt("gWidth", (int)w.value);

        if (PlayerPrefs.GetInt("gHeight") != (int)h.value)
            PlayerPrefs.SetInt("gHeight", (int)h.value);
    }
}

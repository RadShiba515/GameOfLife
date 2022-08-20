using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderInput : MonoBehaviour {
    Slider w;
    Slider h;
    Slider t;

    // Start is called before the first frame update
    void Start() {
        w = GameObject.Find("Width Slider").GetComponent<Slider>();
        h = GameObject.Find("Height Slider").GetComponent<Slider>();
        t = GameObject.Find("Tick Slider").GetComponent<Slider>();
        PlayerPrefs.SetInt("gWidth", (int)w.value);
        PlayerPrefs.SetInt("gHeight", (int)h.value);
        PlayerPrefs.SetFloat("tickRate", t.value);
    }

    // Update is called once per frame
    void Update() {
        if (PlayerPrefs.GetInt("gWidth") != (int)w.value)
            PlayerPrefs.SetInt("gWidth", (int)w.value);

        if (PlayerPrefs.GetInt("gHeight") != (int)h.value)
            PlayerPrefs.SetInt("gHeight", (int)h.value);

        if (PlayerPrefs.GetFloat("tickRate") != t.value)
            PlayerPrefs.SetFloat("tickRate", t.value);
    }
}

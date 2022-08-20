using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Readout : MonoBehaviour
{

    public GameObject sliderGO;
    Slider slider;
    Text txt;
    object val;

    // Start is called before the first frame update
    void Start()
    {
        if (sliderGO != null) {
            slider = sliderGO.GetComponent<Slider>();
            val = slider.value;
        }
        txt = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        val = slider.value;
        if (txt.text != val.ToString())
            txt.text = val.ToString();
    }
}

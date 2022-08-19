using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIInput : MonoBehaviour
{
    // Help popup in corner
    GameObject popup;
    // Help screen
    GameObject help;
    // Generations counter
    GameObject gen;
    // And generation text component!
    Text genTxt;
    // For our generations counter
    SquareArray sa;

    // For scene checking:
    Scene gol;
    Scene sc;

    // Start is called before the first frame update
    void Start()
    {
        gol = SceneManager.GetSceneByName("GameOfLife");
        sc = SceneManager.GetSceneByName("StartupConfig");

        if (gol.isLoaded) {
            // Definitions
            popup = GameObject.Find("Help Popup");
            help = GameObject.Find("Help Panel");
            gen = GameObject.Find("Generation Counter");
            genTxt = gen.GetComponentInChildren<Text>();
            sa = GameObject.Find("Square Array").GetComponent<SquareArray>();

            help.SetActive(false);
            popup.SetActive(true);
            gen.SetActive(true);
        } 
        else if (sc.isLoaded) {
            // we dont really gotta do much here
        }
    }

    // Update is called once per frame
    void Update() {
        if (gol.isLoaded) {
            if (Input.GetKeyDown(KeyCode.H)) {
                // When we press H, show/hide help panel!
                help.SetActive(!(help.activeSelf));
                // While we're at it, hide the popup if we're activating help.
                popup.SetActive((help.activeSelf ? false : true));
                gen.SetActive((help.activeSelf ? false : true));
            }
            // If tab is pressed and help panel isn't showing, hide/show the popup.
            if (Input.GetKeyDown(KeyCode.Tab) && !help.activeSelf) {
                popup.SetActive(!popup.activeSelf);
                gen.SetActive(!gen.activeSelf);
            }
            if (Input.GetKeyDown(KeyCode.Escape)) {
                UnityEngine.SceneManagement.SceneManager.LoadScene("StartupConfig");
            }
            // Update generation counter
            if (gen.activeSelf) {
                genTxt.text = string.Concat("T = ", sa.generation);
            }
        } else if (sc.isLoaded) {
            if(Input.GetKeyDown(KeyCode.Escape)) {
                Application.Quit();
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIResourceDisplay : MonoBehaviour {
    public Sprite icon = null;

    public Color iconColor;
    public Image displayIcon;
    public Image displayIconCC;
    public TextMeshProUGUI count;
    public TextMeshProUGUI countCC;


    // Start is called before the first frame update
    void Start() {
        count.text = "";
        displayIcon.sprite = icon;
    }

    private void Update() {
        displayIconCC.sprite = displayIcon.sprite;
        countCC.text = count.text;
    }
}

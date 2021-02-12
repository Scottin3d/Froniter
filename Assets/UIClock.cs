using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIClock : MonoBehaviour
{
    public TextMeshProUGUI time;
    public TextMeshProUGUI timeCC;

    [SerializeField] float gameTime;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gameTime += Time.deltaTime;
        int hour = (int)(gameTime / 60);
        int minutes = (int)(gameTime % 60);
        string time2text = string.Format("{0}:{1}", hour.ToString("00"), minutes.ToString("00"));
        time.text = time2text;
        timeCC.text = time.text;
    }
}

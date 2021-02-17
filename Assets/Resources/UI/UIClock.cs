using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIClock : MonoBehaviour
{
    public Light SunLight;
    public Color sunColor;
    public TextMeshProUGUI time;
    public TextMeshProUGUI timeCC;

    [SerializeField] float gameTime;
    [SerializeField] Color[] sunCycleColors;
    public float hour;
    public float cycleIncrement;
    public float timeCycle;
    public float cycleColorBlend;
    public int currentCycle;

    // Start is called before the first frame update
    void Start()
    {
        cycleIncrement = 24 / ((sunCycleColors.Length - 1f) * 2f);  // 2.4
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (hour >= 23) {
            gameTime = 0;
        }
        gameTime += Time.deltaTime;
        hour = gameTime / 60;
        float minutes = gameTime % 60;
        string time2text = string.Format("{0}:{1}", hour.ToString("00"), minutes.ToString("00"));
        time.text = time2text;
        timeCC.text = time.text;

        timeCycle = (hour % cycleIncrement);
        float f = hour / cycleIncrement;
        currentCycle = Mathf.FloorToInt(f);

        float v = ((currentCycle - 1) >= 0) ? currentCycle - 1 : 0;
        cycleColorBlend = (hour - (currentCycle * cycleIncrement)) / cycleIncrement;

        if (currentCycle < 1) {
            sunColor = Color.Lerp(sunCycleColors[5], sunCycleColors[4], cycleColorBlend);
        }else if (currentCycle < 2) {
            sunColor = Color.Lerp(sunCycleColors[4], sunCycleColors[3], cycleColorBlend);
        } else if (currentCycle < 3) {
            sunColor = Color.Lerp(sunCycleColors[3], sunCycleColors[2], cycleColorBlend);
        } else if (currentCycle < 4) {
            sunColor = Color.Lerp(sunCycleColors[2], sunCycleColors[1], cycleColorBlend);
        } else if (currentCycle < 5) {
            sunColor = Color.Lerp(sunCycleColors[1], sunCycleColors[0], cycleColorBlend);
        } else if (currentCycle < 6) {
            sunColor = Color.Lerp(sunCycleColors[0], sunCycleColors[1], cycleColorBlend);
        } else if (currentCycle < 7) {
            sunColor = Color.Lerp(sunCycleColors[1], sunCycleColors[2], cycleColorBlend);
        } else if (currentCycle < 8) {
            sunColor = Color.Lerp(sunCycleColors[2], sunCycleColors[3], cycleColorBlend);
        } else if (currentCycle < 9) {
            sunColor = Color.Lerp(sunCycleColors[3], sunCycleColors[4], cycleColorBlend);
        } else if(currentCycle < 10) {
            sunColor = Color.Lerp(sunCycleColors[4], sunCycleColors[5], cycleColorBlend);
        }

        SunLight.color = sunColor;
    }
}

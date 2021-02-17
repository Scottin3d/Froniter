using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITopBar : MonoBehaviour
{
    public UnityEngine.Object colorPresets;
    public Button pause;
    public Button play;
    public Button fastForward;

    private void Awake() {
        Debug.Assert(pause != null, "Please set " + pause + " on " + this + " in the editor!");
        Debug.Assert(play != null, "Please set " + play + " on " + this + " in the editor!");
        Debug.Assert(fastForward != null, "Please set " + fastForward + " on " + this + " in the editor!");
    }
    // Start is called before the first frame update
    void Start()
    {
        pause.onClick.RemoveAllListeners();
        play.onClick.RemoveAllListeners();
        fastForward.onClick.RemoveAllListeners();

        pause.onClick.AddListener(HandlePause);
        play.onClick.AddListener(HandlePlay);
        fastForward.onClick.AddListener(HandleFastForward);

    }

    public void HandlePause() {
        Time.timeScale = 0f;
        
    }
    public void HandlePlay() {
        Time.timeScale = 1f;
    }
    public void HandleFastForward() {
        Time.timeScale = 3f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

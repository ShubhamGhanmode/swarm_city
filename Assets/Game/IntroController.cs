using UnityEngine;
using UnityEngine.Video;

public class IntroController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject introCanvas;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        introCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.anyKeyDown)
            introCanvas.SetActive(false);  // optional: skip video
    }
}

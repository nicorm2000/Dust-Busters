using System;
using UnityEngine;
using UnityEngine.Video;

public class VideoHandler : MonoBehaviour
{
    [SerializeField] private GameObject screen = null;
    [SerializeField] private GameObject loadingScreen = null;
    [SerializeField] private GameObject pcInfo = null;
    [SerializeField] private Animator cameraAnimator = null;

    private VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        screen.SetActive(false);
        pcInfo.SetActive(false);
        loadingScreen.SetActive(true);
    }

    private void OnEnable()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnDisable()
    {
        videoPlayer.loopPointReached -= OnVideoEnd;
    }

    private void Update()
    {
        LogCurrentTime();
    }

    private void LogCurrentTime()
    {
        DateTime now = DateTime.Now;
        string formattedTime = now.ToString("hh:mm tt");
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        cameraAnimator.SetBool("End", true);
        screen.SetActive(true);
        pcInfo.SetActive(true);
        loadingScreen.SetActive(false);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class RenderTextureVideoManager : MonoBehaviour
{
    [SerializeField]
    private RenderTexture targetRenderTexture;
    [SerializeField]
    private VideoClip videoClip;

    private VideoPlayer videoPlayer;
    private RawImage rawImage;

    private float pauseTimestamp = 0.0f;

    private void Awake()
    {
        this.pauseTimestamp = Time.time;
    
        this.videoPlayer = GetComponent<VideoPlayer>();
        this.videoPlayer.clip = this.videoClip;
        this.videoPlayer.targetTexture = this.targetRenderTexture;

        this.rawImage = GetComponent<RawImage>();
        this.rawImage.texture = this.targetRenderTexture;
    }

    public void PauseVideo()
    {
        this.pauseTimestamp = Time.time;
        this.videoPlayer.Pause();
    }

    public void ResumeVideo()
    {
        if (this.videoPlayer.isPaused == false)
        {
            return;
        }
    
        double timeSkipped = Time.time - this.pauseTimestamp;

        double resumeTime = this.videoPlayer.time + timeSkipped;

        if (resumeTime < this.videoPlayer.length)
        {
            this.videoPlayer.time += timeSkipped;
            this.videoPlayer.Play();
        }
        else
        {
            this.videoPlayer.Stop();
        }
    }

    private void OnEnable()
    {
        this.targetRenderTexture.Release();

        this.videoPlayer.frame = 0;
        this.videoPlayer.Play();
    }
    
    private void OnDisable()
    {
        this.targetRenderTexture.Release();
        
        this.videoPlayer.frame = 0;
        this.videoPlayer.Stop();
    }
}

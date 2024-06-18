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

    private void Awake()
    {
        this.videoPlayer = GetComponent<VideoPlayer>();
        this.videoPlayer.clip = this.videoClip;
        this.videoPlayer.targetTexture = this.targetRenderTexture;

        this.rawImage = GetComponent<RawImage>();
        this.rawImage.texture = this.targetRenderTexture;
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

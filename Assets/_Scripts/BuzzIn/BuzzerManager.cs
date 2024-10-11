using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuzzerManager : MonoBehaviour
{
    private bool buzzerActive = true;

    [SerializeField]
    private Image jaredSelectImage;
    [SerializeField]
    private Image stephenSelectImage;
    [SerializeField]
    private Image andrewSelectImage;

    [SerializeField]
    private AudioSource buzzAudio;

    private void OnEnable()
    {
        this.buzzerActive = true;
        this.jaredSelectImage.enabled = false;
        this.stephenSelectImage.enabled = false;
        this.andrewSelectImage.enabled = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (this.buzzerActive == false)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Keypad9))
        {
            this.jaredSelectImage.enabled = true;
            this.buzzerActive = false;
            this.buzzAudio.Play();
        }
        else if (Input.GetKey(KeyCode.E))
        {
            this.stephenSelectImage.enabled = true;
            this.buzzerActive = false;
            this.buzzAudio.Play();
        }
        else if (Input.GetKey(KeyCode.O))
        {
            this.andrewSelectImage.enabled = true;
            this.buzzerActive = false;
            this.buzzAudio.Play();
        }
    }
}

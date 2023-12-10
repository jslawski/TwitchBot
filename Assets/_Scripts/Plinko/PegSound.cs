using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PegSound : MonoBehaviour
{
    [SerializeField]
    private AudioSource bounceSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "cabbage")
        {
            float randomPitch = Random.Range(0.8f, 1.3f);
            bounceSound.pitch = randomPitch;
            bounceSound.Play();
        }
    }
}

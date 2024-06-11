using UnityEngine;

public class PlayAudioOnStairDescent : MonoBehaviour
{
    public AudioClip fallAudioClip;
    private AudioSource audioSource;
    public float fallThreshold = 2.5f;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = fallAudioClip;
    }

    void Update()
    {
        if (transform.position.y < fallThreshold && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}

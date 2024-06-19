using UnityEngine;

public class PlayAudioOnStairDescent : MonoBehaviour
{
    public AudioClip fallAudioClip;
    private AudioSource audioSource;
    public float fallThreshold = 2.5f;
    public GameObject step1;
    public GameObject step2;
    bool hasPlayed;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = fallAudioClip;
    }

    void Update()
    {
        if (transform.position.y < fallThreshold && !audioSource.isPlaying && !hasPlayed)
        {
            audioSource.Play();
            
            Invoke(methodName: "AfterAudio", audioSource.clip.length);
        }
    }

    private void AfterAudio()
    {
        step1.SetActive(false);
        step2.SetActive(true);
        hasPlayed = true;
    }
}

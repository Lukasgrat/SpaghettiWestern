using UnityEngine;

public class LookAtTargetOnThreshold : MonoBehaviour
{
    public AudioClip audioClip;
    public Transform door;
    float endRotation = -65f; // Ending y-axis rotation of door (-38.224 degrees)
    public GameObject instructions;


    private bool hasPlayed = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        //door = GameObject.FindGameObjectWithTag("RoomDoor").transform;

        PlayAudio();
        // Start the delayed execution
        //Invoke("PlayAudio", 5f);
    }

    void PlayAudio()
    {
        if (!hasPlayed)
        {
            audioSource.Play();
            
            Invoke("AfterAudio", 30);
            hasPlayed = true;
        }
    }

    void AfterAudio()
    {
        instructions.SetActive(true);
        Quaternion targetRotation = Quaternion.Euler(0, endRotation, 0);
            door.rotation = targetRotation;

    }
}

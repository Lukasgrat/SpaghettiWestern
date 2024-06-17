using UnityEngine;

public class LookAtTargetOnThreshold : MonoBehaviour
{
    public Transform target;
    public AudioClip audioClip;
    public float lookDuration = 2f;
    public Transform door;
    float endRotation = -38.224f; // Ending y-axis rotation of door (-38.224 degrees)
    public float lerpSpeed = 1.0f; // Controls lerp speed (higher = faster)
    public GameObject instructions;

    private float currentLerpTime = 0.0f;

    private bool hasPlayed = false;
    private AudioSource audioSource;
    private float lookStartTime;
    private Quaternion originalRotation;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        originalRotation = transform.rotation;
        //door = GameObject.FindGameObjectWithTag("RoomDoor").transform;

        PlayAudio();
        // Start the delayed execution
        //Invoke("PlayAudio", 5f);
    }

    void Update()
    {
        if (hasPlayed)
        {
            if (Time.time - lookStartTime >= lookDuration)
            {
                //transform.rotation = originalRotation;
                //enabled = false;
            }
        }
    }

    void PlayAudio()
    {
        if (!hasPlayed)
        {
            //transform.LookAt(target);
            lookStartTime = Time.time;
            audioSource.Play();
            
            Invoke("AfterAudio", 30);
            hasPlayed = true;
        }
    }

    void AfterAudio()
    {
        instructions.SetActive(true);
        currentLerpTime += Time.deltaTime * lerpSpeed;

        // Clamp lerp factor between 0 and 1
        float lerpFactor = Mathf.Clamp01(currentLerpTime);

        // Use Quaternion.Lerp for smooth rotation
        Quaternion targetRotation = Quaternion.Euler(0, endRotation, 0);
        door.rotation = Quaternion.Lerp(door.rotation, targetRotation, lerpFactor);

    }
}

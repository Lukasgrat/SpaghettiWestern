using UnityEngine;

public class LookAtTargetOnThreshold : MonoBehaviour
{
    public Transform target;
    public AudioClip audioClip;
    public float lookDuration = 2f;
    public Transform door;
    public float endRotation = -38.224f; // Ending y-axis rotation (-38.224 degrees)
    public float lerpSpeed = 1.0f; // Controls lerp speed (higher = faster)

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
        door = GameObject.FindGameObjectWithTag("RoomDoor").transform;


        // Start the delayed execution
        Invoke("LookAtTarget", 5f);
    }

    void Update()
    {
        if (hasPlayed)
        {
            if (Time.time - lookStartTime >= lookDuration)
            {
                transform.rotation = originalRotation;
                enabled = false;
            }
        }
    }

    void LookAtTarget()
    {
        if (target != null && !hasPlayed)
        {
            transform.LookAt(target);
            lookStartTime = Time.time;
            audioSource.Play();
            
            Invoke("OpenDoor", audioSource.clip.length);
            hasPlayed = true;
        }
    }

    void OpenDoor()
    {
        currentLerpTime += Time.deltaTime * lerpSpeed;

        // Clamp lerp factor between 0 and 1
        float lerpFactor = Mathf.Clamp01(currentLerpTime);

        // Use Quaternion.Lerp for smooth rotation
        Quaternion targetRotation = Quaternion.Euler(0, endRotation, 0);
        door.rotation = Quaternion.Lerp(door.rotation, targetRotation, lerpFactor);

    }
}

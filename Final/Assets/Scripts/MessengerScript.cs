using UnityEngine;

public class LookAtTargetOnThreshold : MonoBehaviour
{
    public Transform target;
    public AudioClip audioClip;
    public float lookDuration = 2f;

    private bool hasPlayed = false;
    private AudioSource audioSource;
    private float lookStartTime;
    private Quaternion originalRotation;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        originalRotation = transform.rotation;

        // Start the delayed execution
        Invoke("LookAtTarget", 30f);
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
            hasPlayed = true;
        }
    }
}

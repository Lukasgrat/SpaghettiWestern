using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class IntroScene : MonoBehaviour
{
    public Camera mainCamera;
    public Text descriptionText1;
    public Text descriptionText2;
    public Text descriptionText3;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private float duration = 40f;
    private float elapsedTime = 0f;

    private float fadeDuration = 5f;
    private string[] descriptions = new string[] {
      "In the wild wild west...",
      "You are a bounty hunter for hire",
      "Your next task has just been assigned"
    };

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        initialPosition = mainCamera.transform.position;
        targetPosition = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z + 30f);

        descriptionText1.text = "";
        descriptionText2.text = "";
        descriptionText3.text = "";

        StartCoroutine(HandleTextFading());
    }

    void Update()
    {
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            mainCamera.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
        }
        else
        {
            // Deactivate the IntroScene GameObject
            gameObject.SetActive(false);

            // Load the first level additively
            SceneManager.LoadScene("Level1", LoadSceneMode.Additive); // Change "Level1" to your actual first level scene name
        }
    }

    private IEnumerator HandleTextFading()
    {
        yield return StartCoroutine(FadeInAndOutText(descriptionText1, descriptions[0]));
        yield return StartCoroutine(FadeInAndOutText(descriptionText2, descriptions[1]));
        yield return StartCoroutine(FadeInAndOutText(descriptionText3, descriptions[2]));
    }

    private IEnumerator FadeInAndOutText(Text textElement, string description)
    {
        textElement.text = description;
        Color color = textElement.color;

        // Fade In
        for (float fadeInTime = 0; fadeInTime < 1; fadeInTime += Time.deltaTime / fadeDuration)
        {
            color.a = Mathf.Lerp(0, 1, fadeInTime);
            textElement.color = color;
            yield return null;
        }

        yield return new WaitForSeconds(5f);

        // Fade Out
        for (float fadeOutTime = 0; fadeOutTime < 1; fadeOutTime += Time.deltaTime / fadeDuration)
        {
            color.a = Mathf.Lerp(1, 0, fadeOutTime);
            textElement.color = color;
            yield return null;
        }

        textElement.text = "";
    }
}


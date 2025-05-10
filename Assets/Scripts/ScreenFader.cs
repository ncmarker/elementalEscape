using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public GameObject fadePanelPrefab;
    private GameObject fadePanelInstance;


    // imported from online
    public IEnumerator FadeToBlackAndClear(
        float duration,
        System.Action onFadeToBlack = null,
        System.Action onFadeComplete = null
    )
    {
        // Instantiate the fade panel if it doesn't exist
        if (fadePanelInstance == null)
        {
            fadePanelInstance = Instantiate(fadePanelPrefab, FindObjectOfType<Canvas>().transform);
        }

        Image fadeImage = fadePanelInstance.GetComponent<Image>();
        if (fadeImage == null)
        {
            Debug.LogError("Fade panel prefab is missing an Image component!");
            yield break;
        }

        fadePanelInstance.SetActive(true);
        Color color = fadeImage.color;

        // Fade to black
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            if (fadeImage == null) yield break; 
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / duration); 
            fadeImage.color = color;
            yield return null;
        }

        onFadeToBlack?.Invoke(); // Optional action after the fade is complete

        yield return new WaitForSeconds(0.5f);

        // Fade back to clear
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            if (fadeImage == null) yield break; 
            elapsedTime += Time.deltaTime;
            color.a = 1f - Mathf.Clamp01(elapsedTime / duration); 
            fadeImage.color = color;
            yield return null;
        }

        fadePanelInstance.SetActive(false); 
        onFadeComplete?.Invoke(); // Optional action after the fade is complete
    }



}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private string currentLevel; 
    private GameObject player;

    // level endings
    public GameObject winLevelPrefab; 
    public GameObject loseLevelPrefab; 
    public AudioClip winSound;
    public AudioClip loseSound;

    // keep object across all scenes
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        // find player in every new scene
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // check if player has fallen/died and return to levels
    private void Update() {
        if (player != null && player.gameObject.transform.position.y < -10f) {
            Destroy(player);
            PlayerDied();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) Debug.Log("No player found");
    }

    // Load the specified level
    public void LoadLevel(string levelName)
    {
        currentLevel = levelName; 
        SceneManager.LoadScene(levelName); 
    }

    // Load the main menu or levels page
    public void LoadLevelsPage()
    {
        SceneManager.LoadScene("start"); 
    }

    // Restart the current level
    public void RestartLevel()
    {
        if (!string.IsNullOrEmpty(currentLevel)) SceneManager.LoadScene(currentLevel); 
        else Debug.LogWarning("No level is currently loaded!");
    }

    // Quit the game (maybe delete)
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit!"); 
    }

    // ends level after player falls
    public void PlayerDied()
    {
        PlaySound(loseSound);
        StartCoroutine(ShowEndGameScreen(loseLevelPrefab));
    }

    // ends level after player gets to door
    public void LevelCompleted()
    {
        PlaySound(winSound);
        StartCoroutine(ShowEndGameScreen(winLevelPrefab));
    }

    private IEnumerator ShowEndGameScreen(GameObject prefab)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) {
            Debug.LogError("No Canvas found in the scene!");
            yield break;
        }

        GameObject currEndLevelPanel = Instantiate(prefab, canvas.transform);

        // put on top of other UI elements
        RectTransform rectTransform = currEndLevelPanel.GetComponent<RectTransform>();
        if (rectTransform != null) rectTransform.SetAsLastSibling();

        yield return new WaitForSeconds(1.5f); 

        // Fade to black and transition
        ScreenFader fader = FindObjectOfType<ScreenFader>();
        if (fader != null)
        {
            yield return fader.FadeToBlackAndClear(
                1f,
                onFadeToBlack: () =>
                {
                    Destroy(currEndLevelPanel);

                    LoadLevelsPage();
                }
            );
        }
    }

    // used for all sound effects in game
    public void PlaySound(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }
}

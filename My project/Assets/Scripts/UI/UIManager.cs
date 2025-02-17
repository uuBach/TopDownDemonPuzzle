using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private AudioClip gameOverSound;
    public bool gameOver = false;

    private void Awake()
    {
        gameOverScreen.SetActive(false);
    }

    #region Game Over Functions
    //Game over function
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        SoundManager.instance.StopAllSounds();
        SoundManager.instance.PlaySound(gameOverSound);
        Time.timeScale = 0;
        gameOver = true;
}

    //Restart level
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
        gameOver = false;
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
        gameOver = false;
    }

    //Activate game over screen
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        gameOver = false;
    }

    //Quit game/exit play mode if in Editor
    public void Quit()
    {
        Application.Quit(); //Quits the game (only works in build)
    }
    #endregion
}
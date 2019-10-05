using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public int levelDifficulty;
    // Use this for initialization
    void Awake()
    {
        levelDifficulty = 0;
        DontDestroyOnLoad(gameObject);
    }

    public void HardLvl()
    {
        levelDifficulty = 1;
        SceneManager.LoadScene("Level");
    }

    public void EasyLvl()
    {
        levelDifficulty = 0;
        SceneManager.LoadScene("Level");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}

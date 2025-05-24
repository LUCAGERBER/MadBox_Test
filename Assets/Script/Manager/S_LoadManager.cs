using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manage loading
/// </summary>
public class S_LoadManager : MonoBehaviour
{
    public enum Scene
    {
        MainScene,
    }

    private static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}

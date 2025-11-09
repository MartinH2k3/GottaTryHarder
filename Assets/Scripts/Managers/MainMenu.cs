using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
public class MainMenu : MonoBehaviour {
    public void Play() {
        SceneManager.LoadScene(1);
    }

    public void Quit() {
        Application.Quit();
    }
}
}
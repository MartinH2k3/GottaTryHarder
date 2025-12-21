using UnityEngine;

namespace Other
{
public class UnityEventHelpers: MonoBehaviour
{
    public void Continue() => Managers.GameManager.Instance.PauseGame(false);

    public void RestartLevel() => Managers.GameManager.Instance.Restart();

    public void QuitToMainMenu() => Managers.GameManager.Instance.ExitToMenu();

    public void FinishGame() => Managers.GameManager.Instance.End();

}
}
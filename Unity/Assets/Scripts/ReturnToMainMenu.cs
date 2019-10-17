using UnityEngine;
using UnityEngine.SceneManagement;


public class ReturnToMainMenu : MonoBehaviour
{
    private void Start()
    {
        ShowMouseCursor();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ShowMouseCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}

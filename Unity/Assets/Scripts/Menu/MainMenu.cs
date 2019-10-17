using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Dropdown drop1;
    [SerializeField]
    private Dropdown drop2;
    [SerializeField]
    private Dropdown drop3;

    public void PlayGame()
    {
        ApplicationData.IndexOfTypeOfChosenAgent = drop1.value;
        ApplicationData.IndexOfTypeOfChosenAgent2 = drop2.value;
        ApplicationData.IndexOfTypeOfChosenGameSpeed = drop3.value;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
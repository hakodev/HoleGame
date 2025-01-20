using UnityEngine;
using UnityEngine.SceneManagement;

public class KickedOutScreen : MonoBehaviour {
    public void MenuButton() {
        Destroy(transform.root.gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitButton() {
        Application.Quit();
    }

    private void OnApplicationQuit() {
        Destroy(transform.root.gameObject);
    }
}

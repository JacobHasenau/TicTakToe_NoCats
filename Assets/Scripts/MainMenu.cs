using System.Linq;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void ActivateNewScreen(GameObject screen)
    {
        screen.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

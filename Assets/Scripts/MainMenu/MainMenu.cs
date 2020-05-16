using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadSinglePlayerGame()
    {
        SceneManager.LoadScene(1);  //Single Player Game Scene
    }

    public void LoadCoOpModeGame()
    {
        //SceneManager.LoadScene(2);  // Co-Op Mode Scene
    }

}

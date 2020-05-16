using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]    private bool _isGameOver = false;
    [SerializeField]    private bool _isCoOpMode = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
        {
            SceneManager.LoadScene(1);  //Single Player Game Scene

            /*
            if (_isCoOpMode == false)
            {
                SceneManager.LoadScene(1);  //Single Player Game Scene
            }
            else
            {
                //ToDo: Co-Op Mode
                //SceneManager.LoadScene(2);  //Co-op Mode Scene
            }*/
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Space) && _isGameOver == true)
        {
            SceneManager.LoadScene(0);
        }

    }

    public void GameOver()
    {
        _isGameOver = true;
    }

    public bool CoOpMode()
    {
        return _isCoOpMode;
    }

}

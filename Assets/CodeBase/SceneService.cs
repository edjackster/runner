using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneService : IService
{
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    [SerializeField] string ai1Scene, ai2Scene;

    public void AI1Scene() 
    {
        SceneManager.LoadScene(ai1Scene);
    }

    public void AI2Scene()
    {
        SceneManager.LoadScene(ai2Scene);
    }
}

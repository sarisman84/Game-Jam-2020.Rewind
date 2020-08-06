using UnityEngine;
using System.Collections;
using System;

public class MenuScript : MonoBehaviour {
    static MenuScript ins;
    public GameObject gameOverUI, mainMenuUI;
    public static MenuScript GetInstance
    {
        get
        {
            ins = ins ?? FindObjectOfType<MenuScript>() ?? new GameObject("Menu Manager").AddComponent<MenuScript>();
            return ins;
        }
    }



    private void Awake()
    {
        gameOverUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }



    public void CloseGame()
    {
        Application.Quit();
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);

        StartCoroutine(LowerPitch());
       
    }

    private IEnumerator LowerPitch()
    {
        AudioSource player = EffectsManager.GetInstance.CurrentBackgroundMusic;
        player.pitch = 1;
        while (player.pitch > 0.2f)
        {
            player.pitch = Mathf.Lerp(player.pitch, 0.1f, 0.025f);
            yield return new WaitForEndOfFrame();
        }
    }
}

using UnityEngine;
using System.Collections;
using System;

public class PlayerManager {
    private const string emissiveColor = "_EmissionColor";
    PlayerController behaivour;
    public event Action onPlayerDeath;
    Color originalColor, originalEmissiveColor;
    public PlayerManager(PlayerController player)
    {
        behaivour = player;
        renderer = renderer ?? behaivour.transform.GetChild(0).GetComponent<MeshRenderer>();

        originalColor = renderer.sharedMaterial.color;
        originalEmissiveColor = renderer.sharedMaterial.GetColor(emissiveColor);


    }

    int maxAmountOfLives, amountOfLives;
    public void SetLifeAmount(int ammount)
    {
        maxAmountOfLives = ammount;
        amountOfLives = maxAmountOfLives;
        VisualiseHealth();
    }


    public void LooseOneLife()
    {
        amountOfLives--;
        VisualiseHealth();
        if (amountOfLives <= 0)
        {
            KillPlayer();
        }
    }
    MeshRenderer renderer;
    private void VisualiseHealth()
    {

        Color newColor = renderer.sharedMaterial.color;
        Color newEmissiveColor = renderer.sharedMaterial.GetColor(emissiveColor);
        switch (amountOfLives)
        {
            case 3:
                newColor = Color.cyan;
                newEmissiveColor = Color.blue;
                break;
            case 2:
                newColor = Color.green;
                newEmissiveColor = Color.green;
                break;

            case 1:
                newColor = Color.yellow;
                newEmissiveColor = Color.red;
                break;


        }
        renderer.sharedMaterial.SetColor(emissiveColor, newEmissiveColor);
        renderer.sharedMaterial.color = newColor;
    }

    private void KillPlayer()
    {
        behaivour.gameObject.SetActive(false);
        MenuScript.GetInstance.GameOver();
        onPlayerDeath?.Invoke();
    }

    public void RevivePlayer()
    {
        behaivour.gameObject.SetActive(true);
        behaivour.ResetPositionToSpawn();
        if (maxAmountOfLives == 0) SetLifeAmount(3);

        amountOfLives = maxAmountOfLives;
        VisualiseHealth();
    }
}

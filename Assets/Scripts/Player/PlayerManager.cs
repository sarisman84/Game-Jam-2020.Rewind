﻿using UnityEngine;
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
            behaivour.StartCoroutine(KillPlayer());
        }
    }
    MeshRenderer renderer;

    public bool IsAlive { get => amountOfLives > 0; }

    private void VisualiseHealth()
    {

        Color newColor = renderer.sharedMaterial.color;
        Color newEmissiveColor = renderer.sharedMaterial.GetColor(emissiveColor);
        switch (amountOfLives)
        {
            case 3:
                newColor = originalColor;
                newEmissiveColor = originalEmissiveColor;
                break;
            case 2:
                newColor = Color.green;
                newEmissiveColor = Color.green;
                break;

            case 1:
                newColor = Color.red;
                newEmissiveColor = Color.red;
                break;


        }
        renderer.sharedMaterial.SetColor(emissiveColor, newEmissiveColor);
        renderer.sharedMaterial.color = newColor;
    }

    private IEnumerator KillPlayer()
    {
        EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt("PlayerDeath", behaivour.transform.position);
        PostProcessingManager.GetInstance.EnableTimeRewindPP();
        yield return new WaitForSeconds(0.5f);
        behaivour.gameObject.SetActive(false);
        MenuScript.GetInstance.GameOver();
        onPlayerDeath?.Invoke();
        Time.timeScale = 0.01f;
    }

    public void RevivePlayer()
    {
        behaivour.gameObject.SetActive(true);
        behaivour.ResetPositionToSpawn();
        if (maxAmountOfLives == 0) SetLifeAmount(3);

        amountOfLives = maxAmountOfLives;
        VisualiseHealth();
        PostProcessingManager.GetInstance.DisableTimeRewindPP();
        Time.timeScale = 1f;
    }

    public void HealPlayerBy(int value)
    {
        amountOfLives += value;
        amountOfLives = Mathf.Clamp(amountOfLives, 0, maxAmountOfLives);
        VisualiseHealth();
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;

public class Settings : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup masterVolumeMixer;
    [SerializeField] private AudioMixerGroup sfxMixer;
    [SerializeField] private AudioMixerGroup musicMixer;
    [SerializeField] private Image brightPanel;
    [SerializeField] private Slider[] sliders;
    [SerializeField] private Button confirmButton;
    private List<float> sliderValues = new List<float>();

    public void SetMasterVolume(float volume)
    {
        masterVolumeMixer.audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxMixer.audioMixer.SetFloat("SFXVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        musicMixer.audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetBrightness(float bright)
    {
        brightPanel.color = new Color(brightPanel.color.r, brightPanel.color.r, brightPanel.color.r, 1 - bright);
    }

    public void SaveChanges()
    {
        sliderValues.Clear();
        for(int i = 0; i < sliders.Length; i++)
        {
            sliderValues.Add(sliders[i].value);
        }

        PlayerDataManager.Instance.SetSettings(sliderValues);
    }

    //Restablece los valores de los sliders y ajustes a determinados parámetros:
    public void RestoreValues(List<float> values)
    {
        for(int i = 0; i < sliders.Length; i++)
        {
            sliders[i].value = values[i];
        }
        sliderValues = values;
        SetBrightness(sliderValues[0]);
        SetMasterVolume(sliderValues[1]);
        SetMusicVolume(sliderValues[2]);
        SetSFXVolume(sliderValues[3]);
        confirmButton.interactable = false;
    }

    public void SetDefault()
    {
        RestoreValues(new List<float>() { 1, 0, 0, 0 });
        confirmButton.interactable = true;
    }

    public void ExitSettings()
    {
        //Al salir de los settings se te quitan todos los cambios no guardados
        RestoreValues(PlayerDataManager.Instance.GetSettings());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Animations;

public class SpectatorCamera : MonoBehaviour
{
    private PlayerBehaviour[] players;
    private CinemachineVirtualCamera cam;
    private int index = 0;

    private void Awake()
    {
        cam = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
    }

    public void SetSpectate()
    {
        players = GameObject.FindObjectsOfType<PlayerBehaviour>();
        FollowCurrentIndex();
        GetComponent<PlayerInput>().enabled = true;
        GameObject.FindObjectOfType<HUD_CharactersIcon>().SetCharacterPortraits();
        GameObject.FindWithTag("NonSpectable").SetActive(false);
        GameObject.FindWithTag("SpectableHUD").transform.Find("Background").gameObject.SetActive(true);
    }

    public void TogglePlayer(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        index += (int)context.ReadValue<float>();
        if (index < 0) index = players.Length - 1;
        else if (index == players.Length) index = 0;
        FollowCurrentIndex();
    }

    private void FollowCurrentIndex()
    {
        cam.Follow = players[index].transform;
        cam.LookAt = players[index].transform;
    }
}

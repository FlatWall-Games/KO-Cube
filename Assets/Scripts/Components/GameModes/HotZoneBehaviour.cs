using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class HotZoneBehaviour : NetworkBehaviour
{
    [SerializeField] private float controlSpeed = 1; //Velocidad a la que se suman puntos
    [SerializeField] private int maxPoints = 50; //Máximo de puntos que consigue cada equipo por zona (por si hay más de una)
    private int numT1Inside = 0; //Número de jugadores del equipo 1 en la zona
    private int numT2Inside = 0; //Número de jugadores del equipo 2 en la zona
    private float timerT1 = 0; //Temporizador que suma puntos al equipo 1 en función de la velocidad y el tiempo 
    private float timerT2 = 0; //Temporizador que suma puntos al equipo 2 en función de la velocidad y el tiempo
    private int pointsT1 = 0; //Puntos conseguidos en esta zona por el equipo 1
    private int pointsT2 = 0; //Puntos conseguidos en esta zona por el equipo 2
    private HotZoneManager gameManager;
    private Renderer rend;

    void Awake()
    {
        gameManager = GameObject.FindObjectOfType<HotZoneManager>();
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
        
        if(numT1Inside > 0 && numT2Inside == 0 && pointsT1 < maxPoints)
        {
            timerT1 += Time.deltaTime * controlSpeed;
            if(timerT1 >= 1)
            {
                timerT1--;
                gameManager.PointScored("Team1");
                pointsT1++;
                pointsT2--;
            }
        }
        else if(numT2Inside > 0 && numT1Inside == 0 && pointsT2 < maxPoints)
        {
            timerT2 += Time.deltaTime * controlSpeed;
            if (timerT2 >= 1)
            {
                timerT2--;
                gameManager.PointScored("Team2");
                pointsT2++;
                pointsT1--;
            }
        }

        UpdateColorClientRpc(pointsT1, pointsT2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (other.CompareTag("Team1")) numT1Inside++;
        else if (other.CompareTag("Team2")) numT2Inside++;
        other.GetComponent<HealthManager>().OnDead += PlayerDead;
        if (other.GetComponent<CharacterInfo>().characterID == 1) other.GetComponent<AttackManager>().Ulted += PlayerDead;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;
        if (other.CompareTag("Team1")) numT1Inside--;
        else if (other.CompareTag("Team2")) numT2Inside--;
        other.GetComponent<HealthManager>().OnDead -= PlayerDead;
        if (other.GetComponent<CharacterInfo>().characterID == 1) other.GetComponent<AttackManager>().Ulted -= PlayerDead;
    }

    private void OnTriggerStay(Collider other)
    {
        if (numT1Inside == 0 || numT2Inside == 0)
        {
            if(other.CompareTag("Team1") && pointsT1 < maxPoints) transform.Rotate(0, -0.5f, 0);
            else if(other.CompareTag("Team2") && pointsT2 < maxPoints) transform.Rotate(0, 0.5f, 0);
        }
    }

    private void PlayerDead(object s, string tag)
    {
        if (tag.Equals("Team1")) numT1Inside--;
        else numT2Inside--;
        GameObject player = s as GameObject;
        player.GetComponent<HealthManager>().OnDead -= PlayerDead;
        if (player.GetComponent<CharacterInfo>().characterID == 1) player.GetComponent<AttackManager>().Ulted -= PlayerDead;
    }

    [ClientRpc]
    private void UpdateColorClientRpc(int pointsT1, int pointsT2)
    {
        this.pointsT1 = pointsT1;
        this.pointsT2 = pointsT2;
        float factorT1 = 1 - (float)pointsT1 / maxPoints;
        float factorT2 = 1 - (float)pointsT2 / maxPoints;
        Color newColor;
        if (PlayerBehaviour.ownerTag.Equals("Team1")) newColor = new Color(factorT1, 0, factorT2, 0.7f);
        else newColor = new Color(factorT2, 0, factorT1, 0.7f);
        rend.materials[0].color = newColor;
        rend.materials[1].SetColor("_color", newColor);
    }
}

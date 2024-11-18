using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamColorManager : MonoBehaviour
{
    private Light colorLight;
    private string teamTag = "Untagged";
    [SerializeField] private float offsetX;
    [SerializeField] private float offsetY;

    void Awake()
    {
        colorLight = GetComponent<Light>();
        StartCoroutine(GetTeam());
    }

    private void Update()
    {
        if(teamTag.Equals("Team1")) transform.position = transform.parent.position + Vector3.forward * 3 + new Vector3(offsetX, offsetY, 0);
        else transform.position = transform.parent.position - Vector3.forward * 3 + new Vector3(0,4,0);
        transform.rotation = Quaternion.LookRotation(transform.parent.position - this.transform.position);
    }

    public void SetColor(Color color)
    {
        colorLight.color = color;
    }

    IEnumerator GetTeam()
    {
        while (teamTag.Equals("Untagged"))
        {
            teamTag = PlayerBehaviour.ownerTag;
            yield return new WaitForSeconds(0.01f);
        }
    }
}

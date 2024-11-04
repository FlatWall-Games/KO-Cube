using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class FlagBehaviour : NetworkBehaviour
{
    [SerializeField] private string team;
    [SerializeField] private Renderer flagArea;
    private HealthManager carrier;
    private Transform flagOrigin;
    private bool dropped = false;

    [Header("Render materials: ")]
    [SerializeField] private Material blueMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Renderer flagRenderer;

    [Header("Restore params: ")]
    [SerializeField] private Image restoreBar;
    [SerializeField] private float timeToRestore = 5;
    private float timer;
    
    
    void Awake()
    {
        restoreBar.color = flagRenderer.material.color;
        flagArea.material.color = flagRenderer.material.color;
        timer = timeToRestore;
        flagOrigin = transform.parent;
    }

    void Update()
    {
        if(!dropped)
        {
            restoreBar.fillAmount = 1;
            restoreBar.gameObject.SetActive(false);
            timer = timeToRestore;
        }
        else
        {
            restoreBar.gameObject.SetActive(true);
            timer -= Time.deltaTime;
            restoreBar.fillAmount = timer / timeToRestore;
            if (timer <= 0) RestoreFlagPosition();
        }
    }

    public void ToggleMaterial()
    {
        if(flagRenderer.material.color == blueMaterial.color) flagRenderer.material = redMaterial;
        else flagRenderer.material = blueMaterial;
        restoreBar.color = flagRenderer.material.color;
        flagArea.material.color = flagRenderer.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.CompareTag("FlagArea") && other.gameObject != this.flagArea.gameObject)
        {
            //team indica el equipo al que pertenece la bandera. Al ser robada se le asigna el punto al otro equipo
            if(team.Equals("Team1")) GameObject.FindObjectOfType<AGameManager>().PointScored("Team2");
            else GameObject.FindObjectOfType<AGameManager>().PointScored("Team1");
            RestoreFlagPosition();
        }

        if (carrier != null) return;
        
        if ((other.CompareTag("Team1") || other.CompareTag("Team2")) && !other.CompareTag(team))
        {
            this.transform.parent = other.transform;
            this.transform.localPosition = new Vector3(0, this.transform.localPosition.y, -0.5f);
            this.transform.rotation = Quaternion.identity;
            carrier = other.GetComponent<HealthManager>();
            carrier.GetComponent<AttackManager>().carrying = true;
            carrier.OnDead += DropFlag;
            SetDroppedClientRpc(false);
        }
    }
    
    private void DropFlag()
    {
        this.transform.parent = null;
        carrier.OnDead -= DropFlag;
        carrier.GetComponent<AttackManager>().carrying = false;
        carrier = null;
        SetDroppedClientRpc(true);
    }

    private void RestoreFlagPosition()
    {
        DropFlag();
        this.transform.parent = flagOrigin;
        this.transform.localPosition = Vector3.zero;
        SetDroppedClientRpc(false);
    }

    [ClientRpc]
    private void SetDroppedClientRpc(bool d)
    {
        dropped = d;
    }
}

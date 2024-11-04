using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class FlagBehaviour : NetworkBehaviour
{
    [SerializeField] private string team;
    [SerializeField] private Renderer flagArea;
    private HealthManager carrier;
    private Transform flagOrigin;

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
        if(this.transform.parent != null)
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
        if (other.CompareTag("FlagArea") && other.gameObject != this.flagArea.gameObject)
        {
            GameObject.FindObjectOfType<AGameManager>().PointScored(team);
            RestoreFlagPosition();
        }

        if (carrier != null) return;
        
        if ((other.tag.Equals("Team1") || other.tag.Equals("Team2")) && !other.tag.Equals(team))
        {
            this.transform.parent = other.transform;
            this.transform.localPosition = new Vector3(0, this.transform.localPosition.y, -0.5f);
            this.transform.rotation = Quaternion.identity;
            carrier = other.GetComponent<HealthManager>();
            carrier.GetComponent<AttackManager>().carrying = true;
            carrier.OnDead += DropFlag;
        }
    }
    
    private void DropFlag()
    {
        this.transform.parent = null;
        carrier.OnDead -= DropFlag;
        carrier.GetComponent<AttackManager>().carrying = false;
        carrier = null;
    }

    private void RestoreFlagPosition()
    {
        DropFlag();
        this.transform.parent = flagOrigin;
        this.transform.localPosition = Vector3.zero;
    }
}

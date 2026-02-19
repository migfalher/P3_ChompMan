using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class EnemyNavigation : MonoBehaviour
{
    // materials
    public Material[] materialsList;
    private Material regularMaterial;
    public Material vulnerableMat;
    // components
    private ManagerOfGame manager_Script;
    private NavMeshAgent agent;
    private MeshRenderer renderer;
    // variables
    private float regularSpeed;
    private float vulnerableSpeed;
    private string targetTag;

    public string getTargetTag() { return targetTag; }
    public void setTargetTag(string tag) {
        targetTag = tag;
        switch (targetTag)
        {
            case "Hideout":
                renderer.material = vulnerableMat;
                agent.speed = vulnerableSpeed;
                break;
            default:
                renderer.material = regularMaterial;
                agent.speed = regularSpeed;
                break;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // set components
        manager_Script = GameObject.Find("ScriptHolder").GetComponent<ManagerOfGame>();
        targetTag = "Player";
        agent = this.GetComponent<NavMeshAgent>();
        renderer = this.GetComponentInChildren<MeshRenderer>();
        regularSpeed = (this.gameObject.name.Equals("EnemySmall")) ? manager_Script.getEnemySmallSpeed() : manager_Script.getEnemyBigSpeed() ;
        vulnerableSpeed = regularSpeed / manager_Script.getPowerUpSlowDowdDivider();

        // set random material
        regularMaterial = materialsList[(new Random()).Next(0, materialsList.Length)];
        renderer.material = regularMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = GameObject.FindWithTag(targetTag).transform.position;
        agent.SetDestination(targetPos);
    }
}

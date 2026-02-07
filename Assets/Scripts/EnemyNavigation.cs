using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigation : MonoBehaviour
{
    // public objects
    public Material regularMat;
    public Material vulnerableMat;

    // private components
    private float regularSpeed;
    private float vulnerableSpeed;
    private NavMeshAgent agent;
    private MeshRenderer renderer;
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
                renderer.material = regularMat;
                agent.speed = regularSpeed;
                break;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetTag = "Player";
        agent = this.GetComponent<NavMeshAgent>();
        renderer = this.GetComponentInChildren<MeshRenderer>();
        regularSpeed = agent.speed;
        vulnerableSpeed = regularSpeed / 2;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = GameObject.FindWithTag(targetTag).transform.position;
        agent.SetDestination(targetPos);
    }
}

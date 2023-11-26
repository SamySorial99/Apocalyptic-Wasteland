using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class hr_ZombieController : MonoBehaviour
{
    [Header("Stats settings")]
    [SerializeField] private float health = 100;
    //[SerializeField] private Transform MuzzleHead;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject RedArrow;

    [Header("AI settings")]
    [SerializeField] private bool enableWander = true;
    [SerializeField] private LayerMask allMasks;
    [SerializeField] private float fov = 120.0f;
    [SerializeField] private float viewDistance = 10.0f;
    [SerializeField] private float hitDistance = 0.8f;
    [SerializeField] private float wanderRadius = 7.0f;
    [SerializeField] private float loseThreshold = 10.0f;
    [SerializeField] private float attackCooldown = 1.0f;


    private GameObject player;
    private NavMeshAgent agent;
    private Animator animator;

    private bool isAware = false;
    private bool isDetecting = false;
    private bool isHittingPlayer = false;
    private bool isRunning = false;

    private float loseTimer = 0.0f;
    private float lastAttackTime = 0.0f;

    private Vector3 wanderPoint = Vector3.zero;

    private GameManager gm;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        player = GameObject.Find("PlayerSoldier");
        wanderPoint = RandomWanderPoint();
        gm = GameManager.GetInstance();
        RedArrow.SetActive(false);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        if (isAware)
        {
            if (Vector3.Distance(transform.position, player.transform.position) > hitDistance)
            {
                agent.SetDestination(player.transform.position);
                transform.LookAt(player.transform.position);
                CancelInvoke();
                isHittingPlayer = false;
            }
            else
            {
                if(Time.time - lastAttackTime >= attackCooldown)
                    {
                     lastAttackTime = Time.time; // Update the last attack time
                     Debug.Log("Invoked");
                     InvokeRepeating("DamagePlayer", 0.01f, 0);
                     isHittingPlayer = true;
                    }
            }

            if (!isDetecting)
            {
                loseTimer += Time.deltaTime;
                if (loseTimer >= loseThreshold)
                {
                    isAware = false;
                    loseTimer = 0.0f;
                }
            }
        }
        else
        {
            loseTimer = 0.0f;
            if (enableWander)
            {
                Wander();
            }
        }

        SearchForPlayer();


    }

    private void SearchForPlayer()
    {
        transform.LookAt(player.transform.position);
        if (Vector3.Angle(Vector3.forward, transform.InverseTransformPoint(player.transform.position)) < fov / 2.0f)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < viewDistance)
            {

                OnAware();
            }
            else
            {
                isDetecting = false;

            }
        }
        else
        {
            isDetecting = false;
        }
    } 

    public void OnAware()
    {
        isAware = true;
        isDetecting = true;
        RedArrow.SetActive(true);
        transform.LookAt(player.transform.position);
        if(!isRunning)
        {
            isRunning = true;
            animator.SetTrigger("Run");
        }


        hr_AudioManager.instance.PlayScaredShout();
    }

    public void TakeDamage(float amount = 10.0f)
    {
        health -= amount;

        if (health <= 0)
        {
            Destroy(this.gameObject);
            gm.AddPoints(10);
        }
    }

    public Vector3 RandomWanderPoint()
    {
        Vector3 randomPoint = (Random.insideUnitSphere * wanderRadius) + transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPoint, out hit, wanderRadius, allMasks);
        return new Vector3(hit.position.x, transform.position.y, hit.position.z);
    }

    public void Wander()
    {
        if (!isRunning)
        {
            isRunning = true;
            animator.SetTrigger("Run");
        }

        if (Vector3.Distance(transform.position, wanderPoint) < 1.5f)
        {
            wanderPoint = RandomWanderPoint();
            isRunning = false;

        }
        else
        {
            agent.SetDestination(wanderPoint);
        }
    }

    private void DamagePlayer()
    {
        hr_AudioManager.instance.Play("gunshot_01");

        isRunning = false;
        muzzleFlash.Emit(1);
        agent.SetDestination(transform.position);
        animator.SetTrigger("attack");
        Debug.Log("Attackkkk");
        gm.DamagePlayer(10.0f);
    }
}

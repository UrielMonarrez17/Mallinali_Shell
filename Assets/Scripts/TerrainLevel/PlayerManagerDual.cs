using UnityEngine;
using Unity.Cinemachine;
using Pathfinding;

public class PlayerManagerDual : MonoBehaviour
{
    // --- Singleton Instance (Easy access) ---
    public static PlayerManagerDual Instance;

    [Header("Configuration")]
    public CinemachineCamera camera; // Updated for Unity 6
    public GameObject warrior;
    public GameObject turtle;
    public SpecialFloor floorsito;
    public KeyCode switchKey = KeyCode.Tab;

    [Header("Combo State")]
    public int hitsRequired = 3;
    private int currentComboCount = 0;

    // State Tracking
    private GameObject active;
    private GameObject follower;
    private bool underWater;

    void Awake()
    {
        // Setup Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        SetActive(warrior); 
    }

    void Update()
    {
        if (floorsito != null) underWater = floorsito.getUnderWater();
        if (Input.GetKeyDown(switchKey)) Switch();
    }

    // --- LOGIC FOR COMBO ---
    public void RegisterHit()
    {
        currentComboCount++;
        Debug.Log($"Combo: {currentComboCount}/{hitsRequired}");

        if (currentComboCount >= hitsRequired)
        {
            TriggerFollowerAttack();
            currentComboCount = 0; // Reset
        }
    }

    private void TriggerFollowerAttack()
    {
        if (follower != null)
        {
            // We look for PlayerCombat OR the specific Turtle/Warrior combat script on the follower
            var combat = follower.GetComponent<PlayerCombat>();
            
            if (combat != null)
            {
                Debug.Log($"{follower.name} is performing a Combo Assist!");
                // Make sure the Follower uses a bigger range or "Super" logic
                combat.PerformComboAttack(); 
            }
        }
    }
    // -----------------------

    void Switch()
    {
        // Reset combo when switching characters (optional, but recommended)
        currentComboCount = 0; 

        if (active == warrior)
        {
            SetActive(turtle);
            CambiarTarget(turtle.transform);
        }
        else
        {
            SetActive(warrior);
            CambiarTarget(warrior.transform);
        }
    }

    void CambiarTarget(Transform nuevoObjetivo)
    {
        camera.Follow = nuevoObjetivo;
        camera.LookAt = nuevoObjetivo;
    }

    void SetActive(GameObject newActive)
    {
        active = newActive;
        
        // Determine who is who
        if (active == warrior) follower = turtle;
        else follower = warrior;

        // --- Logic to Enable/Disable Components ---
        // 1. Enable Input/Control on Active
        ToggleCharacterControl(active, true);
        ToggleAI(active, false);

        // 2. Enable AI/Follow on Follower
        ToggleCharacterControl(follower, false);
        
        // Only enable AI if not underwater (based on your logic)
        if (!underWater)
        {
            ToggleAI(follower, true, active.transform);
        }
        else
        {
            ToggleAI(follower, false);
        }
    }

    // Helper function to clean up the code
    void ToggleCharacterControl(GameObject charObj, bool isPlayerControlled)
    {
        // Assuming both have these, or check specifically
        var pCtrl = charObj.GetComponent<PlayerController>();
        var tCtrl = charObj.GetComponent<TortugaController>();

        if (pCtrl) pCtrl.enabled = isPlayerControlled;
        if (tCtrl) tCtrl.enabled = isPlayerControlled;
    }

    // Helper function for AI
    void ToggleAI(GameObject charObj, bool enableAI, Transform target = null)
    {
        var companionAI = charObj.GetComponent<CompanionAStar2D>();
        var seeker = charObj.GetComponent<Seeker>();

        if (companionAI) 
        {
            companionAI.enabled = enableAI;
            if (enableAI && target != null) companionAI.target = target;
        }
        if (seeker) seeker.enabled = enableAI;
    }

         public GameObject GetActive()
    {
        return active;
    }
        public GameObject GetFollower()
    {
        return follower;
    }
}
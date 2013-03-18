using UnityEngine;
using System.Collections;

public class Scenario_Predator_Lab : ScenEventListener {

    public GameObject[] DoctorNPC;
    public DoorScript Door;
    public GameObject Vincent;
    public Camera ScenarioCamera;
    public Camera PlayerCamera;
    public ParticleCannonScript ParticleCannon;

    /// <summary>
    /// Prefab and location to spawn the predator player
    /// </summary>
    public GameObject PredatorPlayer;
    public Transform SpawnPredatorPlayerLocation;

    public NPCSpawn npcSpawn = null;

    public bool StartScenarioAtAwake = true;

    public CameraDock[] cameraDock;

    [HideInInspector]
    public Scenario_Predator_Lab Instance;

    void Awake()
    {
        Instance = this;
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	}

    IEnumerator StartScenario()
    {
        foreach (GameObject doctor in DoctorNPC)
        {
            doctor.animation.Play("sit_operatingComputer");
        }
        Util.DeactivateRecurrsive(Vincent);
        yield return StartCoroutine(CameraDock.AutoDock(this.cameraDock, this.ScenarioCamera, this));
    }

    IEnumerator WhiteInCamera()
    {
        ScenarioCamera.SendMessage("WhiteIn");
        yield break;
    }

    IEnumerator CreateWarmHole()
    {
        Debug.Log("In CreateWarmHole");
        yield return StartCoroutine(ParticleCannon.CreateWarmHole());
        //After the warmhole created, wait a bit time and spawn the predator player
        yield return new WaitForSeconds(1.5f);
        SendMessage("SpawnPredatorPlayer");
        yield return new WaitForSeconds(1.5f);
        //To save performance, Perlin eat CPU !
        ParticleCannon.Invoke("CutParticle", 0);
    }

    void EndScenario()
    {
        
    }

    IEnumerator SpawnPredatorPlayer()
    {
        //Object.Instantiate(PredatorPlayer, SpawnPredatorPlayerLocation.position, SpawnPredatorPlayerLocation.rotation);
        Util.ActivateRecurrsive(PredatorPlayer);
        PredatorPlayer.BroadcastMessage("PlayerControlOff");
        //Debug.Break();
        //White out the screen
        ScenarioCamera.SendMessage("WhiteOut", true);
        LevelManager.GameEvent(new GameEvent(GameEventType.EndScenario, null));
        yield return 4f;
        //ScenarioCamera.enabled = false;
        //PlayerCamera.active = true;
        //PlayerCamera.SendMessage("WhiteIn");
        yield return new WaitForSeconds(3.5f);
        //Level start:
        ScenarioCamera.enabled = false;
        PlayerCamera.active = true;
        PlayerCamera.SendMessage("WhiteIn");
        Util.ActivateRecurrsive(Vincent);
        yield return new WaitForSeconds(PlayerCamera.GetComponent<CameraWhiteInOut>().WhiteInLength);
        PredatorPlayer.BroadcastMessage("PlayerControlOn");
    }

    public override void OnEvent(GameEvent gameEvent)
    {
        switch (gameEvent.type)
        {
            case GameEventType.LevelStart:
                if (StartScenarioAtAwake)
                {
                    StartCoroutine(StartScenario());
                }
                else
                {
                    LevelManager.GameEvent(new GameEvent(GameEventType.SkipScenario, this.gameObject));
                }
                break;
            case GameEventType.SkipScenario:
                StopAllCoroutines();
                DisableScenarioInput();
                SendMessage("SpawnPredatorPlayer");
                break;
            case GameEventType.EndScenario:
                DisableScenarioInput();
                break;
            case GameEventType.PlayerControlOn:
                break;
            case GameEventType.NPCDie:
                if (gameEvent.sender == Vincent)
                {
                    npcSpawn.SendMessage("Spawn");
                }
                break;
            default: break;
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        for (int i=0; i<cameraDock.Length ;i ++)
        {
            if (i != cameraDock.Length - 1)
            {
                Gizmos.DrawLine(cameraDock[i].Transform.position, cameraDock[i+1].Transform.position);
            }
            Gizmos.DrawSphere(cameraDock[i].Transform.position, 0.08f);
        }
    }

    public void DisableScenarioInput()
    {
        this.GetComponent<JoyButtonManager>().enabled = false;
        this.GetComponent<JoyButton>().enabled = false;
    }
}

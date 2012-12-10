using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class AIBug : AI {

    public bool IsRoamer = false;
    public float RoamingRadius = 50f;
    public float distanceToStop = 1f; 

    public float attackRange = 10f;
    public float offsensiveRange = 75f;
    public float alertRange = 150f;
    public bool offensive = true;

    private GameObject currentTarget = null;
    private AIBugMovement bugMovement = null;
    private BugAnimation bugAnimation = null;
    private AIBugAttack bugAttack = null;
    private Vector3 bornPosition;
    /// <summary>
    /// Command List - the First command has higher priority than next, and so on..
    /// </summary>
    LinkedList<Command> commandList = new LinkedList<Command>();
	// Use this for initialization
	void Start () {
     //   Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Bugs"), LayerMask.NameToLayer("Bugs"), true);
        bugMovement = this.GetComponent<AIBugMovement>();
        bugAttack = this.GetComponent<AIBugAttack>();
        bugAnimation = this.GetComponent<BugAnimation>();
        bornPosition = this.transform.position;

	}

    void Roaming()
    {
        float randomAngle = Random.Range(0, 360);
        Vector3 nextWaypoint = new Vector3(bornPosition.x + Mathf.Cos(randomAngle) * RoamingRadius,
                                   bornPosition.y,
                                   bornPosition.z + Mathf.Sin(randomAngle) * RoamingRadius);
        Command c = new Command(Command.CommandType.MoveTo, nextWaypoint);
        DispatchCommand(c);
    }


	// Update is called once per frame
    void Update()
    {
        if (commandList.Count > 0)
        {
            ProcessCommands(commandList.First.Value);
        }
        else if (this.IsRoamer)
        {
            Roaming();
        }
        else
        {
            bugAnimation.idle();
            bugMovement.move = false;
        }
	}

    void ProcessCommands(Command cmmd)
    {
        switch (cmmd.Type)
        {
            case Command.CommandType.MoveTo:
                ProcessMoveToCommand(cmmd);
                break;
            case Command.CommandType.AttackAndMove:
                ProcessAttackAndMoveCommand(cmmd);
                break;
            default:
                ProcessMoveToCommand(cmmd);
                break;
        }
    }

    void ProcessMoveToCommand(Command cmmd)
    {
        float distance = Vector3.Distance(cmmd.Position, this.transform.position);
        //If reach target already, stop
        if (distance <= distanceToStop)
        {
            this.animation.CrossFade("idle");
            //Arrived, detach command from command list.
            commandList.Remove(cmmd);
            bugMovement.move = false;
        }
        else
        {
            bugMovement.move = true;
            bugMovement.targetPosition = cmmd.Position;
        }
    }

    void ProcessAttackAndMoveCommand(Command cmmd)
    {
        //Find closest targets
        GameObject target = FindTarget();
        ReceiveDamage targetReceiveDamage = null;
        //Find a target, bite it!
        if ((targetReceiveDamage = target.GetComponent<ReceiveDamage>()) != null && targetReceiveDamage.IsAlive())
        {
            currentTarget = target;
            bool isTargetInMeleeRange = bugAttack.IsTargetInMeleeRange(targetReceiveDamage);
            if (isTargetInMeleeRange) 
            {
                bugMovement.move = false;
                bugAttack.SetTarget(targetReceiveDamage);
                bugAttack.attack = true;
            }
            else
            {
                bugMovement.move = true;
                bugAttack.attack = false;
                bugMovement.targetPosition = target.transform.position;
            }
        }
        //No target found, just move to the command point
        else
        {
            ProcessMoveToCommand(cmmd);
        }
    }

    GameObject FindTarget()
    {
        int PuchBagLayer = LayerMask.NameToLayer("PunchBag");
        int layerMask = 1 << PuchBagLayer;
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, this.offsensiveRange, layerMask);
        GameObject target = null;
        if (colliders != null && colliders.Length > 0)
        {
            Collider closest = Util.findClosest(this.transform.position, colliders);
            target = closest.gameObject;
        }
        return target;
    }



    #region override

    public override void DispatchCommand(Command command)
    {
        commandList.AddLast(command);
    }

    public override void ExecuteCommand(Command cmmd, bool dropOldCommands)
    {
        if (dropOldCommands)
        {
            commandList.Clear();
            commandList.AddFirst(cmmd);
        }
        else
        { 
            commandList.AddFirst(cmmd);
        }
    }
    #endregion

    #region callback

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //We ignore hit with Terrain
        bool isTerrainCollider = hit.collider is TerrainCollider;
        if (!isTerrainCollider)
        {
            //Debug.Log("Hit ! " + hit.collider.gameObject.name);
        }
    }

    #endregion

    #region Gizmo
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, this.alertRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, this.offsensiveRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, this.RoamingRadius);
    }
    #endregion
}

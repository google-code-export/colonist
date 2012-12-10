using UnityEngine;
using System.Collections;

public class Command  {
    public enum CommandType
    {
        MoveTo = 0, //Simply move to
        AttackAndMove = 1, //If see enemy, attack, else, move to
        ShootOnce = 2, // shoot one time per command
        AimAt=3,
        ShootAt = 4 //continuously shooting 
    }
    private CommandType type;

    private bool inProcessing;
    public bool InProcessing
    {
        set { inProcessing = value; }
        get { return inProcessing; }
    }
    public readonly float CreateTime = Time.time;
    public CommandType Type
    {
        get { return type; }
        set { type = value; }
    }
    private Vector3 position;

    public Vector3 Position
    {
        get { return position; }
        set { position = value; }
    }

    public Command(CommandType type,Vector3 position)
    {
        this.type = type;
        this.position = position;
    }
    
}

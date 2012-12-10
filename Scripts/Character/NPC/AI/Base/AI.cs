using UnityEngine;
using System.Collections;

public abstract class AI : MonoBehaviour
{
    
    /// <summary>
    /// Dispatch a command to the units, the unit can optionally to execute or queue the command
    /// </summary>
    /// <param name="pos"></param>
    public abstract void DispatchCommand(Command cmmd);

    /// <summary>
    /// Execute a command immediately
    /// If dropOldCommands is false, all historical commands should be processed following completion of
    /// current command.
    /// Else if dropOldCommands is true, all historical commands should be abandoned..
    /// </summary>
    /// <param name="pos"></param>
    public abstract void ExecuteCommand(Command cmmd, bool dropOldCommands);
}
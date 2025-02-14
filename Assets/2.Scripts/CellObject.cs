using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellObject : MonoBehaviour
{
    protected Vector2Int m_Cell;
    public virtual void Init(Vector2Int cell)
    {
        m_Cell = cell;
    }

    public virtual bool PlayerWantsToEnter()
    {
        return true;
    }
    
    public virtual void PlayerEntered()
    {

    }
}

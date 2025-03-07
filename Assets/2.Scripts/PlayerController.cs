using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 5.0f;
    public Vector2Int Cell { get{ return m_CellPosition;} }
    private bool m_IsMoving;
    private Vector3 m_MoveTarget;
    private BoardManager m_Board;
    private Vector2Int m_CellPosition;
    private bool m_IsGameOver;
    private Animator m_Animator;
    private Vector2Int newCellTarget;
    private bool hasMoved;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void Init()
    {
        m_IsMoving = false;
        m_IsGameOver = false;
    }

    public void GameOver()
    {
        m_IsGameOver = true;
    }

    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        m_Board = boardManager;
        MoveTo(cell, true);
    }

    public void MoveTo(Vector2Int cell, bool immediate)
    {
        m_CellPosition = cell;
        if (immediate)
        {
            m_IsMoving = false;
            transform.position = m_Board.CellToWorld(m_CellPosition);
        }
        else
        {
            m_IsMoving = true;
            m_MoveTarget = m_Board.CellToWorld(m_CellPosition);
        }
        m_Animator.SetBool("Moving", m_IsMoving);
    }

    private void Update()
    {
        if (m_IsGameOver)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                GameManager.Instance.StartNewGame();
            }
            return;
        }

        if (m_IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                m_MoveTarget, MoveSpeed * Time.deltaTime);
            if (transform.position == m_MoveTarget)
            {
                m_IsMoving = false;
                m_Animator.SetBool("Moving", false);
                var cellData = m_Board.GetCellData(m_CellPosition);
                if (cellData.ContainedObject != null)
                {
                    cellData.ContainedObject.PlayerEntered();
                }
            }
            return;
        }

        newCellTarget = m_CellPosition;
        hasMoved = false;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AddTick();
        }
        else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            MoveUp();
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            MoveDown();
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            MoveRight();
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            MoveLeft();
        }
    }

    private void UpdatePlayer()
    {
        if (hasMoved)
        {
            BoardManager.CellData cellData = m_Board.GetCellData(newCellTarget);
            if (cellData != null && cellData.Passable)
            {
                GameManager.Instance.TurnManager.Tick();
                if (cellData.ContainedObject == null)
                {
                    MoveTo(newCellTarget, false);
                }
                else if (cellData.ContainedObject.PlayerWantsToEnter())
                {
                    MoveTo(newCellTarget, false);
                }
                else
                {
                    m_Animator.SetTrigger("Attack");
                }
            }
        }
    }

    public void MoveUp()
    {
        newCellTarget.y += 1;
        hasMoved = true;
        UpdatePlayer();
    }

    public void MoveDown()
    {
        newCellTarget.y -= 1;
        hasMoved = true;
        UpdatePlayer();
    }

    public void MoveLeft()
    {
        newCellTarget.x -= 1;
        hasMoved = true;
        UpdatePlayer();
    }

    public void MoveRight()
    {
        newCellTarget.x += 1;
        hasMoved = true;
        UpdatePlayer();
    }

    public void AddTick()
    {
        if (m_IsGameOver)
        {
            GameManager.Instance.StartNewGame();
        }
        else
        {
            hasMoved = true;
            UpdatePlayer();
        }
    }
}

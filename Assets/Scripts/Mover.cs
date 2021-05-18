using System;
using System.Collections;
using System.Collections.Generic;
using Koffie.SimpleTasks;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private Splitter splitter;
    [SerializeField] private Part partToMove;

    [SerializeField] private bool isMoving;

    private void OnEnable()
    {
        ScrollPart.OnSelected += StartMove;
        Splitter.OnClear += Clear;
    }

    private void OnDisable()
    {
        ScrollPart.OnSelected -= StartMove;
        Splitter.OnClear -= Clear;
    }

    public void Clear()
    {
        isMoving = false;
        partToMove = null;
    }
    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && isMoving)
        {
            Debug.Log("stop move");
            LateStopMove();
        }
    }
    
    public void StartMove(int id)
    {
        var part = splitter.GetPartById(id);
        isMoving = true;
        partToMove = part;
        partToMove.Appear();
    }

    private void StopMove()
    {
        partToMove.Release();
        isMoving = false;
    }

    private STask _stopTask;
    private void LateStopMove()
    {
        _stopTask?.Kill();
        _stopTask = STasks.DoAfterFrames(StopMove, 2);
    }

}

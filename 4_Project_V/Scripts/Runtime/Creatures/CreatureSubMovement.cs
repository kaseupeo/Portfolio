using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CreatureMovement))]
public class CreatureSubMovement : MonoBehaviour
{
    [SerializeField] private float rollTime = 0.5f;
    [SerializeField] private float dashTime = 0.5f;

    public Creature Owner { get; private set; }
    public bool IsRolling { get; private set; }
    public bool IsDash { get; private set; }
    public Define.Direction Direction { get; private set; }

    public void Init(Creature owner)
    {
        Owner = owner;
    }

    private Vector3 DirectionByVector3(Define.Direction direction = 0)
    {
        Direction = direction;
        
        switch (direction)
        {
            case Define.Direction.Forward:
                return transform.forward;
            case Define.Direction.Backward:
                return -transform.forward;
            case Define.Direction.Right:
                return transform.right;
            case Define.Direction.Left:
                return -transform.right;
            default:
                return transform.forward;
        }
    }
    
    #region Roll

    public void Roll(float distance, Vector3 direction)
    {
        Owner.Movement.Stop();

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(transform.forward);

        IsRolling = true;
        StopCoroutine(nameof(RollUpdate));
        StartCoroutine(RollUpdate(distance, direction));
    }

    public void Roll(float distance, Define.Direction direction = 0) 
        => Roll(distance, DirectionByVector3(direction));

    private IEnumerator RollUpdate(float rollDistance, Vector3 direction)
    {
        float currentRollTime = 0f;
        float prevRollDistance = 0f;

        while (true)
        {
            currentRollTime += Time.deltaTime;

            float timePoint = currentRollTime / rollTime;
            float inOutSine = -(Mathf.Cos(Mathf.PI * timePoint) - 1f) / 2f;
            float currentRollDistance = Mathf.Lerp(0f, rollDistance, inOutSine);
            float deltaValue = currentRollDistance - prevRollDistance;

            transform.position += direction * deltaValue;
            prevRollDistance = currentRollDistance;
            
            if (currentRollTime >= rollTime)
                break;

            yield return null;
        }

        IsRolling = false;
    }

    #endregion

    #region Dash

    public void Dash(float distance, Vector3 direction)
    {
        Owner.Movement.Stop();

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(transform.forward);

        IsDash = true;
        StopCoroutine(nameof(DashUpdate));
        StartCoroutine(DashUpdate(distance, direction));
    }

    public void Dash(float distance, Define.Direction direction = 0) 
        => Dash(distance, DirectionByVector3(direction));

    private IEnumerator DashUpdate(float dashDistance, Vector3 direction)
    {
        float currentDashTime = 0f;
        float prevDashDistance = 0f;

        while (true)
        {
            currentDashTime += Time.deltaTime;

            float timePoint = currentDashTime / dashTime;
            float inOutSine = -(Mathf.Cos(Mathf.PI * timePoint) - 1f) / 2f;
            float currentDashDistance = Mathf.Lerp(0f, dashDistance, inOutSine);
            float deltaValue = currentDashDistance - prevDashDistance;

            transform.position += direction * deltaValue;
            prevDashDistance = currentDashDistance;
            
            if (currentDashTime >= rollTime)
                break;

            yield return null;
        }

        IsDash = false;
    }

    #endregion
}
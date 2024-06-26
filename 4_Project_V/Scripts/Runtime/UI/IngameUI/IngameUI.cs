using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameUI : BaseUI
{
    public Transform followTarget;
    public Vector2 followOffset;

    protected override void Awake()
    {
        base.Awake();

    }

    private void LateUpdate()
    {
        if (followTarget == null)
            Managers.Pool.Release(this);

        if (followTarget != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(followTarget.position + (Vector3)followOffset);
        }
    }

    public void SetTarget(Transform target)
    {
        this.followTarget = target;
        if (followTarget != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(followTarget.position + (Vector3)followOffset);
        }
    }
    public void SetOffset(Vector2 offset)
    {
        this.followOffset = offset;
        if (followTarget != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(followTarget.position + (Vector3)followOffset);
        }
    }

}
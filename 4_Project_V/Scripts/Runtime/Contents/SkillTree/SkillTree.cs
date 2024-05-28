using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

public class SkillTree : BaseObject
{
    [SerializeField, HideInInspector]
    private SkillTreeGraph graph;

    public SkillTreeSlotNode[] GetSlotNodes()
        => graph.GetSlotNodes();
}

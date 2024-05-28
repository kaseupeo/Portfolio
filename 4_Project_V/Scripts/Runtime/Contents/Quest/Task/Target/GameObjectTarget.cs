using UnityEngine;

[CreateAssetMenu(fileName = "TARGET_", menuName = "Quest Task Target/GameObject", order = 0)]
public class GameObjectTarget : TaskTarget
{
    [SerializeField] private GameObject value;
        
    public override object Value => value;
        
    public override bool IsEqual(object target)
    {
        var targetAsGameObject = target as GameObject;

        if (targetAsGameObject == null)
            return false;

        return targetAsGameObject.name.Contains(value.name);
    }
}
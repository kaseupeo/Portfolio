using UnityEngine;
using UnityEngine.Serialization;

public class PanelUI : MonoBehaviour
{
    [SerializeField] private PanelType panelType;
    
    public PanelType PanelType => panelType;
}
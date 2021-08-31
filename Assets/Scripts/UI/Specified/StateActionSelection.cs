using UnityEngine;

public class StateActionSelection : MonoBehaviour, ITooltipDisplayable
{
    [TextArea]
    public string Tooltip;
    public string TooltipContent => Tooltip;
}

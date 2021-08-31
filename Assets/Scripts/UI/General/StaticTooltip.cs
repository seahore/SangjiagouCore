using UnityEngine;

public class StaticTooltip : MonoBehaviour, ITooltipDisplayable
{
    [TextArea]
    public string Tooltip;
    public string TooltipContent => Tooltip;
}

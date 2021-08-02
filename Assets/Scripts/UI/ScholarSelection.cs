using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SangjiagouCore;

[RequireComponent(typeof(CanvasRenderer))]
public class ScholarSelection : MonoBehaviour, ITooltipDisplayable
{
    School playerSchool;

    void Start()
    {
        playerSchool = Game.CurrentEntities.GetPlayerSchool(GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().ID);
    }

    public string TooltipContent {
        get {
            var t = playerSchool.FindScholarByName(transform.Find("Name").GetComponent<Text>().text).Action;
            return t is null ? "目前无任务" : t.ToString();
        }
    }

}

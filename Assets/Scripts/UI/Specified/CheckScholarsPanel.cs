using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SangjiagouCore;

public class CheckScholarsPanel : MonoBehaviour
{
    public GameObject MaskPrefab;
    public GameObject ScholarSelection;

    public School playerSchool;

    static readonly string Unknown = "不明";

    void Start()
    {
        var mask = Instantiate(MaskPrefab, transform.parent);
        transform.SetParent(mask.transform);

        var mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        mainCam.GetComponent<Animator>().SetTrigger("Unfocus");

        playerSchool = Game.CurrentEntities.GetPlayerSchool(GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().ID);
    }

    public void Show(List<Scholar> scholars, bool enableSchoolColor = false)
    {
        if (scholars is null) return;
        var list = transform.Find("Scroll View/Viewport/Scholars List");

        var unitHeight = ScholarSelection.GetComponent<RectTransform>().rect.height;
        var unitWidth = ScholarSelection.GetComponent<RectTransform>().rect.width;
        float th = -8;
        float tw = 8;
        for (int i = 0; i < scholars.Count; ++i) {
            var s = scholars[i];
            var o = Instantiate(ScholarSelection, list).transform;
            Destroy(o.GetComponent<ScholarSelection>());
            o.GetComponent<Toggle>().onValueChanged.AddListener((bool isOn) => {
                if (isOn) {
                    transform.Find("Full Name").GetComponent<Text>().text = s.FullName;
                    transform.Find("Courtesy Name").GetComponent<Text>().text = s.CourtesyName;
                    transform.Find("Belong To Text").GetComponent<Text>().text = s.BelongTo.Name;
                    transform.Find("Age Text").GetComponent<Text>().text = s.Age.ToString();
                    if (s.BelongTo == playerSchool) {
                        transform.Find("Location Text").GetComponent<Text>().text = s.Location.ToString();
                        transform.Find("Sophistry Text").GetComponent<Text>().text = s.Sophistry.ToString();
                        transform.Find("Action Text").GetComponent<Text>().text = s.Action is null ? "目前闲在" : s.Action.ToString();
                    } else {
                        transform.Find("Location Text").GetComponent<Text>().text = Unknown;
                        transform.Find("Sophistry Text").GetComponent<Text>().text = Unknown;
                        transform.Find("Action Text").GetComponent<Text>().text = Unknown;
                    }
                }
            });
            o.GetComponent<Toggle>().group = list.GetComponent<ToggleGroup>();
            o.GetComponent<RectTransform>().offsetMin = new Vector2(tw, th - unitHeight);
            o.GetComponent<RectTransform>().offsetMax = new Vector2(tw + unitWidth, th);
            if(enableSchoolColor) 
                o.Find("Button Face").GetComponent<Image>().color = Color.Lerp(s.BelongTo.Color, Color.white, 0.5f);
            o.Find("Avatar").GetComponent<Image>().sprite = s.Avatar;
            o.Find("Name").GetComponent<Text>().text = s.FullCourtesyName;
            if (i % 3 == 2) {
                th -= unitHeight + 5;
                tw = 8;
            } else {
                tw += unitWidth + 5;
            }
        }
    }

    public void ShowScholarsOf(School school)
    {
        if (school is null) return;
        Show(school.Members);
    }

    public void ShowAllScholars()
    {
        Show(Game.CurrentEntities.Scholars, true);
    }

    public void OnCloseButtonClick()
    {
        if (GameObject.Find("Upper UI Canvas").transform.childCount <= 1) {
            var mainCam = GameObject.FindGameObjectWithTag("MainCamera");
            mainCam.GetComponent<Animator>().SetTrigger("Focus");
        }
        GetComponent<Animator>().SetTrigger("Close");
    }

    /// <summary>
    /// 这个函数会在关闭动画的最后调用
    /// </summary>
    public void OnCloseAnimationFinished()
    {
        Destroy(transform.parent.gameObject);
    }
}

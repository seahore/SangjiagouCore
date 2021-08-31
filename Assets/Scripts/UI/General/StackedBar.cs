using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
// using UnityEditor.Animations;
// using AnimatorController = UnityEditor.Animations.AnimatorController;

[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Stacked Bar")]
public class StackedBar : MonoBehaviour
{
    [Serializable]
    public struct StackedBarItem
    {
        [SerializeField]
        string _name;
        public string Name { get => _name; set => _name = value; }
        [SerializeField]
        Color _color;
        public Color Color { get => _color; set => _color = value; }
        [SerializeField]
        float _value;
        public float Value { get => _value; set => _value = value; }
        public StackedBarItem(string name, Color color, float value)
        {
            _name = name;
            _color = color;
            _value = value;
        }
    }
    public List<StackedBarItem> Items;

    public Texture Texture;
    public RuntimeAnimatorController AnimatorController;
    /*
    public AnimationClip NotHover;
    public AnimationClip Enter;
    public AnimationClip Hover;
    public AnimationClip Exit;
    */

    public UnityEvent<StackedBarItem, float> OnEnterBar;

    /*
    AnimatorController _animatorController;
    */

    float[] _itemRatios;
    Transform _renderArea;
    GameObject _nowHover;



    void Start()
    {
        /*
        _animatorController = new AnimatorController();
        _animatorController.name = "TempItemAnimCtrl";
        _animatorController.AddParameter("Hover", UnityEngine.AnimatorControllerParameterType.Bool);
        _animatorController.AddLayer("Base Layer");

        var stateMachine = _animatorController.layers[0].stateMachine;
        var notHoverState = stateMachine.AddState("Not Hover");
        var enterState = stateMachine.AddState("Enter");
        var hoverState = stateMachine.AddState("Hover");
        var exitState = stateMachine.AddState("Exit");
        notHoverState.motion = NotHover;
        enterState.motion = Enter;
        hoverState.motion = Hover;
        exitState.motion = Exit;
        stateMachine.AddEntryTransition(notHoverState);
        AnimatorStateTransition transEnter = new AnimatorStateTransition { destinationState = enterState, hasExitTime = false, duration = 0 };
        transEnter.AddCondition(AnimatorConditionMode.If, 0, "Hover");
        notHoverState.AddTransition(transEnter);
        AnimatorStateTransition transHover = new AnimatorStateTransition { destinationState = hoverState, hasExitTime = true, duration = 0 };
        enterState.AddTransition(transHover);
        AnimatorStateTransition transExit = new AnimatorStateTransition { destinationState = exitState, hasExitTime = false, duration = 0 };
        transExit.AddCondition(AnimatorConditionMode.IfNot, 0, "Hover");
        hoverState.AddTransition(transExit);
        AnimatorStateTransition transNotHover = new AnimatorStateTransition { destinationState = notHoverState, hasExitTime = true, duration = 0 };
        exitState.AddTransition(transNotHover);
        */

        _renderArea = new GameObject("Render Area", typeof(RectTransform)).transform;
        _renderArea.SetParent(transform);
        var rt = _renderArea.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        _renderArea.localScale = Vector3.one;
        Reset();
    }

    public void Reset()
    {
        for (int i = 0; i < _renderArea.childCount; i++) {
            Destroy(_renderArea.GetChild(i).gameObject);
        }

        float sum = 0;
        foreach (var item in Items) {
            sum += item.Value;
        }
        _itemRatios = new float[Items.Count];
        for (int i = 0; i < Items.Count; i++) {
            _itemRatios[i] = Items[i].Value / sum;
        }

        var w = GetComponent<RectTransform>().rect.width;
        float ratioSum = 0;
        for (int i = 0; i < Items.Count; ratioSum += _itemRatios[i++]) {
            var o = new GameObject(i.ToString(), typeof(RectTransform), typeof(Animator), typeof(CanvasRenderer), typeof(RawImage)).transform;
            o.SetParent(_renderArea.transform);
            o.localScale = Vector3.one;
            var rt = o.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2 { x = 0, y = 1 };
            rt.offsetMin = new Vector2 { x = ratioSum * w, y = 0 };
            rt.offsetMax = new Vector2 { x = (ratioSum + _itemRatios[i]) * w, y = 0 };
            o.GetComponent<RawImage>().texture = Texture;
            o.GetComponent<RawImage>().color = Items[i].Color;
            o.GetComponent<Animator>().runtimeAnimatorController = AnimatorController;
        }

        _nowHover = null;
    }

    Vector2 _mpos = new Vector2();

    void Update()
    {
        var t = Mouse.current.position.ReadValue();
        if (_mpos != t) {
            _mpos = t;
            var ped = new PointerEventData(EventSystem.current);
            ped.position = _mpos;
            var results = new List<RaycastResult>();
            GraphicRaycaster raycaster = GetComponentInParent<GraphicRaycaster>();
            raycaster.Raycast(ped, results);

            bool hovering = false;
            foreach (var r in results) {
                if (r.gameObject.transform.parent == _renderArea) {
                    hovering = true;
                    if (_nowHover is null || _nowHover != r.gameObject) {
                        var i = int.Parse(r.gameObject.name);
                        OnEnterBar.Invoke(Items[i], _itemRatios[i]);
                        r.gameObject.GetComponent<Animator>().SetBool("Hover", true);
                        if (!(_nowHover is null))
                            _nowHover.GetComponent<Animator>().SetBool("Hover", false);
                        _nowHover = r.gameObject;
                    }
                }
            }
            if (!hovering) {
                if (!(_nowHover is null))
                    _nowHover.GetComponent<Animator>().SetBool("Hover", false);
                _nowHover = null;
            }
        }
    }

}

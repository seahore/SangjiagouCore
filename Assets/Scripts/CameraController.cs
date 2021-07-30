using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using SangjiagouCore;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public float ScrollSpeed = 10;
    public int ScrollTriggerEdgeWidth = 20;
    public float ScrollEasingTime = 0.1f;
    public float ZoomSpeed = 15;
    public float ZoomEasingTime = 0.1f;
    public float MaxZoomDistance = 10;
    public float MinZoomDistance = 2;
    public int PixelsPerUnit = 100;
    Vector2 scrollEasingFactor;

    Rect upArea;
    Rect downArea;
    Rect rightArea;
    Rect leftArea;
    bool up;
    bool down;
    bool left;
    bool right;
    float zoom;

    Vector3 velocity;
    Vector3 border;
    float targetSize;

    const float CAMERA_STANDARD_SIZE = 5;

    void Start()
    {
        scrollEasingFactor = Vector2.zero;
        targetSize = CAMERA_STANDARD_SIZE;
    }

    void Update()
    {
        border = new Vector3 {
            x = Game.CurrentEntities.MapSize.x,
            y = Game.CurrentEntities.MapSize.y,
            z = 0
        };

        upArea = new Rect(0, Screen.height - ScrollTriggerEdgeWidth, Screen.width, ScrollTriggerEdgeWidth);
        downArea = new Rect(0, 0, Screen.width, ScrollTriggerEdgeWidth);
        rightArea = new Rect(Screen.width - ScrollTriggerEdgeWidth, 1f, ScrollTriggerEdgeWidth, Screen.height);
        leftArea = new Rect(0, 0, ScrollTriggerEdgeWidth, Screen.height);

        // 如果摄像机位置出某一边的边界，那么那个方向的运动被禁止
        up = up && transform.position.y <= border.y;
        down = down && transform.position.y >= 0;
        left = left && transform.position.x >= 0;
        right = right && transform.position.x <= border.x;

        // 更新移动的缓动系数
        if (right) {
            scrollEasingFactor.x = Mathf.Lerp(scrollEasingFactor.x, 1, Time.deltaTime / ScrollEasingTime);
        } else if (left) {
            scrollEasingFactor.x = Mathf.Lerp(scrollEasingFactor.x, -1, Time.deltaTime / ScrollEasingTime);
        } else {
            scrollEasingFactor.x = Mathf.Lerp(scrollEasingFactor.x, 0, Time.deltaTime / ScrollEasingTime);
        }
        if (up) {
            scrollEasingFactor.y = Mathf.Lerp(scrollEasingFactor.y, 1, Time.deltaTime / ScrollEasingTime);
        } else if (down) {
            scrollEasingFactor.y = Mathf.Lerp(scrollEasingFactor.y, -1, Time.deltaTime / ScrollEasingTime);
        } else {
            scrollEasingFactor.y = Mathf.Lerp(scrollEasingFactor.y, 0, Time.deltaTime / ScrollEasingTime);
        }

        velocity = new Vector3 {
            x = ScrollSpeed * scrollEasingFactor.x,
            y = ScrollSpeed * scrollEasingFactor.y,
            z = 0
        };

        // 因为摄像机的缩放是缩小视口，所以当缩放较近时移动速度可能会太快，故乘一个相机当前size和标准size的比例
        velocity *= Time.deltaTime * (GetComponent<Camera>().orthographicSize / CAMERA_STANDARD_SIZE);
        transform.Translate(velocity);

        // 更新缩放的缓动系数
        if (zoom < 0 && targetSize <= MaxZoomDistance) {
            targetSize += ZoomSpeed * Time.deltaTime;
        } else if (zoom > 0 && targetSize >= MinZoomDistance) {
            targetSize -= ZoomSpeed * Time.deltaTime;
        }
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, targetSize, Time.deltaTime / ZoomEasingTime);
    }

    /// <summary>
    /// 摄像机检查鼠标位置并向四个方向中的某几个滚动，包含缓动效果和边界检查
    /// </summary>
    /// <param name="mPos">鼠标位置</param>
    public void Scroll(Vector2 mPos)
    {
        up = upArea.Contains(mPos);
        down = downArea.Contains(mPos);
        left = leftArea.Contains(mPos);
        right = rightArea.Contains(mPos);
    }

    /// <summary>
    /// 摄像机缩放，包含缓动效果和边界检查
    /// </summary>
    /// <param name="y">瞬时缩放的值</param>
    public void Zoom(float y)
    {
        this.zoom = y;
    }
}

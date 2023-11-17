using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class CameraViewport : MonoBehaviour
{
    [SerializeField]
    private float _maxAspectRatio = 1593f / 2048f; // 타이틀 백그라운드에 맞춰놓음.
    private int _reservedWidth = 0;
    private int _reservedHeight = 0;
    private Camera _camera = null;

    private void RefreshViewport(Rect rect)
    {
        var data = Camera.main.GetUniversalAdditionalCameraData();
        if (data == null)
            return;

        for (int i = 0; i < data.cameraStack?.Count; ++i)
        {
            var cam = data.cameraStack[i];
            if (cam == null)
                continue;
            cam.rect = rect;
        }
    }

    private void Update()
    {
        if (_camera == null)
            return;

        int width = Screen.width;
        int height = Screen.height;
        if (_reservedWidth == width && _reservedHeight == height)
            return;

        _reservedWidth = width;
        _reservedHeight = height;

        float currentAspectRatio = (float)width / height;
        float horizontalOffset = 0f;
        if (currentAspectRatio > _maxAspectRatio)
        {
            float targetWidth = Screen.height * _maxAspectRatio;
            float currentWidth = Screen.width;
            horizontalOffset = (currentWidth - targetWidth) / (2 * currentWidth);
        }
        else
        {
            var rect = new Rect(0, 0, 1, 1);
            _camera.rect = rect;
            RefreshViewport(rect);
            return;
        }

        Rect viewPortRect = _camera.rect;
        viewPortRect.x = horizontalOffset;
        viewPortRect.width = 1 - (2 * horizontalOffset);
        _camera.rect = viewPortRect;
        RefreshViewport(viewPortRect);
    }

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        RenderPipelineManager.beginFrameRendering += BeginRender;
    }

    private void OnDestroy()
    {
        RenderPipelineManager.beginFrameRendering -= BeginRender;
    }

    private void BeginRender(ScriptableRenderContext context, Camera[] cameras)
    {
        for (int i = 0; i < cameras.Length; ++i)
        {
            var camera = cameras[i];
            if (camera != null && camera != Camera.main)
                continue;

            Rect wp = camera.rect;
            Rect nr = new Rect(0, 0, 1, 1);

            camera.rect = nr;
            GL.Clear(true, true, Color.black);

            camera.rect = wp;
        }
    }
}
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MiniGame
{
    /// <summary>
    /// Gắn vào MiniGameCanvas.
    /// Khi mở: chuyển sang Screen Space - Camera (để DragDrop hoạt động)
    /// Khi đóng: trả về World Space (nằm trong scene 3D)
    /// Tự động unlock cursor khi mở, lock lại khi đóng.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class MiniGameCanvasSetup : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Mở khóa chuột khi mini game đang mở để player có thể kéo thả")]
        [SerializeField] private bool unlockCursorWhenOpen = true;

        [Tooltip("Khoảng cách canvas với camera khi ở chế độ Screen Space - Camera")]
        [SerializeField] private float planeDistance = 1f;

        private Canvas canvas;
        private RenderMode originalRenderMode;
        private float originalPlaneDistance;
        private Camera originalWorldCamera;
        private CursorLockMode previousCursorLockMode;
        private bool previousCursorVisible;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            // Lưu trạng thái ban đầu
            originalRenderMode    = canvas.renderMode;
            originalPlaneDistance = canvas.planeDistance;
            originalWorldCamera   = canvas.worldCamera;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (Camera.main == null)
            {
                Debug.LogError("[MiniGameCanvas] Không tìm thấy Camera.main!");
                return;
            }

            // ✅ Chuyển sang Screen Space - Camera để DragDrop hoạt động chính xác
            canvas.renderMode    = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera   = Camera.main;
            canvas.planeDistance = planeDistance;
            Debug.Log($"[MiniGameCanvas] Chuyển sang Screen Space - Camera (planeDistance={planeDistance})");

            // Kiểm tra EventSystem
            if (EventSystem.current == null)
                Debug.LogError("[MiniGameCanvas] ⚠️ Không có EventSystem trong scene!");

            // Unlock cursor để kéo thả được
            if (unlockCursorWhenOpen)
            {
                previousCursorLockMode = Cursor.lockState;
                previousCursorVisible  = Cursor.visible;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible   = true;
                Debug.Log("[MiniGameCanvas] Cursor unlocked");
            }
        }

        private void OnDisable()
        {
            // Trả canvas về World Space khi đóng
            canvas.renderMode    = originalRenderMode;
            canvas.worldCamera   = originalWorldCamera;
            canvas.planeDistance = originalPlaneDistance;

            // Khôi phục cursor
            if (unlockCursorWhenOpen)
            {
                Cursor.lockState = previousCursorLockMode;
                Cursor.visible   = previousCursorVisible;
                Debug.Log("[MiniGameCanvas] Cursor restored");
            }
        }

        /// <summary>Mở mini game cho local player.</summary>
        public void OpenForLocalPlayer()
        {
            gameObject.SetActive(true);
            Debug.Log("[MiniGameCanvas] Mở mini game!");
        }

        /// <summary>Đóng mini game.</summary>
        public void Close()
        {
            gameObject.SetActive(false);
            Debug.Log("[MiniGameCanvas] Đóng mini game.");
        }
    }
}
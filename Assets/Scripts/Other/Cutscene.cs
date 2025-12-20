using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Other
{
    public class Cutscene : MonoBehaviour
    {
        [Header("Cutscene Frames (order matters)")]
        [SerializeField] private Image[] frames;

        private InputSystemActions _inputActions;
        private InputAction[] _clickActions;

        private int _currentIndex;

        public event System.Action Finished;

        private void Awake()
        {
            _inputActions = new InputSystemActions();

            // Ensure only the first frame is visible
            for (int i = 0; i < frames.Length; i++)
            {
                if (frames[i] != null)
                    frames[i].gameObject.SetActive(i == 0);
            }

            _currentIndex = 0;
        }

        private void OnEnable()
        {
            _clickActions = new[]
            {
                _inputActions.Player.Attack,
                _inputActions.Player.Jump,
                _inputActions.Player.Interact,
                _inputActions.Player.Dash
            };

            foreach (var action in _clickActions)
            {
                action.Enable();
                action.performed += PlayNextScene;
            }
        }

        private void OnDisable()
        {
            if (_clickActions == null) return;

            foreach (var action in _clickActions)
            {
                action.performed -= PlayNextScene;
                action.Disable();
            }
        }

        private void PlayNextScene(InputAction.CallbackContext ctx)
        {
            // Hide current frame
            if (_currentIndex < frames.Length && frames[_currentIndex] != null)
                frames[_currentIndex].gameObject.SetActive(false);

            _currentIndex++;

            // End of cutscene
            if (_currentIndex >= frames.Length)
            {
                GameManager.Instance.ExitToMenu();
                return;
            }

            // Show next frame
            if (frames[_currentIndex] != null)
                frames[_currentIndex].gameObject.SetActive(true);
        }
    }
}

using UnityEngine;

namespace GameFramework.Game
{
    /// <summary>
    ///     Controller for the cursor's visibility
    /// </summary>
    public class CursorController : MonoBehaviour
    {
        [Tooltip("Hide the cursor when the game starts")]
        [SerializeField]
        private bool hideCursor = true;

        /// <summary>
        ///     The state of the cursor
        /// </summary>
        public bool HideCursor
        {
            set
            {
                Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !value;

                if (hideCursor != value) Debug.Log($"Cursor is {(hideCursor ? "hidden" : "visible")}");

                hideCursor = value;
            }
        }

        private void Start()
        {
            HideCursor = hideCursor;
        }
    }
}
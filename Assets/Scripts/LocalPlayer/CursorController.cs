using UnityEngine;

namespace LocalPlayer
{
    public class CursorController : MonoBehaviour
    {
        [Tooltip("Hide the cursor when the game starts")] [SerializeField]
        private bool hideCursor = true;

        /// <summary>
        ///     Hide the cursor
        /// </summary>
        public bool HideCursor
        {
            set
            {
                hideCursor = value;
                Cursor.lockState = hideCursor ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !hideCursor;
            }
        }

        private void Start()
        {
            HideCursor = hideCursor;
        }
    }
}
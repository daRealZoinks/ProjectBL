using UnityEngine;

public class CursorController : MonoBehaviour
{
    [Tooltip("Hide the cursor when the game starts")]
    [SerializeField]
    private bool hideCursor = true;

    public bool HideCursor
    {
        get => hideCursor;
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

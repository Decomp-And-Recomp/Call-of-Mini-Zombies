using UnityEngine;
using UnityEngine.SceneManagement;

public class TUITextField : TUIControlImpl
{
    public TextFieldType text_type;
    public Vector2 positon = Vector2.zero;
    public int length = 6;
    public GUIStyle style;

    protected string textToEdit = string.Empty;

    public Vector2 tex_off = Vector2.zero;
    public Vector2 content_off = Vector2.zero;

    public OnTextFieldActive callback;

    public new void Start()
    {
        base.Start();
    }

    private void OnGUI()
    {
        float scale = ResetResolution();
        float centerX = Screen.width / 2f;
        float centerY = Screen.height / 2f;
        float left = centerX + (transform.localPosition.x + positon.x) - (size.x * scale) / 2f;
        float top = centerY - (transform.localPosition.y + positon.y) - (size.y * scale) / 2f;

        if (SceneManager.GetActiveScene().name == "NickNameTUI")
        {
            top -= 35f * scale;
        }

        Rect fieldRect = new Rect(left, top, size.x * scale, size.y * scale);

        if (text_type == TextFieldType.text)
        {
            textToEdit = GUI.TextArea(fieldRect, textToEdit, length, style);
        }
        else if (text_type == TextFieldType.password)
        {
            textToEdit = GUI.PasswordField(fieldRect, textToEdit, '*', length, style);
        }

        if (Event.current.type == EventType.KeyDown && Event.current.character == '\n')
        {
            Debug.Log("Sending login request");
        }

        if (textToEdit != null && callback != null)
        {
            callback();
        }
    }

    public float ResetResolution()
    {
        float referenceWidth = 640f;
        return Screen.width / referenceWidth;
    }

    public string GetText()
    {
        return textToEdit;
    }

    public void ResetText()
    {
        textToEdit = string.Empty;
    }
}
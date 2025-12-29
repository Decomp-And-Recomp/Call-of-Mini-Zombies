using UnityEngine;

public class PasswordPanelManager : MonoBehaviour
{
	public bool createOrJoin = true;

	public TUITextField textInput;

	public TUIMeshSprite passwordActive;

	private void Start()
	{
        if (createOrJoin)
        {
            int scale = Mathf.RoundToInt(textInput.ResetResolution());
            textInput.style.overflow.left *= scale;
            textInput.style.padding.right *= scale;
            textInput.style.padding.left *= scale;
            textInput.callback = OnTextFieldActive;
        }
        else
        {
            int scale = Mathf.RoundToInt(textInput.ResetResolution());
            textInput.style.overflow.left *= scale;
            textInput.style.overflow.right *= scale;
            textInput.style.padding.right *= scale;
            textInput.style.padding.left *= scale;
        }
    }

	private void Update()
	{
	}

	private void OnTextFieldActive()
	{
		if (textInput.GetText().Length > 0)
		{
			passwordActive.frameName_Accessor = "mima3";
		}
		else
		{
			passwordActive.frameName_Accessor = "mima2";
		}
	}
}

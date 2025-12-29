using UnityEngine;

public class ShopUIAvatarMover : MonoBehaviour
{
	public GameObject slider_obj;

	public float rotate_val = 1f;

	protected float last_x;

	protected Vector3 ori_pos;

	protected bool is_start_move;

	private void Start()
	{
		ori_pos = base.transform.localPosition;
		if ((bool)GetComponent<TUIButtonSliderCallBack>())
		{
			GetComponent<TUIButtonSliderCallBack>().Begin_delegate = OnSliderBegin;
			GetComponent<TUIButtonSliderCallBack>().move_delegate = OnSliderMove;
			GetComponent<TUIButtonSliderCallBack>().end_delegate = OnSliderEnd;
		}
	}

	private void Update()
	{
	}

	private void OnSliderBegin(TUIInput input)
	{
		last_x = input.position.x;
	}

	private void OnSliderMove(TUIInput input)
	{
		if (slider_obj != null)
		{
			slider_obj.transform.eulerAngles = new Vector3(0f, slider_obj.transform.eulerAngles.y - (input.position.x - last_x) * rotate_val, 0f);
			last_x = input.position.x;
		}
	}

	private void OnSliderEnd(TUIInput input)
	{
		base.transform.localPosition = ori_pos;
	}
}

using UnityEngine;

public class ContentCorrectionPos : MonoBehaviour
{
	private Transform transformActive;
	
	private void Awake()
	{
		transformActive = GetComponent<Transform>();
	}

	private void Start()
	{
		if (transformActive.localPosition.y > 1)
		{
			transformActive.localPosition = new Vector2(transformActive.localPosition.x, 0);
		}
	}
}

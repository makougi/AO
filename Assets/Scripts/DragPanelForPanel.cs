﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DragPanelForPanel : MonoBehaviour, IPointerDownHandler, IDragHandler {

	public Vector2 originalLocalPointerPosition;
	public Vector3 originalPanelLocalPosition;
	public RectTransform panelRectTransform;
	public RectTransform parentRectTransform;

	void Awake () {
		panelRectTransform = transform.parent as RectTransform;
		parentRectTransform = panelRectTransform.parent as RectTransform;
	}

	public void OnPointerDown (PointerEventData data) {
		originalPanelLocalPosition = panelRectTransform.localPosition;
		RectTransformUtility.ScreenPointToLocalPointInRectangle (parentRectTransform, data.position, data.pressEventCamera, out originalLocalPointerPosition);
	}

	public void OnDrag (PointerEventData data) {
		if (panelRectTransform == null || parentRectTransform == null)
			return;

		Vector2 localPointerPosition;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (parentRectTransform, data.position, data.pressEventCamera, out localPointerPosition)) {
			Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
			panelRectTransform.localPosition = originalPanelLocalPosition + offsetToOriginal;
		}

		ClampToWindow ();
	}

	// Clamp panel to area of parent
	private void ClampToWindow () {
		Vector3 pos = panelRectTransform.localPosition;

		RectTransform parentParentRectTransform = parentRectTransform.parent as RectTransform;

		Vector3 minPosition = transform.InverseTransformPoint (transform.parent.parent.TransformPoint (parentParentRectTransform.rect.min - panelRectTransform.rect.min));
		Vector3 maxPosition = transform.InverseTransformPoint (transform.parent.parent.TransformPoint (parentParentRectTransform.rect.max - panelRectTransform.rect.max));

		pos.x = Mathf.Clamp (panelRectTransform.localPosition.x, minPosition.x, maxPosition.x);
		pos.y = Mathf.Clamp (panelRectTransform.localPosition.y, minPosition.y, maxPosition.y);

		panelRectTransform.localPosition = pos;
		Debug.Log ("min " + minPosition);
		Debug.Log ("max " + maxPosition);
		Debug.Log ("juuh");
	}
}

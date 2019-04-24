using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomInputField : InputField {

	public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
	{
		if (isFocused)
		{
			Vector2 mPos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(textComponent.rectTransform, eventData.position, eventData.pressEventCamera, out mPos);
			Vector2 cPos = GetLocalCaretPosition();
			Debug.Log("C:" + cPos + ", M:" + mPos);
		}
		base.OnPointerDown(eventData);
	}

	public Vector2 GetLocalCaretPosition()
	{
		TextGenerator gen = m_TextComponent.cachedTextGenerator;
		int mCaretPosition = caretPosition;

		if (mCaretPosition >= gen.characters.Count - 1)
			mCaretPosition = gen.characters.Count - 1;
		else if (mCaretPosition < 0)
			mCaretPosition = 0;
		if (isFocused)
		{
			UICharInfo charInfo = gen.characters[mCaretPosition];
			float x = (charInfo.cursorPos.x + charInfo.charWidth) / m_TextComponent.pixelsPerUnit;
			float y = (charInfo.cursorPos.y) / m_TextComponent.pixelsPerUnit;
			return new Vector2(x, y);
		}
		else
			return new Vector2(-465.0f, 95.0f);
	}

}

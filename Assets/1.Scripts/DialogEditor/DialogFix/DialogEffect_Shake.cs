using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogEffect_Shake : DialogEffect {

	float duration = 1.0f;

	public DialogEffect_Shake()
	{
		effectName = DialogEffectName.Shake;
	}
	public DialogEffect_Shake(float _duration)
	{
		duration = _duration;
		effectName = DialogEffectName.Shake;
	}
	public override IEnumerator PlayEffect()
	{
		yield return null;
		
	}
	
}

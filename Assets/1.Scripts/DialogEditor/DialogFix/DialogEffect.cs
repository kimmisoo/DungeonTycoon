using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogEffectName {Shake, FadeIn, FadeOut, DialogStart, DialogEnd, GrayScaleOn, GrayScaleOff, BackgroundChange, IllustrationChangeL, IllustrationChangeR,
BigImageChange, SmallImageChange}

public class DialogEffect{

	public DialogEffectName effectName;
	public virtual IEnumerator PlayEffect()
	{
		yield return null;
	}

	
}

/*
 * 카메라에 컴포넌트로 삽입
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Grayscale : MonoBehaviour
{
	#region Variables
	public Shader curShader;
	private Material curMaterial;
	[Range(-1.0f, 1.0f)]
	public float grayScaleAmount = 1.0f;
	#endregion

	#region Properties
	Material material
	{
		get
		{
			if (curMaterial == null)
			{
				curMaterial = new Material(curShader);
				curMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return curMaterial;
		}
	}
	#endregion

	void Start()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}

		if (!curShader && !curShader.isSupported)
		{
			enabled = false;
		}
	}

	void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if (curShader != null)
		{
			material.SetFloat("_LuminosityAmount", grayScaleAmount);
			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);
		}
	}

	/*
    void Update()
    {
        grayScaleAmount = Mathf.Clamp(grayScaleAmount, 0.01f, 1.0f);
    }
    */

	void OnDisable()
	{
		if (curMaterial)
		{
			DestroyImmediate(curMaterial);
		}
	}
}

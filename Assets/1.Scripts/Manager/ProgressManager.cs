using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class ProgressManager : MonoBehaviour {


	//씬 스토리 진행 관련 매니저
	//현재 선택한 캐릭터 스토리 JSON..
	//어느 시점에서 대화 띄울지 정보
	//-> 씬 번호, 클리어한 방 기준? JSON
	//

	
	JSONNode progressJson;
	
	public static ProgressManager Instance
	{
		get
		{
			if (_instance != null)
				return _instance;
			else
			{
				Debug.Log("ProgressManager is Null..");
				return null;
			}
		}
	}

	private static ProgressManager _instance;

	private void Awake()
	{
		_instance = this;
		LoadProgressData();
		
	}
	// Use this for initialization
	public void LoadProgressData()
	{
		progressJson = SimpleJSON.JSON.Parse(Resources.Load<TextAsset>("SceneData/Progress").ToString());
		
	}
}

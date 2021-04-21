using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public enum SpAdvNames
{
	Hana, Iris, Maxi, Murat, OldMan, Yeonhwa, Nyang, Wal
}
public class ProgressManager : MonoBehaviour {


	//씬 스토리 진행 관련 매니저
	//현재 선택한 캐릭터 스토리 JSON..
	//어느 시점에서 대화 띄울지 정보
	//-> 씬 번호, 클리어한 방 기준? JSON
	//

	
	JSONNode dialogBindingJson;
	public Dictionary<string, int> characterDialogProgressData;
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
		if (ProgressManager.Instance != null) // 이미 씬에 있다면(로드했거나 다음씬 진행했을때?)
		{
			Destroy(gameObject);
			return;
		}
		_instance = this;
		LoadProgressData();
		
	}
	private void Start()
	{
		GameObject.DontDestroyOnLoad(gameObject);
		if (SaveLoadManager.Instance.isLoadedGame == false)
		{
			characterDialogProgressData = new Dictionary<string, int>();
			for (int i = 0; i < System.Enum.GetNames(typeof(SpAdvNames)).Length; i++)
			{
				characterDialogProgressData.Add(System.Enum.GetName(typeof(SpAdvNames), i), 1);
			}
		}
	}
	// Use this for initialization
	public void LoadProgressData()
	{
		dialogBindingJson = SimpleJSON.JSON.Parse(Resources.Load<TextAsset>("SceneData/Progress").ToString());
		
	}


	#region Check
	public void SceneStarted(int sceneNum)
	{
		if(dialogBindingJson["stage"][sceneNum]["scenestart"] != null || ((string)dialogBindingJson["stage"][sceneNum]["scenestart"]).Length > 5) // 빈거 찾으면 어떻게되는지 테스트해봐야함.
			DialogManager.Instance.StartDialog(dialogBindingJson["stage"][sceneNum]["scenestart"]);
	}
	public void SceneEnded(int sceneNum)
	{

	}
	public void ConquerStarted(int sceneNum, int areaNum)
	{

	}
	public void ConquerEnded(int sceneNum, int areaNum)
	{

	}
	#endregion
}

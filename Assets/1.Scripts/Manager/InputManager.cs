using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleJSON;

public class InputManager : MonoBehaviour {

	static InputManager _instance;
	public static InputManager Instance
	{
		get
		{
			if (_instance == null)
				Debug.Log("InputManager is NULL!!");
			return _instance;
		}
	}

    public float moveSensitivityX = 1.0f;
    public float moveSensitivityY = 1.0f;

    public bool updateZoomSensitivity = true; // zoom시 x,y sensitivity 조절할것인지 

    public float orthoZoomSpeed = 0.05f;
    public float minZoom = 1.0f;
    public float maxZoom = 20.0f;
    public bool invertMoveX = false;
    public bool invertMoveY = false;
    public float mapWidth = 0.0f; // map 최대 사이즈 
    public float mapHeight = 0.0f;

    public float inertiaDuration = 5.0f; // 보간 

    private Camera _camera;

    private float minX, maxX, minY, maxY;
    private float horizontalExtent, verticalExtent;
    public float scrollVelocity = 0.0f;
    private float timeTouchPhaseEnded;
    public Vector2 scrollDirection = Vector2.zero;

    public bool isCameraMoving = false;
	public bool isObjectMoving = false;

    public GameObject test;
	
    public GameObject selectedObject;
	public GameObject selectEffect;

	private Tile tileWhileMove;

	public float sumMagnitude = 0.0f;

    // 로드 후 카메라 Clamp되는 거 방지용.
    public bool isLoading = false;
	

    // Use this for initialization
    void Start () {
		_instance = this;
        _camera = Camera.main;
		
        maxZoom = 0.5f * (mapWidth / _camera.aspect);
        if (mapWidth > mapHeight)
            maxZoom = 0.5f * mapHeight;

        if (_camera.orthographicSize > maxZoom)
            _camera.orthographicSize = maxZoom;
        CalculateLevelBounds();
		
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (StructureManager.Instance.GetisConstructing() == true)
                StructureManager.Instance.DestroyConstructing();
            else if (UIManager.Instance.GetisShowing() == true)
                UIManager.Instance.HideUI();
            else
            {
                Debug.Log("QUIT!");
                Application.Quit();
            }
        }

        //esc key 입력시 --

        //test.GetComponent<Text>().text = "X =" + _camera.transform.position.x.ToString() + "  XSize = "+Screen.width.ToString()+"\nY =" + _camera.transform.position.y.ToString() + "  YSize = "+Screen.height.ToString()+"\nZ =" + _camera.transform.position.z.ToString() + "\nv =" + scrollVelocity.ToString();
		//테스트용

        if (updateZoomSensitivity)
        {
            moveSensitivityX = _camera.orthographicSize / 15.0f;
            moveSensitivityY = _camera.orthographicSize / 15.0f;
        }

        Touch[] touches = Input.touches;
		
        if (touches.Length < 1)  // 슬라이드 관성 // 터치 놓았을때
        {
			
			//if the camera is currently scrolling
			if (scrollVelocity != 0.0f)
            {
                //slow down over time
                float t = (Time.time - timeTouchPhaseEnded) / inertiaDuration;
                float frameVelocity = Mathf.Lerp(scrollVelocity, 0.0f, 0.5f);

                _camera.transform.position += -(Vector3)scrollDirection.normalized * (frameVelocity * 0.05f) * Time.unscaledDeltaTime * (moveSensitivityX) * 0.2f;
                scrollVelocity = frameVelocity;
                if (t >= 0.5f)
                    scrollVelocity = 0.0f;
            }           
        }

        if (touches.Length > 0 && UIManager.Instance.GetisShowing() == false) //UI가 보이고 있을때는 먹통
		{
			
			//Single touch (move) // 터치가 하나일때
			if (touches.Length == 1)
			{
				sumMagnitude += touches[0].deltaPosition.magnitude;
				if (touches[0].phase == TouchPhase.Began) // 터치가 막 시작 되었을때 
				{
					scrollVelocity = 0.0f;
					GetSelectedObject(touches); //터치가 시작되면 selectedObject 받아옴(raycast)(tag 판별 위해서)
				}
				else if (touches[0].phase == TouchPhase.Moved && (selectedObject != null && selectedObject.tag == "Movable")) // 터치 후 이동 중 일때 && 오브젝트(건물) 이동
				{
					//Movable 오브젝트 이동
					if ((touches[0].deltaPosition.magnitude < 6 || sumMagnitude < 6) && isObjectMoving == false) // 터치 할때마다 화면이 움직이므로... 정지마찰력 역할
					{                                                                   // isCameraMoving은 왜 사용? ==> magnitude가 6 이상으로 카메라를 움직이기 시작했다가 
																						// 6미만으로 떨어지면 다시멈추는 현상을막으려고 사용
																						// 즉 한번 움직이기 시작하면 계속 움직일 수 있게하려고
																						//scrollDirection = touches [0].deltaPosition.normalized;
						
					}
					else
					{
						isObjectMoving = true;
						//Vector2 delta = touches[0].deltaPosition; // deltaPosition == 이전 프레임에서의 터치와 현재 프레임에서의 터치 사이의 벡터

						tileWhileMove = GetTileWhileMoving(touches);
						
						if (tileWhileMove != null && selectedObject.GetComponent<Structure>().GetMovable() == true && StructureManager.Instance.GetisConstructing() == true)
						{
							StructureManager.Instance.MoveStructure(tileWhileMove);
						}
					}



				}
				else if (touches[0].phase == TouchPhase.Moved && (selectedObject == null || selectedObject.tag != "Movable")) // 터치 후 이동 중 일때 && 카메라 이동
				{
					//카메라이동
					if ( (touches[0].deltaPosition.magnitude < 6 || sumMagnitude < 6 ) && isCameraMoving == false) // 터치 할때마다 화면이 움직이므로... 정지마찰력 역할
					{                                                                   // isCameraMoving은 왜 사용? ==> magnitude가 6 이상으로 카메라를 움직이기 시작했다가 
																						// 6미만으로 떨어지면 다시멈추는 현상을막으려고 사용
																						// 즉 한번 움직이기 시작하면 계속 움직일 수 있게하려고
																						//scrollDirection = touches [0].deltaPosition.normalized;
						scrollVelocity = 0;
					}
					else
					{
						isCameraMoving = true;

						Vector2 delta = touches[0].deltaPosition; // deltaPosition == 이전 프레임에서의 터치와 현재 프레임에서의 터치 사이의 벡터

						float positionX = delta.x * moveSensitivityX * Time.unscaledDeltaTime;
						positionX = invertMoveX ? positionX : positionX * -1;

						float positionY = delta.y * moveSensitivityY * Time.unscaledDeltaTime;
						positionY = invertMoveY ? positionY : positionY * -1;

						_camera.transform.position += new Vector3(positionX, positionY, 0); // 

						scrollDirection = touches[0].deltaPosition.normalized;
						scrollVelocity = touches[0].deltaPosition.magnitude / touches[0].deltaTime;


						if (scrollVelocity <= 100)
							scrollVelocity = 0;
						if (scrollVelocity >= 5000)
							scrollVelocity = 5000;
					}
				}
				else if (touches[0].phase == TouchPhase.Ended) // 터치가 끝났을때
				{
					timeTouchPhaseEnded = Time.time;
					if (selectedObject != null)
					{
						Debug.Log(selectedObject.name);
					}
                    if (isCameraMoving == false) // touch일때
                    {
                        if (selectedObject == null)
                        {
							UnselectTile();
							UIManager.Instance.CharacterDeselected();
						}
						else if(selectedObject.tag == "Tile" && selectedObject.GetComponent<Tile>().GetIsActive() == true)
                        {
							SelectTile();
                        }
						//캐릭터 선택 추가해야함...
						else if(selectedObject.tag == "SelectableCharacter")
						{
							if(UIManager.Instance.specPanel.curCharacter != null)
								UIManager.Instance.CharacterDeselected();
							UIManager.Instance.CharacterSelected(selectedObject.GetComponent<Traveler>());
						}
						else if(selectedObject.tag == "guard")
						{
							//do nothing
						}
                        else
                        {
							UnselectTile();
							UIManager.Instance.CharacterDeselected();
						}
                        
                    }
					if(isObjectMoving == false)
					{
						if (selectedObject != null && tileWhileMove != null && selectedObject.tag == "Movable" && selectedObject.GetComponent<Structure>().GetisConstructable() == true)
						{
							StructureManager.Instance.ConstructStructure();
						}
					}
                    isCameraMoving = false;
					isObjectMoving = false;
                    selectedObject = null;
					sumMagnitude = 0.0f;

                }
            }


            //Double touch (zoom)
            if (touches.Length == 2)
            {
                isCameraMoving = true;
				isObjectMoving = false;

                Vector2 cameraViewsize = new Vector2(_camera.pixelWidth, _camera.pixelHeight);

                Touch touchOne = touches[0];
                Touch touchTwo = touches[1];

                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
                Vector2 touchTwoPrevPos = touchTwo.position - touchTwo.deltaPosition;

                float prevTouchDeltaMag = (touchOnePrevPos - touchTwoPrevPos).magnitude;
                float touchDeltaMag = (touchOne.position - touchTwo.position).magnitude;

                float deltaMagDiff = prevTouchDeltaMag - touchDeltaMag;

                //_camera.transform.position += _camera.transform.TransformDirection ((touchOnePrevPos + touchTwoPrevPos - cameraViewsize) * _camera.orthographicSize / cameraViewsize.y);

                _camera.orthographicSize += deltaMagDiff * orthoZoomSpeed;
                _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, minZoom, maxZoom) - 0.001f;

                //_camera.transform.position -= _camera.transform.TransformDirection ((touchOne.position + touchTwo.position - cameraViewsize) * _camera.orthographicSize / cameraViewsize.y);

                CalculateLevelBounds();
            }


        }
    }
    void SelectTile()
    {
		
		selectEffect.transform.position = selectedObject.transform.position;
    }
	void UnselectTile()
	{
		selectEffect.transform.position = new Vector3(9000.0f, 9000.0f, 0.0f);
    }
    void CalculateLevelBounds()
    {
        verticalExtent = _camera.orthographicSize;
        horizontalExtent = _camera.orthographicSize * Screen.width / Screen.height;
        minX = horizontalExtent - mapWidth / 2.0f;
        maxX = mapWidth / 2.0f - horizontalExtent;
        minY = verticalExtent - mapHeight / 2.0f;
        maxY = mapHeight / 2.0f - verticalExtent;
    }

    void LateUpdate()
    {
        Vector3 limitedCameraPosition = _camera.transform.position;
        limitedCameraPosition.x = Mathf.Clamp(limitedCameraPosition.x, minX, maxX);
        limitedCameraPosition.y = Mathf.Clamp(limitedCameraPosition.y, minY, maxY);
        if (isLoading != true)
        {
            _camera.transform.position = limitedCameraPosition;
            isLoading = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapWidth, mapHeight, 0));
    }
	Tile GetTileWhileMoving(Touch[] ts)
	{
		RaycastHit2D[] hits;
		Vector2 pos = Camera.main.ScreenToWorldPoint(ts[0].position);
		hits = Physics2D.RaycastAll(pos, Vector2.zero, 0.0f);
		foreach (RaycastHit2D hit in hits)
		{
			if (hit.collider.tag == "Tile" || hit.collider.tag =="non_Tile")
			{
				return hit.collider.gameObject.GetComponent<Tile>();
			}
		}
		return null;
	}
    void GetSelectedObject(Touch[] ts)
    {
        //레이캐스트로 Object 받아오기
        RaycastHit2D hit;
		Vector2 pos = Camera.main.ScreenToWorldPoint(ts[0].position);
		hit = Physics2D.Raycast(pos, Vector2.zero, 0.0f);
        if (hit.collider != null)
		{ 
            if (hit.collider.gameObject != selectedObject)
            {
                selectedObject = hit.collider.gameObject;
				Debug.Log("selected! / " + selectedObject.name);
               // Debug.Log(selectedObject.GetComponent<Tile>().GetX().ToString() + ", " + selectedObject.GetComponent<Tile>().GetY().ToString() + " is Selected!");
            }
		}
        else
        {
            selectedObject = null;
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Traveler1 : Character {

	enum Type
	{ Human, Elf, Dwarf, Orc, Dog, Cat}
	Type type;
	public Tile dest;
    const int _typeSize = 6;
	public int typeSize { get { return _typeSize; } }
    Structure destStructure;

	// Use this for initialization
	protected override void Start () {
		base.Start();//
        
        
    }
	protected override void Init()
	{
		base.Init();
		type = (Type)Random.Range(0, typeSize);
	}
	protected override void Activate()
	{
        
    } 
    void OnEnable()
    {
		Init();
		Activate();
		StartCoroutine("Acting");
	}
	void OnDisable()
	{
		Deactivate();
	}
	protected override void Deactivate()
	{
		//transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);
		//저장해놔야 할것들이라던지 ~~
		//

		gameObject.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);
	}
	protected override IEnumerator Acting()
	{
        int index = 0;
        List<Structure> strs = StructureManager.Instance.structures;
        TileForMove dest;
		
		moveto.SetCurPos(GameManager.Instance.GetRandomEntrance());
		int wanderCount = 0;
        while (true)
		{
			
			yield return null;
            if (strs.Count >= 1) // 건물이 하나라도 있으면?
            {
                int x, y;
                index = Random.Range(0, strs.Count);
                destStructure = strs[index];
                x = strs[index].entrance[0].GetX();
                y = strs[index].entrance[0].GetY();
                moveto.destination = map.GetLayer(0).GetComponent<TileLayer>().GetTileAsComponent(x, y);

				

            }
            else // 건물이없을때 -> Random Wander
            {
                do
                {
                    moveto.destination = map.GetLayer(0).GetComponent<TileLayer>().GetTileAsComponent(Random.Range((int)0, (int)50), Random.Range((int)0, (int)50)); // 범위 수정해줘야ㅏ함.
                    yield return null;
                } while (moveto.destination == null);
            }
            yield return StartCoroutine("MovetoTarget");
            if(destStructure != null)
            {
                GameManager.Instance.AddGold(destStructure.charge);
                destStructure = null;
            }

			wanderCount++;

			if (wanderCount >= 1)
				break;
        }


		/////////Exit Map;;

		moveto.destination = GameManager.Instance.GetRandomEntrance();
		yield return StartCoroutine("MovetoTarget");

		gameObject.SetActive(false);


	}


	
	IEnumerator MovetoTarget()
	{
		yield return null;
		
		 // init 할때 설정
		
		//acting 영역
		yield return null;
			
            
		yield return StartCoroutine(moveto.Moves());
        //acting() 영역 -/ Target 설정!
        this.transform.position = moveto.myCurPos.GetPosition() + Vector3.up * 0.166f;

        int pre = 0;
	    for (int i = 0; i < moveto.GetPath().Count-1; i++)
		{

			//yield return new WaitForSeconds(0.1f);
				
			if (moveto.GetPath()[i+1].myTilePos.GetX() - moveto.GetPath()[i].myTilePos.GetX() == 1) // move down right
			{
					
				//spriteRenderer.flipX = true;
				if(pre != 1 && pre !=3)
					animator.SetTrigger("MDL");
				pre = 1;
					
			}
			else if (moveto.GetPath()[i + 1].myTilePos.GetX() - moveto.GetPath()[i].myTilePos.GetX() == -1) // move up left
			{
					
				//spriteRenderer.flipX = false;
				if(pre != 2 && pre != 4)
					animator.SetTrigger("MUL");
				pre = 2;
			}
			else if (moveto.GetPath()[i + 1].myTilePos.GetY() - moveto.GetPath()[i].myTilePos.GetY() == 1) // move down left
			{
				//spriteRenderer.flipX = false;
				if(pre != 3 && pre != 1)
					animator.SetTrigger("MDL");
				pre = 3;
			}
			else // move up right
			{
				
				//spriteRenderer.flipX = true;
				if(pre != 4 && pre != 2)
					animator.SetTrigger("MUL");
				pre = 4;
			}			
			yield return StartCoroutine(anim(moveto.GetPath()[i].myTilePos.GetPosition(), moveto.GetPath()[i + 1].myTilePos.GetPosition()));//
			moveto.SetCurPos(moveto.GetPath()[i + 1].myTilePos);
			this.transform.position = moveto.GetCurPos().GetPosition() + Vector3.up * 0.166f;
			
		}
			
			
		// idle 상태.. 
		if (pre == 1) // down right
		{
			//spriteRenderer.flipX = true;
			animator.SetTrigger("IDL");
		}
		else if(pre == 2) // up left
		{
			//spriteRenderer.flipX = false;
			animator.SetTrigger("IUL");
		}
		else if(pre == 3) // down left
		{
			//spriteRenderer.flipX = false;
			animator.SetTrigger("IDL");
		}
		else // up right
		{
			//spriteRenderer.flipX = true;
			animator.SetTrigger("IUL");
		}
			
		

	}
	IEnumerator anim(Vector3 start, Vector3 end)
	{
		yield return null;
		Vector3 d = end - start;
		//po.transform.position = end;
		for(int i = 0; i<20; i++)
		{
			yield return new WaitForSeconds(0.03f);
			transform.position += d * 0.05f ;
		}
	}
	
    
}

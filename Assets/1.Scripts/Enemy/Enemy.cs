using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    
    public enum State
    { Idle, Move, Hunt, Indoor }


    protected TileMap map;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    protected Moveto moveto;
    protected State curState = State.Idle;
    

    public float damageFactor = 1.0f;
    public float damageFactorConstant = 0.0f;
    public float damage = 0.0f;
    public float health;
    /*// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}*/


}

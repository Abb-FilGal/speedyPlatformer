using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    public bool isFacingRight;

    public float moveSpeed;
    public float MaxSpeed;
    public float jumpForce;
    public float moveHorizontal;
    public float moveVertical;
    public bool isAirborne;
    public bool isJumpFalling;
    public float runAcceleration;
    public float runDecceleration;
    public float runAccAmount;
    public float runDeccAmount;
    public float runMaxSpeed;
    public float accInAir;
    public float decInAir;
    public bool OnWall;
    public float wallJumpX;
    public float WallJumpY;


    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        isAirborne = false;
        isFacingRight = true;

        moveSpeed = 5f;
        wallJumpX = 10f;
        WallJumpY = 10f;
        MaxSpeed = 10f;
        jumpForce = 10f;
        accInAir = 0.5f;
        decInAir = 0.5f;
        runAcceleration = 10f;
        runDecceleration = 10f;
		runAccAmount = (50 * runAcceleration) / MaxSpeed;
		runDeccAmount = (50 * runDecceleration) / MaxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveVertical = Input.GetAxisRaw("Vertical");

        if(isAirborne && rb.velocity.y < 0){
            isJumpFalling = true;
            SetGravityScale(1.5f);
        }
        if(Input.GetKeyDown("space") && !isAirborne){
            if(OnWall){
                WallJump();
            }
            else{
                Jump();
            }
        }
    }

    void FixedUpdate(){
        CheckDirectionToFace(moveHorizontal > 0);
        Run();
    }

    void Run(){
        
        float targetSpeed = moveHorizontal * MaxSpeed;

        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, 0.1f);

        float AccRate;

        if(isAirborne){
            AccRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccAmount : runDeccAmount;
        }
        else{
            AccRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccAmount * accInAir : runDeccAmount * decInAir;
        }

        if(Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed)){
            AccRate = 0;
        }

        float SpeedDif = targetSpeed - rb.velocity.x;;

        float movement = AccRate * SpeedDif;

        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    void Jump(){

        // if(rb.velocity.y < 0){
        //     jumpForce -= rb.velocity.y;
        // }

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void WallJump(){
        Vector2 force = new Vector2 (wallJumpX, WallJumpY);
        force.x *= isFacingRight ? -1 : 1;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    void SetGravityScale(float scale){
        rb.gravityScale = scale;
    }

    void Turn(){
        Debug.Log("Turn");
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        isFacingRight = !isFacingRight;
    }

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.tag == "Ground"){
            isAirborne = false;
        }
        if(collision.gameObject.tag == "Wall"){
            OnWall = true;
        }
    }
    void OnTriggerExit2D(Collider2D collision){
        if(collision.gameObject.tag == "Ground"){
            isAirborne = true;
        }
        if(collision.gameObject.tag == "Wall"){
            OnWall = false;
        }
    }

    public void CheckDirectionToFace(bool isMovingRight){
        if(isMovingRight != isFacingRight){
            Turn();
        }
    }
}

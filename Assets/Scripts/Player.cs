using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    public TextMeshProUGUI WINTEXT;
    public float combo;
    public float multiplier = 1;
    public float moveSpeed = 5f;

    [SerializeField] TextMeshProUGUI combo_text;
    [SerializeField] TextMeshProUGUI multiplier_text;
    [SerializeField] Camera cam;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] Transform pogoEnd;
    [SerializeField] Transform wallCheck;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] Animator animator;
    [SerializeField] GameObject canvas;
    [SerializeField] SliderJoint2D pogoJoint;
    Rigidbody2D rb;
    Vector3 startPos;
    public bool isDead { get; set; }
    bool isJumped = false;
    bool isGrounded = false;
    bool isTouchingWall = false;
    float jumpForce;
    float timeInComboRange;
    SavePlayerPos playerPosData;

    private void Awake()
    {
        playerPosData = FindObjectOfType<SavePlayerPos>();
    }

    void Start()
    {
        isDead = false;
        startPos = transform.position;
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.WakeUp();
    }

    void Update()
    {
        combo_text.text = "COMBO: " + combo.ToString();
        multiplier_text.text = "x " + string.Format("{0:0.0}", multiplier);

        multiplier = Mathf.Clamp(1 + combo / 10, 1, 2);

        isGrounded = IsGrounded();
        isTouchingWall = IsTouchingWall();

        PogoJump();
        RotatePlayer();
        Respawn();
        HandlePlayerAnimation();
        MoveInAir();
        MovePlayer();

        OnDeath();
    }
    private void OnDeath()
    {
        if (isDead)
        {
            canvas.SetActive(true);
        }
    }
    private void MovePlayer()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Win")
        {
            WINTEXT.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
        if (collision.gameObject.tag == "Rooks")
        {
            isDead = true;
            Time.timeScale = 0;
        }
    }
    private void HandlePlayerAnimation()
    {
        if (rb.velocity.y < 10 && rb.velocity.y > 1)
        {
            animator.SetBool("OnJump", true);
            isJumped = true;
        }
        else if (rb.velocity.y < -1)
        {
            animator.SetBool("OnFall", true);
            isJumped = false;
            Debug.Log("Fall");
        }
        else if (rb.velocity.y < 11 && isJumped == true)
        {
            Debug.Log("jump and fall");
            animator.SetBool("OnFall", true);
            isJumped = false;
        }
        else if (IsInComboRange())
        {
            isJumped = false;
            animator.SetBool("OnFall", false);
            animator.SetBool("OnHold", false);
            animator.SetBool("OnJump", false);
        }
    }

    private void PogoJump()
    {
        if ((Input.GetKey(KeyCode.Space) && isGrounded) || isTouchingWall)
        {
            jumpForce = Mathf.Clamp(jumpForce + Time.deltaTime * 15, 5, 25);
            animator.SetBool("OnHold", true);
        }
        else if (jumpForce > 0 && (isGrounded || isTouchingWall))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Vector2 direction = (mousePosition - transform.position).normalized;
            rb.AddForce(direction * jumpForce * multiplier, ForceMode2D.Impulse);

            jumpForce = 0;
            if (IsInComboRange())
            {
                combo += 1;
            }
            else
            {
                combo = 0;
            }
            animator.SetBool("OnHold", false);
        }

        if (IsInComboRange())
        {
            timeInComboRange += Time.deltaTime;
        }
        else
        {
            timeInComboRange = 0;
        }

        if (IsInComboRange() && timeInComboRange > 0.25f)
        {
            combo = 0;
        }
    }

    private void RotatePlayer()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = (mousePosition - transform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

        float angleDifference = Mathf.DeltaAngle(rb.rotation, targetAngle);
        float rotationSpeed = Mathf.Lerp(0.1f, 40, Mathf.Abs(angleDifference) / 180f);

        float rotateAmount = targetAngle - rb.rotation;
        rotateAmount = Mathf.Repeat(rotateAmount + 180f, 360f) - 180f;
        float rotateDirection = Mathf.Sign(rotateAmount);

        float torque = rotateDirection * rotationSpeed;

        rb.AddTorque(torque);
    }

    private bool IsInComboRange()
    {
        return Physics2D.CircleCast(pogoEnd.position, 0.3f, Vector2.zero, 0.3f, groundLayer);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(pogoEnd.position, 0.1f, groundLayer) != null;
    }

    private bool IsTouchingWall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.1f, wallLayer) != null;
    }

    private void Respawn()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = startPos;
            transform.rotation = new Quaternion();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            startPos = transform.position;
        }
    }
    public void SavePlayerPosition()
    {
        PlayerPrefs.SetFloat("PlayerX", transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", transform.position.y);
        PlayerPrefs.Save();
    }
    private IEnumerator PogoReset()
    {
        yield return new WaitForSeconds(0.2f);
        JointMotor2D motor = pogoJoint.motor;
        JointTranslationLimits2D limits = pogoJoint.limits;

        motor.motorSpeed = 0;
        limits.min = 1.26f;

        pogoJoint.motor = motor;
        pogoJoint.limits = limits;
    }
    private void MoveInAir()
    {
        if (rb.velocity.y < 0) 
        {
            float moveInput = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(moveInput * 5, rb.velocity.y);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnApplicationQuit()
    {
        SavePlayerPosition();
    }
}
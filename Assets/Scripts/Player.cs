using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Player : MonoBehaviour
{
    public float combo;
    public float multiplier = 1;
    public float moveSpeed = 5f; 

    [SerializeField] TextMeshProUGUI combo_text;
    [SerializeField] TextMeshProUGUI multiplier_text;
    [SerializeField] Camera cam;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform pogoEnd;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] SliderJoint2D pogoJoint;
    [SerializeField] Animator animator;

    Rigidbody2D rb;
    Vector3 startPos;

    bool isJumped = false;
    float jumpForce;
    float timeInComboRange;

    void Start()
    {
        startPos = transform.position;
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.WakeUp();
    }

    void Update()
    {
        combo_text.text = "COMBO: " + combo.ToString();
        multiplier_text.text = "x " + string.Format("{0:0.0}", multiplier.ToString());

        multiplier = Mathf.Clamp(1 + combo / 10, 1, 2);

        PogoJump();
        RotatePlayer();
        Respawn();
        HandlePlayerAnimation();
        MovePlayer();
    }

    private void MovePlayer()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
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
        if (Input.GetKey(KeyCode.Space))
        {
            JointTranslationLimits2D limits = pogoJoint.limits;
            JointMotor2D motor = pogoJoint.motor;

            limits.min = 0.88f;
            limits.max = 1.3f - 0.042f * jumpForce;

            pogoJoint.limits = limits;
            pogoJoint.motor = motor;

            animator.SetBool("OnHold", true);

            jumpForce = Mathf.Clamp(jumpForce + Time.deltaTime * 15, 0, 10);
        }
        else if (jumpForce > 0)
        {
            JointTranslationLimits2D limits = pogoJoint.limits;
            JointMotor2D motor = pogoJoint.motor;
            limits.max = 1.3f;
            pogoJoint.limits = limits;
            motor.motorSpeed = jumpForce * multiplier;

            StartCoroutine(PogoReset());
            pogoJoint.motor = motor;
            jumpForce = 0;

            if (IsInComboRange())
            {
                combo += 1;
            }
            else
            {
                combo = 0;
            }
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

    private void Respawn()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = startPos;
            transform.rotation = new Quaternion();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            SavePlayerPosition();
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

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnApplicationQuit()
    {
        SavePlayerPosition();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VatDiChuyenDuoc : MonoBehaviour
{
    public float VatTocVat;
    public bool DiChuyenTrai = true;
    public float LucDay = 100f; // Lực đẩy khi va chạm với người chơi

    private void FixedUpdate()
    {
        Vector2 DiChuyen = transform.localPosition;
        if (DiChuyenTrai)
            DiChuyen.x -= VatTocVat * Time.deltaTime;
        else
            DiChuyen.x += VatTocVat * Time.deltaTime;

        transform.localPosition = DiChuyen;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra va chạm với Platforms hoặc tường
        if (collision.contacts[0].normal.x > 0)
            DiChuyenTrai = false;
        else
            DiChuyenTrai = true;

        // Kiểm tra va chạm với người chơi
        if (collision.gameObject.CompareTag("Tường"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // Tính hướng đẩy
                Vector2 forceDirection = (collision.transform.position - transform.position).normalized;
                // Áp dụng lực đẩy
                playerRb.AddForce(forceDirection * LucDay, ForceMode2D.Impulse);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    private Rigidbody rb;
    private int collectedAmount;
    public float speed;
    public Text countText;
    public Text winText;

    private void setText()
    {
        countText.text = "Count: " + collectedAmount.ToString();
        winText.text = "";
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collectedAmount = 0;
        setText();
    }

    void FixedUpdate()
    {
        float moveHoriziontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHoriziontal, 0, moveVertical);
        
        rb.AddForce(movement * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            collectedAmount++;
            setText();
        }
        if (collectedAmount == 12)
        {
            winText.text = "You Win !";
        }
    }

}

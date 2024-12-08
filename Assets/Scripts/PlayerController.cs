using UnityEngine;

public class PlayerController : MonoBehaviour
{
    bool moving = false;

    void Start()
    {

    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            moving = true;
        }
        else
        {
            moving = false;
        }
    }
    private void FixedUpdate()
    {
        if (moving) {
            transform.position += new Vector3(0.1f, 0, 0);
        }
    }
}

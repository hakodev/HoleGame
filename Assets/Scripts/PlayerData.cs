using UnityEngine;

public class PlayerData
{
    public float Speed { get; private set; } = 6f;
    public float JumpForce { get; private set; } = 10f;
    public Vector3 velocity = Vector3.zero;

    public Vector3 CalculateMovement(Vector2 inputDirection)
    {
        return new Vector3(inputDirection.x, 0, inputDirection.y) * Speed;
    }

    public float CalculateJumpVelocity(float gravity)
    {
        return Mathf.Sqrt(JumpForce * -2f * gravity);
    }
}

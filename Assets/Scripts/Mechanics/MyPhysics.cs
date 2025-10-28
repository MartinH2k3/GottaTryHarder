using UnityEngine;

namespace Mechanics
{

public interface IPhysicsMovable
{
    Rigidbody2D Rigidbody { get;  }
}

public static class PhysicsMovableExtensions {
    /// <summary> Flips the localScale.x of the Rigidbody2D's transform to turn the object around. </summary>
    public static void TurnAround(this IPhysicsMovable movable) {
        var t = movable.Rigidbody.transform;
        t.localScale = new Vector3(-t.localScale.x, t.localScale.y, t.localScale.z);
    }

    /// <summary> Returns the size of the Rigidbody2D's collider bounds. </summary>
    public static Vector2 GetSize(this IPhysicsMovable movable) {
        var col = movable.Rigidbody.GetComponent<Collider2D>();
        if (col is not null) {
            return col.bounds.size;
        }
        return Vector2.zero;
    }

    /// <summary> Returns the width of the Rigidbody2D's collider bounds. </summary>
    public static float GetSizeX(this IPhysicsMovable movable) {
        var col = movable.Rigidbody.GetComponent<Collider2D>();
        if (col is not null) {
            return col.bounds.size.x;
        }
        return 0f;
    }

    /// <summary> Returns the height of the Rigidbody2D's collider bounds. </summary>
    public static float GetSizeY(this IPhysicsMovable movable) {
        var col = movable.Rigidbody.GetComponent<Collider2D>();
        if (col is not null) {
            return col.bounds.size.y;
        }
        return 0f;
    }

    /// <summary> Adds 2d vector to the Rigidbody2D's velocity. </summary>
    public static void AddVelocity(this IPhysicsMovable movable, Vector2 velocity) {
        if (movable.Rigidbody is not null) {
            movable.Rigidbody.linearVelocity += velocity;
        }
    }

    /// <summary> Adds passed X and Y to the Rigidbody2D's velocity. </summary>
    public static void AddVelocity(this IPhysicsMovable movable, float x, float y) {
        if (movable.Rigidbody is not null) {
            movable.Rigidbody.linearVelocity += new Vector2(x, y);
        }
    }

    /// <summary> Sets the Rigidbody2D's velocity with Vector2 as parameter. </summary>
    public static void SetVelocity(this IPhysicsMovable movable, Vector2 velocity) {
        if (movable.Rigidbody is not null) {
            movable.Rigidbody.linearVelocity = velocity;
        }
    }
    /// <summary> Sets the Rigidbody2D's velocity with x and y axes as parameters. </summary>
    public static void SetVelocity(this IPhysicsMovable movable, float x, float y) {
        if (movable.Rigidbody is not null) {
            movable.Rigidbody.linearVelocity = new Vector2(x, y);
        }
    }

    public static void SetVelocityX(this IPhysicsMovable movable, float x) {
        if (movable.Rigidbody is not null) {
            var v = movable.Rigidbody.linearVelocity;
            movable.Rigidbody.linearVelocity = new Vector2(x, v.y);
        }
    }

    public static void SetVelocityY(this IPhysicsMovable movable, float y) {
        if (movable.Rigidbody is not null) {
            var v = movable.Rigidbody.linearVelocity;
            movable.Rigidbody.linearVelocity = new Vector2(v.x, y);
        }
    }

    /// <summary>Adds a physics force to the Rigidbody2D (default ForceMode2D.Force).</summary>
    public static void AddForce(this IPhysicsMovable movable, Vector2 force, ForceMode2D mode = ForceMode2D.Force) {
        if (movable.Rigidbody is not null) {
            movable.Rigidbody.AddForce(force, mode);
        }
    }

    /// <summary>Adds a physics force to the Rigidbody2D (default ForceMode2D.Force).</summary>
    public static void AddForce(this IPhysicsMovable movable, float x = 0f, float y = 0f, ForceMode2D mode = ForceMode2D.Force) {
        if (movable.Rigidbody is not null) {
            movable.Rigidbody.AddForce(new Vector2(x, y), mode);
        }
    }

    /// <summary>
    /// Gradually accelerate or decelerate the X velocity toward a target speed.
    /// </summary>
    /// <param name="movable">Any object that exposes a Rigidbody2D via IPhysicsMovable.</param>
    /// <param name="targetSpeed">Desired horizontal velocity (units/second).</param>
    /// <param name="acceleration">Rate at which to approach target speed.</param>
    /// <param name="direction">Signed input direction.</param>
    /// <param name="voluntaryMovement">When the movement is done by player/NPC, also flips which way the sprite is looking.</param>
    public static void AccelerateHorizontally(
        this IPhysicsMovable movable,
        float targetSpeed,
        float acceleration,
        float direction = 0f,
        bool voluntaryMovement = true) {
        if (movable.Rigidbody is null) return;

        var v = movable.GetVelocity();
        float diff = targetSpeed - v.x;
        float force = diff * acceleration;

        movable.AddForce(force);

        // Snap to zero at very low speeds to avoid endless micro-drift
        v = movable.GetVelocity();
        if (Mathf.Abs(direction) < 0.01f && Mathf.Abs(v.x) < 0.05f)
            movable.SetVelocity(0f, v.y);

        // Flip facing direction if player initiated
        if (voluntaryMovement && Mathf.Abs(targetSpeed) > 0.01f) {
            var t = movable.Rigidbody.transform;
            t.localScale = new Vector3(Mathf.Sign(direction), t.localScale.y, t.localScale.z);
        }
    }

    /// <summary> Freezes all movement and rotation of the Rigidbody2D. </summary>
    public static void Freeze(this IPhysicsMovable movable) {
        movable.SetVelocity(Vector2.zero);
        movable.Rigidbody.angularVelocity = 0;
        movable.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        movable.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    /// <summary> Undoes physicsMovable.Freeze() method. </summary>
    public static void Unfreeze(this IPhysicsMovable movable) {
        movable.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        movable.Rigidbody.constraints = RigidbodyConstraints2D.None;
    }

    /// <summary> Returns the current velocity of the Rigidbody2D. </summary>
    public static Vector2 GetVelocity(this IPhysicsMovable movable) {
        return movable.Rigidbody.linearVelocity;
    }

    /// <summary> Returns true if the Rigidbody2D is moving along the specified axes. </summary>
    public static bool IsMoving(this IPhysicsMovable movable, bool includeX = true, bool includeY = true) {
        if (movable.Rigidbody is null) return false;

        var velocity = movable.Rigidbody.linearVelocity;
        return (includeX && Mathf.Abs(velocity.x) > Mathf.Epsilon) || (includeY && Mathf.Abs(velocity.y) > Mathf.Epsilon);
    }

    /// <summary> Returns the normalized direction of the Rigidbody2D's velocity. </summary>
    public static Vector2 GetDirection(this IPhysicsMovable movable) {
        return movable.Rigidbody.linearVelocity.normalized;
    }

    /// <summary> Returns true if the Rigidbody2D is set to Kinematic body type. </summary>
    public static bool IsKinematic(this IPhysicsMovable movable) {
        return movable.Rigidbody.bodyType == RigidbodyType2D.Kinematic;
    }

    /// <summary> Sets the Rigidbody2D's sleep mode to NeverSleep. </summary>
    public static void NeverSleep(this IPhysicsMovable movable) {
        if (movable.Rigidbody is not null) {
            movable.Rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
        }
    }

    /// <summary> Sets the Rigidbody2D's gravity scale. </summary>
    public static void SetGravityScale(this IPhysicsMovable movable, float scale) {
        if (movable.Rigidbody is not null) {
            movable.Rigidbody.gravityScale = scale;
        }
    }

    public static float GetGravityScale(this IPhysicsMovable movable) {
        if (movable.Rigidbody is not null) {
            return movable.Rigidbody.gravityScale;
        }
        return 0f;
    }
}

}
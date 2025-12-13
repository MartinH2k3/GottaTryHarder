using System;
using Enemies.States;
using Mechanics;
using Player;
using UnityEngine;

namespace Enemies.Golubok.States
{
public class Patrolling: EnemyState
{
    private float _oscillationStartTime;
    private PlayerController _player;
    private Vector2? _moveTowards;
    private float _lastSeekTime = 0f;
    public event Action PlayerDetected;

    public Patrolling(BaseEnemy enemy) : base(enemy) {}

    public override void Enter() {
        base.Enter();
        E.SetVelocity(Vector2.zero);
        _moveTowards = null;
        E.animator.SetTrigger("Idle");
        _oscillationStartTime = Time.time - E.movementStats.oscillationPeriod/4; // Start in middle of half-oscillation
    }

    public override void FixedTick() {
        base.FixedTick();
        Oscillate();
        if (_moveTowards.HasValue)
            FollowTarget();
        if (_lastSeekTime < Time.time - E.combatStats.detectionPeriod)
            SeekPlayer();
    }

    private void Oscillate() {
        var omega = 2 * Mathf.PI / E.movementStats.oscillationPeriod;
        var elapsed = Time.time - _oscillationStartTime;
        var targetSpeed = E.movementStats.oscillationAmplitude * Mathf.Sin(omega * elapsed);

        E.SetVelocityY(targetSpeed);
    }

    private void FollowTarget() {
        var dir = (_moveTowards!.Value - E.Pos).normalized;
        E.AddForce(dir*E.movementStats.walkSpeed);
    }

    private void SeekPlayer() {
        // If player within detection range circle, set player as target
        if (E.Target == null) {
            Collider2D playerCol = Physics2D.OverlapCircle(E.Pos, E.combatStats.detectionRange, E.playerLayer);
            if (playerCol != null)
                E.Target = playerCol.GetComponent<PlayerController>();
        }
        // if player still not found, wait
        if (E.Target  == null) return;

        // If player found, check line if accessible
        var combinedLayer = E.terrainLayer | E.playerLayer;

        float distanceToPlayer = Vector2.Distance(E.Pos, E.TargetPos);

        if (distanceToPlayer > E.combatStats.detectionRange) {
            _moveTowards = null;
            return;
        }

        Vector2 directionToPlayer = (E.TargetPos - E.Pos).normalized;
        Vector2 perpendicular = Vector2.Perpendicular(directionToPlayer);
        Vector2 perpendicular2 = -perpendicular;


        Debug.DrawRay(E.Pos, directionToPlayer * distanceToPlayer, Color.red, 0.1f);
        // 1. Direct line of sight - Send ray from enemy transform to player transform
        RaycastHit2D directHit = Physics2D.Raycast(E.Pos, directionToPlayer, distanceToPlayer,combinedLayer);
        if (directHit.collider != null &&
            ((1 << directHit.collider.gameObject.layer) & E.playerLayer) != 0) {
            PlayerDetected?.Invoke();
            return;
        }

        // 2. Around corner - Check if reachable around an obstacle in triangular path
        float maxTriangleHeight = Mathf.Sqrt(3)/2 * distanceToPlayer; // Don't go beyond equilateral triangle
        for (float height = maxTriangleHeight / 4; height <= maxTriangleHeight; height += maxTriangleHeight / 4) {
            if (ReachablePolygonally(E.TargetPos, distanceToPlayer, directionToPlayer, perpendicular, combinedLayer, height) ||
                ReachablePolygonally(E.TargetPos, distanceToPlayer, directionToPlayer, perpendicular2, combinedLayer, height))
                return;
        }

        _moveTowards = null;
    }

    private bool ReachablePolygonally(Vector2 playerPos, float distanceToPlayer, Vector2 directionToPlayer,
        Vector2 perpendicular, LayerMask combinedLayer, float shapeHeight) {
        if (ReachableTriangularly(playerPos, distanceToPlayer, directionToPlayer, perpendicular, combinedLayer, shapeHeight))
            return true;

        for (float parallelSideRate = 0.3f; parallelSideRate < 1f; parallelSideRate += 0.5f) {
            if (ReachableHexagonally(playerPos, distanceToPlayer, directionToPlayer, perpendicular, combinedLayer, shapeHeight, parallelSideRate))
                return true;
        }

        return false;
    }

    private bool ReachableTriangularly(Vector2 playerPos, float distanceToPlayer, Vector2 directionToPlayer, Vector2 perpendicular, LayerMask combinedLayer, float triangleHeight) {
        // Check in a triangular path (with side length equal to the distance to player)

        Vector2 otherTrianglePoint = E.Pos + directionToPlayer * distanceToPlayer/2 + perpendicular * triangleHeight;
        Debug.DrawRay(E.Pos, (otherTrianglePoint - E.Pos), Color.green, 0.1f);
        Debug.DrawRay(otherTrianglePoint, (playerPos - otherTrianglePoint), Color.green, 0.1f);

        var sideLength = Vector2.Distance(otherTrianglePoint, E.Pos);
        RaycastHit2D wallHit = Physics2D.Raycast(E.Pos, (otherTrianglePoint - E.Pos).normalized, sideLength, E.terrainLayer);
        if (wallHit.collider != null) return false;

        RaycastHit2D triangularHit = Physics2D.Raycast(otherTrianglePoint, (playerPos - otherTrianglePoint).normalized,
            sideLength, combinedLayer);

        if (triangularHit.collider != null && ((1 << triangularHit.collider.gameObject.layer) & E.playerLayer) != 0) {
            _moveTowards = otherTrianglePoint; // We know the player is reachable if bird goes via this point
            return true;
        }

        return false;
    }

    private bool ReachableHexagonally(Vector2 playerPos, float distanceToPlayer, Vector2 directionToPlayer, Vector2 perpendicular,
        LayerMask combinedLayer, float hexagonHeight, float parallelSideRate) {

        Vector2 point1 = E.Pos + perpendicular * hexagonHeight + directionToPlayer * (distanceToPlayer * (1- parallelSideRate) / 2);
        float shortSideLength = Vector2.Distance(point1, E.Pos);

        float longSideLength = distanceToPlayer * parallelSideRate;
        Vector2 point2 = point1 + directionToPlayer * longSideLength;


        Debug.DrawRay(E.Pos, (point1 - E.Pos).normalized*shortSideLength, Color.blueViolet, 0.1f);
        Debug.DrawRay(point1, (point2 - point1).normalized*longSideLength, Color.blueViolet, 0.1f);
        Debug.DrawRay(point2, (playerPos - point2).normalized*shortSideLength, Color.blueViolet, 0.1f);

        RaycastHit2D wallHit1 = Physics2D.Raycast(E.Pos, (point1 - E.Pos).normalized, shortSideLength, E.terrainLayer);
        if (wallHit1.collider != null) return false;

        RaycastHit2D wallHit2 = Physics2D.Raycast(point1, (point2 - point1).normalized, longSideLength, E.terrainLayer);
        if (wallHit2.collider != null) return false;

        RaycastHit2D hexagonalHit = Physics2D.Raycast(point2, (playerPos - point2).normalized,
            shortSideLength, combinedLayer);

        if (hexagonalHit.collider != null && ((1 << hexagonalHit.collider.gameObject.layer) & E.playerLayer) != 0) {
            _moveTowards = point1;
            return true;
        }

        return false;

    }
}
}
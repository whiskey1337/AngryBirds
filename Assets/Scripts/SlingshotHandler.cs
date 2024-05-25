using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SlingshotHandler : MonoBehaviour
{
    [Header("Line Renderers")]
    [SerializeField] private LineRenderer leftLineRenderer;
    [SerializeField] private LineRenderer rightLineRenderer;

    [Header("Transform References")]
    [SerializeField] private Transform leftLineRendererStartPosition;
    [SerializeField] private Transform rightLineRendererStartPosition;
    [SerializeField] private Transform centerPosition;
    [SerializeField] private Transform idlePosition;

    [Header("Slingshot Stats")]
    [SerializeField] private float maxDistance = 3.5f;
    [SerializeField] private float shotForce = 5f;

    [Header("Scripts")]
    [SerializeField] private SlingshotArea slingshotArea;

    [Header("Bird")]
    [SerializeField] private Bird redBirdPrefab;
    [SerializeField] private float birdPositionOffset = 2f;

    private Vector2 slingshotLinesPosition;

    private Vector2 direction;
    private Vector2 directionNormalized;

    private bool clickedWithinArea;

    private Bird spawnedRedBird;

    private void Awake()
    {
        leftLineRenderer.enabled = false;
        rightLineRenderer.enabled = false;

        SpawnBird();
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && slingshotArea.IsWithinSlingshotArea())
        {
            clickedWithinArea = true;
        }

        if (Mouse.current.leftButton.isPressed && clickedWithinArea)
        {
            DrawSlingshot();
            PositionAndRotationOfBird();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            clickedWithinArea = false;

            spawnedRedBird.LaunchBird(direction, shotForce);
        }
    }

    #region Slingshot Methods

    private void DrawSlingshot()
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        slingshotLinesPosition = centerPosition.position + Vector3.ClampMagnitude(touchPosition - centerPosition.position, maxDistance);

        SetLines(slingshotLinesPosition);

        direction = (Vector2)centerPosition.position - slingshotLinesPosition;
        directionNormalized = direction.normalized;
    }

    private void SetLines(Vector2 position)
    {
        if (!leftLineRenderer.enabled && !rightLineRenderer.enabled)
        {
            leftLineRenderer.enabled = true;
            rightLineRenderer.enabled = true;
        }

        leftLineRenderer.SetPosition(0, position);
        leftLineRenderer.SetPosition(1, leftLineRendererStartPosition.position);

        rightLineRenderer.SetPosition(0, position);
        rightLineRenderer.SetPosition(1, rightLineRendererStartPosition.position);
    }

    #endregion

    #region Bird Methods

    private void SpawnBird()
    {
        SetLines(idlePosition.position);

        Vector2 dir = (centerPosition.position - idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)idlePosition.position + dir * birdPositionOffset;

        spawnedRedBird = Instantiate(redBirdPrefab, spawnPosition, Quaternion.identity);
        spawnedRedBird.transform.right = dir;
    }

    private void PositionAndRotationOfBird()
    {
        spawnedRedBird.transform.position = slingshotLinesPosition + directionNormalized * birdPositionOffset;
        spawnedRedBird.transform.right = directionNormalized;
    }

    #endregion
}

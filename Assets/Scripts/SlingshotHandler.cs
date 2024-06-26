using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using DG.Tweening;

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
    [SerializeField] private Transform elasticTransform;

    [Header("Slingshot Stats")]
    [SerializeField] private float maxDistance = 3.5f;
    [SerializeField] private float shotForce = 5f;
    [SerializeField] private float timeBetweenBirdRespawns = 2f;
    [SerializeField] private float elasticDivider = 1.2f;
    [SerializeField] private AnimationCurve elasticCurve;
    [SerializeField] private float maxAnimationTime = 1f;

    [Header("Scripts")]
    [SerializeField] private SlingshotArea slingshotArea;
    [SerializeField] private CameraManager cameraManager;

    [Header("Bird")]
    [SerializeField] private Bird redBirdPrefab;
    [SerializeField] private float birdPositionOffset = 2f;

    [Header("Sounds")]
    [SerializeField] private AudioClip elasticPulledClip;
    [SerializeField] private AudioClip[] elasticReleasedClips;

    private Vector2 slingshotLinesPosition;

    private Vector2 direction;
    private Vector2 directionNormalized;

    private bool clickedWithinArea;
    private bool birdOnSlingshot;

    private Bird spawnedRedBird;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        leftLineRenderer.enabled = false;
        rightLineRenderer.enabled = false;

        SpawnBird();
    }

    void Update()
    {
        if (InputManager.WasLeftMouseButtonPressed && slingshotArea.IsWithinSlingshotArea())
        {
            clickedWithinArea = true;

            if (birdOnSlingshot)
            {
                SoundManager.instance.PlayClip(elasticPulledClip, audioSource);
                cameraManager.SwitchToFollowCamera(spawnedRedBird.transform);
            }
        }

        if (InputManager.IsLeftMouseButtonPressed && clickedWithinArea && birdOnSlingshot)
        {
            DrawSlingshot();
            PositionAndRotationOfBird();
        }

        if (InputManager.WasLeftMouseButtonReleased && birdOnSlingshot && clickedWithinArea)
        {
            if (GameManager.instance.HasEnoughShots())
            {
                clickedWithinArea = false;
                birdOnSlingshot = false;

                spawnedRedBird.LaunchBird(direction, shotForce);

                SoundManager.instance.PlayRandomClip(elasticReleasedClips, audioSource);

                GameManager.instance.UseShot();
                AnimateSlingshot();

                if (GameManager.instance.HasEnoughShots())
                {
                    StartCoroutine(SpawnBirdAfterTime());
                }
            }
        }
    }

    #region Slingshot Methods

    private void DrawSlingshot()
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);

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
        elasticTransform.DOComplete();
        SetLines(idlePosition.position);

        Vector2 dir = (centerPosition.position - idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)idlePosition.position + dir * birdPositionOffset;

        spawnedRedBird = Instantiate(redBirdPrefab, spawnPosition, Quaternion.identity);
        spawnedRedBird.transform.right = dir;

        birdOnSlingshot = true;
    }

    private void PositionAndRotationOfBird()
    {
        spawnedRedBird.transform.position = slingshotLinesPosition + directionNormalized * birdPositionOffset;
        spawnedRedBird.transform.right = directionNormalized;
    }

    private IEnumerator SpawnBirdAfterTime()
    {
        yield return new WaitForSeconds(timeBetweenBirdRespawns);

        SpawnBird();

        cameraManager.SwitchToIdleCamera();
    }

    #endregion

    #region Animate Slingshot

    private void AnimateSlingshot()
    {
        elasticTransform.position = leftLineRenderer.GetPosition(0);

        float dist = Vector2.Distance(elasticTransform.position, centerPosition.position);

        float time = dist / elasticDivider;

        elasticTransform.DOMove(centerPosition.position, time).SetEase(elasticCurve);
        StartCoroutine(AnimateSlingshotLines(elasticTransform, time));
    }

    private IEnumerator AnimateSlingshotLines(Transform trans, float time)
    {
        float elapsedTime = 0f;
        while(elapsedTime < time && elapsedTime < maxAnimationTime)
        {
            elapsedTime += Time.deltaTime;

            SetLines(trans.position);

            yield return null;
        }
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public TopDownCameraFollow cameraFollow;

    [Header("UI Buttons")]
    public Button kickButton;
    public Button autoKickButton;
    public Button resetButton;

    [Header("Settings")]
    public float kickDetectRadius = 1f;
    public float ballFlightDuration = 1.2f;
    public float ballArcHeight = 3f;
    public float cameraHoldAtGoal = 2f;

    private readonly List<Transform> balls = new List<Transform>();
    private readonly List<Transform> goals = new List<Transform>();
    private readonly HashSet<Transform> flyingBalls = new HashSet<Transform>();
    private Transform nearestBall;

    void Start()
    {
        // Tìm và lưu bóng
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Ball"))
        {
            balls.Add(go.transform);
        }

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Goal"))
            goals.Add(go.transform);

        kickButton.onClick.AddListener(OnKickClicked);
        autoKickButton.onClick.AddListener(OnAutoKickClicked);
        resetButton.onClick.AddListener(OnResetClicked);

        kickButton.gameObject.SetActive(false);
    }

    void Update()
    {
        UpdateNearestBall();
    }

    void UpdateNearestBall()
    {
        nearestBall = null;
        float minDist = float.MaxValue;

        foreach (Transform ball in balls)
        {
            if (flyingBalls.Contains(ball) || !ball.gameObject.activeSelf) continue;
            float d = Vector3.Distance(player.position, ball.position);
            if (d < minDist)
            {
                minDist = d;
                nearestBall = ball;
            }
        }

        bool showKick = nearestBall != null && minDist <= kickDetectRadius;
        kickButton.gameObject.SetActive(showKick);
    }

    void OnKickClicked()
    {
        if (nearestBall == null) return;
        StartCoroutine(KickBallRoutine(nearestBall));
    }

    void OnAutoKickClicked()
    {
        Transform farthest = null;
        float maxDist = -1f;

        foreach (Transform ball in balls)
        {
            if (flyingBalls.Contains(ball) || !ball.gameObject.activeSelf) continue;
            float d = Vector3.Distance(player.position, ball.position);
            if (d > maxDist)
            {
                maxDist = d;
                farthest = ball;
            }
        }

        if (farthest != null)
            StartCoroutine(KickBallRoutine(farthest));
    }

    void OnResetClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    Transform FindNearestGoal(Vector3 fromPos)
    {
        Transform nearest = null;
        float minDist = float.MaxValue;
        foreach (Transform g in goals)
        {
            float d = Vector3.Distance(fromPos, g.position);
            if (d < minDist) { minDist = d; nearest = g; }
        }
        return nearest;
    }

    IEnumerator KickBallRoutine(Transform ball)
    {
        flyingBalls.Add(ball);
        kickButton.gameObject.SetActive(false);

        Transform goal = FindNearestGoal(ball.position);
        if (goal == null)
        {
            flyingBalls.Remove(ball);
            yield break;
        }

        cameraFollow.SetTarget(ball);

        Vector3 start = ball.position;
        Vector3 end = goal.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / ballFlightDuration;
            float clampedT = Mathf.Clamp01(t);
            Vector3 pos = Vector3.Lerp(start, end, clampedT);
            pos.y += ballArcHeight * Mathf.Sin(Mathf.PI * clampedT);
            ball.position = pos;
            yield return null;
        }
        ball.position = end;

        yield return new WaitForSeconds(cameraHoldAtGoal);

        cameraFollow.SetTarget(player);

        ball.gameObject.SetActive(false);

        flyingBalls.Remove(ball);
    }
}
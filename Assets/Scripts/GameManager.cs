using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject ballPrefab;
    [SerializeField] private Transform ballSpawnPoint;

    [SerializeField] private GoalPost blueGoalPosts;
    [SerializeField] private GoalPost orangeGoalPosts;


    private NetworkVariable<int> blueScore = new(0);
    private NetworkVariable<int> orangeScore = new(0);


    public int BlueScore
    {
        get => blueScore.Value;
        private set => blueScore.Value = value;
    }

    public int OrangeScore
    {
        get => blueScore.Value;
        private set => blueScore.Value = value;
    }

    private void Start()
    {
        if (IsServer)
        {
            NetworkManager.Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity).Spawn();
        }
    }

    private void OnEnable()
    {
        blueGoalPosts.OnGoal += OnBlueGoal;
        orangeGoalPosts.OnGoal += OnOrangeGoal;
    }

    private void OnDisable()
    {
        blueGoalPosts.OnGoal -= OnBlueGoal;
        orangeGoalPosts.OnGoal -= OnOrangeGoal;
    }

    private void OnBlueGoal()
    {
        Debug.Log("Orange Scored!");
        if (IsServer)
        {
            OrangeScore++;
        }
    }

    private void OnOrangeGoal()
    {
        Debug.Log("Blue Scored!");
        if (IsServer)
        {
            BlueScore++;
        }
    }
}

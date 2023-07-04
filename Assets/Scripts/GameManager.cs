using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject ballPrefab;
    [SerializeField] private NetworkObject ballInstance;
    [SerializeField] private Transform ballSpawnPoint;

    [SerializeField] private NetworkObject artificialIntelligencePrefab;
    [SerializeField] private int numberOfArtificialIntelligences = 1;
    [SerializeField] private List<Transform> artificialIntelligenceSpawnPoints;

    [SerializeField] private GoalPost blueGoalPosts;
    [SerializeField] private GoalPost orangeGoalPosts;

    private readonly NetworkVariable<int> blueScore = new(writePerm: NetworkVariableWritePermission.Server);
    private readonly NetworkVariable<int> orangeScore = new(writePerm: NetworkVariableWritePermission.Server);

    private readonly List<ArtificialIntelligence> _artificialIntelligences = new();

    public int BlueScore
    {
        get => blueScore.Value;
        private set => blueScore.Value = value;
    }

    public int OrangeScore
    {
        get => orangeScore.Value;
        private set => orangeScore.Value = value;
    }

    private void Start()
    {
        if (IsServer)
        {
            ballInstance = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);

            ballInstance.Spawn();

            for (var i = 0; i < numberOfArtificialIntelligences; i++)
            {
                var artificialIntelligenceInstance = Instantiate(artificialIntelligencePrefab,
                    artificialIntelligenceSpawnPoints[i].position, artificialIntelligenceSpawnPoints[i].rotation);

                artificialIntelligenceInstance.Spawn();

                var artificialIntelligence = artificialIntelligenceInstance.GetComponent<ArtificialIntelligence>();

                artificialIntelligence.Target = ballInstance.transform;

                _artificialIntelligences.Add(artificialIntelligence);
            }
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

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 200, 200));
        GUILayout.Label($"Blue: {BlueScore}");
        GUILayout.Label($"Orange: {OrangeScore}");
        GUILayout.EndArea();
    }

    private void OnBlueGoal()
    {
        if (IsServer) OrangeScore++;
    }

    private void OnOrangeGoal()
    {
        if (IsServer) BlueScore++;
    }
}
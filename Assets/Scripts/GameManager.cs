using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject ballPrefab;
    [SerializeField] private Transform ballSpawnPoint;

    [SerializeField] private ArtificialIntelligence artificialIntelligencePrefab;
    [SerializeField] private int numberOfArtificialIntelligences = 1;
    [SerializeField] private List<Transform> artificialIntelligenceSpawnPoints;

    [SerializeField] private GoalPost blueGoalPosts;
    [SerializeField] private GoalPost orangeGoalPosts;

    private readonly NetworkVariable<int> blueScore = new(writePerm: NetworkVariableWritePermission.Server);
    private readonly NetworkVariable<int> orangeScore = new(writePerm: NetworkVariableWritePermission.Server);

    private List<ArtificialIntelligence> _artificialIntelligences = new();

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

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 200, 200));
        GUILayout.Label($"Blue: {BlueScore}");
        GUILayout.Label($"Orange: {OrangeScore}");
        GUILayout.EndArea();
    }

    private void Start()
    {
        if (IsServer)
        {
            Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity).Spawn();

            for (var i = 0; i < numberOfArtificialIntelligences; i++)
            {
                var artificialIntelligence = Instantiate(artificialIntelligencePrefab, artificialIntelligenceSpawnPoints[i].position, Quaternion.identity);
                artificialIntelligence.GetComponent<NetworkObject>().Spawn();

                artificialIntelligence.Ball = ballPrefab.gameObject;

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

    private void OnBlueGoal()
    {
        if (IsServer) OrangeScore++;
    }

    private void OnOrangeGoal()
    {
        if (IsServer) BlueScore++;
    }
}
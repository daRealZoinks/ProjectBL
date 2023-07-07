using System.Collections.Generic;
using LocalPlayer;
using Unity.Netcode;
using UnityEngine;

namespace Networking
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField] private NetworkObject ballPrefab;
        [SerializeField] private NetworkObject ballInstance;
        [SerializeField] private Transform ballSpawnPoint;

        [SerializeField] private NetworkObject artificialIntelligencePrefab;
        [SerializeField] private bool automaticNumberOfArtificialIntelligences = false;
        [SerializeField] private int numberOfArtificialIntelligences = 1;
        [SerializeField] private List<Transform> artificialIntelligenceSpawnPoints;

        [SerializeField] private GoalPost blueGoalPosts;
        [SerializeField] private GoalPost orangeGoalPosts;

        [SerializeField] private List<ArtificialIntelligence> artificialIntelligences = new();

        private readonly NetworkVariable<int> _blueScore = new(writePerm: NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<int> _orangeScore = new(writePerm: NetworkVariableWritePermission.Server);

        /// <summary>
        ///     The score of the blue team.
        /// </summary>
        public int BlueScore
        {
            get => _blueScore.Value;
            private set => _blueScore.Value = value;
        }

        /// <summary>
        ///     The score of the orange team.
        /// </summary>
        public int OrangeScore
        {
            get => _orangeScore.Value;
            private set => _orangeScore.Value = value;
        }

        private void Start()
        {
            if (!IsServer) return;

            ballInstance = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);

            ballInstance.Spawn();

            if (automaticNumberOfArtificialIntelligences)
                numberOfArtificialIntelligences = 6 - NetworkManager.Singleton.ConnectedClientsList.Count;

            for (var i = 0; i < numberOfArtificialIntelligences; i++)
            {
                var artificialIntelligenceInstance = Instantiate(artificialIntelligencePrefab,
                    artificialIntelligenceSpawnPoints[i].position, artificialIntelligenceSpawnPoints[i].rotation);

                artificialIntelligenceInstance.Spawn();

                var artificialIntelligence = artificialIntelligenceInstance.GetComponent<ArtificialIntelligence>();

                var ballInstanceTransform = ballInstance.transform;

                artificialIntelligence.MoveTarget = ballInstanceTransform;
                artificialIntelligence.LookTarget = ballInstanceTransform;

                artificialIntelligences.Add(artificialIntelligence);
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
}
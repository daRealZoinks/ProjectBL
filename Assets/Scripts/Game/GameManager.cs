using LocalPlayer;
using LocalPlayer.Player;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(CountdownTimer))]
    public class GameManager : NetworkBehaviour
    {
        [SerializeField] private NetworkObject ballPrefab;
        [SerializeField] private Transform ballSpawnPoint;

        [SerializeField] private NetworkObject artificialIntelligencePrefab;
        [SerializeField] private bool automaticNumberOfArtificialIntelligences;
        [SerializeField] private int numberOfArtificialIntelligences = 1;
        [SerializeField] private List<Transform> artificialIntelligenceSpawnPoints;

        [SerializeField] private GoalPost blueGoalPosts;
        [SerializeField] private GoalPost orangeGoalPosts;

        private NetworkObject _ballInstance;

        private readonly List<ArtificialIntelligence> _artificialIntelligences = new();

        private readonly NetworkVariable<int> _blueScore = new();

        private readonly NetworkVariable<int> _minutes = new();
        private readonly NetworkVariable<int> _orangeScore = new();
        private readonly NetworkVariable<bool> _paused = new();
        private readonly NetworkVariable<int> _seconds = new();

        private CountdownTimer _countdownTimer;

        /// <summary>
        ///     The score of the blue team.
        /// </summary>
        private int BlueScore
        {
            get => _blueScore.Value;
            set => _blueScore.Value = value;
        }

        /// <summary>
        ///     The score of the orange team.
        /// </summary>
        private int OrangeScore
        {
            get => _orangeScore.Value;
            set => _orangeScore.Value = value;
        }

        private void Start()
        {
            _countdownTimer = GetComponent<CountdownTimer>();

            if (!IsServer) return;

            _ballInstance = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);

            _ballInstance.Spawn();

            if (automaticNumberOfArtificialIntelligences)
                numberOfArtificialIntelligences = 6 - NetworkManager.Singleton.ConnectedClientsList.Count;

            for (var i = 0; i < numberOfArtificialIntelligences; i++)
            {
                var artificialIntelligenceInstance = Instantiate(artificialIntelligencePrefab,
                    artificialIntelligenceSpawnPoints[i].position, artificialIntelligenceSpawnPoints[i].rotation);

                artificialIntelligenceInstance.Spawn();

                var artificialIntelligence = artificialIntelligenceInstance.GetComponent<ArtificialIntelligence>();

                var ballInstanceTransform = _ballInstance.transform;

                artificialIntelligence.MoveTarget = ballInstanceTransform;
                artificialIntelligence.LookTarget = ballInstanceTransform;

                _artificialIntelligences.Add(artificialIntelligence);
            }

            _countdownTimer.Resume();
            _paused.Value = false;
            _countdownTimer.OnTimerExpired += OnTimerExpired;
        }

        private void Update()
        {
            if (!IsServer) return;

            _countdownTimer.Tick(Time.deltaTime);

            _minutes.Value = _countdownTimer.Minutes;
            _seconds.Value = _countdownTimer.Seconds;
        }

        private void OnEnable()
        {
            blueGoalPosts.OnGoal += OnBlueGoalAsync;
            orangeGoalPosts.OnGoal += OnOrangeGoalAsync;
        }

        private void OnDisable()
        {
            blueGoalPosts.OnGoal -= OnBlueGoalAsync;
            orangeGoalPosts.OnGoal -= OnOrangeGoalAsync;
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 200, 200));
            GUILayout.Label($"Blue: {BlueScore}");
            GUILayout.Label($"Orange: {OrangeScore}");
            GUILayout.Label($"{_minutes.Value:00}:{_seconds.Value:00}");
            if (_paused.Value) GUILayout.Label("Paused");
            GUILayout.EndArea();
        }

        private void OnTimerExpired()
        {
            Debug.Log("Timer expired!");
            // destroy ball 
            _ballInstance.Despawn();
        }

        private async void OnBlueGoalAsync()
        {
            if (IsServer)
            {
                OrangeScore++;
            }

            await ResetBallAsync();
        }

        private async void OnOrangeGoalAsync()
        {
            if (IsServer)
            {
                BlueScore++;
            }

            await ResetBallAsync();
        }

        // TODO: remake this function
        private async Task ResetBallAsync()
        {

            if (!_ballInstance) return;

            if (!IsServer) return;

            // Pause the timer
            _countdownTimer.Pause();
            _paused.Value = true;

            // Disable the ball
            DisableBallClientRpc();
            DisableBallServerRpc();

            // Wait for 3 seconds
            await Task.Delay(TimeSpan.FromSeconds(3f));

            // Reset the ball
            var ballInstanceTransform = _ballInstance.transform;
            ballInstanceTransform.SetPositionAndRotation(ballSpawnPoint.position, Quaternion.identity);

            _ballInstance.GetComponent<Rigidbody>().velocity = Vector3.zero;
            _ballInstance.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            // Enable the ball
            EnableBallClientRpc();
            EnableBallServerRpc();

            // Unpause the timer
            _countdownTimer.Resume();
            _paused.Value = false;
        }

        [ClientRpc]
        void DisableBallClientRpc()
        {
            _ballInstance.gameObject.SetActive(false);
        }

        [ServerRpc]
        void DisableBallServerRpc()
        {
            _ballInstance.gameObject.SetActive(false);
        }

        [ClientRpc]
        void EnableBallClientRpc()
        {
            _ballInstance.gameObject.SetActive(true);
        }

        [ServerRpc]
        void EnableBallServerRpc()
        {
            _ballInstance.gameObject.SetActive(true);
        }
    }
}
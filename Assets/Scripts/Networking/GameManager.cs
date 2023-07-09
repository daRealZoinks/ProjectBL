using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LocalPlayer;
using Unity.Netcode;
using UnityEngine;

namespace Networking
{
    [RequireComponent(typeof(CountdownTimer))]
    public class GameManager : NetworkBehaviour
    {
        [SerializeField] private NetworkObject ballPrefab;
        [SerializeField] private NetworkObject ballInstance;
        [SerializeField] private Transform ballSpawnPoint;

        [SerializeField] private NetworkObject artificialIntelligencePrefab;
        [SerializeField] private bool automaticNumberOfArtificialIntelligences;
        [SerializeField] private int numberOfArtificialIntelligences = 1;
        [SerializeField] private List<Transform> artificialIntelligenceSpawnPoints;

        [SerializeField] private GoalPost blueGoalPosts;
        [SerializeField] private GoalPost orangeGoalPosts;

        [SerializeField] private List<ArtificialIntelligence> artificialIntelligences = new();

        private readonly NetworkVariable<int> _blueScore = new();

        private readonly NetworkVariable<int> _minutes = new();
        private readonly NetworkVariable<int> _orangeScore = new();
        private readonly NetworkVariable<int> _seconds = new();

        private CountdownTimer _countdownTimer;

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
            _countdownTimer = GetComponent<CountdownTimer>();

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

            _countdownTimer.Resume();
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
            GUILayout.Label($"{_countdownTimer.Minutes:00}:{_countdownTimer.Seconds:00}");
            if (_countdownTimer.IsPaused) GUILayout.Label("Paused");
            GUILayout.EndArea();
        }

        private void OnTimerExpired()
        {
            Debug.Log("Timer expired!");
            // destroy ball 
            ballInstance.Despawn();
        }

        private async void OnBlueGoalAsync()
        {
            if (!IsServer) return;

            OrangeScore++;

            await ResetBallAsync();
        }

        private async void OnOrangeGoalAsync()
        {
            if (!IsServer) return;

            BlueScore++;

            await ResetBallAsync();
        }

        private async Task ResetBallAsync()
        {
            // Pause the timer
            _countdownTimer.Pause();

            // Disable the ball
            ballInstance.gameObject.SetActive(false);

            // Wait for 3 seconds
            await Task.Delay(TimeSpan.FromSeconds(3f));

            // Reset the ball
            var ballInstanceTransform = ballInstance.transform;
            ballInstanceTransform.position = ballSpawnPoint.position;
            ballInstanceTransform.rotation = Quaternion.identity;
            
            ballInstance.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ballInstance.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            // Enable the ball
            ballInstance.gameObject.SetActive(true);

            // Unpause the timer
            _countdownTimer.Resume();
        }
    }
}
using System.Linq;
using UnityEngine;

namespace GameFramework.Game
{
    public class TeamPod : MonoBehaviour
    {
        public bool IsReady { get; private set; }

        private void Update()
        {
            var podTransform = transform;
            var offset = (podTransform.right + podTransform.forward) * 2.5f + 3 * podTransform.up;

            var colliders = Physics.OverlapBox(podTransform.position + offset, 2.5f * Vector3.one, Quaternion.identity);

            var player = colliders.FirstOrDefault(c => c.CompareTag("Player"));

            // i know this doesnt make that much sense, but its checking if the player is null
            IsReady = player;
        }
    }
}
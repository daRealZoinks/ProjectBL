namespace LocalPlayer.Player
{
    public enum NetworkSmoothingMode
    {
        /// <summary>
        ///     No smoothing, only change position as network position updates are received.
        /// </summary>
        Disabled,


        /// <summary>
        ///     Linear interpolation from source to target.
        /// </summary>
        Linear,


        /// <summary>
        ///     Faster as you are further from target.
        /// </summary>
        Exponential
    }
}
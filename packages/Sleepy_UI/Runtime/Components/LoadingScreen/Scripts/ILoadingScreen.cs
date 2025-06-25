namespace Sleepy.UI
{
    public interface ILoadingScreen
    {
        /// <summary>
        /// Shows the loading screen.
        /// </summary>
        public void ShowLoadingScreen();

        /// <summary>
        /// Hides the loading screen.
        /// </summary>
        public void HideLoadingScreen();

        /// <summary>
        /// Update the progress(text or bar)
        /// </summary>
        /// <param name="progress"></param>
        public void UpdateLoadingScreen(float progress);
    }
}
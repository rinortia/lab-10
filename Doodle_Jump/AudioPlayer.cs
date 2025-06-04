using System.Media;
using WMPLib;

namespace Doodle_Jump
{
    public class AudioPlayer
    {
        private SoundPlayer backgroundPlayer;

        public AudioPlayer()
        {
            backgroundPlayer = new SoundPlayer();
        }

        public void PlayBackgroundMusic(string filePath)
        {
            try
            {
                backgroundPlayer.SoundLocation = filePath;
                backgroundPlayer.PlayLooping();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing music: {ex.Message}");
            }
        }

        public void StopBackgroundMusic()
        {
            backgroundPlayer.Stop();
        }
    }
}
using System.Media;

public class ExternalSound
{
    private SoundPlayer basePlayer;
    public SoundPlayer BasePlayer
    {
        get { return basePlayer; }
    }

    public ExternalSound(string filePath)
    {
        basePlayer = new SoundPlayer(filePath);
        basePlayer.Load();
    }

    public ExternalSound(System.IO.Stream fileStream)
    {
        basePlayer = new SoundPlayer(fileStream);
        basePlayer.Load();
    }

    public void Play()
    {
        if (basePlayer.IsLoadCompleted)
            basePlayer.Play();
    }

    public void Dispose()
    {
        if (this != null)
        {
            basePlayer.Stop();
            basePlayer.Dispose();
        }
    }
}


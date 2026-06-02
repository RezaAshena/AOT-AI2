namespace AOT_UI.Services;

public class DashboardState
{
    public DateTime LastRefreshedUtc { get; private set; } = DateTime.UtcNow;

    public void MarkRefreshed()
    {
        LastRefreshedUtc = DateTime.UtcNow;
    }
}

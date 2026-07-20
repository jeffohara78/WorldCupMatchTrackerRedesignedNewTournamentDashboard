namespace WorldCupMatchTrackerRedesigned;

public class Match
{
    public int Id { get; set; }

    public string Stage { get; set; } = "";
    public string Group { get; set; } = "";

    public string HomeTeam { get; set; } = "";
    public string AwayTeam { get; set; } = "";

    public int HomeScore { get; set; }
    public int AwayScore { get; set; }

    public string KnockoutWinner { get; set; } = "";

    public bool IsGroupMatch()
    {
        return Stage.StartsWith("Group", StringComparison.OrdinalIgnoreCase);
    }

    public bool IsKnockoutMatch()
    {
        return !IsGroupMatch();
    }

    public string GetScoreDisplay()
    {
        return $"{HomeTeam} {HomeScore} - {AwayScore} {AwayTeam}";
    }

    public string GetWinner()
    {
        if (HomeScore > AwayScore)
        {
            return HomeTeam;
        }

        if (AwayScore > HomeScore)
        {
            return AwayTeam;
        }

        if (!string.IsNullOrWhiteSpace(KnockoutWinner))
        {
            return KnockoutWinner;
        }

        return "Draw";
    }
}
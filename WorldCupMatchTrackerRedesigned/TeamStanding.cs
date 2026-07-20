namespace WorldCupMatchTrackerRedesigned;

public class TeamStanding
{
    public string TeamName { get; set; } = "";
    public string Group { get; set; } = "";

    public int Played { get; set; }
    public int Wins { get; set; }
    public int Draws { get; set; }
    public int Losses { get; set; }

    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }

    public int GoalDifference => GoalsFor - GoalsAgainst;
    public int Points => Wins * 3 + Draws;
}
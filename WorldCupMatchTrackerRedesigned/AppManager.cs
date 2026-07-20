namespace WorldCupMatchTrackerRedesigned;

public class AppManager
{
    private readonly JsonStorageService storageService = new JsonStorageService();
    private List<Match> matches = new List<Match>();

    public AppManager()
    {
        matches = storageService.LoadMatches();
    }

    public void Run()
    {
        bool running = true;

        while (running)
        {
            Console.Clear();
            ShowHeader();

            // Updated: July 20, 2026 at 2:44 PM MST
            // Added a Tournament Dashboard as the first menu option and
            // renumbered the existing application options.
            Console.WriteLine("1. View Tournament Dashboard");
            Console.WriteLine("2. Add Completed Match");
            Console.WriteLine("3. View All Matches");
            Console.WriteLine("4. View Group Standings");
            Console.WriteLine("5. View Qualified Teams");
            Console.WriteLine("6. View Knockout Bracket");
            Console.WriteLine("7. Edit Match");
            Console.WriteLine("8. Delete Match");
            Console.WriteLine("9. Save & Exit");
            Console.WriteLine();
            Console.Write("Choose an option: ");

            string choice = Console.ReadLine() ?? "";

            // Updated: July 20, 2026 at 2:44 PM MST
            // Added routing for the Tournament Dashboard and updated the
            // menu option numbers.
            switch (choice)
            {
                case "1":
                    ViewTournamentDashboard();
                    break;

                case "2":
                    AddCompletedMatch();
                    break;

                case "3":
                    ViewAllMatches();
                    break;

                case "4":
                    ViewGroupStandings();
                    break;

                case "5":
                    ViewQualifiedTeams();
                    break;

                case "6":
                    ViewKnockoutBracket();
                    break;

                case "7":
                    EditMatch();
                    break;

                case "8":
                    DeleteMatch();
                    break;

                case "9":
                    SaveData();
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    Pause();
                    break;
            }
        }
    }
    // Updated: July 20, 2026 at 2:44 PM MST
    // Added a tournament dashboard that summarizes match progress,
    // scoring, qualification status, stage activity, and the champion.
    private void ViewTournamentDashboard()
    {
        Console.Clear();
        ShowHeader();
        Console.WriteLine("=== Tournament Dashboard ===");
        Console.WriteLine();

        if (matches.Count == 0)
        {
            Console.WriteLine("No matches have been entered yet.");
            Console.WriteLine();
            Console.WriteLine("Add a completed match to begin tracking the tournament.");
            Pause();
            return;
        }

        List<Match> groupMatches = matches
            .Where(m => m.IsGroupMatch())
            .ToList();

        List<Match> knockoutMatches = matches
            .Where(m => m.IsKnockoutMatch())
            .ToList();

        int totalMatches = matches.Count;

        int totalGoals = matches.Sum(
            m => m.HomeScore + m.AwayScore);

        double averageGoals = totalMatches > 0
            ? (double)totalGoals / totalMatches
            : 0;

        int drawnMatches = matches.Count(
            m => m.HomeScore == m.AwayScore);

        int decisiveMatches = totalMatches - drawnMatches;

        List<TeamStanding> standings = CalculateStandings();

        int teamsTracked = standings
            .Select(s => s.TeamName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        int groupsTracked = standings
            .Select(s => s.Group)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        List<TeamStanding> qualifiedTeams = GetQualifiedTeams();

        Match? highestScoringMatch = matches
            .OrderByDescending(m => m.HomeScore + m.AwayScore)
            .ThenBy(m => m.Id)
            .FirstOrDefault();

        Match? finalMatch = matches
            .Where(m => m.Stage.Equals(
                "Final",
                StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(m => m.Id)
            .FirstOrDefault();

        Console.WriteLine("TOURNAMENT OVERVIEW");
        Console.WriteLine("-------------------");
        Console.WriteLine($"Total matches entered:       {totalMatches}");
        Console.WriteLine($"Group-stage matches:         {groupMatches.Count}");
        Console.WriteLine($"Knockout matches:            {knockoutMatches.Count}");
        Console.WriteLine($"Teams tracked:               {teamsTracked}");
        Console.WriteLine($"Groups represented:          {groupsTracked}/12");
        Console.WriteLine();

        Console.WriteLine("SCORING SUMMARY");
        Console.WriteLine("---------------");
        Console.WriteLine($"Total goals scored:          {totalGoals}");
        Console.WriteLine($"Average goals per match:     {averageGoals:F2}");
        Console.WriteLine($"Decisive matches:            {decisiveMatches}");
        Console.WriteLine($"Drawn matches:               {drawnMatches}");
        Console.WriteLine();

        Console.WriteLine("QUALIFICATION PROGRESS");
        Console.WriteLine("----------------------");
        Console.WriteLine(
            $"Current Round of 32 field:   {qualifiedTeams.Count}/32");

        DisplayQualificationProgressBar(qualifiedTeams.Count, 32);

        Console.WriteLine();

        Console.WriteLine("KNOCKOUT-STAGE PROGRESS");
        Console.WriteLine("-----------------------");
        DisplayStageProgress("Round of 32", 16);
        DisplayStageProgress("Round of 16", 8);
        DisplayStageProgress("Quarterfinal", 4);
        DisplayStageProgress("Semifinal", 2);
        DisplayStageProgress("Final", 1);

        Console.WriteLine();

        Console.WriteLine("TOURNAMENT HIGHLIGHTS");
        Console.WriteLine("---------------------");

        if (highestScoringMatch != null)
        {
            int matchGoals =
                highestScoringMatch.HomeScore +
                highestScoringMatch.AwayScore;

            Console.WriteLine(
                $"Highest-scoring match:       " +
                $"{highestScoringMatch.GetScoreDisplay()} " +
                $"({matchGoals} total goals)");
        }

        if (finalMatch != null)
        {
            Console.WriteLine(
                $"Tournament champion:         {finalMatch.GetWinner()}");
        }
        else
        {
            Console.WriteLine(
                "Tournament champion:         Not determined");
        }

        Pause();
    }
    // Updated: July 20, 2026 at 2:44 PM MST
    // Added a visual console progress bar for the Round of 32
    // qualification field.
    private void DisplayQualificationProgressBar(
        int currentTeams,
        int requiredTeams)
    {
        const int barWidth = 32;

        double completionPercentage = requiredTeams > 0
            ? (double)currentTeams / requiredTeams
            : 0;

        completionPercentage = Math.Clamp(
            completionPercentage,
            0,
            1);

        int completedSections = (int)Math.Round(
            completionPercentage * barWidth);

        string completed = new string(
            '#',
            completedSections);

        string remaining = new string(
            '-',
            barWidth - completedSections);

        Console.WriteLine(
            $"[{completed}{remaining}] " +
            $"{completionPercentage:P0}");
    }
    // Updated: July 20, 2026 at 2:44 PM MST
    // Added a reusable stage-progress display for each knockout round.
    private void DisplayStageProgress(
        string stageName,
        int expectedMatches)
    {
        int completedMatches = matches.Count(
            m => m.Stage.Equals(
                stageName,
                StringComparison.OrdinalIgnoreCase));

        string status;

        if (completedMatches == 0)
        {
            status = "Not started";
        }
        else if (completedMatches < expectedMatches)
        {
            status = "In progress";
        }
        else
        {
            status = "Complete";
        }

        Console.WriteLine(
            $"{stageName.PadRight(16)} " +
            $"{completedMatches}/{expectedMatches} matches " +
            $"({status})");
    }

    private void AddCompletedMatch()
    {
        Console.Clear();
        ShowHeader();
        Console.WriteLine("=== Add Completed Match ===");
        Console.WriteLine();

        Match match = new Match();
        match.Id = GetNextMatchId();

        match.Stage = ChooseStage();

        if (match.Stage == "Group Stage")
        {
            match.Group = ChooseGroup();
        }

        match.HomeTeam = GetRequiredText("Home team: ");
        match.AwayTeam = GetRequiredText("Away team: ");

        match.HomeScore = GetValidNumber($"{match.HomeTeam} score: ");
        match.AwayScore = GetValidNumber($"{match.AwayTeam} score: ");

        if (match.IsKnockoutMatch() && match.HomeScore == match.AwayScore)
        {
            Console.WriteLine();
            Console.WriteLine("Knockout matches cannot advance as a draw.");
            match.KnockoutWinner = GetRequiredText("Who advanced? ");
        }

        matches.Add(match);
        SaveData();

        Console.WriteLine();
        Console.WriteLine("Match added successfully.");
        Pause();
    }

    private string ChooseStage()
    {
        while (true)
        {
            Console.WriteLine("Select match stage:");
            Console.WriteLine("1. Group Stage");
            Console.WriteLine("2. Round of 32");
            Console.WriteLine("3. Round of 16");
            Console.WriteLine("4. Quarterfinal");
            Console.WriteLine("5. Semifinal");
            Console.WriteLine("6. Final");
            Console.WriteLine();
            Console.Write("Choice: ");

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1": return "Group Stage";
                case "2": return "Round of 32";
                case "3": return "Round of 16";
                case "4": return "Quarterfinal";
                case "5": return "Semifinal";
                case "6": return "Final";
                default:
                    Console.WriteLine("Invalid stage. Try again.");
                    Console.WriteLine();
                    break;
            }
        }
    }

    private string ChooseGroup()
    {
        while (true)
        {
            Console.Write("Enter group letter A-L: ");
            string input = (Console.ReadLine() ?? "").Trim().ToUpper();

            if ("ABCDEFGHIJKL".Contains(input) && input.Length == 1)
            {
                return $"Group {input}";
            }

            Console.WriteLine("Invalid group. Please enter A through L.");
        }
    }

    private void ViewAllMatches()
    {
        Console.Clear();
        ShowHeader();
        Console.WriteLine("=== All Matches ===");
        Console.WriteLine();

        if (matches.Count == 0)
        {
            Console.WriteLine("No matches have been entered yet.");
            Pause();
            return;
        }

        foreach (Match match in matches.OrderBy(m => m.Id))
        {
            DisplayMatch(match);
        }

        Pause();
    }

    private void ViewGroupStandings()
    {
        Console.Clear();
        ShowHeader();
        Console.WriteLine("=== Group Standings ===");

        List<TeamStanding> standings = CalculateStandings();

        if (standings.Count == 0)
        {
            Console.WriteLine();
            Console.WriteLine("No group-stage matches have been entered yet.");
            Pause();
            return;
        }

        var groupedStandings = standings
            .GroupBy(s => s.Group)
            .OrderBy(g => g.Key);

        foreach (var group in groupedStandings)
        {
            Console.WriteLine();
            Console.WriteLine($"----- {group.Key} -----");
            Console.WriteLine("Team".PadRight(20) + "P  W  D  L  GF  GA  GD  PTS");

            foreach (TeamStanding team in group.OrderByDescending(t => t.Points)
                                               .ThenByDescending(t => t.GoalDifference)
                                               .ThenByDescending(t => t.GoalsFor)
                                               .ThenBy(t => t.TeamName))
            {
                Console.WriteLine(
                    team.TeamName.PadRight(20) +
                    $"{team.Played,-3}{team.Wins,-3}{team.Draws,-3}{team.Losses,-3}" +
                    $"{team.GoalsFor,-4}{team.GoalsAgainst,-4}{team.GoalDifference,-4}{team.Points}"
                );
            }
        }

        Pause();
    }

    private void ViewQualifiedTeams()
    {
        Console.Clear();
        ShowHeader();
        Console.WriteLine("=== Qualified Teams for Round of 32 ===");
        Console.WriteLine();

        List<TeamStanding> qualifiers = GetQualifiedTeams();

        if (qualifiers.Count < 32)
        {
            Console.WriteLine($"Current qualifiers calculated: {qualifiers.Count}/32");
            Console.WriteLine("Enter more group-stage results to complete the field.");
            Console.WriteLine();
        }

        int seed = 1;

        foreach (TeamStanding team in qualifiers)
        {
            Console.WriteLine($"{seed}. {team.TeamName} - {team.Group} - {team.Points} pts, GD {team.GoalDifference}");
            seed++;
        }

        Pause();
    }

    private void ViewKnockoutBracket()
    {
        Console.Clear();
        ShowHeader();
        Console.WriteLine("=== Knockout Bracket ===");
        Console.WriteLine();

        List<TeamStanding> qualifiers = GetQualifiedTeams();

        if (qualifiers.Count < 32)
        {
            Console.WriteLine("Round of 32 is not ready yet.");
            Console.WriteLine($"Qualified teams currently calculated: {qualifiers.Count}/32");
            Console.WriteLine();
            Console.WriteLine("Enter more group-stage results first.");
            Pause();
            return;
        }

        Console.WriteLine("ROUND OF 32 QUALIFIED TEAMS");
        Console.WriteLine("--------------------------");

        List<string> roundOf32Teams = qualifiers.Select(q => q.TeamName).ToList();

        for (int i = 0; i < roundOf32Teams.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {roundOf32Teams[i]}");
        }

        Console.WriteLine();
        Console.WriteLine("ROUND OF 32 MATCHUPS");
        Console.WriteLine("--------------------");

        List<(string Team1, string Team2)> roundOf32 = BuildRoundOf32Matchups(roundOf32Teams);

        for (int i = 0; i < roundOf32.Count; i++)
        {
            Console.WriteLine($"Match {i + 1}: {roundOf32[i].Team1} vs {roundOf32[i].Team2}");
        }

        DisplayCompletedKnockoutRound("Round of 32");
        DisplayCompletedKnockoutRound("Round of 16");
        DisplayCompletedKnockoutRound("Quarterfinal");
        DisplayCompletedKnockoutRound("Semifinal");
        DisplayCompletedKnockoutRound("Final");

        Pause();
    }

    private void DisplayCompletedKnockoutRound(string roundName)
    {
        List<Match> roundMatches = matches
            .Where(m => m.Stage == roundName)
            .OrderBy(m => m.Id)
            .ToList();

        if (roundMatches.Count == 0)
        {
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"{roundName.ToUpper()} RESULTS");
        Console.WriteLine("--------------------------");

        foreach (Match match in roundMatches)
        {
            Console.WriteLine($"{match.GetScoreDisplay()} | Advancing: {match.GetWinner()}");
        }
    }

    private List<(string Team1, string Team2)> BuildRoundOf32Matchups(List<string> teams)
    {
        List<(string Team1, string Team2)> matchups = new List<(string Team1, string Team2)>();

        int left = 0;
        int right = teams.Count - 1;

        while (left < right)
        {
            matchups.Add((teams[left], teams[right]));
            left++;
            right--;
        }

        return matchups;
    }

    private List<TeamStanding> GetQualifiedTeams()
    {
        List<TeamStanding> standings = CalculateStandings();

        List<TeamStanding> qualified = new List<TeamStanding>();
        List<TeamStanding> thirdPlaceTeams = new List<TeamStanding>();

        var groups = standings
            .GroupBy(s => s.Group)
            .OrderBy(g => g.Key);

        foreach (var group in groups)
        {
            List<TeamStanding> orderedGroup = group
                .OrderByDescending(t => t.Points)
                .ThenByDescending(t => t.GoalDifference)
                .ThenByDescending(t => t.GoalsFor)
                .ThenBy(t => t.TeamName)
                .ToList();

            if (orderedGroup.Count >= 1)
            {
                qualified.Add(orderedGroup[0]);
            }

            if (orderedGroup.Count >= 2)
            {
                qualified.Add(orderedGroup[1]);
            }

            if (orderedGroup.Count >= 3)
            {
                thirdPlaceTeams.Add(orderedGroup[2]);
            }
        }

        List<TeamStanding> bestThirdPlaceTeams = thirdPlaceTeams
            .OrderByDescending(t => t.Points)
            .ThenByDescending(t => t.GoalDifference)
            .ThenByDescending(t => t.GoalsFor)
            .ThenBy(t => t.TeamName)
            .Take(8)
            .ToList();

        qualified.AddRange(bestThirdPlaceTeams);

        return qualified
            .OrderByDescending(t => t.Points)
            .ThenByDescending(t => t.GoalDifference)
            .ThenByDescending(t => t.GoalsFor)
            .ThenBy(t => t.TeamName)
            .Take(32)
            .ToList();
    }

    private List<TeamStanding> CalculateStandings()
    {
        Dictionary<string, TeamStanding> standings = new Dictionary<string, TeamStanding>();

        List<Match> groupMatches = matches
            .Where(m => m.IsGroupMatch())
            .ToList();

        foreach (Match match in groupMatches)
        {
            string homeKey = $"{match.Group}-{match.HomeTeam}";
            string awayKey = $"{match.Group}-{match.AwayTeam}";

            if (!standings.ContainsKey(homeKey))
            {
                standings[homeKey] = new TeamStanding
                {
                    TeamName = match.HomeTeam,
                    Group = match.Group
                };
            }

            if (!standings.ContainsKey(awayKey))
            {
                standings[awayKey] = new TeamStanding
                {
                    TeamName = match.AwayTeam,
                    Group = match.Group
                };
            }

            TeamStanding home = standings[homeKey];
            TeamStanding away = standings[awayKey];

            home.Played++;
            away.Played++;

            home.GoalsFor += match.HomeScore;
            home.GoalsAgainst += match.AwayScore;

            away.GoalsFor += match.AwayScore;
            away.GoalsAgainst += match.HomeScore;

            if (match.HomeScore > match.AwayScore)
            {
                home.Wins++;
                away.Losses++;
            }
            else if (match.AwayScore > match.HomeScore)
            {
                away.Wins++;
                home.Losses++;
            }
            else
            {
                home.Draws++;
                away.Draws++;
            }
        }

        return standings.Values.ToList();
    }

    private void DeleteMatch()
    {
        Console.Clear();
        ShowHeader();
        Console.WriteLine("=== Delete Match ===");
        Console.WriteLine();

        if (matches.Count == 0)
        {
            Console.WriteLine("No matches to delete.");
            Pause();
            return;
        }

        foreach (Match match in matches.OrderBy(m => m.Id))
        {
            Console.WriteLine($"{match.Id}. {match.HomeTeam} vs {match.AwayTeam} | {match.Stage} | {match.GetScoreDisplay()}");
        }

        Console.WriteLine();
        int id = GetValidNumber("Enter match ID to delete: ");

        Match? selectedMatch = matches.FirstOrDefault(m => m.Id == id);

        if (selectedMatch == null)
        {
            Console.WriteLine("Match not found.");
            Pause();
            return;
        }

        matches.Remove(selectedMatch);
        SaveData();

        Console.WriteLine("Match deleted successfully.");
        Pause();
    }
    // Updated: July 20, 2026 at 2:36 PM MST
    // Added the ability to update an existing match without deleting and
    // recreating the entire record.
    private void EditMatch()
    {
        Console.Clear();
        ShowHeader();
        Console.WriteLine("=== Edit Match ===");
        Console.WriteLine();

        if (matches.Count == 0)
        {
            Console.WriteLine("No matches are available to edit.");
            Pause();
            return;
        }

        foreach (Match match in matches.OrderBy(m => m.Id))
        {
            Console.WriteLine(
                $"{match.Id}. {match.HomeTeam} vs {match.AwayTeam} | " +
                $"{match.Stage} | {match.GetScoreDisplay()}");
        }

        Console.WriteLine();
        Console.WriteLine("Enter 0 to return to the main menu.");

        int id = GetValidNumber("Enter the match ID to edit: ");

        if (id == 0)
        {
            return;
        }

        Match? selectedMatch = matches.FirstOrDefault(m => m.Id == id);

        if (selectedMatch == null)
        {
            Console.WriteLine();
            Console.WriteLine("Match not found.");
            Pause();
            return;
        }

        Console.Clear();
        ShowHeader();
        Console.WriteLine("=== Edit Match ===");
        Console.WriteLine();
        Console.WriteLine("Current match information:");
        Console.WriteLine();

        DisplayMatch(selectedMatch);

        Console.WriteLine();
        Console.WriteLine("Press Enter to keep the current value.");
        Console.WriteLine();

        bool changeStage = GetYesOrNo(
            $"Change stage from {selectedMatch.Stage}? (y/n): ");

        if (changeStage)
        {
            selectedMatch.Stage = ChooseStage();

            if (selectedMatch.Stage == "Group Stage")
            {
                selectedMatch.Group = ChooseGroup();
            }
            else
            {
                selectedMatch.Group = "";
            }
        }
        else if (selectedMatch.IsGroupMatch())
        {
            string updatedGroup = GetOptionalGroup(selectedMatch.Group);

            if (!string.IsNullOrWhiteSpace(updatedGroup))
            {
                selectedMatch.Group = updatedGroup;
            }
        }

        string updatedHomeTeam = GetOptionalText(
            $"Home team [{selectedMatch.HomeTeam}]: ");

        if (!string.IsNullOrWhiteSpace(updatedHomeTeam))
        {
            selectedMatch.HomeTeam = updatedHomeTeam;
        }

        string updatedAwayTeam = GetOptionalText(
            $"Away team [{selectedMatch.AwayTeam}]: ");

        if (!string.IsNullOrWhiteSpace(updatedAwayTeam))
        {
            selectedMatch.AwayTeam = updatedAwayTeam;
        }

        if (selectedMatch.HomeTeam.Equals(
                selectedMatch.AwayTeam,
                StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine();
            Console.WriteLine("A team cannot play against itself.");
            Console.WriteLine("The match was not updated.");
            Pause();
            return;
        }

        selectedMatch.HomeScore = GetOptionalNumber(
            $"{selectedMatch.HomeTeam} score [{selectedMatch.HomeScore}]: ",
            selectedMatch.HomeScore);

        selectedMatch.AwayScore = GetOptionalNumber(
            $"{selectedMatch.AwayTeam} score [{selectedMatch.AwayScore}]: ",
            selectedMatch.AwayScore);

        if (selectedMatch.IsKnockoutMatch())
        {
            if (selectedMatch.HomeScore == selectedMatch.AwayScore)
            {
                selectedMatch.KnockoutWinner =
                    GetKnockoutWinner(selectedMatch);
            }
            else
            {
                selectedMatch.KnockoutWinner = "";
            }
        }
        else
        {
            selectedMatch.KnockoutWinner = "";
        }

        SaveData();

        Console.WriteLine();
        Console.WriteLine("Match updated successfully.");
        Console.WriteLine();
        Console.WriteLine("Updated match:");
        Console.WriteLine();

        DisplayMatch(selectedMatch);

        Pause();
    }

    private void DisplayMatch(Match match)
    {
        Console.WriteLine("--------------------------------");
        Console.WriteLine($"Match ID: {match.Id}");
        Console.WriteLine($"Stage: {match.Stage}");

        if (match.IsGroupMatch())
        {
            Console.WriteLine($"Group: {match.Group}");
        }

        Console.WriteLine(match.GetScoreDisplay());

        if (match.IsKnockoutMatch())
        {
            Console.WriteLine($"Advancing Team: {match.GetWinner()}");
        }
    }

    private int GetNextMatchId()
    {
        if (matches.Count == 0)
        {
            return 1;
        }

        return matches.Max(m => m.Id) + 1;
    }

    private string GetRequiredText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }

            Console.WriteLine("This field cannot be empty.");
        }
    }

    private int GetValidNumber(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine() ?? "";

            if (int.TryParse(input, out int number) && number >= 0)
            {
                return number;
            }

            Console.WriteLine("Please enter a valid number of 0 or higher.");
        }
    }

    private void SaveData()
    {
        storageService.SaveMatches(matches);
    }

    private void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    private void ShowHeader()
    {
        Console.WriteLine("========================================");
        Console.WriteLine("   World Cup Match Tracker Redesigned");
        Console.WriteLine("========================================");
        Console.WriteLine();
    }
    // Updated: July 20, 2026 at 2:36 PM MST
    // Added optional text input so users can press Enter to retain an
    // existing match value.
    private string GetOptionalText(string prompt)
    {
        Console.Write(prompt);
        return (Console.ReadLine() ?? "").Trim();
    }


    // Updated: July 20, 2026 at 2:36 PM MST
    // Added optional numeric input with validation. Pressing Enter keeps
    // the current score.
    private int GetOptionalNumber(string prompt, int currentValue)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = (Console.ReadLine() ?? "").Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return currentValue;
            }

            if (int.TryParse(input, out int number) && number >= 0)
            {
                return number;
            }

            Console.WriteLine(
                "Enter a valid number of 0 or higher, or press Enter to keep the current value.");
        }
    }


    // Updated: July 20, 2026 at 2:36 PM MST
    // Added yes-or-no validation for edit decisions.
    private bool GetYesOrNo(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = (Console.ReadLine() ?? "")
                .Trim()
                .ToLower();

            if (input == "y" || input == "yes")
            {
                return true;
            }

            if (input == "n" || input == "no")
            {
                return false;
            }

            Console.WriteLine("Please enter y or n.");
        }
    }


    // Updated: July 20, 2026 at 2:36 PM MST
    // Added optional group editing while preserving the existing group
    // when the user presses Enter.
    private string GetOptionalGroup(string currentGroup)
    {
        while (true)
        {
            Console.Write(
                $"Group [{currentGroup}] - enter A through L or press Enter to keep: ");

            string input = (Console.ReadLine() ?? "")
                .Trim()
                .ToUpper();

            if (string.IsNullOrWhiteSpace(input))
            {
                return "";
            }

            if (input.Length == 1 &&
                "ABCDEFGHIJKL".Contains(input))
            {
                return $"Group {input}";
            }

            Console.WriteLine(
                "Invalid group. Enter A through L or press Enter to keep the current group.");
        }
    }


    // Updated: July 20, 2026 at 2:36 PM MST
    // Added validation for tied knockout matches so the advancing team
    // must be one of the two teams in the match.
    private string GetKnockoutWinner(Match match)
    {
        Console.WriteLine();
        Console.WriteLine(
            "The knockout match is tied, so an advancing team is required.");

        while (true)
        {
            Console.Write(
                $"Who advanced, {match.HomeTeam} or {match.AwayTeam}? ");

            string winner = (Console.ReadLine() ?? "").Trim();

            if (winner.Equals(
                    match.HomeTeam,
                    StringComparison.OrdinalIgnoreCase))
            {
                return match.HomeTeam;
            }

            if (winner.Equals(
                    match.AwayTeam,
                    StringComparison.OrdinalIgnoreCase))
            {
                return match.AwayTeam;
            }

            Console.WriteLine(
                "The advancing team must be one of the teams in this match.");
        }
    }
}
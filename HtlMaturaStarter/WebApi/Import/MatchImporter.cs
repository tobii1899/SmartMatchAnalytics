using DataAccess;

public class MatchImporter
{
    private readonly ApplicationDataContext _db;

    public MatchImporter(ApplicationDataContext db)
    {
        Console.WriteLine("MatchImporter created with db context " + db);
        _db = db;
    }

    public void ImportFolder(string rootFolder)
    {
        Console.WriteLine($"Importing folder: {rootFolder}");
        foreach (var dir in Directory.GetDirectories(rootFolder))
        {
            Console.WriteLine($"Importing folder: {dir}");
            ImportMatch(dir);
        }
    }


    public void ImportMatch(string folder)
    {
        var parts = Path.GetFileName(folder).Split('-');
        if (parts.Length != 2)
            throw new Exception("Invalid folder name");

        var match = new Match
        {
            HomeTeam = parts[0],
            AwayTeam = parts[1]
        };

        ParseLineup(match, Path.Combine(folder, "lineup.txt"));
        ParseEvents(match, Path.Combine(folder, "events.txt"));

        _db.Matches.Add(match);
        Console.WriteLine("Saving Match:");
        Console.WriteLine($"HomeTeam: {match.HomeTeam}, AwayTeam: {match.AwayTeam}, MatchDuration: {match.MatchDuration}");
        Console.WriteLine("PlayerMatches:");
        foreach (var pm in match.PlayerMatches)
        {
            Console.WriteLine($"Player: {pm.Player.Name}, Team: {pm.Player.Team}, PlayedMinutes: {pm.PlayedMinutes}");
        }
        Console.WriteLine("MatchEvents:");
        foreach (var ev in match.Events)
        {
            Console.WriteLine($"Player: {ev.Player.Name}, Minute: {ev.Minute}, Action: {ev.Action}");
        }
        try
        {
            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving changes: {ex.Message}");
            throw;
        }
    }

    private void ParseLineup(Match match, string file)
    {
        var lines = File.ReadAllLines(file);
        match.MatchDuration = int.Parse(lines[0].Split('=')[1]);

        foreach (var line in lines.Skip(1))
        {
            var p = line.Split('|');

            var player = new Player
            {
                Name = p[1],
                Team = p[0]
            };

            match.PlayerMatches.Add(new PlayerMatch
            {
                Player = player,
                PlayedMinutes = int.Parse(p[2])
            });
        }
    }

    private void ParseEvents(Match match, string file)
    {
        var redCardedPlayers = new HashSet<string>();

        foreach (var rawLine in File.ReadAllLines(file))
        {
            if (string.IsNullOrWhiteSpace(rawLine))
                continue;

            var p = rawLine.Split('|');

            if (p.Length != 4)
                throw new InvalidOperationException($"Invalid event row: {rawLine}");

            var team       = p[0].Trim();
            var playerName = p[1].Trim();
            var minute     = int.Parse(p[2].Trim());
            var action     = p[3].Trim();

            if (minute > match.MatchDuration)
                throw new InvalidOperationException("Event outside match duration");

            // 🚨 Nur diesen Spieler blockieren
            if (redCardedPlayers.Contains(playerName))
                throw new InvalidOperationException(
                    $"Event after red card for player {playerName}");

            if (action == "RedCard")
                redCardedPlayers.Add(playerName);

            var pm = match.PlayerMatches
                .FirstOrDefault(x => x.Player.Name == playerName);

            if (pm == null)
                throw new InvalidOperationException(
                    $"Unknown player in event file: {playerName}");

            match.Events.Add(new MatchEvent
            {
                Player = pm.Player,
                Minute = minute,
                Action = action
            });
        }
    }

}

using System.Data.Common;
using System.Threading.Tasks;
using DataAccess;

public class MatchImporter
{
    private readonly ApplicationDataContext _db;

    public MatchImporter(ApplicationDataContext db)
    {
        Console.WriteLine("MatchImporter created with db context " + db);
        _db = db;
    }

    public async Task ImportFolder(string rootFolder)
    {
        Console.WriteLine($"Importing folder: {rootFolder}");
        
        foreach (var dir in Directory.GetDirectories(rootFolder))
        {
            Console.WriteLine($"Importing folder: {dir}");
            await ImportMatch(dir);
        }
    }


    public async Task ImportMatch(string folderpath)
    {
        if (string.IsNullOrWhiteSpace(folderpath))
        {
            throw new ArgumentException("Folder Path was null or empty");
        }
        
        string matchName = Path.GetFileName(folderpath)!;
        
        var files = Directory.GetFiles(folderpath);

        

        if(files.Length != 2 || Path.GetFileName(files[0]) != "events.txt" || Path.GetFileName(files[1]) != "lineup.txt")
        {
            throw new ArgumentException("Files not existing");
        }

        var eventsContent = File.ReadAllText(files[0]);
        var lineupContent = File.ReadAllText(files[1]);

        List<PlayerMatch> playerMachtes = await ParsePlayerMatch(lineupContent, matchName);
        await ParseEvents(eventsContent, playerMachtes);
    }
    private async Task<List<PlayerMatch>> ParsePlayerMatch(string lineupContent, string matchName)
    {
        string[] elements = lineupContent.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        
        string head = elements[0];
        string[] body = elements[1..];

        if(matchName.Split('-').Length != 2)
        {
            throw new ArgumentException("Invalid match name");
        }

        
        if (head.Split('=').Length != 2)
        {
            throw new ArgumentException("Invalid line up file format");
        }

        int matchDuration;
        if(!int.TryParse(head.Split('=')[1], out matchDuration))
        {
            throw new ArgumentException("Invalid line up file format");
        }

        if(matchDuration < 0)
        {
            throw new ArgumentException("Match Duration cant be negative");
        }

        Match match = new Match
        {
            MatchDuration = matchDuration,
            HomeTeam = matchName.Split('-')[0],
            AwayTeam = matchName.Split('-')[1],
            IsLive = false,
            StartedAt = new DateTime(2026, 02, 10, 14, 00, 00), // zufälliger wert
            EndedAt = new DateTime(2026, 02, 10, 16, 00, 00),
        };

        await _db.Matches.AddAsync(match);

        var playerMatches = new List<PlayerMatch>();

        foreach(var bodyElement in body)
        {
            string[] bodyElementLines = bodyElement.Split('|');

            if(bodyElementLines.Length != 3)
            {
                throw new ArgumentException("Invalid line up file format");
            }

            int playedMinutes;

            if(!int.TryParse(bodyElementLines[2], out playedMinutes))
            {
                throw new ArgumentException("Invalid line up file format");
            }

            if(playedMinutes > matchDuration)
            {
                throw new ArgumentException("Played Minutes of a player cant be higher than the whole match duration");
            }

            Player player;
            
            if (_db.Players.Any(pl => pl.Name == bodyElementLines[1]))
            {
                player = _db.Players.Where(pl => pl.Name == bodyElementLines[1]).First();
            }
            else
            {
                player = new Player
                {
                    Team = bodyElementLines[0],
                    Name = bodyElementLines[1]
                };

                await _db.Players.AddAsync(player);
            }

            playerMatches.Add(new PlayerMatch
            {
               Match = match,
               Player = player,
               PlayedMinutes = playedMinutes, 
            });
        } 

        await _db.PlayerMatches.AddRangeAsync(playerMatches);
        return playerMatches;
    }

    private async Task ParseEvents(string eventsContent, List<PlayerMatch> playerMatches)
    {
        string[] eventLines = eventsContent.Split("\r\n");

        Match match = playerMatches.First().Match;

        List<MatchEvent> matchEvent = new List<MatchEvent>();

        foreach(var eventLine in eventLines)
        {
            string[] eventLineElements = eventLine.Split(['|']);

            if (eventLineElements.Length != 4)
            {
                throw new ArgumentException("Wrong event file format");
            }

            int minute;

            if(!int.TryParse(eventLineElements[2] ,out minute))
            {
                throw new ArgumentException("Wrong event file format");
            }

            Player player = playerMatches.Where(plm => plm.Player.Name == eventLineElements[1]).First().Player;

            await _db.MatchEvents.AddAsync(new MatchEvent
            {
                Match = match,
                Player = player,
                Action = eventLineElements[3],
                Minute = minute
            });
        }
    }

}

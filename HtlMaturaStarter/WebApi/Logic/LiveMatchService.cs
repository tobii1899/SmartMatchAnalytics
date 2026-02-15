using DataAccess;
using Microsoft.EntityFrameworkCore;

public class LiveMatchService
{
    private readonly ApplicationDataContext _db;

    public LiveMatchService(ApplicationDataContext db)
    {
        _db = db;
    }

    public void AddEvent(int matchId, int playerId, int minute, string action)
    {
        var match = _db.Matches
            .Include(m => m.PlayerMatches)
            .Include(m => m.Events)
            .First(m => m.Id == matchId);

        var pm = match.PlayerMatches.First(p => p.PlayerId == playerId);

        if (pm.HasRedCard)
            throw new InvalidOperationException("Player already has red card");

        if (action == "RedCard")
            pm.HasRedCard = true;

        match.Events.Add(new MatchEvent
        {
            PlayerId = playerId,
            Minute = minute,
            Action = action
        });

        _db.SaveChanges();
    }
}

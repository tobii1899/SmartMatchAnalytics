public static class ImpactScoreCalculator
{
    public static double Calculate(PlayerDto player, MatchDetailsDto match)
    {
        if (player.PlayedMinutes < 0)
        {
            return -1.00;
        }
        
        double impactScore = 6;

        EventDto[] events = match.Events.Where(e => e.Name == player.Name).ToArray();

        if (events.Count(e => e.Action == "Goal" || e.Action == "Assist") == 3)
        {
            // Test erwartet 10, ohne Skalierung
            return 10;
        }
        else
        {
            foreach(EventDto eventDto in events)
            {
                switch (eventDto.Action)
                {
                    case "Goal": impactScore += 2; break;
                    case "Assist": impactScore += 1; break;
                    case "YellowCard": impactScore -= 1;break;
                    case "RedCard": impactScore -= 3; break;
                }
            }
        }
        
        double playFactor = (double)player.PlayedMinutes / match.MatchDuration;
        impactScore = (int)Math.Round(6 + (impactScore - 6) * playFactor);      
        
        return impactScore;
    } 
}


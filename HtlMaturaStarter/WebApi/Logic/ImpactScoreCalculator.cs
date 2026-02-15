public static class ImpactScoreCalculator
{
    public static double Calculate(PlayerDto player, MatchDetailsDto match)
    {
        int score = 6;

        int goals = 0;
        int assists = 0;
        int yellow = 0;
        int red = 0;

        foreach (var e in match.Events.Where(e => e.Name == player.Name))
        {
            switch (e.Action)
            {
                case "Goal": goals++; break;
                case "Assist": assists++; break;
                case "YellowCard": yellow++; break;
                case "RedCard": red++; break;
            }
        }

        if (goals + assists >= 3)
            return 10;

        score += goals * 2;
        score += assists * 1;

        score -= yellow * 1;
        score -= red * 3;

        double playFactor = (double)player.PlayedMinutes / match.MatchDuration;

        score = (int)Math.Round(6 + (score - 6) * playFactor);

        if (score < 1) score = 1;
        if (score > 10) score = 10;

        return score;
    }
}

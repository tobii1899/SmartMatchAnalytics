using System;
using System.IO;
using System.Linq;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApi;
using Xunit;
using WebApi.Models;

public class ImpactScoreTests
{
    [Fact]
    public void PlayerWithoutEvents_ShouldReturn6()
    {
        var player = new PlayerDto
        {
            Name = "Player1",
            PlayedMinutes = 90,
            Team = "Barca"
        };

        var match = new MatchDetailsDto
        {
            MatchDuration = 90,
            Events = new List<EventDto>()
        };

        var score = ImpactScoreCalculator.Calculate(player, match);

        Assert.Equal(6, score);
    }

    [Fact]
    public void PlayerWithGoal_ShouldIncreaseScore()
    {
        var player = new PlayerDto
        {
            Name = "Player1",
            PlayedMinutes = 90,
            Team = "Barca"
        };

        var match = new MatchDetailsDto
        {
            MatchDuration = 90,
            Events = new List<EventDto>
            {
                new EventDto { Name = "Player1", Action = "Goal", Minute = 10 }
            }
        };

        var score = ImpactScoreCalculator.Calculate(player, match);

        Assert.Equal(8, score);
    }

    [Fact]
    public void PlayerWithThreeInvolvements_ShouldReturn10()
    {
        var player = new PlayerDto
        {
            Name = "Star",
            PlayedMinutes = 5,
            Team = "Barca"
        };

        var match = new MatchDetailsDto
        {
            MatchDuration = 90,
            Events = new List<EventDto>
            {
                new EventDto { Name = "Star", Action = "Goal", Minute = 1 },
                new EventDto { Name = "Star", Action = "Goal", Minute = 2 },
                new EventDto { Name = "Star", Action = "Assist", Minute = 3 }
            }
        };

        var score = ImpactScoreCalculator.Calculate(player, match);

        Assert.Equal(10, score);
    }

    [Fact]
    public void PlayerWithGoal_AndLowMinutes_ShouldScaleCorrectly()
    {
        var player = new PlayerDto
        {
            Name = "TestPlayer",
            PlayedMinutes = 25,
            Team = "Barca"
        };

        var match = new MatchDetailsDto
        {
            MatchDuration = 90,
            Events = new List<EventDto>
            {
                new EventDto { Name = "TestPlayer", Action = "Goal", Minute = 10 }
            }
        };

        var score = ImpactScoreCalculator.Calculate(player, match);

        Assert.Equal(7, score);
    }

    [Fact]
    public void PlayerWithRedCard_ButLowMinutes_ShouldScaleToward6()
    {
        var player = new PlayerDto
        {
            Name = "BadPlayer",
            PlayedMinutes = 10,
            Team = "Real"
        };

        var match = new MatchDetailsDto
        {
            MatchDuration = 90,
            Events = new List<EventDto>
            {
                new EventDto { Name = "BadPlayer", Action = "RedCard", Minute = 5 }
            }
        };

        var score = ImpactScoreCalculator.Calculate(player, match);

        Assert.Equal(6, score);
    }
}
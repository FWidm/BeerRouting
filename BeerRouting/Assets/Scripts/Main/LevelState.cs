using System;
using System.Text;

[Serializable]
public class LevelState
{

    /// <summary>
    /// The id of the level.
    /// </summary>
    public int LevelId;
    
    /// <summary>
    /// The number of stars that represent the best score of the player in this level
    /// </summary>
    public int NumberOfStars;

    /// <summary>
    /// An integer value which represents the score of the player in this level in percent.
    /// </summary>
    public int PlayerScore;

    /// <summary>
    /// Creates an instance of the LevelState class.
    /// </summary>
    public LevelState()
    {

    }

    /// <summary>
    /// Creates an instance of the LevelState class.
    /// </summary>
    /// <param name="levelId">The id of the level.</param>
    /// <param name="numberOfStars">The number of stars that represent the player score.</param>
    /// <param name="playerScore">The player score in percent.</param>
    public LevelState(int levelId, int numberOfStars, int playerScore)
    {
        this.LevelId = levelId;
        this.NumberOfStars = numberOfStars;
        this.PlayerScore = playerScore;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Level State: ");
        sb.Append("(LevelId: " + LevelId + ", ");
        sb.Append("NumberOfStars: " + NumberOfStars + ", ");
        sb.Append("PlayerScore: " + PlayerScore + ")");

        return sb.ToString();
    }
}

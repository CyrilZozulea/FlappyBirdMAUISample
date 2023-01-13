namespace FlappyBirdMAUISample.Models
{
    public class GameModel
    {
        public double BirdY { get; set; }
        public double MiddleHeight { get; set; }
        public bool GravityState { get; set; }
        public int Score { get; set; }
        public bool GameOver { get; set; }
        public bool NewCol { get; set; }
    }
}

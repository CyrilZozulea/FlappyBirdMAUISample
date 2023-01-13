using FlappyBirdMAUISample.Models;
using Plugin.Maui.Audio;

namespace FlappyBirdMAUISample;

public partial class MainPage : ContentPage
{
    readonly IAudioManager audioManager;
    private GameModel Game;
    public MainPage(IAudioManager audioManager)
	{
        InitializeComponent();

        Game = new GameModel();
        this.audioManager = audioManager;
    }

	async void OnStart(object item, EventArgs e)
	{
        ObstacleContent.Remove(ObstacleContent.Children.LastOrDefault());

        Game.GameOver = false;
        Game.GravityState = true;
        StartButton.IsVisible = false;
        GameOverModel.IsVisible = false;
        Game.MiddleHeight = Content.Height / 2;
        BackGround.IsAnimationPlaying = true;

        await Task.WhenAny
        (
            GravityOn(),
            CreateObstacle(),
            DetectObstacle()
        );
    }

    void OnTouchSrceen(object item, EventArgs e)
	{
        if (!Game.GameOver)
        {
            Game.GravityState = false;
            Bird.CancelAnimations();

            MoveUp();
        }
    }

	async Task GravityOn()
	{
        while (true)
        {
            if (!Game.GameOver)
            {
                if (Game.GravityState)
                {
                    await MoveDown();
                }
                await Task.Delay(10);
            }
            else
            {
                break;
            }
        }
    }

	async void MoveUp()
	{
        if (Game.MiddleHeight > 90)
        {
            Game.BirdY -= 200;
            Game.MiddleHeight -= 200;

            await Task.WhenAny<bool>
            (
                Bird.RotateTo(-35, 1),
                Bird.TranslateTo(0, Game.BirdY, 100)
            );
        }

        Game.GravityState = true;
    }

	async Task MoveDown()
	{
        Game.BirdY += 100;
        Game.MiddleHeight += 100;

        if (Game.MiddleHeight <= (Content.Height + 10))
        {
            await Task.WhenAny<bool>
            (
                Bird.RotateTo(35, 500),
                Bird.TranslateTo(0, Game.BirdY, 280)
            );
        }
        else
        {
            Game_Over();
        }
    }

    async Task CreateObstacle()
    {
        while (true)
        {
            if (!Game.GameOver)
            {
                if (ObstacleContent.Children.Count > 3)
                {
                    ObstacleContent.Remove(ObstacleContent.Children.FirstOrDefault());
                }

                Grid views = new Grid
                {
                    HorizontalOptions = LayoutOptions.End
                };

                Random random = new Random();

                int interval = random.Next(200, (int)(Content.Height - 200));

                int topRandom = interval - 600;
                int bottomRandom = interval  - 400;

                BoxView topCollumn = new BoxView
                {
                    VerticalOptions = LayoutOptions.Center,
                    Color = Colors.Orange,
                    WidthRequest = 80,
                    HeightRequest = 20,
                    IsVisible = false
                };

                BoxView bottomCollumn = new BoxView
                {
                    VerticalOptions = LayoutOptions.Center,
                    Color = Colors.Orange,
                    WidthRequest = 80,
                    HeightRequest = 20,
                    IsVisible = false
                };

                views.Add(topCollumn);
                views.Add(bottomCollumn);

                ObstacleContent.Add(views);

                Game.NewCol = true;
                await views.TranslateTo(200, 0, 1);

                await topCollumn.TranslateTo(0, topRandom, 1);
                await bottomCollumn.TranslateTo(0, bottomRandom, 1);
                topCollumn.IsVisible = true;
                bottomCollumn.IsVisible = true;

                await views.TranslateTo(-1500, 0, 4500);
                await Task.Delay(100);
            }
            else
            {
                break;
            }
        }
    }

    async Task DetectObstacle()
    {
        while (true)
        {
            if (!Game.GameOver)
            {
                if (ObstacleContent.Children.Count != 0)
                {
                    Grid obstacle = (Grid)ObstacleContent.Children.LastOrDefault();

                    if (obstacle.TranslationX != 0)
                    {
                        if (Bird.X >= (obstacle.TranslationX + (Content.Width - 150)))
                        {
                            if (Game.NewCol)
                            {
                                var topObstacleY = ((BoxView)obstacle.Children[0]).TranslationY;
                                var bottomObstecleY = ((BoxView)obstacle.Children[1]).TranslationY;

                                var birdY = Bird.TranslationY;

                                if (birdY >= 0)
                                {
                                    if (bottomObstecleY <= 0)
                                    {
                                        Game_Over();
                                    }
                                    else
                                    {
                                        if (birdY < bottomObstecleY)
                                        {
                                            Game.Score++;
                                            ScoreCount.Text = Game.Score.ToString();

                                            var audioPlayer = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("sound.mp3"));
                                            audioPlayer.Play();
                                        }
                                        else
                                        {
                                            Game_Over();
                                        }
                                    }
                                }
                                else if (birdY <= 0)
                                {
                                    if (topObstacleY >= 0)
                                    {
                                        Game_Over();
                                    }
                                    else
                                    {
                                        if (birdY > topObstacleY)
                                        {
                                            Game.Score++;
                                            ScoreCount.Text = Game.Score.ToString();

                                            var audioPlayer = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("sound.mp3"));
                                            audioPlayer.Play();
                                        }
                                        else
                                        {
                                            Game_Over();
                                        }
                                    }
                                }

                                Game.NewCol = false;
                            }
                        }
                    }
                }
            }
            else
            {
                break;
            }
            await Task.Delay(100);
        }
    }

    async void Game_Over()
    {
        var audioPlayer = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("gameover.wav"));
        audioPlayer.Play();

        FinalScore.Text = $"Sroce: {Game.Score}";

        Game.BirdY = 0;
        Game.Score = 0;
        ScoreCount.Text = "0";
        Game.GameOver = true;
        Game.GravityState = false;
        GameOverModel.IsVisible = true;
        BackGround.IsAnimationPlaying = false;

        ((Grid)ObstacleContent.Children.LastOrDefault()).CancelAnimations();

        await Bird.RotateTo(0, 0);
        await Bird.TranslateTo(0, 0, 0);
    }

    void ExitGame(object item, EventArgs e)
    {
        Application.Current.Quit();
    }
}


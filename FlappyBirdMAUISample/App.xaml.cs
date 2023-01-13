using Plugin.Maui.Audio;

namespace FlappyBirdMAUISample;

public partial class App : Application
{
	public App(IAudioManager audioManager)
	{
		InitializeComponent();

		MainPage = new MainPage(audioManager);
	}
}

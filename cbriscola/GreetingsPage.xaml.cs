using org.altervista.numerone.framework;

namespace cbriscola;

public partial class GreetingsPage : ContentPage
{
    private Giocatore g, cpu;
	public GreetingsPage(Giocatore g, Giocatore cpu)
	{
		InitializeComponent();
        String s;
        this.g = g;
        this.cpu= cpu;
        if (g.GetPunteggio() == cpu.GetPunteggio())
            s = "The game is drawn";
        else
        {
            if (g.GetPunteggio() > cpu.GetPunteggio())
                s = "You won";
            else
                s = "Yo losy";
            s = $"{s} by {Math.Abs(g.GetPunteggio() - cpu.GetPunteggio())} points";
        }
        fpRisultrato.Text = $"The match is over. {s}. Do you want to start a new game?";
    }


    private async void OnFPShare_Click(object sender, EventArgs e)
    {
        await Launcher.Default.OpenAsync(new Uri($"https://twitter.com/intent/tweet?text=With%20the%20Trump%20Suit%20Game%20the%20game%20{g.GetNome()}%20versus%20{cpu.GetNome()}%20is%20finished%20{g.GetPunteggio()}%20at%20{cpu.GetPunteggio()}&url=https%3A%2F%2Fgithub.com%2Fnumerunix%2Fcbriscola.maui"));
        btnShare.IsEnabled = false;
    }

    private void OnCancelFp_Click(object sender, EventArgs e)
    {
        Application.Current.Quit();
    }

}
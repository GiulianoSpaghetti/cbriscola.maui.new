using CBriscola;

namespace cbriscola;

public partial class FinePartitaPage : ContentPage
{
    private Giocatore g, cpu;
	public FinePartitaPage(Giocatore g, Giocatore cpu)
	{
        string s;
        this.g=g; this.cpu=cpu;
		InitializeComponent();
        if (g.getPunteggio() == cpu.getPunteggio())
            s = "The game is drawn";
        else
        {
            if (g.getPunteggio() > cpu.getPunteggio())
                s = "You won";
            else
                s = "Yo losy";
            s = $"{s} by {Math.Abs(g.getPunteggio() - cpu.getPunteggio())} points";
        }
        Risultato.Text = $"The match is over. {s}. Do you want to start a new game?";

    }
    private async void OnShare_Click(object sender, EventArgs e)
    {
        await Launcher.Default.OpenAsync(new Uri($"https://twitter.com/intent/tweet?text=With%20the%20Trump%20Suit%20Game%20the%20game%20{g.getNome()}%20versus%20{cpu.getNome()}%20is%20finished%20{g.getPunteggio()}%20at%20{cpu.getPunteggio()}&url=https%3A%2F%2Fgithub.com%2Fnumerunix%2Fcbriscola.maui"));
        Condividi.IsEnabled = false;
    }

    private void OnCancel_Click(object sender, EventArgs e)
    {
        Application.Current.Quit();
    }
}
using org.altervista.numerone.framework;

namespace cbriscola;

public partial class GreetingsPage : ContentPage
{
    Giocatore g, cpu;
	public GreetingsPage(Giocatore g, Giocatore cpu)
	{
		InitializeComponent();
		String s;
        this.g = g;
        this.cpu=cpu;
        if (g.GetPunteggio() == cpu.GetPunteggio())
            s = "La partita è patta";
        else
        {
            if (g.GetPunteggio() > cpu.GetPunteggio())
                s = "Hai vinto";
            else
                s = "Hai perso";
            s = $"{s} per {Math.Abs(g.GetPunteggio() - cpu.GetPunteggio())}  punti";
        }
        fpRisultrato.Text = $"La partita è finita. {s}. Vuoi effettuare una nuova partita?";
    }
    private async void OnFPShare_Click(object sender, EventArgs e)
    {
        await Launcher.Default.OpenAsync(new Uri($"https://twitter.com/intent/tweet?text=Con%20la%20CBriscola%20la%20partita%20{g.GetNome()}%20contro%20{cpu.GetNome()}%20%C3%A8%20finita%20{g.GetPunteggio()}%20a%20{cpu.GetPunteggio()}&url=https%3A%2F%2Fgithub.com%2Fnumerunix%2Fcbriscola.maui"));
        btnCondividi.IsEnabled = false;
    }

    private void OnCancelFp_Click(object sender, EventArgs e)
    {
        Application.Current.Quit();
    }
}
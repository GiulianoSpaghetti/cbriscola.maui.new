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
            s = "La partita è patta";
        else
        {
            if (g.getPunteggio() > cpu.getPunteggio())
                s = "Hai vinto";
            else
                s = "Hai perso";
            s = $"{s} per {Math.Abs(g.getPunteggio() - cpu.getPunteggio())} punti";
        }
        Risultato.Text = $"La partita è finita. {s}. Vui giocarne un'altra?";

    }
    private async void OnShare_Click(object sender, EventArgs e)
    {
        await Launcher.Default.OpenAsync(new Uri($"https://twitter.com/intent/tweet?text=Con%20Cbriscola%202.0%20il%20match%20{g.getNome()}%20contro%20{cpu.getNome()}%20è%20finito%20{g.getPunteggio()}%20a%20{cpu.getPunteggio()}&url=https%3A%2F%2Fgithub.com%2Fnumerunix%2Fcbriscola.maui"));
        Condividi.IsEnabled = false;
    }

    private void OnCancel_Click(object sender, EventArgs e)
    {
        Application.Current.Quit();
    }
}
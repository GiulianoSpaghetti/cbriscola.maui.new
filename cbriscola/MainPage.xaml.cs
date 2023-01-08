using CBriscola;

namespace cbriscola;

public partial class MainPage : ContentPage
{
    int count = 0;

    private static giocatore g, cpu, primo, secondo, temp;
    private static mazzo m;
    private static carta c, c1, briscola;
    private static Image cartaCpu = new Image {
        Source = ImageSource.FromFile(@"C:\\Users\\numer\\source\\repos\\retro_carte_pc.png")
    };
    private static Image i, i1;
    private static UInt16 secondi = 5;
    private static bool avvisaTalloneFinito = true, briscolaDaPunti = false;
    private static IDispatcherTimer t;
    private string s;
    elaboratoreCarteBriscola e;
    public MainPage()
    {
        this.InitializeComponent();
        e = new elaboratoreCarteBriscola(briscolaDaPunti);
        m = new mazzo(e);
        carta.inizializza(40, cartaHelperBriscola.getIstanza(e));
        g = new giocatore(new giocatoreHelperUtente(), "Giulio", 3);
        cpu = new giocatore(new giocatoreHelperCpu(elaboratoreCarteBriscola.getCartaBriscola()), "Cpu", 3);
        primo = g;
        secondo = cpu;
        briscola = carta.getCarta(elaboratoreCarteBriscola.getCartaBriscola());
        Image[] img = new Image[3];
        for (UInt16 i = 0; i < 3; i++)
        {
            g.addCarta(m);
            cpu.addCarta(m);

        }
        NomeUtente.Text = g.getNome();
        NomeCpu.Text = cpu.getNome();
        Utente0.Source = g.getImmagine(0).Source;
        Utente1.Source = g.getImmagine(1).Source;
        Utente2.Source = g.getImmagine(2).Source;
        Cpu0.Source = cartaCpu.Source;
        Cpu1.Source = cartaCpu.Source;
        Cpu2.Source = cartaCpu.Source;
        PuntiCpu.Text = $"Punti di {cpu.getNome()}: {cpu.getPunteggio()}";
        PuntiUtente.Text = $"Punti di {g.getNome()}: {g.getPunteggio()}";
        NelMazzoRimangono.Text = $"Nel mazzo rimangono {m.getNumeroCarte()} carte";
        CartaBriscola.Text = $"Il seme di briscola è: {briscola.getSemeStr()}";
        lbCartaBriscola.Text = "La carta che designa il seme di briscola può dar punti";
        lbAvvisaTallone.Text = "Avvisa quando il tallone finisce";
        opNomeUtente.Text = "Nome Utente";
        opNomeCpu.Text = "NomeCpu";
        Secondi.Text = "Secondi";
        InfoApplicazione.Text = "Applicazione";
        OpzioniApplicazione.Text = "Applicazione";
        OpzioniInformazioni.Text = "Informazioni";
        AppInformazioni.Text = "Informazioni";
        AppOpzioni.Text = "Opzioni";
        Briscola.Source = briscola.getImmagine().Source;
        t = Dispatcher.CreateTimer();
        t.Interval = TimeSpan.FromSeconds(secondi);
        t.Tick += (s, e) =>
        {
            c = primo.getCartaGiocata();
            c1 = secondo.getCartaGiocata();
            if ((c.CompareTo(c1) > 0 && c.stessoSeme(c1)) || (c1.stessoSeme(briscola) && !c.stessoSeme(briscola)))
            {
                temp = secondo;
                secondo = primo;
                primo = temp;
            }

            primo.aggiornaPunteggio(secondo);
            PuntiCpu.Text = $"Punti di {cpu.getNome()}: {cpu.getPunteggio()}";
            PuntiUtente.Text = $"Punti di {g.getNome()}: {g.getPunteggio()}";
            if (aggiungiCarte())
            {
                NelMazzoRimangono.Text = $"Nel mazzo rimangono {m.getNumeroCarte()} carte";
                CartaBriscola.Text = $"Il seme di Briscola è: {briscola.getSemeStr()}";
                if (Briscola.IsVisible && m.getNumeroCarte() == 0)
                {
                    NelMazzoRimangono.IsVisible = false;
                    Briscola.IsVisible = false;
                }
                Utente0.Source = g.getImmagine(0).Source;
                if (cpu.getNumeroCarte() > 1)
                    Utente1.Source = g.getImmagine(1).Source;
                if (cpu.getNumeroCarte() > 2)
                    Utente2.Source = g.getImmagine(2).Source;
                i.IsVisible = true;
                i1.IsVisible = true;
                Giocata0.IsVisible = false;
                Giocata1.IsVisible = false;
                if (cpu.getNumeroCarte() == 2)
                {
                    Utente2.IsVisible = false;
                    Cpu2.IsVisible = false;
                }
                if (cpu.getNumeroCarte() == 1)
                {
                    Utente1.IsVisible = false;
                    Cpu1.IsVisible = false;
                }
                if (primo == cpu)
                {
                    i1 = giocaCpu();
                }

            }
            else
            {
                if (g.getPunteggio() == cpu.getPunteggio())
                    s = "La partita è patta";
                else
                {
                    if (g.getPunteggio() > cpu.getPunteggio())
                        s = "Hai vinto";
                    else
                        s = "Hai perso";
                    s = $"{s} per {Math.Abs(g.getPunteggio() - cpu.getPunteggio())}  punti";
                }
                fpRisultrato.Text = "La partita è finita. Vuoi effettuare una nuova partita?";
                Applicazione.IsVisible = false;
                FinePartita.IsVisible = true;
            }
            t.Stop();
        };
    }
    private Image giocaUtente(Image img)
    {
        UInt16 quale = 0;
        Image img1 = Utente0;
        if (img == Utente1)
        {
            quale = 1;
            img1 = Utente1;
        }
        if (img == Utente2)
        {
            quale = 2;
            img1 = Utente2;
        }
        Giocata0.IsVisible = true;
        Giocata0.Source = img1.Source;
        img1.IsVisible = false;
        g.gioca(quale);
        return img1;
    }

    private void OnInfo_Click(object sender, EventArgs e)
    {
        Applicazione.IsVisible = false;
        GOpzioni.IsVisible = false;
        Info.IsVisible = true;
    }

    private void OnApp_Click(object sender, EventArgs e)
    {
        GOpzioni.IsVisible = false;
        Info.IsVisible = false;
        Applicazione.IsVisible = true;
    }
    private void OnOpzioni_Click(object sender, EventArgs e)
    {
        GOpzioni.IsVisible = true;
        Info.IsVisible = false;
        Applicazione.IsVisible = false;
        txtNomeUtente.Text = g.getNome();
        txtCpu.Text = cpu.getNome();
        txtSecondi.Text = secondi.ToString();
        cbCartaBriscola.IsChecked = briscolaDaPunti;
        cbAvvisaTallone.IsChecked = avvisaTalloneFinito;
    }

    private void OnOkFp_Click(object sender, EventArgs evt)
    {
        bool cartaBriscola = true;
        FinePartita.IsVisible = false;
        if (cbCartaBriscola.IsChecked == false)
            cartaBriscola = false;
        e = new elaboratoreCarteBriscola(cartaBriscola);
        m = new mazzo(e);
        briscola = carta.getCarta(elaboratoreCarteBriscola.getCartaBriscola());
        g = new giocatore(new giocatoreHelperUtente(), g.getNome(), 3);
        cpu = new giocatore(new giocatoreHelperCpu(elaboratoreCarteBriscola.getCartaBriscola()), cpu.getNome(), 3);
        for (UInt16 i = 0; i < 3; i++)
        {
            g.addCarta(m);
            cpu.addCarta(m);

        }
        Utente0.Source = g.getImmagine(0).Source;
        Utente0.IsVisible = true;
        Utente1.Source = g.getImmagine(1).Source;
        Utente1.IsVisible = true;
        Utente2.Source = g.getImmagine(2).Source;
        Utente2.IsVisible = true;
        Cpu0.Source = cartaCpu.Source;
        Cpu0.IsVisible = true;
        Cpu1.Source = cartaCpu.Source;
        Cpu1.IsVisible = true;
        Cpu2.Source = cartaCpu.Source;
        Cpu2.IsVisible= true;
        Giocata0.IsVisible = false;
        Giocata1.IsVisible = false;
        PuntiCpu.Text = $"Punti di {cpu.getNome()}: {cpu.getPunteggio()}";
        PuntiUtente.Text = $"Punti di {g.getNome()}: {g.getPunteggio()}";
        NelMazzoRimangono.Text = $"Nel mazzo rimangono {m.getNumeroCarte()} carte";
        NelMazzoRimangono.IsVisible = true;
        CartaBriscola.Text = $"Il seme di briscola è: {briscola.getSemeStr()}";
        CartaBriscola.IsVisible = true;
        Briscola.Source = briscola.getImmagine().Source;
        Briscola.IsVisible = true;
        primo = g;
        secondo = cpu;
        Briscola.Source = briscola.getImmagine().Source;
        Applicazione.IsVisible = true;
    }
    private void OnCancelFp_Click(object sender, EventArgs e)
    {
        Application.Current.Quit();
    }

    private Image giocaCpu()
    {
        UInt16 quale = 0;
        Image img1 = Cpu0;
        if (primo == cpu)
            cpu.gioca(0);
        else
            cpu.gioca(0, g);
        quale = cpu.getICartaGiocata();
        if (quale == 1)
            img1 = Cpu1;
        if (quale == 2)
            img1 = Cpu2;
        Giocata1.IsVisible = true;
        Giocata1.Source = cpu.getCartaGiocata().getImmagine().Source;
        img1.IsVisible = false;
        return img1;
    }
    private static bool aggiungiCarte()
    {
        try
        {
            primo.addCarta(m);
            secondo.addCarta(m);
        }
        catch (IndexOutOfRangeException e)
        {
            return false;
        }
        return true;
    }

    private void Image_Tapped(object Sender, EventArgs arg)
    {
        Image img = (Image)Sender;
        t.Start();
        i = giocaUtente(img);
        if (secondo == cpu)
            i1 = giocaCpu();
    }
    public void OnOk_Click(Object source, EventArgs evt)
    {
        g.setNome(txtNomeUtente.Text);
        cpu.setNome(txtCpu.Text);
        if (cbCartaBriscola.IsChecked == false)
            briscolaDaPunti = false;
        else
            briscolaDaPunti = true;
        if (cbAvvisaTallone.IsChecked == false)
            avvisaTalloneFinito = false;
        else
            avvisaTalloneFinito = true;
        try
        {
            secondi = UInt16.Parse(txtSecondi.Text);
        }
        catch (FormatException ex)
        {
            txtSecondi.Text = "Valore non valido";
            return;
        }
        t.Interval = TimeSpan.FromSeconds(secondi);
        NomeUtente.Text = g.getNome();
        NomeCpu.Text = cpu.getNome();
        GOpzioni.IsVisible = false;
        Applicazione.IsVisible = true;
    }

    private async void OnFPShare_Click(object sender, EventArgs e)
    {
        await Launcher.Default.OpenAsync(new Uri($"https://twitter.com/intent/tweet?text=Con%20la%20CBriscola%20la%20partita%20{g.getNome()}%20contro%20{cpu.getNome()}%20%C3%A8%20finita%20{g.getPunteggio()}%20a%20{cpu.getPunteggio()}&url=https%3A%2F%2Fgithub.com%2Fnumerunix%2Fcbriscolauwp"));
    }


    private async void OnSito_Click(object sender, EventArgs e)
    {
        await Launcher.Default.OpenAsync(new Uri("https://github.com/numerunix/cbriscolauwp"));
    }
}



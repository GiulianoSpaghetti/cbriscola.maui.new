using CBriscola;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Reflection;

namespace cbriscola;

public partial class MainPage : ContentPage
{
    private static Giocatore g, cpu, primo, secondo, temp;
    private static Mazzo m;
    private static Carta c, c1, briscola;
    private static UInt16 secondi = 5;
    private static bool avvisaTalloneFinito = true, briscolaDaPunti = false;
    private static IDispatcherTimer t;
    private TapGestureRecognizer gesture;
    private elaboratoreCarteBriscola e;
    public MainPage()
    {
        this.InitializeComponent();
        briscolaDaPunti = Preferences.Get("briscolaDaPunti", false);
        avvisaTalloneFinito = Preferences.Get("avvisaTalloneFinito", true);
        secondi = (UInt16)Preferences.Get("secondi", 5);

        e = new elaboratoreCarteBriscola(briscolaDaPunti);
        m = new Mazzo(e);
        Carta.inizializza(40, CartaHelperBriscola.getIstanza(e));
        g = new Giocatore(new GiocatoreHelperUtente(), Preferences.Get("nomeUtente", "Giulio"), 3);
        cpu = new Giocatore(new GiocatoreHelperCpu(elaboratoreCarteBriscola.getCartaBriscola()), Preferences.Get("nomeCpu", "Cpu"), 3);
        primo = g;
        secondo = cpu;
        briscola = Carta.getCarta(elaboratoreCarteBriscola.getCartaBriscola());
        gesture = new TapGestureRecognizer();
        gesture.Tapped += Image_Tapped;
        for (UInt16 i = 0; i < 3; i++)
        {
            g.addCarta(m);
            cpu.addCarta(m);

        }
        visualizzaImmagine(g.getID(0), 1, 0, true);
        visualizzaImmagine(g.getID(1), 1, 1, true);
        visualizzaImmagine(g.getID(2), 1, 2, true);

        NomeUtente.Text = g.getNome();
        NomeCpu.Text = cpu.getNome();
        PuntiCpu.Text = $"{cpu.getNome()} points: {cpu.getPunteggio()}";
        PuntiUtente.Text = $"{g.getNome()} points: {g.getPunteggio()}";
        NelMazzoRimangono.Text = $"There are {m.getNumeroCarte()} cards left in the Deck";
        CartaBriscola.Text = $"The trump suit is: {briscola.getSemeStr()}";
        lbCartaBriscola.Text = "The Card designating the trump suit can score points";
        lbAvvisaTallone.Text = "Alerts when the deck ends";
        opNomeUtente.Text = "Username";
        opNomeCpu.Text = "CPU name";
        Secondi.Text = "Seconds during which to show the plays";
        InfoApplicazione.Text = "Application";
        OpzioniApplicazione.Text = "Application";
        OpzioniInformazioni.Text = "Informations";
        AppInformazioni.Text = "Informations";
        AppOpzioni.Text = "Options";
        visualizzaImmagine(Carta.getCarta(elaboratoreCarteBriscola.getCartaBriscola()).getID(), 4, 4, false);

        t = Dispatcher.CreateTimer();
        t.Interval = TimeSpan.FromSeconds(secondi);
        t.Tick += (s, e) =>
        {
            Informazioni.Text = "";
            c = primo.getCartaGiocata();
            c1 = secondo.getCartaGiocata();
            ((Image)this.FindByName(c.getID())).IsVisible = false;
            ((Image)this.FindByName(c1.getID())).IsVisible = false;
            if ((c.CompareTo(c1) > 0 && c.stessoSeme(c1)) || (c1.stessoSeme(briscola) && !c.stessoSeme(briscola)))
            {
                temp = secondo;
                secondo = primo;
                primo = temp;
            }

            primo.aggiornaPunteggio(secondo);
            PuntiCpu.Text = $"{cpu.getNome()} points: {cpu.getPunteggio()}";
            PuntiUtente.Text = $"{g.getNome()} points: {g.getPunteggio()}";
            if (aggiungiCarte())
            {
                NelMazzoRimangono.Text = $"There are {m.getNumeroCarte()} cards left in the Deck";
                CartaBriscola.Text = $"The trump suit is: {briscola.getSemeStr()}";
                if (m.getNumeroCarte() == 0)
                {
                    ((Image)this.FindByName(Carta.getCarta(elaboratoreCarteBriscola.getCartaBriscola()).getID())).IsVisible = false;
                    NelMazzoRimangono.IsVisible = false;
                    if (avvisaTalloneFinito)
                        Informazioni.Text = "The Deck is finished";
                }
                for (UInt16 i = 0; i < g.getNumeroCarte(); i++)
                {
                    visualizzaImmagine(g.getID(i), 1, i, true);
                    ((Image)this.FindByName("Cpu" + i)).IsVisible = true;
                }
                if (cpu.getNumeroCarte() == 2)
                    Cpu2.IsVisible = false;
                if (cpu.getNumeroCarte() == 1)
                    Cpu1.IsVisible = false;

                if (primo == cpu)
                {
                    giocaCpu();
                    if (cpu.getCartaGiocata().stessoSeme(briscola))
                        Informazioni.Text = $"The CPU has played the {cpu.getCartaGiocata().getValore() + 1} of Briscola";
                    else if (cpu.getCartaGiocata().getPunteggio() > 0)
                        Informazioni.Text = $"The CPU has played the {cpu.getCartaGiocata().getValore() + 1} of {cpu.getCartaGiocata().getSemeStr()}";
                }

            }
            else
            {
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
                fpRisultrato.Text = $"The match is over. {s}. Do you want to start a new game?";
                Applicazione.IsVisible = false;
                FinePartita.IsVisible = true;
            }
            t.Stop();
        };
    }

    private void visualizzaImmagine(String id, UInt16 i, UInt16 j, bool abilitaGesture)
    {
        Image img;
        img = (Image)this.FindByName(id);
        Applicazione.SetRow(img, i);
        Applicazione.SetColumn(img, j);
        img.IsVisible = true;
        if (abilitaGesture)
            img.GestureRecognizers.Add(gesture);
        else
            img.GestureRecognizers.Clear();

    }
    private void giocaUtente(Image img)
    {
        UInt16 quale = 0;
        Image img1;
        for (UInt16 i = 1; i < g.getNumeroCarte(); i++)
        {
            img1 = (Image)this.FindByName(g.getID(i));
            if (img.Id == img1.Id)
                quale = i;
        }
        visualizzaImmagine(g.getID(quale), 2, 0, false);
        g.gioca(quale);
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
        e = new elaboratoreCarteBriscola(briscolaDaPunti);
        m = new Mazzo(e);
        briscola = Carta.getCarta(elaboratoreCarteBriscola.getCartaBriscola());
        g = new Giocatore(new GiocatoreHelperUtente(), g.getNome(), 3);
        cpu = new Giocatore(new GiocatoreHelperCpu(elaboratoreCarteBriscola.getCartaBriscola()), cpu.getNome(), 3);
        for (UInt16 i = 0; i < 3; i++)
        {
            g.addCarta(m);
            cpu.addCarta(m);
        }
        visualizzaImmagine(g.getID(0), 1, 0, true);
        visualizzaImmagine(g.getID(1), 1, 1, true);
        visualizzaImmagine(g.getID(2), 1, 2, true);

        Cpu0.IsVisible = true;
        Cpu1.IsVisible = true;
        Cpu2.IsVisible = true;
        PuntiCpu.Text = $"{cpu.getNome()} points: {cpu.getPunteggio()}";
        PuntiUtente.Text = $"{g.getNome()} points: {g.getPunteggio()}";
        NelMazzoRimangono.Text = $"There are {m.getNumeroCarte()} cards left in the Deck";
        NelMazzoRimangono.IsVisible = true;
        CartaBriscola.Text = $"The trump suit is: {briscola.getSemeStr()}";
        CartaBriscola.IsVisible = true;
        primo = g;
        secondo = cpu;
        visualizzaImmagine(Carta.getCarta(elaboratoreCarteBriscola.getCartaBriscola()).getID(), 4, 4, false);
        FinePartita.IsVisible = false;
        Applicazione.IsVisible = true;
    }
    private void OnCancelFp_Click(object sender, EventArgs e)
    {
        Application.Current.Quit();
    }

    private void giocaCpu()
    {
        UInt16 quale = 0;
        Image img1 = Cpu0;
        if (primo == cpu)
            cpu.gioca(0);
        else
            cpu.gioca(0, g);
        quale = cpu.getICartaGiocata();
        img1 = (Image)this.FindByName("Cpu" + quale);
        img1.IsVisible = false;
        visualizzaImmagine(cpu.getCartaGiocata().getID(), 2, 2, false);
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
        giocaUtente(img);
        if (secondo == cpu)
            giocaCpu();
    }
    public void OnOk_Click(Object source, EventArgs evt)
    {
        Preferences.Set("nomeUtente", txtNomeUtente.Text);
        Preferences.Set("nomeCpu", txtCpu.Text);
        g.setNome(txtNomeUtente.Text);
        cpu.setNome(txtCpu.Text);
        if (cbCartaBriscola.IsChecked == false)
            briscolaDaPunti = false;
        else
            briscolaDaPunti = true;
        Preferences.Set("briscolaDaPunti", briscolaDaPunti);
        if (cbAvvisaTallone.IsChecked == false)
            avvisaTalloneFinito = false;
        else
            avvisaTalloneFinito = true;
        Preferences.Set("avvisaTalloneFinito", avvisaTalloneFinito);

        try
        {
            secondi = UInt16.Parse(txtSecondi.Text);
        }
        catch (FormatException ex)
        {
            txtSecondi.Text = "Invalid rvalue";
            return;
        }
        Preferences.Set("secondi", secondi);

        t.Interval = TimeSpan.FromSeconds(secondi);
        NomeUtente.Text = g.getNome();
        NomeCpu.Text = cpu.getNome();
        GOpzioni.IsVisible = false;
        Applicazione.IsVisible = true;
    }

    private async void OnFPShare_Click(object sender, EventArgs e)
    {
        await Launcher.Default.OpenAsync(new Uri($"https://twitter.com/intent/tweet?text=With%20the%20Trump%20Suit%20Game%20the%20game%20{g.getNome()}%versus%20{cpu.getNome()}%20is%20finished%20{g.getPunteggio()}%20at%20{cpu.getPunteggio()}&url=https%3A%2F%2Fgithub.com%2Fnumerunix%2Fcbriscola.maui"));
    }


    private async void OnSito_Click(object sender, EventArgs e)
    {
        await Launcher.Default.OpenAsync(new Uri("https://github.com/numerunix/cbriscola.maui"));
    }
}
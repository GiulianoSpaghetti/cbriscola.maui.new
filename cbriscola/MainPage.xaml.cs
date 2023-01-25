using CBriscola;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Reflection;

namespace cbriscola;

public partial class MainPage : ContentPage
{
    private static giocatore g, cpu, primo, secondo, temp;
    private static mazzo m;
    private static carta c, c1, briscola;
    private static UInt16 secondi = 5;
    private static bool avvisaTalloneFinito = true, briscolaDaPunti = false;
    private static IDispatcherTimer t;
    private TapGestureRecognizer gesture;
    private elaboratoreCarteBriscola e;
    public MainPage()
    {
        Image img;
        this.InitializeComponent();
        briscolaDaPunti = Preferences.Get("briscolaDaPunti", false);
        avvisaTalloneFinito = Preferences.Get("avvisaTalloneFinito", true);
        secondi = (UInt16)Preferences.Get("secondi", 5);

        e = new elaboratoreCarteBriscola(briscolaDaPunti);
        m = new mazzo(e);
        carta.inizializza(40, cartaHelperBriscola.getIstanza(e));
        g = new giocatore(new giocatoreHelperUtente(), Preferences.Get("nomeUtente", "Giulio"), 3);
        cpu = new giocatore(new giocatoreHelperCpu(elaboratoreCarteBriscola.getCartaBriscola()), Preferences.Get("nomeCpu", "Cpu"), 3);
        primo = g;
        secondo = cpu;
        briscola = carta.getCarta(elaboratoreCarteBriscola.getCartaBriscola());
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
        visualizzaImmagine(carta.getCarta(elaboratoreCarteBriscola.getCartaBriscola()).getID(), 4, 4, false);

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
            PuntiCpu.Text = $"Punti di {cpu.getNome()}: {cpu.getPunteggio()}";
            PuntiUtente.Text = $"Punti di {g.getNome()}: {g.getPunteggio()}";
            if (aggiungiCarte())
            {
                NelMazzoRimangono.Text = $"Nel mazzo rimangono {m.getNumeroCarte()} carte";
                CartaBriscola.Text = $"Il seme di Briscola è: {briscola.getSemeStr()}";
                if (m.getNumeroCarte() == 0)
                {
                    ((Image)this.FindByName(carta.getCarta(elaboratoreCarteBriscola.getCartaBriscola()).getID())).IsVisible = false;
                    NelMazzoRimangono.IsVisible = false;
                    if (avvisaTalloneFinito)
                        Informazioni.Text = "Il tallone è finito";
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
                        Informazioni.Text = $"La CPU ha giocato il {cpu.getCartaGiocata().getValore() + 1} di Briscola";
                    else if (cpu.getCartaGiocata().getPunteggio() > 0)
                        Informazioni.Text = $"La CPU ha giocato il {cpu.getCartaGiocata().getValore() + 1} di {cpu.getCartaGiocata().getSemeStr()}";
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
                fpRisultrato.Text = $"La partita è finita. {s} Vuoi effettuare una nuova partita?";
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
        m = new mazzo(e);
        briscola = carta.getCarta(elaboratoreCarteBriscola.getCartaBriscola());
        g = new giocatore(new giocatoreHelperUtente(), g.getNome(), 3);
        cpu = new giocatore(new giocatoreHelperCpu(elaboratoreCarteBriscola.getCartaBriscola()), cpu.getNome(), 3);
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
        PuntiCpu.Text = $"Punti di {cpu.getNome()}: {cpu.getPunteggio()}";
        PuntiUtente.Text = $"Punti di {g.getNome()}: {g.getPunteggio()}";
        NelMazzoRimangono.Text = $"Nel mazzo rimangono {m.getNumeroCarte()} carte";
        NelMazzoRimangono.IsVisible = true;
        CartaBriscola.Text = $"Il seme di briscola è: {briscola.getSemeStr()}";
        CartaBriscola.IsVisible = true;
        primo = g;
        secondo = cpu;
        visualizzaImmagine(carta.getCarta(elaboratoreCarteBriscola.getCartaBriscola()).getID(), 4, 4, false);
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
            txtSecondi.Text = "Valore non valido";
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
        await Launcher.Default.OpenAsync(new Uri($"https://twitter.com/intent/tweet?text=Con%20la%20CBriscola%20la%20partita%20{g.getNome()}%20contro%20{cpu.getNome()}%20%C3%A8%20finita%20{g.getPunteggio()}%20a%20{cpu.getPunteggio()}&url=https%3A%2F%2Fgithub.com%2Fnumerunix%2Fcbriscola.maui"));
    }


    private async void OnSito_Click(object sender, EventArgs e)
    {
        await Launcher.Default.OpenAsync(new Uri("https://github.com/numerunix/cbriscola.maui"));
    }
}
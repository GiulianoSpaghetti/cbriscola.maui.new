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
        Carta.inizializza(40, CartaHelperBriscola.getIstanza());
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
                Navigation.PushAsync(new FinePartitaPage(g, cpu));
                nuovaPartita();
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
        Navigation.PushAsync(new InfoPage());
    }


    private void OnOpzioni_Click(object sender, EventArgs e)
    {
        Navigation.PushAsync(new OpzioniPage());
        g.setNome(Preferences.Get("nomeUtente", "Giulio"));
        cpu.setNome(Preferences.Get("nomeCpu", "Cpu"));
        avvisaTalloneFinito = Preferences.Get("avvisaTalloneFinito", true);
        briscolaDaPunti = Preferences.Get("briscolaDaPunti", false);
        secondi = (UInt16) Preferences.Get("secondi", 5);
        t.Interval = TimeSpan.FromSeconds(secondi);
        NomeUtente.Text = g.getNome();
        NomeCpu.Text = cpu.getNome();
    }

    private void nuovaPartita()
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

    }
    private void OnNuovaPartita_Click(object sender, EventArgs evt)
    {
        nuovaPartita();
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
}
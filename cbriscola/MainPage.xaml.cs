using org.altervista.numerone.framework;
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
    private ElaboratoreCarteBriscola e;
    public MainPage()
    {
        this.InitializeComponent();
        briscolaDaPunti = Preferences.Get("briscolaDaPunti", false);
        avvisaTalloneFinito = Preferences.Get("avvisaTalloneFinito", true);
        secondi = (UInt16)Preferences.Get("secondi", 5);

        e = new ElaboratoreCarteBriscola(briscolaDaPunti);
        m = new Mazzo(e);
        Carta.Inizializza(40, CartaHelperBriscola.GetIstanza());
        g = new Giocatore(new GiocatoreHelperUtente(), Preferences.Get("nomeUtente", "Giulio"), 3);
        cpu = new Giocatore(new GiocatoreHelperCpu(ElaboratoreCarteBriscola.GetCartaBriscola()), Preferences.Get("nomeCpu", "Cpu"), 3);
        primo = g;
        secondo = cpu;
        briscola = Carta.GetCarta(ElaboratoreCarteBriscola.GetCartaBriscola());
        gesture = new TapGestureRecognizer();
        gesture.Tapped += Image_Tapped;
        for (UInt16 i = 0; i < 3; i++)
        {
            g.AddCarta(m);
            cpu.AddCarta(m);

        }
        visualizzaImmagine(g.GetID(0), 1, 0, true);
        visualizzaImmagine(g.GetID(1), 1, 1, true);
        visualizzaImmagine(g.GetID(2), 1, 2, true);

        NomeUtente.Text = g.GetNome();
        NomeCpu.Text = cpu.GetNome();
        PuntiCpu.Text = $"Punti di {cpu.GetNome()}: {cpu.GetPunteggio()}";
        PuntiUtente.Text = $"Punti di {g.GetNome()}: {g.GetPunteggio()}";
        NelMazzoRimangono.Text = $"Nel mazzo rimangono {m.GetNumeroCarte()} carte";
        CartaBriscola.Text = $"Il seme di briscola è: {briscola.GetSemeStr()}";
        visualizzaImmagine(Carta.GetCarta(ElaboratoreCarteBriscola.GetCartaBriscola()).GetID(), 4, 4, false);

        t = Dispatcher.CreateTimer();
        t.Interval = TimeSpan.FromSeconds(secondi);
        t.Tick += (s, e) =>
        {
            Informazioni.Text = "";
            c = primo.GetCartaGiocata();
            c1 = secondo.GetCartaGiocata();
            ((Image)this.FindByName(c.GetID())).IsVisible = false;
            ((Image)this.FindByName(c1.GetID())).IsVisible = false;
            if ((c.CompareTo(c1) > 0 && c.StessoSeme(c1)) || (c1.StessoSeme(briscola) && !c.StessoSeme(briscola)))
            {
                temp = secondo;
                secondo = primo;
                primo = temp;
            }

            primo.AggiornaPunteggio(secondo);
            PuntiCpu.Text = $"Punti di {cpu.GetNome()}: {cpu.GetPunteggio()}";
            PuntiUtente.Text = $"Punti di {g.GetNome()}: {g.GetPunteggio()}";
            if (AggiungiCarte())
            {
                NelMazzoRimangono.Text = $"Nel mazzo rimangono {m.GetNumeroCarte()} carte";
                CartaBriscola.Text = $"Il seme di briscola è: {briscola.GetSemeStr()}";
                if (m.GetNumeroCarte() == 0)
                {
                    ((Image)this.FindByName(Carta.GetCarta(ElaboratoreCarteBriscola.GetCartaBriscola()).GetID())).IsVisible = false;
                    NelMazzoRimangono.IsVisible = false;
                    if (avvisaTalloneFinito)
                        Informazioni.Text = "Il mazzo è finito";
                }
                for (UInt16 i = 0; i < g.GetNumeroCarte(); i++)
                {
                    visualizzaImmagine(g.GetID(i), 1, i, true);
                    ((Image)this.FindByName("Cpu" + i)).IsVisible = true;
                }
                if (cpu.GetNumeroCarte() == 2)
                    Cpu2.IsVisible = false;
                if (cpu.GetNumeroCarte() == 1)
                    Cpu1.IsVisible = false;

                if (primo == cpu)
                {
                    GiocaCpu();
                    if (cpu.GetCartaGiocata().StessoSeme(briscola))
                        Informazioni.Text = $"La CPU ha giocato il {cpu.GetCartaGiocata().GetValore() + 1} di briscola";
                    else if (cpu.GetCartaGiocata().GetPunteggio() > 0)
                        Informazioni.Text = $"La cpu ha giocato il {cpu.GetCartaGiocata().GetValore() + 1} di {cpu.GetCartaGiocata().GetSemeStr()}";
                }

            }
            else
            {
                Navigation.PushAsync(new FinePartitaPage(g, cpu));
                NuovaPartita();
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
    private void GiocaUtente(Image img)
    {
        UInt16 quale = 0;
        Image img1;
        for (UInt16 i = 1; i < g.GetNumeroCarte(); i++)
        {
            img1 = (Image)this.FindByName(g.GetID(i));
            if (img.Id == img1.Id)
                quale = i;
        }
        visualizzaImmagine(g.GetID(quale), 2, 0, false);
        g.Gioca(quale);
    }

    private void OnInfo_Click(object sender, EventArgs e)
    {
        Navigation.PushAsync(new InfoPage());
    }


    private void OnOpzioni_Click(object sender, EventArgs e)
    {
        Navigation.PushAsync(new OpzioniPage());
        g.SetNome(Preferences.Get("nomeUtente", "Giulio"));
        cpu.SetNome(Preferences.Get("nomeCpu", "Cpu"));
        avvisaTalloneFinito = Preferences.Get("avvisaTalloneFinito", true);
        briscolaDaPunti = Preferences.Get("briscolaDaPunti", false);
        secondi = (UInt16) Preferences.Get("secondi", 5);
        t.Interval = TimeSpan.FromSeconds(secondi);
        NomeUtente.Text = g.GetNome();
        NomeCpu.Text = cpu.GetNome();
    }

    private void NuovaPartita()
    {
        Image img;
        for (UInt16 i=0; i<40; i++)
        {
            img = (Image)this.FindByName(Carta.GetCarta(i).GetID());
            img.IsVisible = false;
            img.GestureRecognizers.Clear();

        }
        e = new ElaboratoreCarteBriscola(briscolaDaPunti);
        m = new Mazzo(e);
        briscola = Carta.GetCarta(ElaboratoreCarteBriscola.GetCartaBriscola());
        g = new Giocatore(new GiocatoreHelperUtente(), g.GetNome(), 3);
        cpu = new Giocatore(new GiocatoreHelperCpu(ElaboratoreCarteBriscola.GetCartaBriscola()), cpu.GetNome(), 3);
        for (UInt16 i = 0; i < 3; i++)
        {
            g.AddCarta(m);
            cpu.AddCarta(m);
        }
        visualizzaImmagine(g.GetID(0), 1, 0, true);
        visualizzaImmagine(g.GetID(1), 1, 1, true);
        visualizzaImmagine(g.GetID(2), 1, 2, true);

        Cpu0.IsVisible = true;
        Cpu1.IsVisible = true;
        Cpu2.IsVisible = true;
        PuntiCpu.Text = $"Punti di {cpu.GetNome()}: {cpu.GetPunteggio()}";
        PuntiUtente.Text = $"Punti di {g.GetNome()}: {g.GetPunteggio()}";
        NelMazzoRimangono.Text = $"Nel mazzo rimangono {m.GetNumeroCarte()} carte";
        CartaBriscola.Text = $"Il seme di briscola è: {briscola.GetSemeStr()}";
        NelMazzoRimangono.IsVisible = true;
        CartaBriscola.IsVisible = true;
        primo = g;
        secondo = cpu;
        visualizzaImmagine(Carta.GetCarta(ElaboratoreCarteBriscola.GetCartaBriscola()).GetID(), 4, 4, false);

    }
    private void OnNuovaPartita_Click(object sender, EventArgs evt)
    {
        NuovaPartita();
    }
    private void OnCancelFp_Click(object sender, EventArgs e)
    {
        Application.Current.Quit();
    }

    private void GiocaCpu()
    {
        UInt16 quale = 0;
        Image img1 = Cpu0;
        if (primo == cpu)
            cpu.Gioca(0);
        else
            cpu.Gioca(0, g);
        quale = cpu.GetICartaGiocata();
        img1 = (Image)this.FindByName("Cpu" + quale);
        img1.IsVisible = false;
        visualizzaImmagine(cpu.GetCartaGiocata().GetID(), 2, 2, false);
    }
    private static bool AggiungiCarte()
    {
        try
        {
            primo.AddCarta(m);
            secondo.AddCarta(m);
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
        GiocaUtente(img);
        if (secondo == cpu)
            GiocaCpu();
    }
}
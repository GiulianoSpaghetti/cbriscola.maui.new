﻿using org.altervista.numerone.framework;
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
        Carta.Inizializza(40, CartaHelperBriscola.GetIstanza(e));
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
        VisualizzaImmagine(g.GetID(0), 1, 0, true);
        VisualizzaImmagine(g.GetID(1), 1, 1, true);
        VisualizzaImmagine(g.GetID(2), 1, 2, true);

        NomeUtente.Text = g.GetNome();
        NomeCpu.Text = cpu.GetNome();
        PuntiCpu.Text = $"{cpu.GetNome()} points: {cpu.GetPunteggio()}";
        PuntiUtente.Text = $"{g.GetNome()} points: {g.GetPunteggio()}";
        NelMazzoRimangono.Text = $"There are {m.GetNumeroCarte()} cards left in the Deck";
        CartaBriscola.Text = $"The trump suit is: {briscola.GetSemeStr()}";
        VisualizzaImmagine(Carta.GetCarta(ElaboratoreCarteBriscola.GetCartaBriscola()).GetID(), 4, 4, false);

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
            PuntiCpu.Text = $"{cpu.GetNome()} points: {cpu.GetPunteggio()}";
            PuntiUtente.Text = $"{g.GetNome()} points: {g.GetPunteggio()}";
            if (AggiungiCarte())
            {
                NelMazzoRimangono.Text = $"There are {m.GetNumeroCarte()} cards left in the Deck";
                CartaBriscola.Text = $"The trump suit is: {briscola.GetSemeStr()}";
                if (m.GetNumeroCarte() == 0)
                {
                    ((Image)this.FindByName(Carta.GetCarta(ElaboratoreCarteBriscola.GetCartaBriscola()).GetID())).IsVisible = false;
                    NelMazzoRimangono.IsVisible = false;
                    if (avvisaTalloneFinito)
                        Informazioni.Text = "The Deck is finished";
                }
                for (UInt16 i = 0; i < g.GetNumeroCarte(); i++)
                {
                    VisualizzaImmagine(g.GetID(i), 1, i, true);
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
                        Informazioni.Text = $"The CPU has played the {cpu.GetCartaGiocata().GetValore() + 1} of Briscola";
                    else if (cpu.GetCartaGiocata().GetPunteggio() > 0)
                        Informazioni.Text = $"The CPU has played the {cpu.GetCartaGiocata().GetValore() + 1} of {cpu.GetCartaGiocata().GetSemeStr()}";
                }

            }
            else
            {
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
                Applicazione.IsVisible = false;
                btnShare.IsEnabled = true;
                FinePartita.IsVisible = true;
            }
            t.Stop();
        };
    }

    private void VisualizzaImmagine(String id, UInt16 i, UInt16 j, bool abilitaGesture)
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
        VisualizzaImmagine(g.GetID(quale), 2, 0, false);
        g.Gioca(quale);
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
        txtNomeUtente.Text = g.GetNome();
        txtCpu.Text = cpu.GetNome();
        txtSecondi.Text = secondi.ToString();
        cbCartaBriscola.IsChecked = briscolaDaPunti;
        cbAvvisaTallone.IsChecked = avvisaTalloneFinito;
    }

    private void OnOkFp_Click(object sender, EventArgs evt)
    {
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
        VisualizzaImmagine(g.GetID(0), 1, 0, true);
        VisualizzaImmagine(g.GetID(1), 1, 1, true);
        VisualizzaImmagine(g.GetID(2), 1, 2, true);

        Cpu0.IsVisible = true;
        Cpu1.IsVisible = true;
        Cpu2.IsVisible = true;
        PuntiCpu.Text = $"{cpu.GetNome()} points: {cpu.GetPunteggio()}";
        PuntiUtente.Text = $"{g.GetNome()} points: {g.GetPunteggio()}";
        NelMazzoRimangono.Text = $"There are {m.GetNumeroCarte()} cards left in the Deck";
        NelMazzoRimangono.IsVisible = true;
        CartaBriscola.Text = $"The trump suit is: {briscola.GetSemeStr()}";
        CartaBriscola.IsVisible = true;
        primo = g;
        secondo = cpu;
        VisualizzaImmagine(Carta.GetCarta(ElaboratoreCarteBriscola.GetCartaBriscola()).GetID(), 4, 4, false);
        FinePartita.IsVisible = false;
        Applicazione.IsVisible = true;
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
        VisualizzaImmagine(cpu.GetCartaGiocata().GetID(), 2, 2, false);
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
    public void OnOk_Click(Object source, EventArgs evt)
    {
        Preferences.Set("nomeUtente", txtNomeUtente.Text);
        Preferences.Set("nomeCpu", txtCpu.Text);
        g.SetNome(txtNomeUtente.Text);
        cpu.SetNome(txtCpu.Text);
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
        if (secondi>10)
        {
            txtSecondi.Text = "rvalue too high";
            return;
        }
        Preferences.Set("secondi", secondi);

        t.Interval = TimeSpan.FromSeconds(secondi);
        NomeUtente.Text = g.GetNome();
        NomeCpu.Text = cpu.GetNome();
        GOpzioni.IsVisible = false;
        Applicazione.IsVisible = true;
    }

    private async void OnFPShare_Click(object sender, EventArgs e)
    {
        await Launcher.Default.OpenAsync(new Uri($"https://twitter.com/intent/tweet?text=With%20the%20Trump%20Suit%20Game%20the%20game%20{g.GetNome()}%20versus%20{cpu.GetNome()}%20is%20finished%20{g.GetPunteggio()}%20at%20{cpu.GetPunteggio()}&url=https%3A%2F%2Fgithub.com%2Fnumerunix%2Fcbriscola.maui"));
        btnShare.IsEnabled= false;
    }


    private async void OnSito_Click(object sender, EventArgs e)
    {
        await Launcher.Default.OpenAsync(new Uri("https://github.com/numerunix/cbriscola.maui"));
    }
}
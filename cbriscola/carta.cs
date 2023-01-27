/*
 *  This code is distribuited under GPL 3.0 or, at your opinion, any later version
 *  CBriscola 0.1
 *
 *  Created by numerunix on 22/05/22.
 *  Copyright 2022 Some rights reserved.
 *
 */


using Microsoft.Maui.Controls.PlatformConfiguration;
using System;

namespace CBriscola
{
    class Carta
    {
        private UInt16 seme,
                   valore,
                   punteggio;
        private string semeStr;
        private CartaHelperBriscola helper;
        private static Carta[] carte = new Carta[40];
        private Carta(UInt16 n, CartaHelperBriscola h)
        {
            helper = h;
            seme = helper.getSeme(n);
            valore = helper.getValore(n);
            punteggio = helper.getPunteggio(n);
            semeStr = helper.getSemeStr(n);
        }
        public static void inizializza(UInt16 n, CartaHelperBriscola h)
        {
            for (UInt16 i = 0; i < n; i++)
            {
                carte[i] = new Carta(i, h);

            }
        }
        public static Carta getCarta(UInt16 quale) { return carte[quale]; }
        public UInt16 getSeme() { return seme; }
        public UInt16 getValore() { return valore; }
        public UInt16 getPunteggio() { return punteggio; }
        public string getSemeStr() { return semeStr; }
        public bool stessoSeme(Carta c1) { if (c1 == null) return false; else return seme == c1.getSeme(); }
        public int CompareTo(Carta c1)
        {
            if (c1 == null)
                return 1;
            else
                return helper.CompareTo(helper.getNumero(getSeme(), getValore()), helper.getNumero(c1.getSeme(), c1.getValore()));
        }

        public override string ToString()
        {
            return $"{valore + 1} di {semeStr}{(stessoSeme(helper.getCartaBriscola()) ? "*" : " ")} ";
        }

        public String getID()
        {
            return $"n{seme * 10 + valore}";
        }
    }
}

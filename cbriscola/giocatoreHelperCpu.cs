/*
  *  This code is distribuited under GPL 3.0 or, at your opinion, any later version
 *  CBriscola 2.0
 *
 *  Created by Giulio Sorrentno on 28/01/23.
 *  Copyright 2023 Some rights reserved.
 *
 */

using System;

namespace org.altervista.numerone.framework
{
    class GiocatoreHelperCpu : GiocatoreHelper
    {
        private readonly Carta briscola;
        private UInt16 GetBriscola(Carta[] mano)
        {
            UInt16 i;
            for (i = 0; i < mano.Length; i++)
                if (briscola.StessoSeme(mano[i]))
                    break;
            return i;
        }
        public GiocatoreHelperCpu(UInt16 b)
        {
            briscola = Carta.GetCarta(b);
        }
        private UInt16 GetSoprataglio(Carta[] mano, Carta c, bool maggiore)
        {
            bool trovata = false;
            UInt16 i;
            if (maggiore)
            {
                for (i = (UInt16)(mano.Length - 1); i > 0; i--)
                    if (c.StessoSeme(mano[i]) && c.CompareTo(mano[i]) > 0)
                    {
                        trovata = true;
                        break;
                    }
                    else if (c.StessoSeme(mano[i]) && mano[i].CompareTo(c) > 0)
                        break;
            }
            else
            {
                for (i = 0; i < mano.Length; i++)
                    if (c.StessoSeme(mano[i]) && c.CompareTo(mano[i]) > 0)
                    {
                        trovata = true;
                        break;
                    }
            }
            if (trovata)
                return i;
            else
                return (UInt16)mano.Length;
        }
        public UInt16 Gioca(UInt16 x, Carta[] mano, UInt16 numeroCarte)
        {
            UInt16 i;
            for (i = (UInt16)(numeroCarte - 1); i > 0; i--) ;
            if ((mano[i].GetPunteggio() > 4 || briscola.StessoSeme(mano[i])))
                i = 0;
            return i;

        }
        public UInt16 Gioca(UInt16 x, Carta[] mano, UInt16 numeroCarte, Carta c)
        {
            UInt16 i = (UInt16)ElaboratoreCarteBriscola.r.Next(0, UInt16.MaxValue);
            if (!briscola.StessoSeme(c))
            {
                if ((i = GetSoprataglio(mano, c, true)) < numeroCarte)
                    return i;
                if (c.GetPunteggio() > 0 && (i = GetBriscola(mano)) < numeroCarte)
                {
                    if (c.GetPunteggio() > 4)
                        return i;
                    if (mano[i].GetPunteggio() > 0)
                        if (ElaboratoreCarteBriscola.r.Next() % 10 < 5)
                            return i;
                }
            }
            else
            {
                if (ElaboratoreCarteBriscola.r.Next() % 10 < 5 && (i = GetSoprataglio(mano, c, false)) < numeroCarte)
                    return i;
            }
            i = 0;
            return i;
        }
        public void AggiornaPunteggio(ref UInt16 punteggioAttuale, Carta c, Carta c1) { punteggioAttuale = (UInt16)(punteggioAttuale + c.GetPunteggio() + c1.GetPunteggio()); }
    }
}
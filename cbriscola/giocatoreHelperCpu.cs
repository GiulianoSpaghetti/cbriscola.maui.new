/*
  *  This code is distribuited under GPL 3.0 or, at your opinion, any later version
 *  CBriscola 0.1
 *
 *  Created by numerunix on 22/05/22.
 *  Copyright 2022 Some rights reserved.
 *
 */

using System;

namespace CBriscola
{
    class GiocatoreHelperCpu : GiocatoreHelper
    {
        private Carta briscola;
        private UInt16 getBriscola(Carta[] mano)
        {
            UInt16 i;
            for (i = 0; i < mano.Length; i++)
                if (briscola.stessoSeme(mano[i]))
                    break;
            return i;
        }
        public GiocatoreHelperCpu(UInt16 b)
        {
            briscola = Carta.getCarta(b);
        }
        private UInt16 getSoprataglio(Carta[] mano, Carta c, bool maggiore)
        {
            bool trovata = false;
            UInt16 i;
            if (maggiore)
            {
                for (i = (UInt16)(mano.Length - 1); i > 0; i--)
                    if (c.stessoSeme(mano[i]) && c.CompareTo(mano[i]) > 0)
                    {
                        trovata = true;
                        break;
                    }
                    else if (c.stessoSeme(mano[i]) && mano[i].CompareTo(c) > 0)
                        break;
            }
            else
            {
                for (i = 0; i < mano.Length; i++)
                    if (c.stessoSeme(mano[i]) && c.CompareTo(mano[i]) > 0)
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
        public UInt16 gioca(UInt16 x, Carta[] mano, UInt16 numeroCarte)
        {
            UInt16 i;
            for (i = (UInt16)(numeroCarte - 1); i > 0; i--) ;
            if ((mano[i].getPunteggio() > 4 || briscola.stessoSeme(mano[i])))
                i = 0;
            return i;

        }
        public UInt16 gioca(UInt16 x, Carta[] mano, UInt16 numeroCarte, Carta c)
        {
            UInt16 i = (UInt16)elaboratoreCarteBriscola.r.Next(0, UInt16.MaxValue);
            if (!briscola.stessoSeme(c))
            {
                if ((i = getSoprataglio(mano, c, true)) < numeroCarte)
                    return i;
                if (c.getPunteggio() > 0 && (i = getBriscola(mano)) < numeroCarte)
                {
                    if (c.getPunteggio() > 4)
                        return i;
                    if (mano[i].getPunteggio() > 0)
                        if (elaboratoreCarteBriscola.r.Next() % 10 < 5)
                            return i;
                }
            }
            else
            {
                if (elaboratoreCarteBriscola.r.Next() % 10 < 5 && (i = getSoprataglio(mano, c, false)) < numeroCarte)
                    return i;
            }
            i = 0;
            return i;
        }
        public void aggiornaPunteggio(ref UInt16 punteggioAttuale, Carta c, Carta c1) { punteggioAttuale = (UInt16)(punteggioAttuale + c.getPunteggio() + c1.getPunteggio()); }
    }
}
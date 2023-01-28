/*
  *  This code is distribuited under GPL 3.0 or, at your opinion, any later version
 *  CBriscola 2.0
 *
 *  Created by Giulio Sorrentino on 28/01/23.
 *  Copyright 2023 Some rights reserved.
 *
 */

using System;

namespace org.altervista.numerone.framework
{
	class giocatoreHelperUtente : giocatoreHelper
	{
		public giocatoreHelperUtente()
		{
			;
		}
		public UInt16 Gioca(UInt16 i, Carta[] v, UInt16 numeroCarte)
		{
			if (i < numeroCarte)
				return i;
			else
				throw new ArgumentException("");
		}
		public UInt16 Gioca(UInt16 i, Carta[] v, UInt16 numeroCarte, Carta c)
		{
			return Gioca(i, v, numeroCarte);
		}
		public void AggiornaPunteggio(ref UInt16 punteggioAttuale, Carta c, Carta c1)
		{
			punteggioAttuale = (UInt16)(punteggioAttuale + c.GetPunteggio() + c1.GetPunteggio());
		}

    };
}
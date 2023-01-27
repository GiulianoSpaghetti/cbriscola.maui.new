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
	class GiocatoreHelperUtente : GiocatoreHelper
	{
		public GiocatoreHelperUtente()
		{
			;
		}
		public UInt16 gioca(UInt16 i, Carta[] v, UInt16 numeroCarte)
		{
			if (i < numeroCarte)
				return i;
			else
				throw new ArgumentException("");
		}
		public UInt16 gioca(UInt16 i, Carta[] v, UInt16 numeroCarte, Carta c)
		{
			return gioca(i, v, numeroCarte);
		}
		public void aggiornaPunteggio(ref UInt16 punteggioAttuale, Carta c, Carta c1)
		{
			punteggioAttuale = (UInt16)(punteggioAttuale + c.getPunteggio() + c1.getPunteggio());
		}

    };
}
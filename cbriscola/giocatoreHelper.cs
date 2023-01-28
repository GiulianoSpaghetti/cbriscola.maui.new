/*
 *  This code is distribuited under GPL 3.0 or, at your opinion, any later version
 *  CBriscola 2.0
 *
 *  Created by Giulio Sorrentino on 28/01/23.
 *  Copyright 2023 Some rights reserved.
 *
 */

using System;

namespace CBriscola
{
	interface giocatoreHelper
	{
		UInt16 gioca(UInt16 i, Carta[] v, UInt16 numeroCarte);
		UInt16 gioca(UInt16 i, Carta[] v, UInt16 numeroCarte, Carta c);
		void aggiornaPunteggio(ref UInt16 punteggio, Carta c, Carta c1);

    };
}
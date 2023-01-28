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
	public class elaboratoreCarteBriscola : elaboratoreCarte
	{
		private const UInt16 numeroCarte = 40;
		private bool[] doppione;
		private static UInt16 CartaBriscola;
		private bool inizio,
				 briscolaDaPunti;
		public static Random r = new Random();
		public elaboratoreCarteBriscola(bool punti = true)
		{
			inizio = true;
			briscolaDaPunti = punti;
			doppione = new bool[40];
			for (int i = 0; i < 40; i++)
				doppione[i] = false;
		}
		public UInt16 getCarta()
		{
			UInt16 fine = (UInt16)(r.Next(0, 39) % numeroCarte),
			Carta = (UInt16)((fine + 1) % numeroCarte);
			while (doppione[Carta] && Carta != fine)
				Carta = (UInt16)((Carta + 1) % numeroCarte);
			if (doppione[Carta])
				throw new ArgumentException("Chiamato elaboratoreCarteItaliane::getCarta() quando non ci sono più carte da elaborare");
			else
			{
				if (inizio)
				{
					UInt16 valore = (UInt16)(Carta % 10);
					if (!briscolaDaPunti && (valore == 0 || valore == 2 || valore > 6))
					{
						Carta = (UInt16)(Carta - valore + 1);
					}
					if (!briscolaDaPunti)
						Carta = CartaHelperBriscola.getIstanza().getNumero(CartaHelperBriscola.getIstanza().getSeme(Carta), 1);
					CartaBriscola = Carta;
					inizio = false;
				}
				doppione[Carta] = true;
				return Carta;
			}
		}

		public static UInt16 getCartaBriscola() { return CartaBriscola; }
	}
}
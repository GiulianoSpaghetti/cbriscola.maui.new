/*
 *  This code is distribuited under GPL 3.0 or, at your opinion, any later version
 *  CBriscola 2.0
 *
 *  Created by Giulio Sorrentino on 28/01/23.
 *  Copyright 2023 Some rights reserved.
 *
 */

using System;

namespace CBriscola {
	public class CartaHelperBriscola : CartaHelper {
		private UInt16 CartaBriscola;
		public CartaHelperBriscola(UInt16 briscola) { CartaBriscola = briscola; }
		private static CartaHelperBriscola istanza;
		public static CartaHelperBriscola getIstanza() {
			if (istanza == null) {
				istanza = new CartaHelperBriscola(elaboratoreCarteBriscola.getCartaBriscola());
			}
			return istanza;
		}
		public UInt16 getSeme(UInt16 Carta) {
			return (UInt16)(Carta / 10);
		}
		public UInt16 getValore(UInt16 Carta) {
			return (UInt16)(Carta % 10);
		}
		public UInt16 getPunteggio(UInt16 Carta) {
			UInt16 valore = 0;
			switch (Carta % 10) {
				case 0: valore = 11; break;
				case 2: valore = 10; break;
				case 9: valore = 4; break;
				case 8: valore = 3; break;
				case 7: valore = 2; break;
			}
			return valore;
		}
		public string getSemeStr(UInt16 Carta) {
			string s = "a";
			switch (Carta / 10) {
				case 0: s = "bastoni"; break;
                case 1: s = "coppe"; break;
                case 2: s = "denari"; break;
                case 3: s = "spade"; break;
            }
            return s;
		}

		public UInt16 getNumero(UInt16 seme, UInt16 valore) {
			if (seme > 4 || valore > 9)
				throw new ArgumentException($"Chiamato CartaHelperBriscola::getNumero con seme={seme} e valore={valore}");
			return (UInt16)(seme * 10 + valore);
		}

		public Carta getCartaBriscola() { return Carta.getCarta(CartaBriscola); }

		public int CompareTo(UInt16 Carta, UInt16 Carta1) {
			UInt16 punteggio = getPunteggio(Carta),
				   punteggio1 = getPunteggio(Carta1),
				   valore = getValore(Carta),
				   valore1 = getValore(Carta1),
				   semeBriscola = getSeme(CartaBriscola),
				   semeCarta = getSeme(Carta),
					  semeCarta1 = getSeme(Carta1);
			if (punteggio < punteggio1)
				return 1;
			else if (punteggio > punteggio1)
				return -1;
			else {
				if (valore < valore1 || (semeCarta1 == semeBriscola && semeCarta != semeBriscola))
					return 1;
				else if (valore > valore1 || (semeCarta == semeBriscola && semeCarta1 != semeBriscola))
					return -1;
				else
					return 0;
			}
		}
	}
}
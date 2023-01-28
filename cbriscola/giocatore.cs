/*
 *  This code is distribuited under GPL 3.0 or, at your opinion, any later version
 *  CBriscola 0.1
 *
 *  Created by numerunix on 22/05/22.
 *  Copyright 2022 Some rights reserved.
 *
 */

using System;

namespace CBriscola {
	public class Giocatore
	{
		private string nome;
		private Carta[] mano;
		private bool ordinaMano;
		private UInt16 numeroCarte;
		private UInt16 iCarta;
		private UInt16 iCartaGiocata;
		private UInt16 punteggio;
		private GiocatoreHelper helper;
		public enum Carta_GIOCATA { NESSUNA_Carta_GIOCATA = UInt16.MaxValue };
		public Giocatore(GiocatoreHelper h, string n, UInt16 carte, bool ordina = true)
		{
			ordinaMano = ordina;
			numeroCarte = carte;
			iCartaGiocata = (UInt16)(Carta_GIOCATA.NESSUNA_Carta_GIOCATA);
			punteggio = 0;
			helper = h;
			nome = n;
			mano = new Carta[3];
			iCarta = 0;
		}
		public string getNome() { return nome; }
		public void setNome(string n) { nome = n; }
		public bool getFlagOrdina() { return ordinaMano; }
		public void setFlagOrdina(bool ordina) { ordinaMano = ordina; }
		public void addCarta(Mazzo m)
		{
			UInt16 i = 0;
			Carta temp;
			if (iCarta == numeroCarte && iCartaGiocata == (UInt16)Carta_GIOCATA.NESSUNA_Carta_GIOCATA)
				throw new ArgumentException($"Chiamato Giocatore::setCarta con mano.size()==numeroCarte=={numeroCarte}");
			if (iCartaGiocata != (UInt16)Carta_GIOCATA.NESSUNA_Carta_GIOCATA)
			{
				for (i = iCartaGiocata; i < numeroCarte - 1; i++)
					mano[i] = mano[i + 1];
				mano[i] = null;
				iCartaGiocata = (UInt16)Carta_GIOCATA.NESSUNA_Carta_GIOCATA;
				mano[iCarta - 1] = sostituisciCartaGiocata(m);
				for (i = (UInt16)(iCarta - 2); i < UInt16.MaxValue && iCarta > 1 && mano[i].CompareTo(mano[i + 1]) < 0; i--)
				{
					temp = mano[i];
					mano[i] = mano[i+1];
					mano[i+1] = temp;
				}
				return;
			}
			ordina(m);


		}

		private void ordina(Mazzo m)
		{
			UInt16 i = 0;
			UInt16 j = 0;
			Carta c = sostituisciCartaGiocata(m);
			for (i = 0; i < iCarta && mano[i] != null && c.CompareTo(mano[i]) < 0; i++) ;
			for (j = (UInt16)(numeroCarte - 1); j > i; j--)
				mano[j] = mano[j - 1];
			mano[i] = c;
			iCarta++;
		}
		private Carta sostituisciCartaGiocata(Mazzo m)
		{
			Carta c;
			try
			{
				c = Carta.getCarta(m.getCarta());
			}
			catch (IndexOutOfRangeException e)
			{
				numeroCarte--;
				iCarta--;
				if (numeroCarte == 0)
					throw e;
				return mano[numeroCarte];
			}
			return c;
		}
		public Carta getCartaGiocata()
		{
			return mano[iCartaGiocata];
		}
		public UInt16 getPunteggio() { return punteggio; }
		public void gioca(UInt16 i)
		{
			iCartaGiocata = helper.gioca(i, mano, numeroCarte);
		}
		public void gioca(UInt16 i, Giocatore g1)
		{
			iCartaGiocata = helper.gioca(i, mano, numeroCarte, g1.getCartaGiocata());
		}
		public void aggiornaPunteggio(Giocatore g)
		{
			helper.aggiornaPunteggio(ref punteggio, getCartaGiocata(), g.getCartaGiocata());
		}

		public String getID(UInt16 quale)
		{
			String s = mano[quale].getID();
			return s;
		}

		public UInt16 getICartaGiocata()
		{
			return iCartaGiocata;
		}

		public UInt16 getNumeroCarte()
		{
			return numeroCarte;
		}
	}

}
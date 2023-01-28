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
	public interface CartaHelper
	{
		UInt16 getSeme(UInt16 Carta);
		UInt16 getValore(UInt16 Carta);
		UInt16 getPunteggio(UInt16 Carta);
		string getSemeStr(UInt16 Carta);
		UInt16 getNumero(UInt16 seme, UInt16 valore);
	};
}
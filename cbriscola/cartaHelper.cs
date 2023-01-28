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
	interface cartaHelper
	{
		UInt16 getSeme(UInt16 carta);
		UInt16 getValore(UInt16 carta);
		UInt16 getPunteggio(UInt16 carta);
		string getSemeStr(UInt16 carta);
		UInt16 getNumero(UInt16 seme, UInt16 valore);
	};
}
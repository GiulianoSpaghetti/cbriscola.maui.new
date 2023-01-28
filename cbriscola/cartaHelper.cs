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
	public interface CartaHelper
	{
		UInt16 GetSeme(UInt16 carta);
		UInt16 GetValore(UInt16 carta);
		UInt16 GetPunteggio(UInt16 carta);
		string GetSemeStr(UInt16 carta);
		UInt16 GetNumero(UInt16 seme, UInt16 valore);
	};
}
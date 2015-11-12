/**
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015 Dreambound Studios AB
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace Dreambound.Telnet
{
	public enum RFC854 : int
	{
		SE       = 240, // End of subnegotiation parameters.
		NOP      = 241, // No operation.
		DATAMARK = 242, // The data stream portion of a Synch. This should always be accompanied by a TCP Urgent notification.
		BREAK    = 243, // NVT character BRK.
		IP       = 244, // The function IP.
		AO       = 245, // The function AO.
		AYT      = 246, // The function AYT. ("Are you There"). Used to check for keepalive timings.
		EC       = 247, // The function EC.
		EL       = 248, // The function EL.
		SB       = 250, // Indicates that what follows is subnegotiation of the indicated option.
		WILL     = 251, // Indicates the desire to begin performing, or confirmation that you are now performing, the indicated option.
		WONT     = 252, // Indicates the refusal to perform, or continue performing, the indicated option.
		DO       = 253, // Indicates the request that the other party perform, or confirmation that you are expecting the other party to perform, the indicated option.
		DONT     = 254, // Indicates the demand that the other party stop performing, or confirmation that you are no longer expecting the other party to perform, the indicated option.
		IAC      = 255, // Interpret as Command - the escape char to start all the above things.
	}

	public enum RFC864 : int
	{
		GA = 249, //  The GA signal.
	}

	public enum RFC857 : int
	{
		ECHO = 1,
	}

	public enum RFC858 : int
	{
		SGA = 3,	// Suppress goahead
	}

	public enum RFC1184 : int
	{
		LINEMODE = 34,
		MODE     = 1,
		EDIT     = 1,
		TRAPSIG  = 2,
		MODE_ACK = 4,
		SOFT_TAB = 8,
		LIT_ECHO = 16,    	
	}

	public enum RFC1091 : int
	{
		TERMTYPE = 24,
		IS       = 0,
		SEND     = 1,
	}

	public enum RFC1073 : int
	{
		NAWS = 31,
	}	
}

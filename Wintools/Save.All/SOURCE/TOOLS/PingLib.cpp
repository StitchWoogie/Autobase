// Ping.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "afxsock.h"

typedef struct icmphdr {
	BYTE	i_type;						// ICMP packet type
	BYTE	i_code;						// type subcode
	USHORT	i_chksum;					// packet checksum
	USHORT	i_id;						// unique packet ID
	USHORT	i_seq;						// packet sequence number
	ULONG	timestamp;					// timestamp
} IcmpHeader;

#define DEFAULT_PACKET_SIZE	32
#define MAX_PACKET_SIZE		1024

#define ICMP_ECHO		8

void FillIcmpData(char *icmp_data, int datasize)
{
	IcmpHeader	*icmphdr;
	char		*datapart;

	icmphdr = (IcmpHeader *)icmp_data;

	icmphdr->i_type  = ICMP_ECHO;
	icmphdr->i_code  = 0;
	icmphdr->i_id    = (USHORT)GetCurrentProcessId();
	icmphdr->i_chksum= 0;
	icmphdr->i_seq   = 0;

	datapart = icmp_data + sizeof(IcmpHeader);
	//
	// Place some junk data into the buffer
	//
	memset(datapart, 'E', datasize - sizeof(IcmpHeader));

	return;
}

USHORT Checksum(USHORT *buffer, int size)
{
	unsigned long	cksum=0;

	while (size > 1)
	{
		cksum += *buffer++;
		size  -= sizeof(USHORT);
	}
	if (size)
		cksum += *(UCHAR *)buffer;
	cksum =  (cksum >> 16) + (cksum & 0xffff);
	cksum += (cksum >> 16);

	return (USHORT)(~cksum);
}

typedef struct iphdr {
	unsigned int	h_len:4;			// length of the header
	unsigned int	version:4;			// Version of IP
	unsigned char	tos;				// Type of service
	unsigned short	total_len;			// total length of packet
	unsigned short	ident;				// unique identifier
	unsigned short	frag_and_flags;		// flags
	unsigned char	ttl;				// time to live value
	unsigned char	proto;				// protocol (TCP, UDP, etc.)
	unsigned short	checksum;			// IP checksum
	unsigned int	sourceIP;			// source IP address
	unsigned int	destIP;				// destination IP address
} IpHeader;

#define ICMP_MIN		8
#define ICMP_ECHOREPLY	0

#define PNGOKAY				0
#define PNGEBADRESPONSE		1
#define PNGEINVALIDADDR		2
#define PNGEINIT			3

bool DecodeResponse(char *data, int nbytes, struct sockaddr_in *addr)
{
	IpHeader	*iphdr;
	IcmpHeader	*icmphdr;
	DWORD		 ptime=0;
	int			m_iState;				// current state of Ping subsystem

	unsigned short	iphdrlen;

	iphdr = (IpHeader *)data;
	iphdrlen = iphdr->h_len * 4;

	if (nbytes < (iphdrlen + ICMP_MIN)) {
		m_iState = PNGEBADRESPONSE;
		return false;
	}
    //
	// Find the start of the ICMP header in the reply packet
	//
	icmphdr = (IcmpHeader *)(data + iphdrlen);
    //
	// Check the packet type to make sure its a ICMP reply
	//
	if (icmphdr->i_type != ICMP_ECHOREPLY)
	{
		m_iState = PNGEBADRESPONSE;
		return false;
	}
	// Check to see if this is our packet
	//
	if (icmphdr->i_id != (USHORT)GetCurrentProcessId())
	{
		m_iState = PNGEBADRESPONSE;
		return false;
	}
	//
	// calculate avg, min, max and update the properties
	//
	/*
	m_dwTotalTime += (ptime = GetTickCount() - icmphdr->timestamp);
	m_dwPacketCount++;

	m_pingctrl->SetAverageTime(m_dwTotalTime/m_dwPacketCount);
	
	m_pingctrl->SetMinTime(MIN(m_pingctrl->GetMinTime(), ptime));
	m_pingctrl->SetMaxTime(MAX(m_pingctrl->GetMaxTime(), ptime));

	m_pingctrl->FireOnEchoReplyEvent(ptime);	
	*/

	return true;
}

bool Ping(const char *ip)
{
	int		timeout,
			bread,
			datasize,
			fromlen;
	DWORD	dwTimeout;
	char   *icmp_data,
		   *recvbuf;
	USHORT	seq_no = 0;
	CString	destaddr;

	struct sockaddr_in	dest,
						from;
	unsigned int		addr;				// destination's addr
	struct hostent	   *hp = NULL;
	
	int			m_iState;				// current state of Ping subsystem

	destaddr  = ip;
	dwTimeout = 2;

	SOCKET m_sICMP = INVALID_SOCKET;

	if ((m_sICMP = socket(AF_INET, SOCK_RAW, IPPROTO_ICMP)) == INVALID_SOCKET)
	{
		return false;
	}
	// Set the receive and send timeout values
	//
	timeout = dwTimeout;
	if ((bread = setsockopt(m_sICMP, SOL_SOCKET, SO_RCVTIMEO, (char *)&timeout,
						sizeof(timeout))) == SOCKET_ERROR)
	{
		closesocket(m_sICMP);
		return false;
	}

	timeout = dwTimeout;
	if ((bread = setsockopt(m_sICMP, SOL_SOCKET, SO_SNDTIMEO, (char *)&timeout,
						sizeof(timeout))) == SOCKET_ERROR)
	{
		closesocket(m_sICMP);
		return false;
	}
	timeout = dwTimeout;
	//
	// Try to resolve the given address
	//
	if ((addr = inet_addr(destaddr)) == INADDR_NONE)
	{
		hp = gethostbyname(destaddr);
		if (hp)
			memcpy(&(dest.sin_addr), hp->h_addr, hp->h_length);
		else
		{
			closesocket(m_sICMP);
			return false;
		}
	}
	else
		dest.sin_addr.s_addr = addr;

	if (hp)
		dest.sin_family = hp->h_addrtype;
	else
		dest.sin_family = AF_INET;

	datasize = DEFAULT_PACKET_SIZE;

	datasize += sizeof(IcmpHeader);
	//
	// Allocate the ICMP packet along with a receive buffer.
	//
	icmp_data = (char *)HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY, MAX_PACKET_SIZE);
	recvbuf = (char *)HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY, MAX_PACKET_SIZE);

	if ((!icmp_data) || (!recvbuf))		// out of memory
		ExitProcess(1);
	//
	// Initialize the ICMP header
	//
	FillIcmpData(icmp_data, datasize);
	//
	// Loop for the requested number of packets
	//
	int	bwrote;

	// Fill in some more data in the ICMP header
	//
	((IcmpHeader *)icmp_data)->i_chksum = 0;
	((IcmpHeader *)icmp_data)->timestamp = GetTickCount();
	((IcmpHeader *)icmp_data)->i_seq = seq_no++;
	((IcmpHeader *)icmp_data)->i_chksum = Checksum((USHORT *)icmp_data, datasize);
	//
	// Send the ICMP packet to the destination
	//
	if ((bwrote = sendto(m_sICMP, icmp_data, datasize, 0, (struct sockaddr *)&dest,
					sizeof(dest))) == SOCKET_ERROR)
	{
		if ((m_iState = WSAGetLastError()) == WSAETIMEDOUT)
		{
			return false;
		}
		HeapFree (GetProcessHeap(), 0, icmp_data);
		HeapFree (GetProcessHeap(), 0, recvbuf);

		return false;
	}
	
	//
	// There is data pending. Read the packet.
	//
	fromlen = sizeof(from);
	if ((bread = recvfrom(m_sICMP, recvbuf, MAX_PACKET_SIZE, 0, (struct sockaddr *)&from,
						&fromlen)) == SOCKET_ERROR)
	{
		if ((m_iState = WSAGetLastError()) == WSAETIMEDOUT)
		{		
			return false;
		}
		HeapFree (GetProcessHeap(), 0, icmp_data);
		HeapFree (GetProcessHeap(), 0, recvbuf);

		return false;
	}

	closesocket(m_sICMP);
	
	bool retn = DecodeResponse(recvbuf, bread, &from);

	HeapFree (GetProcessHeap(), 0, icmp_data);
    HeapFree (GetProcessHeap(), 0, recvbuf);

	return retn;
}


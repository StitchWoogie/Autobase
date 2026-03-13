#if !defined(__TOTALDEF_H)
#define __TOTALDEF_H

#pragma pack(push, 1)

// Communication define
#define SOH 0x01
#define	STX	0x02		// start of text
#define	ETX	0x03		// end of text
#define	EOT	0x04   	// end of transmission
#define	ENQ	0x05   	// enquiry
#define  ACK	0x06   	// acknowledge
#define	LF		0x0A   	// line feed
#define	CR		0x0D		// carriage return
#define	DLE	0x10		
#define	NAK	0x15		// negative acknowledge

#pragma pack(pop)

#endif


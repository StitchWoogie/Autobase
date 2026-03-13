#if	!defined (__LZWTOOL_H)
#define __LZWTOOL_H

#if	!defined (__AFX_H)
#include <afx.h>
#endif

#if	!defined (__STDIO_H)
#include <stdio.h>
#endif

#if	!defined (__COMPILER_HPP)
#include <compiler.hpp>
#endif

class lzwReadClass {
		// State for GetCode and LZWReadByte 
		char code_buf[256+4];	// current input data block */
		int last_byte;				// # of bytes in code_buf */
		int last_bit;				// # of bits in code_buf */
		int cur_bit;				// next bit index to read */
		BOOL out_of_blocks;	// TRUE if hit terminator data block */

		int input_code_size;		// codesize given in GIF file */
		int clear_code,end_code;// values for Clear and End codes */

		int code_size;				// current actual code size */
		int limit_code;			// 2^code_size */
		int max_code;				// first unused code value */
		BOOL first_time;		// flags first call to LZWReadByte */

		// Private state for LZWReadByte 
		int oldcode;				// previous LZW symbol */
		int firstcode;				// first byte of oldcode's expansion */

		/* LZW symbol table and expansion stack */
		SHORT FAR *symbol_head;	/* => table of prefix symbols */
		BYTE  FAR *symbol_tail;	/* => table of suffix bytes */
		BYTE  FAR *symbol_stack;/* => stack for symbol expansions */
		BYTE  FAR *sp;				/* stack pointer */

		FILE  *inFile;

		int GetDataBlock (char *buf);

	public:
		void  SetFile(FILE *in);
		void  SetInputCodeSize(int size) { input_code_size = size; }
		lzwReadClass();
		~lzwReadClass();
		void  Init();
		void  ReInit();
		int   ReadByte();
		int   GetCode();
};

class lzwWriteClass {
		typedef struct {
			/* State for packing variable-width codes into a bitstream */
			int n_bits;			/* current number of bits/code */
			short maxcode;		/* maximum code, given n_bits */
			int init_bits;		/* initial n_bits ... restored after clear */
			long cur_accum;		/* holds bits not yet output */
			int cur_bits;			/* # of bits in cur_accum */

			// LZW string construction */
			short waiting_code;	/* symbol not yet output; may be extendable */
			BOOL  first_byte;		/* if TRUE, waiting_code is not valid */

			/* State for LZW code assignment */
			short ClearCode;		/* clear code (doesn't change) */
			short EOFCode;		/* EOF code (ditto) */
			short free_code;		/* first not-yet-used symbol code */

			/* LZW hash table */
			__int16 *hash_code;		/* => hash table of symbol codes */
			__int32 FAR *hash_value;	/* => hash table of symbol values */

			/* GIF data packet construction buffer */
			int bytesinpkt;		/* # of bytes in current packet */
			char packetbuf[256];		/* workspace for accumulating packet */
		} DEST_INFO;

		DEST_INFO dinfo;

		void clear_hash();
		void output(short code);
		void flush_packet ();
		void clear_block();
		FILE *outFile;
	public:
		lzwWriteClass();
		~lzwWriteClass();
		void Init(int i_bits);
		void compress_byte(int c);
		void compress_term ();
		void SetFile(FILE *out) { outFile = out; }
};

#define	MAX_LZW_BITS	12						// maximum LZW code size 
#define	LZW_TABLE_SIZE	(1<<MAX_LZW_BITS) // # of possible LZW symbols 


#endif


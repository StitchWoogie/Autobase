#include "stdafx.h"
#if	defined (__BORLANDC__)
#include <mem.h>
#endif
#include <lzwtool.h>

#define HSIZE		5003	// hash table size for 80% occupancy 
#define MAXCODE(n_bits)	(((short) 1 << (n_bits)) - 1)

lzwWriteClass :: lzwWriteClass()
{
	// Allocate space for hash table 
	dinfo.hash_code  = new __int16[HSIZE];
	dinfo.hash_value = new __int32[HSIZE];
}

lzwWriteClass :: ~lzwWriteClass()
{
	delete dinfo.hash_code;
	delete dinfo.hash_value;
}

//---------------------------------------
// Fill the hash table with empty entries 
//---------------------------------------
void lzwWriteClass :: clear_hash()
{
	// It's sufficient to zero hash_code[] 
	memset(dinfo.hash_code, 0, HSIZE * sizeof(short));
}

// flush any accumulated data 
void lzwWriteClass :: flush_packet ()
{
	if (dinfo.bytesinpkt > 0) {	/* never write zero-length packet */
		dinfo.packetbuf[0] = (char) dinfo.bytesinpkt++;
		if(fwrite(dinfo.packetbuf, 1, dinfo.bytesinpkt, outFile) != (unsigned)dinfo.bytesinpkt) {
			//ERREXIT(dinfo.cinfo, JERR_FILE_WRITE);
		}
		
		dinfo.bytesinpkt = 0;
  }
}


// Add a character to current packet; flush to disk if necessary 
#define CHAR_OUT(dinfo,c)  \
	{ (dinfo).packetbuf[++(dinfo).bytesinpkt] = (char) (c);  \
	    if ((dinfo).bytesinpkt >= 255)  \
	      flush_packet();  \
	}

/* Routine to convert variable-width codes into a byte stream */

void lzwWriteClass :: output (short code)
// Emit a code of n_bits bits 
// Uses cur_accum and cur_bits to reblock into 8-bit bytes 
{
	dinfo.cur_accum |= ((long) code) << dinfo.cur_bits;
	dinfo.cur_bits += dinfo.n_bits;

	while (dinfo.cur_bits >= 8) {
		CHAR_OUT(dinfo, dinfo.cur_accum & 0xFF);
		dinfo.cur_accum >>= 8;
		dinfo.cur_bits -= 8;
	}

	/*
   * If the next entry is going to be too big for the code size,
   * then increase it, if possible.  We do this here to ensure
   * that it's done in sync with the decoder's codesize increases.
   */
	if (dinfo.free_code > dinfo.maxcode) {
		dinfo.n_bits++;
		if (dinfo.n_bits == MAX_LZW_BITS)
			dinfo.maxcode = LZW_TABLE_SIZE; /* free_code will never exceed this */
		else
			dinfo.maxcode = MAXCODE(dinfo.n_bits);
	}
}

// Initialize LZW compressor 
void lzwWriteClass :: Init(int i_bits)
{
	/* init all the state variables */
	dinfo.n_bits = dinfo.init_bits = i_bits;
	dinfo.maxcode = MAXCODE(dinfo.n_bits);
	dinfo.ClearCode = ((short) 1 << (i_bits - 1));
	dinfo.EOFCode = dinfo.ClearCode + 1;
	dinfo.free_code = dinfo.ClearCode + 2;
	dinfo.first_byte = TRUE;	/* no waiting symbol yet */
	/* init output buffering vars */
	dinfo.bytesinpkt = 0;
	dinfo.cur_accum = 0;
	dinfo.cur_bits = 0;
	/* clear hash table */
	clear_hash();
	/* GIF specifies an initial Clear code */
	output(dinfo.ClearCode);
}


#define HASH_ENTRY(prefix,suffix)  ((((__int32) (prefix)) << 8) | (suffix))

// Accept and compress one 8-bit byte 

void lzwWriteClass :: compress_byte(int c)
{
	register short i;
	register short disp;
	register long probe_value;

	if (dinfo.first_byte) {	// need to initialize waiting_code 
		dinfo.waiting_code = c;
		dinfo.first_byte = FALSE;
		return;
	}

	// Probe hash table to see if a symbol exists for
   // waiting_code followed by c.
   // If so, replace waiting_code by that symbol and return.
   
	i = ((short) c << (MAX_LZW_BITS-8)) + dinfo.waiting_code;
	// i is less than twice 2**MAX_LZW_BITS, therefore less than twice HSIZE 
	if (i >= HSIZE)
		i -= HSIZE;

	probe_value = HASH_ENTRY(dinfo.waiting_code, c);
  
	if (dinfo.hash_code[i] != 0) { // is first probed slot empty? 
		if (dinfo.hash_value[i] == probe_value) {
			dinfo.waiting_code = dinfo.hash_code[i];
			return;
		}
		if (i == 0)			// secondary hash (after G. Knott) 
			disp = 1;
		else
			disp = HSIZE - i;
		for (;;) {
			i -= disp;
			if (i < 0)
				i += HSIZE;
			if (dinfo.hash_code[i] == 0)
				break;			/* hit empty slot */
			if (dinfo.hash_value[i] == probe_value) {
				dinfo.waiting_code = dinfo.hash_code[i];
				return;
			}
		}
  }

  // here when hashtable[i] is an empty slot; desired symbol not in table 
  output(dinfo.waiting_code);
  if (dinfo.free_code < LZW_TABLE_SIZE) {
		dinfo.hash_code[i] = dinfo.free_code++; // add symbol to hashtable 
		dinfo.hash_value[i] = probe_value;
  } 
  else
		clear_block();
  dinfo.waiting_code = c;
}

// Reset compressor and issue a Clear code 
void lzwWriteClass :: clear_block()
{
  clear_hash();				// delete all the symbols 
  dinfo.free_code = dinfo.ClearCode + 2;
  output(dinfo.ClearCode);	// inform decoder 
  dinfo.n_bits = dinfo.init_bits;	// reset code size 
  dinfo.maxcode = MAXCODE(dinfo.n_bits);
}

// Clean up at end 
void lzwWriteClass :: compress_term ()
{
  // Flush out the buffered code 
  if (!dinfo.first_byte)
    output(dinfo.waiting_code);
  // Send an EOF code 
  output(dinfo.EOFCode);
  // Flush the bit-packing buffer 
  if (dinfo.cur_bits > 0) {
    CHAR_OUT(dinfo, dinfo.cur_accum & 0xFF);
  }
  // Flush the packet buffer 
  flush_packet();
}





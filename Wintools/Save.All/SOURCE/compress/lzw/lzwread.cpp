#include "stdafx.h"
#include <lzwtool.h>

lzwReadClass :: lzwReadClass()
{
	symbol_head = new SHORT[LZW_TABLE_SIZE];
	symbol_tail = new BYTE [LZW_TABLE_SIZE];
	symbol_stack= new BYTE [LZW_TABLE_SIZE];
}

lzwReadClass :: ~lzwReadClass()
{
	delete symbol_head;
	delete symbol_tail;
	delete symbol_stack;
}

int lzwReadClass :: GetDataBlock (char *buf)
// Read a GIF data block, which has a leading count byte 
// A zero-length block marks the end of a data block sequence 
{
  int count;

  count = fgetc(inFile);
  if (count > 0) {
     if(fread(buf, 1, count, inFile) != (unsigned)count)	return 0;
  }
  return count;
}

void lzwReadClass :: SetFile(FILE *in)
{
	inFile = in;
}

void lzwReadClass :: ReInit()
// (Re)initialize LZW state; shared code for startup and Clear processing 
{
  code_size  = input_code_size + 1;
  limit_code = clear_code << 1;	// 2^code_size 
  max_code   = clear_code + 2;	// first unused code value 
  sp         = symbol_stack;		// init stack to empty 
}

void lzwReadClass :: Init()
// Initialize for a series of LZWReadByte (and hence GetCode) calls 
{
	 // GetCode initialization 
	last_byte = 2;		// make safe to "recopy last two bytes" 
	last_bit = 0;		// nothing in the buffer 
	cur_bit = 0;		// force buffer load on first call 
	out_of_blocks = FALSE;

	// LZWReadByte initialization: 
	// compute special code values (note that these do not change later) 
	clear_code = 1 << input_code_size;
	end_code   = clear_code + 1;
	first_time = TRUE;
	ReInit();
}

int lzwReadClass :: GetCode()
// Fetch the next code_size bits from the GIF data 
// We assume code_size is less than 16 
{
	register long accum;
	int offs, ret, count;

	while ( (cur_bit + code_size) > last_bit) {
		// Time to reload the buffer 
		if (out_of_blocks) {
			//WARNMS(cinfo, JWRN_GIF_NOMOREDATA);
			return end_code;	// fake something useful 
		}

		// preserve last two bytes of what we have -- assume code_size <= 16 
		code_buf[0] = code_buf[last_byte-2];
		code_buf[1] = code_buf[last_byte-1];

		// Load more bytes; set flag if we reach the terminator block 
		if ((count = GetDataBlock(&code_buf[2])) == 0) {
			out_of_blocks = TRUE;
//			WARNMS(cinfo, JWRN_GIF_NOMOREDATA);
			return end_code;	// fake something useful 
		}

		// Reset counters 
		cur_bit = (cur_bit - last_bit) + 16;
		last_byte = 2 + count;
		last_bit =  last_byte * 8;
	}

	// Form up next 24 bits in accum 
	offs = cur_bit >> 3;	// byte containing cur_bit 
#ifdef CHAR_IS_UNSIGNED
	accum = code_buf[offs+2];
	accum <<= 8;
	accum |= code_buf[offs+1];
	accum <<= 8;
	accum |= code_buf[offs];
#else
	accum = code_buf[offs+2] & 0xFF;
	accum <<= 8;
	accum |= code_buf[offs+1] & 0xFF;
	accum <<= 8;
	accum |= code_buf[offs] & 0xFF;
#endif

	// Right-align cur_bit in accum, then mask off desired number of bits 
	accum >>= (cur_bit & 7);
	ret = ((int) accum) & ((1 << code_size) - 1);
  
	cur_bit += code_size;
	return ret;
}

int lzwReadClass :: ReadByte()
{
	register int code;		// current working code 
	int incode;			// saves actual input code 

	// First time, just eat the expected Clear code(s) and return next code, 
	// which is expected to be a raw byte. 
	if (first_time) {
		first_time = FALSE;
		code = clear_code;	// enables sharing code with Clear case 
	} 
	else {
		// If any codes are stacked from a previously read symbol, return them 
		if (sp > symbol_stack)
			return (int) *(-- sp);
		// Time to read a new symbol 
		code = GetCode();
	}

	if (code == clear_code) {
		// Reinit state, swallow any extra Clear codes, and 
		// return next code, which is expected to be a raw byte. 
		ReInit();
		do {
			code = GetCode();
		} while (code == clear_code);
		if (code > clear_code) { // make sure it is a raw byte 
			//WARNMS(cinfo, JWRN_GIF_BADDATA);
			code = 0;			// use something valid 
		}
		// make firstcode, oldcode valid! 
		firstcode = oldcode = code;
		return code;
	}

	if (code == end_code) {
		// Skip the rest of the image, unless GetCode already read terminator 
		if (!out_of_blocks) {
			//SkipDataBlocks();
			out_of_blocks = TRUE;
		}
		// Complain that there's not enough data 
		// WARNMS(cinfo, JWRN_GIF_ENDCODE);
		// Pad data with 0's 
		return 0;			// fake something usable 
	}

	// Got normal raw byte or LZW symbol 
	incode = code;		// save for a moment 
  
	if (code >= max_code) { // special case for not-yet-defined symbol 
		// code == max_code is OK; anything bigger is bad data 
		if (code > max_code) {
			//WARNMS(sinfo->cinfo, JWRN_GIF_BADDATA);
			incode = 0;		// prevent creation of loops in symbol table 
		}
		// this symbol will be defined as oldcode/firstcode 
		*(sp++) = (BYTE) firstcode;
		code = oldcode;
	}

	// If it's a symbol, expand it into the stack 
	while (code >= clear_code) {
		*(sp++) = symbol_tail[code]; // tail is a byte value 
		code = symbol_head[code]; // head is another LZW symbol 
	}
	// At this point code just represents a raw byte 
	firstcode = code;	// save for possible future use 

	// If there's room in table, 
	if ((code = max_code) < LZW_TABLE_SIZE) {
		// Define a new symbol = prev sym + head of this sym's expansion 
		symbol_head[code] = oldcode;
		symbol_tail[code] = (BYTE) firstcode;
		max_code++;
		// Is it time to increase code_size? 
		if ((max_code >= limit_code) &&
			(code_size < MAX_LZW_BITS)) {
			code_size++;
			limit_code <<= 1;	// keep equal to 2^code_size 
		}
	}
  
	oldcode = incode;	// save last input symbol for future use 
	return firstcode;	// return first byte of symbol's expansion 
}


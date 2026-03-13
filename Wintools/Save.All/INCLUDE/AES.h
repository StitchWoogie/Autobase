#if !defined(__AES_H)
#define __AES_H

typedef unsigned char uint8_t;
#ifndef _ATMEL_CONTROLLER
typedef unsigned int uint32_t;
#endif
typedef uint8_t state_t[4][4];

typedef struct {
	unsigned int Nk;	//#define Nk 4		
	unsigned int Nr;
	state_t* state;
	uint8_t RoundKey[176];
	const uint8_t* Key;
} AES128_VARS;

void AES128_SetKey(AES128_VARS *vars, const BYTE *key, int key_size);
void AES128_encrypt(AES128_VARS *vars, BYTE* input, BYTE* output);
void AES128_decrypt(AES128_VARS *vars, BYTE* input, BYTE *output);

#endif

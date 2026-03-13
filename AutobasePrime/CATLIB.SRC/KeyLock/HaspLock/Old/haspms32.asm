;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  HASPMS32.ASM
;;
;;
;;  Description:
;;        This file links the application to the procedure that checks
;;        the HASP key. This file performs the following:
;;
;;          a. Gets the parameters from the application stack.
;;          b. Initialize the appropriate registers.
;;          c. Calls haspreg, procedure that checks the HASP key.
;;	    d. Receives the return values from haspreg and moves them to.
;;	       the stack.
;;
;;  Compilation instructions:
;;
;;        ml -Zm -c -Cx -coff haspms32.asm
;;    
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

.386P

_TEXT SEGMENT  BYTE PUBLIC USE32 'CODE'
	ASSUME   CS:_TEXT

	extrn haspreg : near
	public _hasp		

;
; Frame structure after pushing EBP.
;
RetCode4	equ	[EBP+40]    	
RetCode3	equ	[EBP+36]    	
RetCode2	equ	[EBP+32]    	
RetCode1	equ	[EBP+28]    	
PlugNameHi	equ	[EBP+24]    	
PlugNameLow	equ	[EBP+20]    	
Lptnum		equ	[EBP+16]    	
SeedCode	equ	[EBP+12]    	
Cmd		equ	[EBP+8 ]    	

_hasp proc near

	push	Ebp
	mov	Ebp, Esp
	
	push	Eax
	push	Ebx
	push	Ecx
	push	Edx
	push	Edi  
	push	Esi

	mov	Esi, RetCode1
	mov	Edi, [Esi]

	mov	Ebx, 0
	mov	Ebx, Cmd
	mov	bh, bl
	mov	bl, 0
	add	Ebx, LptNum

	mov	Eax, SeedCode
	mov	Ecx, PlugNameLow
	mov	Edx, PlugNameHi

	cmp	bh,50	       
	jb	NotBlockOperation  
 	mov	Esi, RetCode4
	mov	Eax, [Esi]

NotBlockOperation:

	mov	Esi, RetCode2
	mov	Esi, [Esi]

	push	Ebp
	call	haspreg
	pop	Ebp

	mov	Edi, RetCode1
	mov	[Edi], Eax
	mov	Edi, RetCode2
	mov	[Edi], Ebx
	mov	Edi, RetCode3
	mov	[Edi], Ecx
	mov	Edi, RetCode4
	mov	[Edi], Edx

	pop	Esi
	pop	Edi
	pop	Edx
	pop	Ecx
	pop	Ebx
	pop	Eax

	pop	Ebp

	ret

_hasp endp

_TEXT   ENDS

       END





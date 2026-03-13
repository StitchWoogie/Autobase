.MODEL large

ifdef ??version 	;for Turbo Assembler
	MASM51
	QUIRKS
endif
 
.286P

.code

public C b8

b8 PROC y, u:BYTE
		push bp
		mov bp, sp
		mov	al, u
		cbw					;//AX = scaled U (sign extended)
		mov	dx,456			;//(456/256 = 226/127)
		imul	dx
		mov	al,ah
		mov	ah,dl
		add ax, y
		jge  @F
		xor ax, ax
		pop bp
		ret

	@@:
		cmp ax, 255
		jg  @F
		pop bp
		ret
	@@:
		mov ax, 255
		pop bp
		ret
							;//AX = unscaled U
ENDP

public C r8
r8 PROC y, v:BYTE
	push bp
	mov bp, sp
		mov	al, v
		cbw					;//AX = scaled V (sign extended)
		mov	dx,361			;//(361/256 = 179/127)
		imul	dx
		mov	al, ah
		mov	ah, dl
		add ax, y
		jge  @F
		xor ax, ax
		pop bp
		ret

	@@:
		cmp ax, 255
		jg  @F
		pop bp
		ret

	@@:
		mov ax, 255
		pop bp
		ret						;//AX = unscaled V
ENDP
;g8( y, u, v ) = min(255,max(0,(436L*y-130L*r8(y,v)-50L*b8(y,u))>>8))
;green=1.706L*y-.509L*r8(y,v)-.194L*b8(y,u)

public C g8

g8 PROC y, u:BYTE, v:BYTE
	push bp
	mov bp, sp

		mov dx, y
		mov ax, 218
		mul dx
		shr ax, 7
		mov cx, ax

		push word ptr v
		push y
		call r8
		add sp, 4

		mov  dx, 65
		mul  dx
		shr ax, 7
		mov bx, ax

		push word ptr u
		push y
		call b8
		add sp, 4

		mov dx, 25
		mul dx
		shr ax, 7

		neg ax
		add ax, cx
		sub ax, bx

		cmp ax, 0
		jge @F
		xor ax, ax
		pop bp
		ret

	@@:
		cmp ax, 255
		jg  @F
		pop bp
		ret

	@@:
		mov ax, 255
		pop bp
		ret
endp

end
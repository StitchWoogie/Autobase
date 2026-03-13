#ifndef	__F_FORMAT_H
#define	__F_FORMAT_H

enum tagFILEFORMAT {
	NONE_FORMAT,		// unknowned format

	//******************************************
	//	그림 화일 (image)
	//******************************************
	PCX_FORMAT, 	// paint format
	GIF_FORMAT,     // compuServe에 의해 규약된 포맷
	TIF_FORMAT,     //
	PIC_FORMAT,	// dr.halo 에서 쓰는 포맷
			// 또는 PC 마우스로 유명한 MSC에서 규약한 화일
			// 포맷으로 PC-paint 와 Paul Mace Sofrware 사의
			// Grasp라는 프로그램에서 지원하는 페인팅 프로그램인
			// Pictor 등에서 지원한다.
	BMP_FORMAT,     // windows format
	CUT_FORMAT,	// 닥터할로의 부분이미지 저장
	SCX_FORMAT,	// 드로잉 프로그램인 ColorRIX와 EGA Paint 내에서
			// 지원하며, RIX의 데모로서 활용
	TGA_FORMAT,	// AT&T사의 Targa board 에서 주로 사용하는 화일 포맷
			// 압축과 비압축 두가지 제공
	MSP_FORMAT,	// Ms windows 내의 paint programm 에서 지원하던
			// 포맷으로 단색만을 지원한다.
	CLP_FORMAT,	// windows 환경하의 clipbloar내에서 image의 저장
			// 방법으로 사용 bmp와는 호환성이 없다.
	WPG_FORMAT,	// Word-Perfect 5.0 & 5.1 에서 사용
			// bitmap 방식 stroke 방식, vector graphic 방식의
			// format을 모두지원하며, 단색과 칼라를 지원
	LBM_FORMAT,	// 초기에 Commodere Amiga 기종에서 사용되던 image
			// file format으로 amiga 기종의 display mode에
			// 준하여 설계되었기 때문에 Amiga 기종의 고유기능을
			// 사용하는 내용이 몇가지 추가되어 있다.
			// PC 에서는 Deluxe Paint II Enhanced Version과
			// Digital Vision사의 Computer Eyes Video, scanner
			// board 와 같은 Package 에서지원 상당한 압축률
	MAC_FORMAT,	// MacPaint 라는 drawing programm에서 지원하며
			// 단색만을 지원
	IMG_FORMAT,
	GEM_FORMAT,	// DTP software인 Ventura Publisher 내에서 사용하는
			// graphic format이다. 일반적으로 GEM을 확장자로
			// 사용하나 IMG로 확장면이 변경되어 사용되기도
			// 하며, 단색과 color를 지원
	JPG_FORMAT,	// (Joint Phothgraphic Experts Group) 는 국제
			// 표준화 기구인 CCITT와 ISO에 의해 정의된 정지영상
			// 압축에 대한 국제표준이다.
	MMP_FORMAT,	//
	EPS_FORMAT,
	PUT_FORMAT,	//
	ART_FORMAT,	// 하늘
	HGE_FORMAT,	//

	SPT_FORMAT,	//

	WMF_FORMAT,	// windows meta file

	ICO_FORMAT,	// 윈도우의 아이콘화일

	FLI_FORMAT,	// Autodesk사의 FLI 형식(애니매이션)
	FLC_FORMAT,	// ''

	//********************************************
	//	음성 화일
	//********************************************
	VOC_FORMAT,	// voice format
	WAV_FORMAT,
	SND_FORMAT,	// raw wave - no header file
	NTI_FORMAT,	// amiga sample as used by tetra format
	_8SV_FORMAT,	// amiga IFF sound files

	VOC_NO_FORMAT,  // 확장자만 *.VOC 이고 진정한 VOC 화일이 아니다.
			// 얼토당토 않는 포맷지원자를 위해...

	//********************************************
	// 	음악화일
	//********************************************
	ROL_FORMAT,  	// adlib file
	BNK_FORMAT,
	MDI_FORMAT,	// midi format
	MID_FORMAT,
	IMS_FORMAT,	// ims format

	//*********************************************
	//	Word processer
	//*********************************************
	HWP_FORMAT,   	// word file
	HWP20_FORMAT,	// hwp 2.0 format
	HWP21_FORMAT,	// hwp 2.1 format
	GWP_FORMAT,	// 21th word
	HANA_FORMAT,	// hana word format

	//***********************************
	//	일반 text file
	//***********************************
	TXT_FORMAT,     // ascii file
	DOC_FORMAT,	//

	C_FORMAT,
	H_FORMAT,
	CPP_FORMAT,
	ASM_FORMAT,
	INC_FORMAT,
	BAS_FORMAT,
	COB_FORMAT,
	PAS_FORMAT,
	FOR_FORMAT,

	//************************************
	//	D_BASE
	//************************************
	DBF_FORMAT,

	//************************************
	//	압축된 화일들
	//************************************
	LZH_FORMAT,	//
	ZIP_FORMAT,
	ARJ_FORMAT,
	ARC_FORMAT,

	EXE_FORMAT,
	COM_FORMAT,
	BAT_FORMAT,
	OBJ_FORMAT,

	SEE_SLIDE_FORMAT,

	PNG_FORMAT,
};

int GetFileFormat(char *filename);
int TestExtension(char *filename, char *ext);

#endif
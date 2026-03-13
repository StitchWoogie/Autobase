#include "stdafx.h"

#include "plc_scan.h"

int CommStatusLocalOne(GLOBAL_PORT_STRUCT *pt);
void WatchDogPlcScanReset(int port);

void PlcProtocolUnInitOne(GLOBAL_PORT_STRUCT *port);

// 1개의 함수를 사용
static DWORD WINAPI PortThread_Common(LPVOID lpparam)
{
	GLOBAL_PORT_STRUCT *pt = (GLOBAL_PORT_STRUCT*)lpparam;

	if(CoInitialize(NULL) != S_OK) {
		bell();	
	}

	THREAD_PORT_STRUCT *thread = &pt->threadInfo;;
	int sleep_cycle = 1;

	sleep_cycle = pt->nThreadCycle;

	if(sleep_cycle < 1)	sleep_cycle = 1;

	thread->bDo  = ON;
	thread->bEnd = OFF;

	while(thread->bDo) {
		WatchDogPlcScanReset(pt->local.no);
		Sleep(sleep_cycle);
		CommStatusLocalOne(pt);
	}

	// thread->bEnd = ON;

	PlcProtocolUnInitOne(pt);				// 2011-11-18 추가
	PlcDeviceUnInit(&pt->local.device);		// 2011-11-18 추가

	CoUninitialize();

	thread->bEnd = ON;	// 2011-11-18 이 부분이 모든일을 끝내고 플래그를 살려주어야 한다. WaitForSingleObject 을 사용하면 bEnd는 필요없다.

	return 0;
}

void PortThreadInit(int port)
{
	GLOBAL_PORT_STRUCT *pt = &portBuf[port];

	if(pt->bActiveFlag == 0)		return;	// 2007.9.19 추가
	if(pt->bActiveThread == OFF)	return;

	THREAD_PORT_STRUCT *thread;

	thread = &pt->threadInfo;

	thread->handle = CreateThread(NULL, 0, PortThread_Common, pt, 0, &thread->id);	
}

void PortThreadUnInit(int port)
{
	THREAD_PORT_STRUCT *thread;

	thread = &portBuf[port].threadInfo;

	if(thread->handle == NULL)	return;

	thread->bDo = OFF;

	/* 2011- 11-18 이전
	TimeOutClass timeout;
	while(thread->bEnd == OFF) {
		Sleep(1);
		if(timeout.IsTimeOut(30))	break;
	}
	*/

	// Wait until all threads have terminated. 2011-11-18
	WaitForSingleObject(thread->handle, 30000);
	CloseHandle(thread->handle); // 2011-11-18 추가
	//

	thread->handle = NULL;
	thread->id = 0;
}

/*
// 256이하는 이전의 루틴 그 이상은 PortThread_CommonNew를 사용한다. 별로 필요 없을 듯 하다.
static DWORD WINAPI PortThread_CommonNew(LPVOID lpparam)
{
	GLOBAL_PORT_STRUCT *pt = (GLOBAL_PORT_STRUCT*)lpparam;

	if(CoInitialize(NULL) != S_OK) {
		bell();	
	}

	THREAD_PORT_STRUCT *thread = &pt->threadInfo;;
	int sleep_cycle = 1;

	sleep_cycle = pt->nThreadCycle;

	if(sleep_cycle < 1)	sleep_cycle = 1;

	thread->bDo  = ON;
	thread->bEnd = OFF;

	while(thread->bDo) {
		WatchDogPlcScanReset(pt->local.no);
		Sleep(sleep_cycle);
		CommStatusLocalOne(pt);
	}

	thread->bEnd = ON;

	CoUninitialize();

	return 0;
}

static void PortThread_Common(int port)
{
	if(CoInitialize(NULL) != S_OK) {
		bell();	
	}

	THREAD_PORT_STRUCT *thread = &portBuf[port].threadInfo;
	int sleep_cycle = 1;

	sleep_cycle = portBuf[port].nThreadCycle;

	if(sleep_cycle < 1)	sleep_cycle = 1;

	thread->bDo  = ON;
	thread->bEnd = OFF;

	while(thread->bDo) {
		WatchDogPlcScanReset(port);
		Sleep(sleep_cycle);
		CommStatusLocalOne(&portBuf[port]);
	}

	thread->bEnd = ON;

	CoUninitialize();
}

static DWORD WINAPI PortThread_0(LPVOID)	{	PortThread_Common(0);	return 0;	}
static DWORD WINAPI PortThread_1(LPVOID)	{	PortThread_Common(1);	return 0;	}
static DWORD WINAPI PortThread_2(LPVOID)	{	PortThread_Common(2);	return 0;	}
static DWORD WINAPI PortThread_3(LPVOID)	{	PortThread_Common(3);	return 0;	}
static DWORD WINAPI PortThread_4(LPVOID)	{	PortThread_Common(4);	return 0;	}
static DWORD WINAPI PortThread_5(LPVOID)	{	PortThread_Common(5);	return 0;	}
static DWORD WINAPI PortThread_6(LPVOID)	{	PortThread_Common(6);	return 0;	}
static DWORD WINAPI PortThread_7(LPVOID)	{	PortThread_Common(7);	return 0;	}
static DWORD WINAPI PortThread_8(LPVOID)	{	PortThread_Common(8);	return 0;	}
static DWORD WINAPI PortThread_9(LPVOID)	{	PortThread_Common(9);	return 0;	}
static DWORD WINAPI PortThread_10(LPVOID)	{	PortThread_Common(10);	return 0;	}
static DWORD WINAPI PortThread_11(LPVOID)	{	PortThread_Common(11);	return 0;	}
static DWORD WINAPI PortThread_12(LPVOID)	{	PortThread_Common(12);	return 0;	}
static DWORD WINAPI PortThread_13(LPVOID)	{	PortThread_Common(13);	return 0;	}
static DWORD WINAPI PortThread_14(LPVOID)	{	PortThread_Common(14);	return 0;	}
static DWORD WINAPI PortThread_15(LPVOID)	{	PortThread_Common(15);	return 0;	}
static DWORD WINAPI PortThread_16(LPVOID)	{	PortThread_Common(16);	return 0;	}
static DWORD WINAPI PortThread_17(LPVOID)	{	PortThread_Common(17);	return 0;	}
static DWORD WINAPI PortThread_18(LPVOID)	{	PortThread_Common(18);	return 0;	}
static DWORD WINAPI PortThread_19(LPVOID)	{	PortThread_Common(19);	return 0;	}
static DWORD WINAPI PortThread_20(LPVOID)	{	PortThread_Common(20);	return 0;	}
static DWORD WINAPI PortThread_21(LPVOID)	{	PortThread_Common(21);	return 0;	}
static DWORD WINAPI PortThread_22(LPVOID)	{	PortThread_Common(22);	return 0;	}
static DWORD WINAPI PortThread_23(LPVOID)	{	PortThread_Common(23);	return 0;	}
static DWORD WINAPI PortThread_24(LPVOID)	{	PortThread_Common(24);	return 0;	}
static DWORD WINAPI PortThread_25(LPVOID)	{	PortThread_Common(25);	return 0;	}
static DWORD WINAPI PortThread_26(LPVOID)	{	PortThread_Common(26);	return 0;	}
static DWORD WINAPI PortThread_27(LPVOID)	{	PortThread_Common(27);	return 0;	}
static DWORD WINAPI PortThread_28(LPVOID)	{	PortThread_Common(28);	return 0;	}
static DWORD WINAPI PortThread_29(LPVOID)	{	PortThread_Common(29);	return 0;	}
static DWORD WINAPI PortThread_30(LPVOID)	{	PortThread_Common(30);	return 0;	}
static DWORD WINAPI PortThread_31(LPVOID)	{	PortThread_Common(31);	return 0;	}
static DWORD WINAPI PortThread_32(LPVOID)	{	PortThread_Common(32);	return 0;	}
static DWORD WINAPI PortThread_33(LPVOID)	{	PortThread_Common(33);	return 0;	}
static DWORD WINAPI PortThread_34(LPVOID)	{	PortThread_Common(34);	return 0;	}
static DWORD WINAPI PortThread_35(LPVOID)	{	PortThread_Common(35);	return 0;	}
static DWORD WINAPI PortThread_36(LPVOID)	{	PortThread_Common(36);	return 0;	}
static DWORD WINAPI PortThread_37(LPVOID)	{	PortThread_Common(37);	return 0;	}
static DWORD WINAPI PortThread_38(LPVOID)	{	PortThread_Common(38);	return 0;	}
static DWORD WINAPI PortThread_39(LPVOID)	{	PortThread_Common(39);	return 0;	}
static DWORD WINAPI PortThread_40(LPVOID)	{	PortThread_Common(40);	return 0;	}
static DWORD WINAPI PortThread_41(LPVOID)	{	PortThread_Common(41);	return 0;	}
static DWORD WINAPI PortThread_42(LPVOID)	{	PortThread_Common(42);	return 0;	}
static DWORD WINAPI PortThread_43(LPVOID)	{	PortThread_Common(43);	return 0;	}
static DWORD WINAPI PortThread_44(LPVOID)	{	PortThread_Common(44);	return 0;	}
static DWORD WINAPI PortThread_45(LPVOID)	{	PortThread_Common(45);	return 0;	}
static DWORD WINAPI PortThread_46(LPVOID)	{	PortThread_Common(46);	return 0;	}
static DWORD WINAPI PortThread_47(LPVOID)	{	PortThread_Common(47);	return 0;	}
static DWORD WINAPI PortThread_48(LPVOID)	{	PortThread_Common(48);	return 0;	}
static DWORD WINAPI PortThread_49(LPVOID)	{	PortThread_Common(49);	return 0;	}
static DWORD WINAPI PortThread_50(LPVOID)	{	PortThread_Common(50);	return 0;	}
static DWORD WINAPI PortThread_51(LPVOID)	{	PortThread_Common(51);	return 0;	}
static DWORD WINAPI PortThread_52(LPVOID)	{	PortThread_Common(52);	return 0;	}
static DWORD WINAPI PortThread_53(LPVOID)	{	PortThread_Common(53);	return 0;	}
static DWORD WINAPI PortThread_54(LPVOID)	{	PortThread_Common(54);	return 0;	}
static DWORD WINAPI PortThread_55(LPVOID)	{	PortThread_Common(55);	return 0;	}
static DWORD WINAPI PortThread_56(LPVOID)	{	PortThread_Common(56);	return 0;	}
static DWORD WINAPI PortThread_57(LPVOID)	{	PortThread_Common(57);	return 0;	}
static DWORD WINAPI PortThread_58(LPVOID)	{	PortThread_Common(58);	return 0;	}
static DWORD WINAPI PortThread_59(LPVOID)	{	PortThread_Common(59);	return 0;	}
static DWORD WINAPI PortThread_60(LPVOID)	{	PortThread_Common(60);	return 0;	}
static DWORD WINAPI PortThread_61(LPVOID)	{	PortThread_Common(61);	return 0;	}
static DWORD WINAPI PortThread_62(LPVOID)	{	PortThread_Common(62);	return 0;	}
static DWORD WINAPI PortThread_63(LPVOID)	{	PortThread_Common(63);	return 0;	}
static DWORD WINAPI PortThread_64(LPVOID)	{	PortThread_Common(64);	return 0;	}
static DWORD WINAPI PortThread_65(LPVOID)	{	PortThread_Common(65);	return 0;	}
static DWORD WINAPI PortThread_66(LPVOID)	{	PortThread_Common(66);	return 0;	}
static DWORD WINAPI PortThread_67(LPVOID)	{	PortThread_Common(67);	return 0;	}
static DWORD WINAPI PortThread_68(LPVOID)	{	PortThread_Common(68);	return 0;	}
static DWORD WINAPI PortThread_69(LPVOID)	{	PortThread_Common(69);	return 0;	}
static DWORD WINAPI PortThread_70(LPVOID)	{	PortThread_Common(70);	return 0;	}
static DWORD WINAPI PortThread_71(LPVOID)	{	PortThread_Common(71);	return 0;	}
static DWORD WINAPI PortThread_72(LPVOID)	{	PortThread_Common(72);	return 0;	}
static DWORD WINAPI PortThread_73(LPVOID)	{	PortThread_Common(73);	return 0;	}
static DWORD WINAPI PortThread_74(LPVOID)	{	PortThread_Common(74);	return 0;	}
static DWORD WINAPI PortThread_75(LPVOID)	{	PortThread_Common(75);	return 0;	}
static DWORD WINAPI PortThread_76(LPVOID)	{	PortThread_Common(76);	return 0;	}
static DWORD WINAPI PortThread_77(LPVOID)	{	PortThread_Common(77);	return 0;	}
static DWORD WINAPI PortThread_78(LPVOID)	{	PortThread_Common(78);	return 0;	}
static DWORD WINAPI PortThread_79(LPVOID)	{	PortThread_Common(79);	return 0;	}
static DWORD WINAPI PortThread_80(LPVOID)	{	PortThread_Common(80);	return 0;	}
static DWORD WINAPI PortThread_81(LPVOID)	{	PortThread_Common(81);	return 0;	}
static DWORD WINAPI PortThread_82(LPVOID)	{	PortThread_Common(82);	return 0;	}
static DWORD WINAPI PortThread_83(LPVOID)	{	PortThread_Common(83);	return 0;	}
static DWORD WINAPI PortThread_84(LPVOID)	{	PortThread_Common(84);	return 0;	}
static DWORD WINAPI PortThread_85(LPVOID)	{	PortThread_Common(85);	return 0;	}
static DWORD WINAPI PortThread_86(LPVOID)	{	PortThread_Common(86);	return 0;	}
static DWORD WINAPI PortThread_87(LPVOID)	{	PortThread_Common(87);	return 0;	}
static DWORD WINAPI PortThread_88(LPVOID)	{	PortThread_Common(88);	return 0;	}
static DWORD WINAPI PortThread_89(LPVOID)	{	PortThread_Common(89);	return 0;	}
static DWORD WINAPI PortThread_90(LPVOID)	{	PortThread_Common(90);	return 0;	}
static DWORD WINAPI PortThread_91(LPVOID)	{	PortThread_Common(91);	return 0;	}
static DWORD WINAPI PortThread_92(LPVOID)	{	PortThread_Common(92);	return 0;	}
static DWORD WINAPI PortThread_93(LPVOID)	{	PortThread_Common(93);	return 0;	}
static DWORD WINAPI PortThread_94(LPVOID)	{	PortThread_Common(94);	return 0;	}
static DWORD WINAPI PortThread_95(LPVOID)	{	PortThread_Common(95);	return 0;	}
static DWORD WINAPI PortThread_96(LPVOID)	{	PortThread_Common(96);	return 0;	}
static DWORD WINAPI PortThread_97(LPVOID)	{	PortThread_Common(97);	return 0;	}
static DWORD WINAPI PortThread_98(LPVOID)	{	PortThread_Common(98);	return 0;	}
static DWORD WINAPI PortThread_99(LPVOID)	{	PortThread_Common(99);	return 0;	}

static DWORD WINAPI PortThread_100(LPVOID)	{	PortThread_Common(100);	return 0;	}
static DWORD WINAPI PortThread_101(LPVOID)	{	PortThread_Common(101);	return 0;	}
static DWORD WINAPI PortThread_102(LPVOID)	{	PortThread_Common(102);	return 0;	}
static DWORD WINAPI PortThread_103(LPVOID)	{	PortThread_Common(103);	return 0;	}
static DWORD WINAPI PortThread_104(LPVOID)	{	PortThread_Common(104);	return 0;	}
static DWORD WINAPI PortThread_105(LPVOID)	{	PortThread_Common(105);	return 0;	}
static DWORD WINAPI PortThread_106(LPVOID)	{	PortThread_Common(106);	return 0;	}
static DWORD WINAPI PortThread_107(LPVOID)	{	PortThread_Common(107);	return 0;	}
static DWORD WINAPI PortThread_108(LPVOID)	{	PortThread_Common(108);	return 0;	}
static DWORD WINAPI PortThread_109(LPVOID)	{	PortThread_Common(109);	return 0;	}
static DWORD WINAPI PortThread_110(LPVOID)	{	PortThread_Common(110);	return 0;	}
static DWORD WINAPI PortThread_111(LPVOID)	{	PortThread_Common(111);	return 0;	}
static DWORD WINAPI PortThread_112(LPVOID)	{	PortThread_Common(112);	return 0;	}
static DWORD WINAPI PortThread_113(LPVOID)	{	PortThread_Common(113);	return 0;	}
static DWORD WINAPI PortThread_114(LPVOID)	{	PortThread_Common(114);	return 0;	}
static DWORD WINAPI PortThread_115(LPVOID)	{	PortThread_Common(115);	return 0;	}
static DWORD WINAPI PortThread_116(LPVOID)	{	PortThread_Common(116);	return 0;	}
static DWORD WINAPI PortThread_117(LPVOID)	{	PortThread_Common(117);	return 0;	}
static DWORD WINAPI PortThread_118(LPVOID)	{	PortThread_Common(118);	return 0;	}
static DWORD WINAPI PortThread_119(LPVOID)	{	PortThread_Common(119);	return 0;	}
static DWORD WINAPI PortThread_120(LPVOID)	{	PortThread_Common(120);	return 0;	}
static DWORD WINAPI PortThread_121(LPVOID)	{	PortThread_Common(121);	return 0;	}
static DWORD WINAPI PortThread_122(LPVOID)	{	PortThread_Common(122);	return 0;	}
static DWORD WINAPI PortThread_123(LPVOID)	{	PortThread_Common(123);	return 0;	}
static DWORD WINAPI PortThread_124(LPVOID)	{	PortThread_Common(124);	return 0;	}
static DWORD WINAPI PortThread_125(LPVOID)	{	PortThread_Common(125);	return 0;	}
static DWORD WINAPI PortThread_126(LPVOID)	{	PortThread_Common(126);	return 0;	}
static DWORD WINAPI PortThread_127(LPVOID)	{	PortThread_Common(127);	return 0;	}
static DWORD WINAPI PortThread_128(LPVOID)	{	PortThread_Common(128);	return 0;	}
static DWORD WINAPI PortThread_129(LPVOID)	{	PortThread_Common(129);	return 0;	}
static DWORD WINAPI PortThread_130(LPVOID)	{	PortThread_Common(130);	return 0;	}
static DWORD WINAPI PortThread_131(LPVOID)	{	PortThread_Common(131);	return 0;	}
static DWORD WINAPI PortThread_132(LPVOID)	{	PortThread_Common(132);	return 0;	}
static DWORD WINAPI PortThread_133(LPVOID)	{	PortThread_Common(133);	return 0;	}
static DWORD WINAPI PortThread_134(LPVOID)	{	PortThread_Common(134);	return 0;	}
static DWORD WINAPI PortThread_135(LPVOID)	{	PortThread_Common(135);	return 0;	}
static DWORD WINAPI PortThread_136(LPVOID)	{	PortThread_Common(136);	return 0;	}
static DWORD WINAPI PortThread_137(LPVOID)	{	PortThread_Common(137);	return 0;	}
static DWORD WINAPI PortThread_138(LPVOID)	{	PortThread_Common(138);	return 0;	}
static DWORD WINAPI PortThread_139(LPVOID)	{	PortThread_Common(139);	return 0;	}
static DWORD WINAPI PortThread_140(LPVOID)	{	PortThread_Common(140);	return 0;	}
static DWORD WINAPI PortThread_141(LPVOID)	{	PortThread_Common(141);	return 0;	}
static DWORD WINAPI PortThread_142(LPVOID)	{	PortThread_Common(142);	return 0;	}
static DWORD WINAPI PortThread_143(LPVOID)	{	PortThread_Common(143);	return 0;	}
static DWORD WINAPI PortThread_144(LPVOID)	{	PortThread_Common(144);	return 0;	}
static DWORD WINAPI PortThread_145(LPVOID)	{	PortThread_Common(145);	return 0;	}
static DWORD WINAPI PortThread_146(LPVOID)	{	PortThread_Common(146);	return 0;	}
static DWORD WINAPI PortThread_147(LPVOID)	{	PortThread_Common(147);	return 0;	}
static DWORD WINAPI PortThread_148(LPVOID)	{	PortThread_Common(148);	return 0;	}
static DWORD WINAPI PortThread_149(LPVOID)	{	PortThread_Common(149);	return 0;	}
static DWORD WINAPI PortThread_150(LPVOID)	{	PortThread_Common(150);	return 0;	}
static DWORD WINAPI PortThread_151(LPVOID)	{	PortThread_Common(151);	return 0;	}
static DWORD WINAPI PortThread_152(LPVOID)	{	PortThread_Common(152);	return 0;	}
static DWORD WINAPI PortThread_153(LPVOID)	{	PortThread_Common(153);	return 0;	}
static DWORD WINAPI PortThread_154(LPVOID)	{	PortThread_Common(154);	return 0;	}
static DWORD WINAPI PortThread_155(LPVOID)	{	PortThread_Common(155);	return 0;	}
static DWORD WINAPI PortThread_156(LPVOID)	{	PortThread_Common(156);	return 0;	}
static DWORD WINAPI PortThread_157(LPVOID)	{	PortThread_Common(157);	return 0;	}
static DWORD WINAPI PortThread_158(LPVOID)	{	PortThread_Common(158);	return 0;	}
static DWORD WINAPI PortThread_159(LPVOID)	{	PortThread_Common(159);	return 0;	}
static DWORD WINAPI PortThread_160(LPVOID)	{	PortThread_Common(160);	return 0;	}
static DWORD WINAPI PortThread_161(LPVOID)	{	PortThread_Common(161);	return 0;	}
static DWORD WINAPI PortThread_162(LPVOID)	{	PortThread_Common(162);	return 0;	}
static DWORD WINAPI PortThread_163(LPVOID)	{	PortThread_Common(163);	return 0;	}
static DWORD WINAPI PortThread_164(LPVOID)	{	PortThread_Common(164);	return 0;	}
static DWORD WINAPI PortThread_165(LPVOID)	{	PortThread_Common(165);	return 0;	}
static DWORD WINAPI PortThread_166(LPVOID)	{	PortThread_Common(166);	return 0;	}
static DWORD WINAPI PortThread_167(LPVOID)	{	PortThread_Common(167);	return 0;	}
static DWORD WINAPI PortThread_168(LPVOID)	{	PortThread_Common(168);	return 0;	}
static DWORD WINAPI PortThread_169(LPVOID)	{	PortThread_Common(169);	return 0;	}
static DWORD WINAPI PortThread_170(LPVOID)	{	PortThread_Common(170);	return 0;	}
static DWORD WINAPI PortThread_171(LPVOID)	{	PortThread_Common(171);	return 0;	}
static DWORD WINAPI PortThread_172(LPVOID)	{	PortThread_Common(172);	return 0;	}
static DWORD WINAPI PortThread_173(LPVOID)	{	PortThread_Common(173);	return 0;	}
static DWORD WINAPI PortThread_174(LPVOID)	{	PortThread_Common(174);	return 0;	}
static DWORD WINAPI PortThread_175(LPVOID)	{	PortThread_Common(175);	return 0;	}
static DWORD WINAPI PortThread_176(LPVOID)	{	PortThread_Common(176);	return 0;	}
static DWORD WINAPI PortThread_177(LPVOID)	{	PortThread_Common(177);	return 0;	}
static DWORD WINAPI PortThread_178(LPVOID)	{	PortThread_Common(178);	return 0;	}
static DWORD WINAPI PortThread_179(LPVOID)	{	PortThread_Common(179);	return 0;	}
static DWORD WINAPI PortThread_180(LPVOID)	{	PortThread_Common(180);	return 0;	}
static DWORD WINAPI PortThread_181(LPVOID)	{	PortThread_Common(181);	return 0;	}
static DWORD WINAPI PortThread_182(LPVOID)	{	PortThread_Common(182);	return 0;	}
static DWORD WINAPI PortThread_183(LPVOID)	{	PortThread_Common(183);	return 0;	}
static DWORD WINAPI PortThread_184(LPVOID)	{	PortThread_Common(184);	return 0;	}
static DWORD WINAPI PortThread_185(LPVOID)	{	PortThread_Common(185);	return 0;	}
static DWORD WINAPI PortThread_186(LPVOID)	{	PortThread_Common(186);	return 0;	}
static DWORD WINAPI PortThread_187(LPVOID)	{	PortThread_Common(187);	return 0;	}
static DWORD WINAPI PortThread_188(LPVOID)	{	PortThread_Common(188);	return 0;	}
static DWORD WINAPI PortThread_189(LPVOID)	{	PortThread_Common(189);	return 0;	}
static DWORD WINAPI PortThread_190(LPVOID)	{	PortThread_Common(190);	return 0;	}
static DWORD WINAPI PortThread_191(LPVOID)	{	PortThread_Common(191);	return 0;	}
static DWORD WINAPI PortThread_192(LPVOID)	{	PortThread_Common(192);	return 0;	}
static DWORD WINAPI PortThread_193(LPVOID)	{	PortThread_Common(193);	return 0;	}
static DWORD WINAPI PortThread_194(LPVOID)	{	PortThread_Common(194);	return 0;	}
static DWORD WINAPI PortThread_195(LPVOID)	{	PortThread_Common(195);	return 0;	}
static DWORD WINAPI PortThread_196(LPVOID)	{	PortThread_Common(196);	return 0;	}
static DWORD WINAPI PortThread_197(LPVOID)	{	PortThread_Common(197);	return 0;	}
static DWORD WINAPI PortThread_198(LPVOID)	{	PortThread_Common(198);	return 0;	}
static DWORD WINAPI PortThread_199(LPVOID)	{	PortThread_Common(199);	return 0;	}

static DWORD WINAPI PortThread_200(LPVOID)	{	PortThread_Common(200);	return 0;	}
static DWORD WINAPI PortThread_201(LPVOID)	{	PortThread_Common(201);	return 0;	}
static DWORD WINAPI PortThread_202(LPVOID)	{	PortThread_Common(202);	return 0;	}
static DWORD WINAPI PortThread_203(LPVOID)	{	PortThread_Common(203);	return 0;	}
static DWORD WINAPI PortThread_204(LPVOID)	{	PortThread_Common(204);	return 0;	}
static DWORD WINAPI PortThread_205(LPVOID)	{	PortThread_Common(205);	return 0;	}
static DWORD WINAPI PortThread_206(LPVOID)	{	PortThread_Common(206);	return 0;	}
static DWORD WINAPI PortThread_207(LPVOID)	{	PortThread_Common(207);	return 0;	}
static DWORD WINAPI PortThread_208(LPVOID)	{	PortThread_Common(208);	return 0;	}
static DWORD WINAPI PortThread_209(LPVOID)	{	PortThread_Common(209);	return 0;	}
static DWORD WINAPI PortThread_210(LPVOID)	{	PortThread_Common(210);	return 0;	}
static DWORD WINAPI PortThread_211(LPVOID)	{	PortThread_Common(211);	return 0;	}
static DWORD WINAPI PortThread_212(LPVOID)	{	PortThread_Common(212);	return 0;	}
static DWORD WINAPI PortThread_213(LPVOID)	{	PortThread_Common(213);	return 0;	}
static DWORD WINAPI PortThread_214(LPVOID)	{	PortThread_Common(214);	return 0;	}
static DWORD WINAPI PortThread_215(LPVOID)	{	PortThread_Common(215);	return 0;	}
static DWORD WINAPI PortThread_216(LPVOID)	{	PortThread_Common(216);	return 0;	}
static DWORD WINAPI PortThread_217(LPVOID)	{	PortThread_Common(217);	return 0;	}
static DWORD WINAPI PortThread_218(LPVOID)	{	PortThread_Common(218);	return 0;	}
static DWORD WINAPI PortThread_219(LPVOID)	{	PortThread_Common(219);	return 0;	}
static DWORD WINAPI PortThread_220(LPVOID)	{	PortThread_Common(220);	return 0;	}
static DWORD WINAPI PortThread_221(LPVOID)	{	PortThread_Common(221);	return 0;	}
static DWORD WINAPI PortThread_222(LPVOID)	{	PortThread_Common(222);	return 0;	}
static DWORD WINAPI PortThread_223(LPVOID)	{	PortThread_Common(223);	return 0;	}
static DWORD WINAPI PortThread_224(LPVOID)	{	PortThread_Common(224);	return 0;	}
static DWORD WINAPI PortThread_225(LPVOID)	{	PortThread_Common(225);	return 0;	}
static DWORD WINAPI PortThread_226(LPVOID)	{	PortThread_Common(226);	return 0;	}
static DWORD WINAPI PortThread_227(LPVOID)	{	PortThread_Common(227);	return 0;	}
static DWORD WINAPI PortThread_228(LPVOID)	{	PortThread_Common(228);	return 0;	}
static DWORD WINAPI PortThread_229(LPVOID)	{	PortThread_Common(229);	return 0;	}
static DWORD WINAPI PortThread_230(LPVOID)	{	PortThread_Common(230);	return 0;	}
static DWORD WINAPI PortThread_231(LPVOID)	{	PortThread_Common(231);	return 0;	}
static DWORD WINAPI PortThread_232(LPVOID)	{	PortThread_Common(232);	return 0;	}
static DWORD WINAPI PortThread_233(LPVOID)	{	PortThread_Common(233);	return 0;	}
static DWORD WINAPI PortThread_234(LPVOID)	{	PortThread_Common(234);	return 0;	}
static DWORD WINAPI PortThread_235(LPVOID)	{	PortThread_Common(235);	return 0;	}
static DWORD WINAPI PortThread_236(LPVOID)	{	PortThread_Common(236);	return 0;	}
static DWORD WINAPI PortThread_237(LPVOID)	{	PortThread_Common(237);	return 0;	}
static DWORD WINAPI PortThread_238(LPVOID)	{	PortThread_Common(238);	return 0;	}
static DWORD WINAPI PortThread_239(LPVOID)	{	PortThread_Common(239);	return 0;	}
static DWORD WINAPI PortThread_240(LPVOID)	{	PortThread_Common(240);	return 0;	}
static DWORD WINAPI PortThread_241(LPVOID)	{	PortThread_Common(241);	return 0;	}
static DWORD WINAPI PortThread_242(LPVOID)	{	PortThread_Common(242);	return 0;	}
static DWORD WINAPI PortThread_243(LPVOID)	{	PortThread_Common(243);	return 0;	}
static DWORD WINAPI PortThread_244(LPVOID)	{	PortThread_Common(244);	return 0;	}
static DWORD WINAPI PortThread_245(LPVOID)	{	PortThread_Common(245);	return 0;	}
static DWORD WINAPI PortThread_246(LPVOID)	{	PortThread_Common(246);	return 0;	}
static DWORD WINAPI PortThread_247(LPVOID)	{	PortThread_Common(247);	return 0;	}
static DWORD WINAPI PortThread_248(LPVOID)	{	PortThread_Common(248);	return 0;	}
static DWORD WINAPI PortThread_249(LPVOID)	{	PortThread_Common(249);	return 0;	}
static DWORD WINAPI PortThread_250(LPVOID)	{	PortThread_Common(250);	return 0;	}
static DWORD WINAPI PortThread_251(LPVOID)	{	PortThread_Common(251);	return 0;	}
static DWORD WINAPI PortThread_252(LPVOID)	{	PortThread_Common(252);	return 0;	}
static DWORD WINAPI PortThread_253(LPVOID)	{	PortThread_Common(253);	return 0;	}
static DWORD WINAPI PortThread_254(LPVOID)	{	PortThread_Common(254);	return 0;	}
static DWORD WINAPI PortThread_255(LPVOID)	{	PortThread_Common(255);	return 0;	}

static void Init_000_049(int port, THREAD_PORT_STRUCT *thread)
{
	if(port == 0)		thread->handle = CreateThread(NULL, 0, PortThread_0, NULL, 0, &thread->id);
	else if(port == 1)	thread->handle = CreateThread(NULL, 0, PortThread_1, NULL, 0, &thread->id);
	else if(port == 2)	thread->handle = CreateThread(NULL, 0, PortThread_2, NULL, 0, &thread->id);
	else if(port == 3)	thread->handle = CreateThread(NULL, 0, PortThread_3, NULL, 0, &thread->id);
	else if(port == 4)	thread->handle = CreateThread(NULL, 0, PortThread_4, NULL, 0, &thread->id);
	else if(port == 5)	thread->handle = CreateThread(NULL, 0, PortThread_5, NULL, 0, &thread->id);
	else if(port == 6)	thread->handle = CreateThread(NULL, 0, PortThread_6, NULL, 0, &thread->id);
	else if(port == 7)	thread->handle = CreateThread(NULL, 0, PortThread_7, NULL, 0, &thread->id);
	else if(port == 8)	thread->handle = CreateThread(NULL, 0, PortThread_8, NULL, 0, &thread->id);
	else if(port == 9)	thread->handle = CreateThread(NULL, 0, PortThread_9, NULL, 0, &thread->id);
	else if(port == 10)	thread->handle = CreateThread(NULL, 0, PortThread_10, NULL, 0, &thread->id);
	else if(port == 11)	thread->handle = CreateThread(NULL, 0, PortThread_11, NULL, 0, &thread->id);
	else if(port == 12)	thread->handle = CreateThread(NULL, 0, PortThread_12, NULL, 0, &thread->id);
	else if(port == 13)	thread->handle = CreateThread(NULL, 0, PortThread_13, NULL, 0, &thread->id);
	else if(port == 14)	thread->handle = CreateThread(NULL, 0, PortThread_14, NULL, 0, &thread->id);
	else if(port == 15)	thread->handle = CreateThread(NULL, 0, PortThread_15, NULL, 0, &thread->id);
	else if(port == 16)	thread->handle = CreateThread(NULL, 0, PortThread_16, NULL, 0, &thread->id);
	else if(port == 17)	thread->handle = CreateThread(NULL, 0, PortThread_17, NULL, 0, &thread->id);
	else if(port == 18)	thread->handle = CreateThread(NULL, 0, PortThread_18, NULL, 0, &thread->id);
	else if(port == 19)	thread->handle = CreateThread(NULL, 0, PortThread_19, NULL, 0, &thread->id);
	else if(port == 20)	thread->handle = CreateThread(NULL, 0, PortThread_20, NULL, 0, &thread->id);
	else if(port == 21)	thread->handle = CreateThread(NULL, 0, PortThread_21, NULL, 0, &thread->id);
	else if(port == 22)	thread->handle = CreateThread(NULL, 0, PortThread_22, NULL, 0, &thread->id);
	else if(port == 23)	thread->handle = CreateThread(NULL, 0, PortThread_23, NULL, 0, &thread->id);
	else if(port == 24)	thread->handle = CreateThread(NULL, 0, PortThread_24, NULL, 0, &thread->id);
	else if(port == 25)	thread->handle = CreateThread(NULL, 0, PortThread_25, NULL, 0, &thread->id);
	else if(port == 26)	thread->handle = CreateThread(NULL, 0, PortThread_26, NULL, 0, &thread->id);
	else if(port == 27)	thread->handle = CreateThread(NULL, 0, PortThread_27, NULL, 0, &thread->id);
	else if(port == 28)	thread->handle = CreateThread(NULL, 0, PortThread_28, NULL, 0, &thread->id);
	else if(port == 29)	thread->handle = CreateThread(NULL, 0, PortThread_29, NULL, 0, &thread->id);
	else if(port == 30)	thread->handle = CreateThread(NULL, 0, PortThread_30, NULL, 0, &thread->id);
	else if(port == 31)	thread->handle = CreateThread(NULL, 0, PortThread_31, NULL, 0, &thread->id);
	else if(port == 32)	thread->handle = CreateThread(NULL, 0, PortThread_32, NULL, 0, &thread->id);
	else if(port == 33)	thread->handle = CreateThread(NULL, 0, PortThread_33, NULL, 0, &thread->id);
	else if(port == 34)	thread->handle = CreateThread(NULL, 0, PortThread_34, NULL, 0, &thread->id);
	else if(port == 35)	thread->handle = CreateThread(NULL, 0, PortThread_35, NULL, 0, &thread->id);
	else if(port == 36)	thread->handle = CreateThread(NULL, 0, PortThread_36, NULL, 0, &thread->id);
	else if(port == 37)	thread->handle = CreateThread(NULL, 0, PortThread_37, NULL, 0, &thread->id);
	else if(port == 38)	thread->handle = CreateThread(NULL, 0, PortThread_38, NULL, 0, &thread->id);
	else if(port == 39)	thread->handle = CreateThread(NULL, 0, PortThread_39, NULL, 0, &thread->id);
	else if(port == 40)	thread->handle = CreateThread(NULL, 0, PortThread_40, NULL, 0, &thread->id);
	else if(port == 41)	thread->handle = CreateThread(NULL, 0, PortThread_41, NULL, 0, &thread->id);
	else if(port == 42)	thread->handle = CreateThread(NULL, 0, PortThread_42, NULL, 0, &thread->id);
	else if(port == 43)	thread->handle = CreateThread(NULL, 0, PortThread_43, NULL, 0, &thread->id);
	else if(port == 44)	thread->handle = CreateThread(NULL, 0, PortThread_44, NULL, 0, &thread->id);
	else if(port == 45)	thread->handle = CreateThread(NULL, 0, PortThread_45, NULL, 0, &thread->id);
	else if(port == 46)	thread->handle = CreateThread(NULL, 0, PortThread_46, NULL, 0, &thread->id);
	else if(port == 47)	thread->handle = CreateThread(NULL, 0, PortThread_47, NULL, 0, &thread->id);
	else if(port == 48)	thread->handle = CreateThread(NULL, 0, PortThread_48, NULL, 0, &thread->id);
	else if(port == 49)	thread->handle = CreateThread(NULL, 0, PortThread_49, NULL, 0, &thread->id);

	else;
}

static void Init_050_099(int port, THREAD_PORT_STRUCT *thread)
{
	if(port == 50)		thread->handle = CreateThread(NULL, 0, PortThread_50, NULL, 0, &thread->id);
	else if(port == 51)	thread->handle = CreateThread(NULL, 0, PortThread_51, NULL, 0, &thread->id);
	else if(port == 52)	thread->handle = CreateThread(NULL, 0, PortThread_52, NULL, 0, &thread->id);
	else if(port == 53)	thread->handle = CreateThread(NULL, 0, PortThread_53, NULL, 0, &thread->id);
	else if(port == 54)	thread->handle = CreateThread(NULL, 0, PortThread_54, NULL, 0, &thread->id);
	else if(port == 55)	thread->handle = CreateThread(NULL, 0, PortThread_55, NULL, 0, &thread->id);
	else if(port == 56)	thread->handle = CreateThread(NULL, 0, PortThread_56, NULL, 0, &thread->id);
	else if(port == 57)	thread->handle = CreateThread(NULL, 0, PortThread_57, NULL, 0, &thread->id);
	else if(port == 58)	thread->handle = CreateThread(NULL, 0, PortThread_58, NULL, 0, &thread->id);
	else if(port == 59)	thread->handle = CreateThread(NULL, 0, PortThread_59, NULL, 0, &thread->id);
	else if(port == 60)	thread->handle = CreateThread(NULL, 0, PortThread_60, NULL, 0, &thread->id);
	else if(port == 61)	thread->handle = CreateThread(NULL, 0, PortThread_61, NULL, 0, &thread->id);
	else if(port == 62)	thread->handle = CreateThread(NULL, 0, PortThread_62, NULL, 0, &thread->id);
	else if(port == 63)	thread->handle = CreateThread(NULL, 0, PortThread_63, NULL, 0, &thread->id);
	else if(port == 64)	thread->handle = CreateThread(NULL, 0, PortThread_64, NULL, 0, &thread->id);
	else if(port == 65)	thread->handle = CreateThread(NULL, 0, PortThread_65, NULL, 0, &thread->id);
	else if(port == 66)	thread->handle = CreateThread(NULL, 0, PortThread_66, NULL, 0, &thread->id);
	else if(port == 67)	thread->handle = CreateThread(NULL, 0, PortThread_67, NULL, 0, &thread->id);
	else if(port == 68)	thread->handle = CreateThread(NULL, 0, PortThread_68, NULL, 0, &thread->id);
	else if(port == 69)	thread->handle = CreateThread(NULL, 0, PortThread_69, NULL, 0, &thread->id);
	else if(port == 70)	thread->handle = CreateThread(NULL, 0, PortThread_70, NULL, 0, &thread->id);
	else if(port == 71)	thread->handle = CreateThread(NULL, 0, PortThread_71, NULL, 0, &thread->id);
	else if(port == 72)	thread->handle = CreateThread(NULL, 0, PortThread_72, NULL, 0, &thread->id);
	else if(port == 73)	thread->handle = CreateThread(NULL, 0, PortThread_73, NULL, 0, &thread->id);
	else if(port == 74)	thread->handle = CreateThread(NULL, 0, PortThread_74, NULL, 0, &thread->id);
	else if(port == 75)	thread->handle = CreateThread(NULL, 0, PortThread_75, NULL, 0, &thread->id);
	else if(port == 76)	thread->handle = CreateThread(NULL, 0, PortThread_76, NULL, 0, &thread->id);
	else if(port == 77)	thread->handle = CreateThread(NULL, 0, PortThread_77, NULL, 0, &thread->id);
	else if(port == 78)	thread->handle = CreateThread(NULL, 0, PortThread_78, NULL, 0, &thread->id);
	else if(port == 79)	thread->handle = CreateThread(NULL, 0, PortThread_79, NULL, 0, &thread->id);
	else if(port == 80)	thread->handle = CreateThread(NULL, 0, PortThread_80, NULL, 0, &thread->id);
	else if(port == 81)	thread->handle = CreateThread(NULL, 0, PortThread_81, NULL, 0, &thread->id);
	else if(port == 82)	thread->handle = CreateThread(NULL, 0, PortThread_82, NULL, 0, &thread->id);
	else if(port == 83)	thread->handle = CreateThread(NULL, 0, PortThread_83, NULL, 0, &thread->id);
	else if(port == 84)	thread->handle = CreateThread(NULL, 0, PortThread_84, NULL, 0, &thread->id);
	else if(port == 85)	thread->handle = CreateThread(NULL, 0, PortThread_85, NULL, 0, &thread->id);
	else if(port == 86)	thread->handle = CreateThread(NULL, 0, PortThread_86, NULL, 0, &thread->id);
	else if(port == 87)	thread->handle = CreateThread(NULL, 0, PortThread_87, NULL, 0, &thread->id);
	else if(port == 88)	thread->handle = CreateThread(NULL, 0, PortThread_88, NULL, 0, &thread->id);
	else if(port == 89)	thread->handle = CreateThread(NULL, 0, PortThread_89, NULL, 0, &thread->id);
	else if(port == 90)	thread->handle = CreateThread(NULL, 0, PortThread_90, NULL, 0, &thread->id);
	else if(port == 91)	thread->handle = CreateThread(NULL, 0, PortThread_91, NULL, 0, &thread->id);
	else if(port == 92)	thread->handle = CreateThread(NULL, 0, PortThread_92, NULL, 0, &thread->id);
	else if(port == 93)	thread->handle = CreateThread(NULL, 0, PortThread_93, NULL, 0, &thread->id);
	else if(port == 94)	thread->handle = CreateThread(NULL, 0, PortThread_94, NULL, 0, &thread->id);
	else if(port == 95)	thread->handle = CreateThread(NULL, 0, PortThread_95, NULL, 0, &thread->id);
	else if(port == 96)	thread->handle = CreateThread(NULL, 0, PortThread_96, NULL, 0, &thread->id);
	else if(port == 97)	thread->handle = CreateThread(NULL, 0, PortThread_97, NULL, 0, &thread->id);
	else if(port == 98)	thread->handle = CreateThread(NULL, 0, PortThread_98, NULL, 0, &thread->id);
	else if(port == 99)	thread->handle = CreateThread(NULL, 0, PortThread_99, NULL, 0, &thread->id);

	else;
}

static void Init_100_149(int port, THREAD_PORT_STRUCT *thread)
{
	if(port == 100)			thread->handle = CreateThread(NULL, 0, PortThread_100, NULL, 0, &thread->id);
	else if(port == 101)	thread->handle = CreateThread(NULL, 0, PortThread_101, NULL, 0, &thread->id);
	else if(port == 102)	thread->handle = CreateThread(NULL, 0, PortThread_102, NULL, 0, &thread->id);
	else if(port == 103)	thread->handle = CreateThread(NULL, 0, PortThread_103, NULL, 0, &thread->id);
	else if(port == 104)	thread->handle = CreateThread(NULL, 0, PortThread_104, NULL, 0, &thread->id);
	else if(port == 105)	thread->handle = CreateThread(NULL, 0, PortThread_105, NULL, 0, &thread->id);
	else if(port == 106)	thread->handle = CreateThread(NULL, 0, PortThread_106, NULL, 0, &thread->id);
	else if(port == 107)	thread->handle = CreateThread(NULL, 0, PortThread_107, NULL, 0, &thread->id);
	else if(port == 108)	thread->handle = CreateThread(NULL, 0, PortThread_108, NULL, 0, &thread->id);
	else if(port == 109)	thread->handle = CreateThread(NULL, 0, PortThread_109, NULL, 0, &thread->id);
	else if(port == 110)	thread->handle = CreateThread(NULL, 0, PortThread_110, NULL, 0, &thread->id);
	else if(port == 111)	thread->handle = CreateThread(NULL, 0, PortThread_111, NULL, 0, &thread->id);
	else if(port == 112)	thread->handle = CreateThread(NULL, 0, PortThread_112, NULL, 0, &thread->id);
	else if(port == 113)	thread->handle = CreateThread(NULL, 0, PortThread_113, NULL, 0, &thread->id);
	else if(port == 114)	thread->handle = CreateThread(NULL, 0, PortThread_114, NULL, 0, &thread->id);
	else if(port == 115)	thread->handle = CreateThread(NULL, 0, PortThread_115, NULL, 0, &thread->id);
	else if(port == 116)	thread->handle = CreateThread(NULL, 0, PortThread_116, NULL, 0, &thread->id);
	else if(port == 117)	thread->handle = CreateThread(NULL, 0, PortThread_117, NULL, 0, &thread->id);
	else if(port == 118)	thread->handle = CreateThread(NULL, 0, PortThread_118, NULL, 0, &thread->id);
	else if(port == 119)	thread->handle = CreateThread(NULL, 0, PortThread_119, NULL, 0, &thread->id);
	else if(port == 120)	thread->handle = CreateThread(NULL, 0, PortThread_120, NULL, 0, &thread->id);
	else if(port == 121)	thread->handle = CreateThread(NULL, 0, PortThread_121, NULL, 0, &thread->id);
	else if(port == 122)	thread->handle = CreateThread(NULL, 0, PortThread_122, NULL, 0, &thread->id);
	else if(port == 123)	thread->handle = CreateThread(NULL, 0, PortThread_123, NULL, 0, &thread->id);
	else if(port == 124)	thread->handle = CreateThread(NULL, 0, PortThread_124, NULL, 0, &thread->id);
	else if(port == 125)	thread->handle = CreateThread(NULL, 0, PortThread_125, NULL, 0, &thread->id);
	else if(port == 126)	thread->handle = CreateThread(NULL, 0, PortThread_126, NULL, 0, &thread->id);
	else if(port == 127)	thread->handle = CreateThread(NULL, 0, PortThread_127, NULL, 0, &thread->id);
	else if(port == 128)	thread->handle = CreateThread(NULL, 0, PortThread_128, NULL, 0, &thread->id);
	else if(port == 129)	thread->handle = CreateThread(NULL, 0, PortThread_129, NULL, 0, &thread->id);
	else if(port == 130)	thread->handle = CreateThread(NULL, 0, PortThread_130, NULL, 0, &thread->id);
	else if(port == 131)	thread->handle = CreateThread(NULL, 0, PortThread_131, NULL, 0, &thread->id);
	else if(port == 132)	thread->handle = CreateThread(NULL, 0, PortThread_132, NULL, 0, &thread->id);
	else if(port == 133)	thread->handle = CreateThread(NULL, 0, PortThread_133, NULL, 0, &thread->id);
	else if(port == 134)	thread->handle = CreateThread(NULL, 0, PortThread_134, NULL, 0, &thread->id);
	else if(port == 135)	thread->handle = CreateThread(NULL, 0, PortThread_135, NULL, 0, &thread->id);
	else if(port == 136)	thread->handle = CreateThread(NULL, 0, PortThread_136, NULL, 0, &thread->id);
	else if(port == 137)	thread->handle = CreateThread(NULL, 0, PortThread_137, NULL, 0, &thread->id);
	else if(port == 138)	thread->handle = CreateThread(NULL, 0, PortThread_138, NULL, 0, &thread->id);
	else if(port == 139)	thread->handle = CreateThread(NULL, 0, PortThread_139, NULL, 0, &thread->id);
	else if(port == 140)	thread->handle = CreateThread(NULL, 0, PortThread_140, NULL, 0, &thread->id);
	else if(port == 141)	thread->handle = CreateThread(NULL, 0, PortThread_141, NULL, 0, &thread->id);
	else if(port == 142)	thread->handle = CreateThread(NULL, 0, PortThread_142, NULL, 0, &thread->id);
	else if(port == 143)	thread->handle = CreateThread(NULL, 0, PortThread_143, NULL, 0, &thread->id);
	else if(port == 144)	thread->handle = CreateThread(NULL, 0, PortThread_144, NULL, 0, &thread->id);
	else if(port == 145)	thread->handle = CreateThread(NULL, 0, PortThread_145, NULL, 0, &thread->id);
	else if(port == 146)	thread->handle = CreateThread(NULL, 0, PortThread_146, NULL, 0, &thread->id);
	else if(port == 147)	thread->handle = CreateThread(NULL, 0, PortThread_147, NULL, 0, &thread->id);
	else if(port == 148)	thread->handle = CreateThread(NULL, 0, PortThread_148, NULL, 0, &thread->id);
	else if(port == 149)	thread->handle = CreateThread(NULL, 0, PortThread_149, NULL, 0, &thread->id);

	else;
}

static void Init_150_199(int port, THREAD_PORT_STRUCT *thread)
{
	if(port == 150)			thread->handle = CreateThread(NULL, 0, PortThread_150, NULL, 0, &thread->id);
	else if(port == 151)	thread->handle = CreateThread(NULL, 0, PortThread_151, NULL, 0, &thread->id);
	else if(port == 152)	thread->handle = CreateThread(NULL, 0, PortThread_152, NULL, 0, &thread->id);
	else if(port == 153)	thread->handle = CreateThread(NULL, 0, PortThread_153, NULL, 0, &thread->id);
	else if(port == 154)	thread->handle = CreateThread(NULL, 0, PortThread_154, NULL, 0, &thread->id);
	else if(port == 155)	thread->handle = CreateThread(NULL, 0, PortThread_155, NULL, 0, &thread->id);
	else if(port == 156)	thread->handle = CreateThread(NULL, 0, PortThread_156, NULL, 0, &thread->id);
	else if(port == 157)	thread->handle = CreateThread(NULL, 0, PortThread_157, NULL, 0, &thread->id);
	else if(port == 158)	thread->handle = CreateThread(NULL, 0, PortThread_158, NULL, 0, &thread->id);
	else if(port == 159)	thread->handle = CreateThread(NULL, 0, PortThread_159, NULL, 0, &thread->id);
	else if(port == 160)	thread->handle = CreateThread(NULL, 0, PortThread_160, NULL, 0, &thread->id);
	else if(port == 161)	thread->handle = CreateThread(NULL, 0, PortThread_161, NULL, 0, &thread->id);
	else if(port == 162)	thread->handle = CreateThread(NULL, 0, PortThread_162, NULL, 0, &thread->id);
	else if(port == 163)	thread->handle = CreateThread(NULL, 0, PortThread_163, NULL, 0, &thread->id);
	else if(port == 164)	thread->handle = CreateThread(NULL, 0, PortThread_164, NULL, 0, &thread->id);
	else if(port == 165)	thread->handle = CreateThread(NULL, 0, PortThread_165, NULL, 0, &thread->id);
	else if(port == 166)	thread->handle = CreateThread(NULL, 0, PortThread_166, NULL, 0, &thread->id);
	else if(port == 167)	thread->handle = CreateThread(NULL, 0, PortThread_167, NULL, 0, &thread->id);
	else if(port == 168)	thread->handle = CreateThread(NULL, 0, PortThread_168, NULL, 0, &thread->id);
	else if(port == 169)	thread->handle = CreateThread(NULL, 0, PortThread_169, NULL, 0, &thread->id);
	else if(port == 170)	thread->handle = CreateThread(NULL, 0, PortThread_170, NULL, 0, &thread->id);
	else if(port == 171)	thread->handle = CreateThread(NULL, 0, PortThread_171, NULL, 0, &thread->id);
	else if(port == 172)	thread->handle = CreateThread(NULL, 0, PortThread_172, NULL, 0, &thread->id);
	else if(port == 173)	thread->handle = CreateThread(NULL, 0, PortThread_173, NULL, 0, &thread->id);
	else if(port == 174)	thread->handle = CreateThread(NULL, 0, PortThread_174, NULL, 0, &thread->id);
	else if(port == 175)	thread->handle = CreateThread(NULL, 0, PortThread_175, NULL, 0, &thread->id);
	else if(port == 176)	thread->handle = CreateThread(NULL, 0, PortThread_176, NULL, 0, &thread->id);
	else if(port == 177)	thread->handle = CreateThread(NULL, 0, PortThread_177, NULL, 0, &thread->id);
	else if(port == 178)	thread->handle = CreateThread(NULL, 0, PortThread_178, NULL, 0, &thread->id);
	else if(port == 179)	thread->handle = CreateThread(NULL, 0, PortThread_179, NULL, 0, &thread->id);
	else if(port == 180)	thread->handle = CreateThread(NULL, 0, PortThread_180, NULL, 0, &thread->id);
	else if(port == 181)	thread->handle = CreateThread(NULL, 0, PortThread_181, NULL, 0, &thread->id);
	else if(port == 182)	thread->handle = CreateThread(NULL, 0, PortThread_182, NULL, 0, &thread->id);
	else if(port == 183)	thread->handle = CreateThread(NULL, 0, PortThread_183, NULL, 0, &thread->id);
	else if(port == 184)	thread->handle = CreateThread(NULL, 0, PortThread_184, NULL, 0, &thread->id);
	else if(port == 185)	thread->handle = CreateThread(NULL, 0, PortThread_185, NULL, 0, &thread->id);
	else if(port == 186)	thread->handle = CreateThread(NULL, 0, PortThread_186, NULL, 0, &thread->id);
	else if(port == 187)	thread->handle = CreateThread(NULL, 0, PortThread_187, NULL, 0, &thread->id);
	else if(port == 188)	thread->handle = CreateThread(NULL, 0, PortThread_188, NULL, 0, &thread->id);
	else if(port == 189)	thread->handle = CreateThread(NULL, 0, PortThread_189, NULL, 0, &thread->id);
	else if(port == 190)	thread->handle = CreateThread(NULL, 0, PortThread_190, NULL, 0, &thread->id);
	else if(port == 191)	thread->handle = CreateThread(NULL, 0, PortThread_191, NULL, 0, &thread->id);
	else if(port == 192)	thread->handle = CreateThread(NULL, 0, PortThread_192, NULL, 0, &thread->id);
	else if(port == 193)	thread->handle = CreateThread(NULL, 0, PortThread_193, NULL, 0, &thread->id);
	else if(port == 194)	thread->handle = CreateThread(NULL, 0, PortThread_194, NULL, 0, &thread->id);
	else if(port == 195)	thread->handle = CreateThread(NULL, 0, PortThread_195, NULL, 0, &thread->id);
	else if(port == 196)	thread->handle = CreateThread(NULL, 0, PortThread_196, NULL, 0, &thread->id);
	else if(port == 197)	thread->handle = CreateThread(NULL, 0, PortThread_197, NULL, 0, &thread->id);
	else if(port == 198)	thread->handle = CreateThread(NULL, 0, PortThread_198, NULL, 0, &thread->id);
	else if(port == 199)	thread->handle = CreateThread(NULL, 0, PortThread_199, NULL, 0, &thread->id);

	else;
}

static void Init_200_255(int port, THREAD_PORT_STRUCT *thread)
{
	if(port == 200)			thread->handle = CreateThread(NULL, 0, PortThread_200, NULL, 0, &thread->id);
	else if(port == 201)	thread->handle = CreateThread(NULL, 0, PortThread_201, NULL, 0, &thread->id);
	else if(port == 202)	thread->handle = CreateThread(NULL, 0, PortThread_202, NULL, 0, &thread->id);
	else if(port == 203)	thread->handle = CreateThread(NULL, 0, PortThread_203, NULL, 0, &thread->id);
	else if(port == 204)	thread->handle = CreateThread(NULL, 0, PortThread_204, NULL, 0, &thread->id);
	else if(port == 205)	thread->handle = CreateThread(NULL, 0, PortThread_205, NULL, 0, &thread->id);
	else if(port == 206)	thread->handle = CreateThread(NULL, 0, PortThread_206, NULL, 0, &thread->id);
	else if(port == 207)	thread->handle = CreateThread(NULL, 0, PortThread_207, NULL, 0, &thread->id);
	else if(port == 208)	thread->handle = CreateThread(NULL, 0, PortThread_208, NULL, 0, &thread->id);
	else if(port == 209)	thread->handle = CreateThread(NULL, 0, PortThread_209, NULL, 0, &thread->id);
	else if(port == 210)	thread->handle = CreateThread(NULL, 0, PortThread_210, NULL, 0, &thread->id);
	else if(port == 211)	thread->handle = CreateThread(NULL, 0, PortThread_211, NULL, 0, &thread->id);
	else if(port == 212)	thread->handle = CreateThread(NULL, 0, PortThread_212, NULL, 0, &thread->id);
	else if(port == 213)	thread->handle = CreateThread(NULL, 0, PortThread_213, NULL, 0, &thread->id);
	else if(port == 214)	thread->handle = CreateThread(NULL, 0, PortThread_214, NULL, 0, &thread->id);
	else if(port == 215)	thread->handle = CreateThread(NULL, 0, PortThread_215, NULL, 0, &thread->id);
	else if(port == 216)	thread->handle = CreateThread(NULL, 0, PortThread_216, NULL, 0, &thread->id);
	else if(port == 217)	thread->handle = CreateThread(NULL, 0, PortThread_217, NULL, 0, &thread->id);
	else if(port == 218)	thread->handle = CreateThread(NULL, 0, PortThread_218, NULL, 0, &thread->id);
	else if(port == 219)	thread->handle = CreateThread(NULL, 0, PortThread_219, NULL, 0, &thread->id);
	else if(port == 220)	thread->handle = CreateThread(NULL, 0, PortThread_220, NULL, 0, &thread->id);
	else if(port == 221)	thread->handle = CreateThread(NULL, 0, PortThread_221, NULL, 0, &thread->id);
	else if(port == 222)	thread->handle = CreateThread(NULL, 0, PortThread_222, NULL, 0, &thread->id);
	else if(port == 223)	thread->handle = CreateThread(NULL, 0, PortThread_223, NULL, 0, &thread->id);
	else if(port == 224)	thread->handle = CreateThread(NULL, 0, PortThread_224, NULL, 0, &thread->id);
	else if(port == 225)	thread->handle = CreateThread(NULL, 0, PortThread_225, NULL, 0, &thread->id);
	else if(port == 226)	thread->handle = CreateThread(NULL, 0, PortThread_226, NULL, 0, &thread->id);
	else if(port == 227)	thread->handle = CreateThread(NULL, 0, PortThread_227, NULL, 0, &thread->id);
	else if(port == 228)	thread->handle = CreateThread(NULL, 0, PortThread_228, NULL, 0, &thread->id);
	else if(port == 229)	thread->handle = CreateThread(NULL, 0, PortThread_229, NULL, 0, &thread->id);
	else if(port == 230)	thread->handle = CreateThread(NULL, 0, PortThread_230, NULL, 0, &thread->id);
	else if(port == 231)	thread->handle = CreateThread(NULL, 0, PortThread_231, NULL, 0, &thread->id);
	else if(port == 232)	thread->handle = CreateThread(NULL, 0, PortThread_232, NULL, 0, &thread->id);
	else if(port == 233)	thread->handle = CreateThread(NULL, 0, PortThread_233, NULL, 0, &thread->id);
	else if(port == 234)	thread->handle = CreateThread(NULL, 0, PortThread_234, NULL, 0, &thread->id);
	else if(port == 235)	thread->handle = CreateThread(NULL, 0, PortThread_235, NULL, 0, &thread->id);
	else if(port == 236)	thread->handle = CreateThread(NULL, 0, PortThread_236, NULL, 0, &thread->id);
	else if(port == 237)	thread->handle = CreateThread(NULL, 0, PortThread_237, NULL, 0, &thread->id);
	else if(port == 238)	thread->handle = CreateThread(NULL, 0, PortThread_238, NULL, 0, &thread->id);
	else if(port == 239)	thread->handle = CreateThread(NULL, 0, PortThread_239, NULL, 0, &thread->id);
	else if(port == 240)	thread->handle = CreateThread(NULL, 0, PortThread_240, NULL, 0, &thread->id);
	else if(port == 241)	thread->handle = CreateThread(NULL, 0, PortThread_241, NULL, 0, &thread->id);
	else if(port == 242)	thread->handle = CreateThread(NULL, 0, PortThread_242, NULL, 0, &thread->id);
	else if(port == 243)	thread->handle = CreateThread(NULL, 0, PortThread_243, NULL, 0, &thread->id);
	else if(port == 244)	thread->handle = CreateThread(NULL, 0, PortThread_244, NULL, 0, &thread->id);
	else if(port == 245)	thread->handle = CreateThread(NULL, 0, PortThread_245, NULL, 0, &thread->id);
	else if(port == 246)	thread->handle = CreateThread(NULL, 0, PortThread_246, NULL, 0, &thread->id);
	else if(port == 247)	thread->handle = CreateThread(NULL, 0, PortThread_247, NULL, 0, &thread->id);
	else if(port == 248)	thread->handle = CreateThread(NULL, 0, PortThread_248, NULL, 0, &thread->id);
	else if(port == 249)	thread->handle = CreateThread(NULL, 0, PortThread_249, NULL, 0, &thread->id);
	else if(port == 250)	thread->handle = CreateThread(NULL, 0, PortThread_250, NULL, 0, &thread->id);
	else if(port == 251)	thread->handle = CreateThread(NULL, 0, PortThread_251, NULL, 0, &thread->id);
	else if(port == 252)	thread->handle = CreateThread(NULL, 0, PortThread_252, NULL, 0, &thread->id);
	else if(port == 253)	thread->handle = CreateThread(NULL, 0, PortThread_253, NULL, 0, &thread->id);
	else if(port == 254)	thread->handle = CreateThread(NULL, 0, PortThread_254, NULL, 0, &thread->id);
	else if(port == 255)	thread->handle = CreateThread(NULL, 0, PortThread_255, NULL, 0, &thread->id);

	else;
}

void PortThreadInit(int port)
{
	GLOBAL_PORT_STRUCT *pt = &portBuf[port];

	if(pt->bActiveFlag == 0)		return;	// 2007.9.19 추가
	if(pt->bActiveThread == OFF)	return;

	THREAD_PORT_STRUCT *thread;

	thread = &pt->threadInfo;

	if(port >= 0 && port <= 49) {
		Init_000_049(port, thread);
	}
	else if(port >= 50 && port <= 99) {
		Init_050_099(port, thread);
	}
	else if(port >= 100 && port <= 149) {
		Init_100_149(port, thread);
	}
	else if(port >= 150 && port <= 199) {
		Init_150_199(port, thread);
	}
	else if(port >= 200 && port <= 255) {
		Init_200_255(port, thread);
	}
	else {
		thread->handle = CreateThread(NULL, 0, PortThread_CommonNew, pt, 0, &thread->id);
	}
}

void PortThreadUnInit(int port)
{
	THREAD_PORT_STRUCT *thread;

	thread = &portBuf[port].threadInfo;

	if(thread->handle == NULL)	return;

	thread->bDo = OFF;

	TimeOutClass timeout;
	while(thread->bEnd == OFF) {
		Sleep(1);
		if(timeout.IsTimeOut(30))	break;
	}

	thread->handle = NULL;
	thread->id = 0;
}*/

/* 256개 이하 전용
typedef struct {
	HANDLE	handle;
	DWORD	id;
	char	bEnd;
	char	bDo;
} THREAD_PORT_STRUCT;

static THREAD_PORT_STRUCT threadPort[MAX_PORT];

int CommStatusLocalOne(GLOBAL_PORT_STRUCT *pt);
void WatchDogPlcScanReset(int port);

static void PortThread_Common(int port)
{
	if(CoInitialize(NULL) != S_OK) {
		bell();	
	}

	THREAD_PORT_STRUCT *thread = &threadPort[port];
	int sleep_cycle = 1;

	sleep_cycle = portBuf[port].nThreadCycle;

	if(sleep_cycle < 1)	sleep_cycle = 1;

	thread->bDo  = ON;
	thread->bEnd = OFF;

	while(thread->bDo) {
		WatchDogPlcScanReset(port);
		Sleep(sleep_cycle);
		CommStatusLocalOne(&portBuf[port]);
	}

	thread->bEnd = ON;

	CoUninitialize();
}

static DWORD WINAPI PortThread_0(LPVOID)	{	PortThread_Common(0);	return 0;	}
static DWORD WINAPI PortThread_1(LPVOID)	{	PortThread_Common(1);	return 0;	}
static DWORD WINAPI PortThread_2(LPVOID)	{	PortThread_Common(2);	return 0;	}
static DWORD WINAPI PortThread_3(LPVOID)	{	PortThread_Common(3);	return 0;	}
static DWORD WINAPI PortThread_4(LPVOID)	{	PortThread_Common(4);	return 0;	}
static DWORD WINAPI PortThread_5(LPVOID)	{	PortThread_Common(5);	return 0;	}
static DWORD WINAPI PortThread_6(LPVOID)	{	PortThread_Common(6);	return 0;	}
static DWORD WINAPI PortThread_7(LPVOID)	{	PortThread_Common(7);	return 0;	}
static DWORD WINAPI PortThread_8(LPVOID)	{	PortThread_Common(8);	return 0;	}
static DWORD WINAPI PortThread_9(LPVOID)	{	PortThread_Common(9);	return 0;	}
static DWORD WINAPI PortThread_10(LPVOID)	{	PortThread_Common(10);	return 0;	}
static DWORD WINAPI PortThread_11(LPVOID)	{	PortThread_Common(11);	return 0;	}
static DWORD WINAPI PortThread_12(LPVOID)	{	PortThread_Common(12);	return 0;	}
static DWORD WINAPI PortThread_13(LPVOID)	{	PortThread_Common(13);	return 0;	}
static DWORD WINAPI PortThread_14(LPVOID)	{	PortThread_Common(14);	return 0;	}
static DWORD WINAPI PortThread_15(LPVOID)	{	PortThread_Common(15);	return 0;	}
static DWORD WINAPI PortThread_16(LPVOID)	{	PortThread_Common(16);	return 0;	}
static DWORD WINAPI PortThread_17(LPVOID)	{	PortThread_Common(17);	return 0;	}
static DWORD WINAPI PortThread_18(LPVOID)	{	PortThread_Common(18);	return 0;	}
static DWORD WINAPI PortThread_19(LPVOID)	{	PortThread_Common(19);	return 0;	}
static DWORD WINAPI PortThread_20(LPVOID)	{	PortThread_Common(20);	return 0;	}
static DWORD WINAPI PortThread_21(LPVOID)	{	PortThread_Common(21);	return 0;	}
static DWORD WINAPI PortThread_22(LPVOID)	{	PortThread_Common(22);	return 0;	}
static DWORD WINAPI PortThread_23(LPVOID)	{	PortThread_Common(23);	return 0;	}
static DWORD WINAPI PortThread_24(LPVOID)	{	PortThread_Common(24);	return 0;	}
static DWORD WINAPI PortThread_25(LPVOID)	{	PortThread_Common(25);	return 0;	}
static DWORD WINAPI PortThread_26(LPVOID)	{	PortThread_Common(26);	return 0;	}
static DWORD WINAPI PortThread_27(LPVOID)	{	PortThread_Common(27);	return 0;	}
static DWORD WINAPI PortThread_28(LPVOID)	{	PortThread_Common(28);	return 0;	}
static DWORD WINAPI PortThread_29(LPVOID)	{	PortThread_Common(29);	return 0;	}
static DWORD WINAPI PortThread_30(LPVOID)	{	PortThread_Common(30);	return 0;	}
static DWORD WINAPI PortThread_31(LPVOID)	{	PortThread_Common(31);	return 0;	}
static DWORD WINAPI PortThread_32(LPVOID)	{	PortThread_Common(32);	return 0;	}
static DWORD WINAPI PortThread_33(LPVOID)	{	PortThread_Common(33);	return 0;	}
static DWORD WINAPI PortThread_34(LPVOID)	{	PortThread_Common(34);	return 0;	}
static DWORD WINAPI PortThread_35(LPVOID)	{	PortThread_Common(35);	return 0;	}
static DWORD WINAPI PortThread_36(LPVOID)	{	PortThread_Common(36);	return 0;	}
static DWORD WINAPI PortThread_37(LPVOID)	{	PortThread_Common(37);	return 0;	}
static DWORD WINAPI PortThread_38(LPVOID)	{	PortThread_Common(38);	return 0;	}
static DWORD WINAPI PortThread_39(LPVOID)	{	PortThread_Common(39);	return 0;	}
static DWORD WINAPI PortThread_40(LPVOID)	{	PortThread_Common(40);	return 0;	}
static DWORD WINAPI PortThread_41(LPVOID)	{	PortThread_Common(41);	return 0;	}
static DWORD WINAPI PortThread_42(LPVOID)	{	PortThread_Common(42);	return 0;	}
static DWORD WINAPI PortThread_43(LPVOID)	{	PortThread_Common(43);	return 0;	}
static DWORD WINAPI PortThread_44(LPVOID)	{	PortThread_Common(44);	return 0;	}
static DWORD WINAPI PortThread_45(LPVOID)	{	PortThread_Common(45);	return 0;	}
static DWORD WINAPI PortThread_46(LPVOID)	{	PortThread_Common(46);	return 0;	}
static DWORD WINAPI PortThread_47(LPVOID)	{	PortThread_Common(47);	return 0;	}
static DWORD WINAPI PortThread_48(LPVOID)	{	PortThread_Common(48);	return 0;	}
static DWORD WINAPI PortThread_49(LPVOID)	{	PortThread_Common(49);	return 0;	}
static DWORD WINAPI PortThread_50(LPVOID)	{	PortThread_Common(50);	return 0;	}
static DWORD WINAPI PortThread_51(LPVOID)	{	PortThread_Common(51);	return 0;	}
static DWORD WINAPI PortThread_52(LPVOID)	{	PortThread_Common(52);	return 0;	}
static DWORD WINAPI PortThread_53(LPVOID)	{	PortThread_Common(53);	return 0;	}
static DWORD WINAPI PortThread_54(LPVOID)	{	PortThread_Common(54);	return 0;	}
static DWORD WINAPI PortThread_55(LPVOID)	{	PortThread_Common(55);	return 0;	}
static DWORD WINAPI PortThread_56(LPVOID)	{	PortThread_Common(56);	return 0;	}
static DWORD WINAPI PortThread_57(LPVOID)	{	PortThread_Common(57);	return 0;	}
static DWORD WINAPI PortThread_58(LPVOID)	{	PortThread_Common(58);	return 0;	}
static DWORD WINAPI PortThread_59(LPVOID)	{	PortThread_Common(59);	return 0;	}
static DWORD WINAPI PortThread_60(LPVOID)	{	PortThread_Common(60);	return 0;	}
static DWORD WINAPI PortThread_61(LPVOID)	{	PortThread_Common(61);	return 0;	}
static DWORD WINAPI PortThread_62(LPVOID)	{	PortThread_Common(62);	return 0;	}
static DWORD WINAPI PortThread_63(LPVOID)	{	PortThread_Common(63);	return 0;	}
static DWORD WINAPI PortThread_64(LPVOID)	{	PortThread_Common(64);	return 0;	}
static DWORD WINAPI PortThread_65(LPVOID)	{	PortThread_Common(65);	return 0;	}
static DWORD WINAPI PortThread_66(LPVOID)	{	PortThread_Common(66);	return 0;	}
static DWORD WINAPI PortThread_67(LPVOID)	{	PortThread_Common(67);	return 0;	}
static DWORD WINAPI PortThread_68(LPVOID)	{	PortThread_Common(68);	return 0;	}
static DWORD WINAPI PortThread_69(LPVOID)	{	PortThread_Common(69);	return 0;	}
static DWORD WINAPI PortThread_70(LPVOID)	{	PortThread_Common(70);	return 0;	}
static DWORD WINAPI PortThread_71(LPVOID)	{	PortThread_Common(71);	return 0;	}
static DWORD WINAPI PortThread_72(LPVOID)	{	PortThread_Common(72);	return 0;	}
static DWORD WINAPI PortThread_73(LPVOID)	{	PortThread_Common(73);	return 0;	}
static DWORD WINAPI PortThread_74(LPVOID)	{	PortThread_Common(74);	return 0;	}
static DWORD WINAPI PortThread_75(LPVOID)	{	PortThread_Common(75);	return 0;	}
static DWORD WINAPI PortThread_76(LPVOID)	{	PortThread_Common(76);	return 0;	}
static DWORD WINAPI PortThread_77(LPVOID)	{	PortThread_Common(77);	return 0;	}
static DWORD WINAPI PortThread_78(LPVOID)	{	PortThread_Common(78);	return 0;	}
static DWORD WINAPI PortThread_79(LPVOID)	{	PortThread_Common(79);	return 0;	}
static DWORD WINAPI PortThread_80(LPVOID)	{	PortThread_Common(80);	return 0;	}
static DWORD WINAPI PortThread_81(LPVOID)	{	PortThread_Common(81);	return 0;	}
static DWORD WINAPI PortThread_82(LPVOID)	{	PortThread_Common(82);	return 0;	}
static DWORD WINAPI PortThread_83(LPVOID)	{	PortThread_Common(83);	return 0;	}
static DWORD WINAPI PortThread_84(LPVOID)	{	PortThread_Common(84);	return 0;	}
static DWORD WINAPI PortThread_85(LPVOID)	{	PortThread_Common(85);	return 0;	}
static DWORD WINAPI PortThread_86(LPVOID)	{	PortThread_Common(86);	return 0;	}
static DWORD WINAPI PortThread_87(LPVOID)	{	PortThread_Common(87);	return 0;	}
static DWORD WINAPI PortThread_88(LPVOID)	{	PortThread_Common(88);	return 0;	}
static DWORD WINAPI PortThread_89(LPVOID)	{	PortThread_Common(89);	return 0;	}
static DWORD WINAPI PortThread_90(LPVOID)	{	PortThread_Common(90);	return 0;	}
static DWORD WINAPI PortThread_91(LPVOID)	{	PortThread_Common(91);	return 0;	}
static DWORD WINAPI PortThread_92(LPVOID)	{	PortThread_Common(92);	return 0;	}
static DWORD WINAPI PortThread_93(LPVOID)	{	PortThread_Common(93);	return 0;	}
static DWORD WINAPI PortThread_94(LPVOID)	{	PortThread_Common(94);	return 0;	}
static DWORD WINAPI PortThread_95(LPVOID)	{	PortThread_Common(95);	return 0;	}
static DWORD WINAPI PortThread_96(LPVOID)	{	PortThread_Common(96);	return 0;	}
static DWORD WINAPI PortThread_97(LPVOID)	{	PortThread_Common(97);	return 0;	}
static DWORD WINAPI PortThread_98(LPVOID)	{	PortThread_Common(98);	return 0;	}
static DWORD WINAPI PortThread_99(LPVOID)	{	PortThread_Common(99);	return 0;	}

static DWORD WINAPI PortThread_100(LPVOID)	{	PortThread_Common(100);	return 0;	}
static DWORD WINAPI PortThread_101(LPVOID)	{	PortThread_Common(101);	return 0;	}
static DWORD WINAPI PortThread_102(LPVOID)	{	PortThread_Common(102);	return 0;	}
static DWORD WINAPI PortThread_103(LPVOID)	{	PortThread_Common(103);	return 0;	}
static DWORD WINAPI PortThread_104(LPVOID)	{	PortThread_Common(104);	return 0;	}
static DWORD WINAPI PortThread_105(LPVOID)	{	PortThread_Common(105);	return 0;	}
static DWORD WINAPI PortThread_106(LPVOID)	{	PortThread_Common(106);	return 0;	}
static DWORD WINAPI PortThread_107(LPVOID)	{	PortThread_Common(107);	return 0;	}
static DWORD WINAPI PortThread_108(LPVOID)	{	PortThread_Common(108);	return 0;	}
static DWORD WINAPI PortThread_109(LPVOID)	{	PortThread_Common(109);	return 0;	}
static DWORD WINAPI PortThread_110(LPVOID)	{	PortThread_Common(110);	return 0;	}
static DWORD WINAPI PortThread_111(LPVOID)	{	PortThread_Common(111);	return 0;	}
static DWORD WINAPI PortThread_112(LPVOID)	{	PortThread_Common(112);	return 0;	}
static DWORD WINAPI PortThread_113(LPVOID)	{	PortThread_Common(113);	return 0;	}
static DWORD WINAPI PortThread_114(LPVOID)	{	PortThread_Common(114);	return 0;	}
static DWORD WINAPI PortThread_115(LPVOID)	{	PortThread_Common(115);	return 0;	}
static DWORD WINAPI PortThread_116(LPVOID)	{	PortThread_Common(116);	return 0;	}
static DWORD WINAPI PortThread_117(LPVOID)	{	PortThread_Common(117);	return 0;	}
static DWORD WINAPI PortThread_118(LPVOID)	{	PortThread_Common(118);	return 0;	}
static DWORD WINAPI PortThread_119(LPVOID)	{	PortThread_Common(119);	return 0;	}
static DWORD WINAPI PortThread_120(LPVOID)	{	PortThread_Common(120);	return 0;	}
static DWORD WINAPI PortThread_121(LPVOID)	{	PortThread_Common(121);	return 0;	}
static DWORD WINAPI PortThread_122(LPVOID)	{	PortThread_Common(122);	return 0;	}
static DWORD WINAPI PortThread_123(LPVOID)	{	PortThread_Common(123);	return 0;	}
static DWORD WINAPI PortThread_124(LPVOID)	{	PortThread_Common(124);	return 0;	}
static DWORD WINAPI PortThread_125(LPVOID)	{	PortThread_Common(125);	return 0;	}
static DWORD WINAPI PortThread_126(LPVOID)	{	PortThread_Common(126);	return 0;	}
static DWORD WINAPI PortThread_127(LPVOID)	{	PortThread_Common(127);	return 0;	}
static DWORD WINAPI PortThread_128(LPVOID)	{	PortThread_Common(128);	return 0;	}
static DWORD WINAPI PortThread_129(LPVOID)	{	PortThread_Common(129);	return 0;	}
static DWORD WINAPI PortThread_130(LPVOID)	{	PortThread_Common(130);	return 0;	}
static DWORD WINAPI PortThread_131(LPVOID)	{	PortThread_Common(131);	return 0;	}
static DWORD WINAPI PortThread_132(LPVOID)	{	PortThread_Common(132);	return 0;	}
static DWORD WINAPI PortThread_133(LPVOID)	{	PortThread_Common(133);	return 0;	}
static DWORD WINAPI PortThread_134(LPVOID)	{	PortThread_Common(134);	return 0;	}
static DWORD WINAPI PortThread_135(LPVOID)	{	PortThread_Common(135);	return 0;	}
static DWORD WINAPI PortThread_136(LPVOID)	{	PortThread_Common(136);	return 0;	}
static DWORD WINAPI PortThread_137(LPVOID)	{	PortThread_Common(137);	return 0;	}
static DWORD WINAPI PortThread_138(LPVOID)	{	PortThread_Common(138);	return 0;	}
static DWORD WINAPI PortThread_139(LPVOID)	{	PortThread_Common(139);	return 0;	}
static DWORD WINAPI PortThread_140(LPVOID)	{	PortThread_Common(140);	return 0;	}
static DWORD WINAPI PortThread_141(LPVOID)	{	PortThread_Common(141);	return 0;	}
static DWORD WINAPI PortThread_142(LPVOID)	{	PortThread_Common(142);	return 0;	}
static DWORD WINAPI PortThread_143(LPVOID)	{	PortThread_Common(143);	return 0;	}
static DWORD WINAPI PortThread_144(LPVOID)	{	PortThread_Common(144);	return 0;	}
static DWORD WINAPI PortThread_145(LPVOID)	{	PortThread_Common(145);	return 0;	}
static DWORD WINAPI PortThread_146(LPVOID)	{	PortThread_Common(146);	return 0;	}
static DWORD WINAPI PortThread_147(LPVOID)	{	PortThread_Common(147);	return 0;	}
static DWORD WINAPI PortThread_148(LPVOID)	{	PortThread_Common(148);	return 0;	}
static DWORD WINAPI PortThread_149(LPVOID)	{	PortThread_Common(149);	return 0;	}
static DWORD WINAPI PortThread_150(LPVOID)	{	PortThread_Common(150);	return 0;	}
static DWORD WINAPI PortThread_151(LPVOID)	{	PortThread_Common(151);	return 0;	}
static DWORD WINAPI PortThread_152(LPVOID)	{	PortThread_Common(152);	return 0;	}
static DWORD WINAPI PortThread_153(LPVOID)	{	PortThread_Common(153);	return 0;	}
static DWORD WINAPI PortThread_154(LPVOID)	{	PortThread_Common(154);	return 0;	}
static DWORD WINAPI PortThread_155(LPVOID)	{	PortThread_Common(155);	return 0;	}
static DWORD WINAPI PortThread_156(LPVOID)	{	PortThread_Common(156);	return 0;	}
static DWORD WINAPI PortThread_157(LPVOID)	{	PortThread_Common(157);	return 0;	}
static DWORD WINAPI PortThread_158(LPVOID)	{	PortThread_Common(158);	return 0;	}
static DWORD WINAPI PortThread_159(LPVOID)	{	PortThread_Common(159);	return 0;	}
static DWORD WINAPI PortThread_160(LPVOID)	{	PortThread_Common(160);	return 0;	}
static DWORD WINAPI PortThread_161(LPVOID)	{	PortThread_Common(161);	return 0;	}
static DWORD WINAPI PortThread_162(LPVOID)	{	PortThread_Common(162);	return 0;	}
static DWORD WINAPI PortThread_163(LPVOID)	{	PortThread_Common(163);	return 0;	}
static DWORD WINAPI PortThread_164(LPVOID)	{	PortThread_Common(164);	return 0;	}
static DWORD WINAPI PortThread_165(LPVOID)	{	PortThread_Common(165);	return 0;	}
static DWORD WINAPI PortThread_166(LPVOID)	{	PortThread_Common(166);	return 0;	}
static DWORD WINAPI PortThread_167(LPVOID)	{	PortThread_Common(167);	return 0;	}
static DWORD WINAPI PortThread_168(LPVOID)	{	PortThread_Common(168);	return 0;	}
static DWORD WINAPI PortThread_169(LPVOID)	{	PortThread_Common(169);	return 0;	}
static DWORD WINAPI PortThread_170(LPVOID)	{	PortThread_Common(170);	return 0;	}
static DWORD WINAPI PortThread_171(LPVOID)	{	PortThread_Common(171);	return 0;	}
static DWORD WINAPI PortThread_172(LPVOID)	{	PortThread_Common(172);	return 0;	}
static DWORD WINAPI PortThread_173(LPVOID)	{	PortThread_Common(173);	return 0;	}
static DWORD WINAPI PortThread_174(LPVOID)	{	PortThread_Common(174);	return 0;	}
static DWORD WINAPI PortThread_175(LPVOID)	{	PortThread_Common(175);	return 0;	}
static DWORD WINAPI PortThread_176(LPVOID)	{	PortThread_Common(176);	return 0;	}
static DWORD WINAPI PortThread_177(LPVOID)	{	PortThread_Common(177);	return 0;	}
static DWORD WINAPI PortThread_178(LPVOID)	{	PortThread_Common(178);	return 0;	}
static DWORD WINAPI PortThread_179(LPVOID)	{	PortThread_Common(179);	return 0;	}
static DWORD WINAPI PortThread_180(LPVOID)	{	PortThread_Common(180);	return 0;	}
static DWORD WINAPI PortThread_181(LPVOID)	{	PortThread_Common(181);	return 0;	}
static DWORD WINAPI PortThread_182(LPVOID)	{	PortThread_Common(182);	return 0;	}
static DWORD WINAPI PortThread_183(LPVOID)	{	PortThread_Common(183);	return 0;	}
static DWORD WINAPI PortThread_184(LPVOID)	{	PortThread_Common(184);	return 0;	}
static DWORD WINAPI PortThread_185(LPVOID)	{	PortThread_Common(185);	return 0;	}
static DWORD WINAPI PortThread_186(LPVOID)	{	PortThread_Common(186);	return 0;	}
static DWORD WINAPI PortThread_187(LPVOID)	{	PortThread_Common(187);	return 0;	}
static DWORD WINAPI PortThread_188(LPVOID)	{	PortThread_Common(188);	return 0;	}
static DWORD WINAPI PortThread_189(LPVOID)	{	PortThread_Common(189);	return 0;	}
static DWORD WINAPI PortThread_190(LPVOID)	{	PortThread_Common(190);	return 0;	}
static DWORD WINAPI PortThread_191(LPVOID)	{	PortThread_Common(191);	return 0;	}
static DWORD WINAPI PortThread_192(LPVOID)	{	PortThread_Common(192);	return 0;	}
static DWORD WINAPI PortThread_193(LPVOID)	{	PortThread_Common(193);	return 0;	}
static DWORD WINAPI PortThread_194(LPVOID)	{	PortThread_Common(194);	return 0;	}
static DWORD WINAPI PortThread_195(LPVOID)	{	PortThread_Common(195);	return 0;	}
static DWORD WINAPI PortThread_196(LPVOID)	{	PortThread_Common(196);	return 0;	}
static DWORD WINAPI PortThread_197(LPVOID)	{	PortThread_Common(197);	return 0;	}
static DWORD WINAPI PortThread_198(LPVOID)	{	PortThread_Common(198);	return 0;	}
static DWORD WINAPI PortThread_199(LPVOID)	{	PortThread_Common(199);	return 0;	}

static DWORD WINAPI PortThread_200(LPVOID)	{	PortThread_Common(200);	return 0;	}
static DWORD WINAPI PortThread_201(LPVOID)	{	PortThread_Common(201);	return 0;	}
static DWORD WINAPI PortThread_202(LPVOID)	{	PortThread_Common(202);	return 0;	}
static DWORD WINAPI PortThread_203(LPVOID)	{	PortThread_Common(203);	return 0;	}
static DWORD WINAPI PortThread_204(LPVOID)	{	PortThread_Common(204);	return 0;	}
static DWORD WINAPI PortThread_205(LPVOID)	{	PortThread_Common(205);	return 0;	}
static DWORD WINAPI PortThread_206(LPVOID)	{	PortThread_Common(206);	return 0;	}
static DWORD WINAPI PortThread_207(LPVOID)	{	PortThread_Common(207);	return 0;	}
static DWORD WINAPI PortThread_208(LPVOID)	{	PortThread_Common(208);	return 0;	}
static DWORD WINAPI PortThread_209(LPVOID)	{	PortThread_Common(209);	return 0;	}
static DWORD WINAPI PortThread_210(LPVOID)	{	PortThread_Common(210);	return 0;	}
static DWORD WINAPI PortThread_211(LPVOID)	{	PortThread_Common(211);	return 0;	}
static DWORD WINAPI PortThread_212(LPVOID)	{	PortThread_Common(212);	return 0;	}
static DWORD WINAPI PortThread_213(LPVOID)	{	PortThread_Common(213);	return 0;	}
static DWORD WINAPI PortThread_214(LPVOID)	{	PortThread_Common(214);	return 0;	}
static DWORD WINAPI PortThread_215(LPVOID)	{	PortThread_Common(215);	return 0;	}
static DWORD WINAPI PortThread_216(LPVOID)	{	PortThread_Common(216);	return 0;	}
static DWORD WINAPI PortThread_217(LPVOID)	{	PortThread_Common(217);	return 0;	}
static DWORD WINAPI PortThread_218(LPVOID)	{	PortThread_Common(218);	return 0;	}
static DWORD WINAPI PortThread_219(LPVOID)	{	PortThread_Common(219);	return 0;	}
static DWORD WINAPI PortThread_220(LPVOID)	{	PortThread_Common(220);	return 0;	}
static DWORD WINAPI PortThread_221(LPVOID)	{	PortThread_Common(221);	return 0;	}
static DWORD WINAPI PortThread_222(LPVOID)	{	PortThread_Common(222);	return 0;	}
static DWORD WINAPI PortThread_223(LPVOID)	{	PortThread_Common(223);	return 0;	}
static DWORD WINAPI PortThread_224(LPVOID)	{	PortThread_Common(224);	return 0;	}
static DWORD WINAPI PortThread_225(LPVOID)	{	PortThread_Common(225);	return 0;	}
static DWORD WINAPI PortThread_226(LPVOID)	{	PortThread_Common(226);	return 0;	}
static DWORD WINAPI PortThread_227(LPVOID)	{	PortThread_Common(227);	return 0;	}
static DWORD WINAPI PortThread_228(LPVOID)	{	PortThread_Common(228);	return 0;	}
static DWORD WINAPI PortThread_229(LPVOID)	{	PortThread_Common(229);	return 0;	}
static DWORD WINAPI PortThread_230(LPVOID)	{	PortThread_Common(230);	return 0;	}
static DWORD WINAPI PortThread_231(LPVOID)	{	PortThread_Common(231);	return 0;	}
static DWORD WINAPI PortThread_232(LPVOID)	{	PortThread_Common(232);	return 0;	}
static DWORD WINAPI PortThread_233(LPVOID)	{	PortThread_Common(233);	return 0;	}
static DWORD WINAPI PortThread_234(LPVOID)	{	PortThread_Common(234);	return 0;	}
static DWORD WINAPI PortThread_235(LPVOID)	{	PortThread_Common(235);	return 0;	}
static DWORD WINAPI PortThread_236(LPVOID)	{	PortThread_Common(236);	return 0;	}
static DWORD WINAPI PortThread_237(LPVOID)	{	PortThread_Common(237);	return 0;	}
static DWORD WINAPI PortThread_238(LPVOID)	{	PortThread_Common(238);	return 0;	}
static DWORD WINAPI PortThread_239(LPVOID)	{	PortThread_Common(239);	return 0;	}
static DWORD WINAPI PortThread_240(LPVOID)	{	PortThread_Common(240);	return 0;	}
static DWORD WINAPI PortThread_241(LPVOID)	{	PortThread_Common(241);	return 0;	}
static DWORD WINAPI PortThread_242(LPVOID)	{	PortThread_Common(242);	return 0;	}
static DWORD WINAPI PortThread_243(LPVOID)	{	PortThread_Common(243);	return 0;	}
static DWORD WINAPI PortThread_244(LPVOID)	{	PortThread_Common(244);	return 0;	}
static DWORD WINAPI PortThread_245(LPVOID)	{	PortThread_Common(245);	return 0;	}
static DWORD WINAPI PortThread_246(LPVOID)	{	PortThread_Common(246);	return 0;	}
static DWORD WINAPI PortThread_247(LPVOID)	{	PortThread_Common(247);	return 0;	}
static DWORD WINAPI PortThread_248(LPVOID)	{	PortThread_Common(248);	return 0;	}
static DWORD WINAPI PortThread_249(LPVOID)	{	PortThread_Common(249);	return 0;	}
static DWORD WINAPI PortThread_250(LPVOID)	{	PortThread_Common(250);	return 0;	}
static DWORD WINAPI PortThread_251(LPVOID)	{	PortThread_Common(251);	return 0;	}
static DWORD WINAPI PortThread_252(LPVOID)	{	PortThread_Common(252);	return 0;	}
static DWORD WINAPI PortThread_253(LPVOID)	{	PortThread_Common(253);	return 0;	}
static DWORD WINAPI PortThread_254(LPVOID)	{	PortThread_Common(254);	return 0;	}
static DWORD WINAPI PortThread_255(LPVOID)	{	PortThread_Common(255);	return 0;	}

static void Init_000_049(int port, THREAD_PORT_STRUCT *thread)
{
	if(port == 0)		thread->handle = CreateThread(NULL, 0, PortThread_0, NULL, 0, &thread->id);
	else if(port == 1)	thread->handle = CreateThread(NULL, 0, PortThread_1, NULL, 0, &thread->id);
	else if(port == 2)	thread->handle = CreateThread(NULL, 0, PortThread_2, NULL, 0, &thread->id);
	else if(port == 3)	thread->handle = CreateThread(NULL, 0, PortThread_3, NULL, 0, &thread->id);
	else if(port == 4)	thread->handle = CreateThread(NULL, 0, PortThread_4, NULL, 0, &thread->id);
	else if(port == 5)	thread->handle = CreateThread(NULL, 0, PortThread_5, NULL, 0, &thread->id);
	else if(port == 6)	thread->handle = CreateThread(NULL, 0, PortThread_6, NULL, 0, &thread->id);
	else if(port == 7)	thread->handle = CreateThread(NULL, 0, PortThread_7, NULL, 0, &thread->id);
	else if(port == 8)	thread->handle = CreateThread(NULL, 0, PortThread_8, NULL, 0, &thread->id);
	else if(port == 9)	thread->handle = CreateThread(NULL, 0, PortThread_9, NULL, 0, &thread->id);
	else if(port == 10)	thread->handle = CreateThread(NULL, 0, PortThread_10, NULL, 0, &thread->id);
	else if(port == 11)	thread->handle = CreateThread(NULL, 0, PortThread_11, NULL, 0, &thread->id);
	else if(port == 12)	thread->handle = CreateThread(NULL, 0, PortThread_12, NULL, 0, &thread->id);
	else if(port == 13)	thread->handle = CreateThread(NULL, 0, PortThread_13, NULL, 0, &thread->id);
	else if(port == 14)	thread->handle = CreateThread(NULL, 0, PortThread_14, NULL, 0, &thread->id);
	else if(port == 15)	thread->handle = CreateThread(NULL, 0, PortThread_15, NULL, 0, &thread->id);
	else if(port == 16)	thread->handle = CreateThread(NULL, 0, PortThread_16, NULL, 0, &thread->id);
	else if(port == 17)	thread->handle = CreateThread(NULL, 0, PortThread_17, NULL, 0, &thread->id);
	else if(port == 18)	thread->handle = CreateThread(NULL, 0, PortThread_18, NULL, 0, &thread->id);
	else if(port == 19)	thread->handle = CreateThread(NULL, 0, PortThread_19, NULL, 0, &thread->id);
	else if(port == 20)	thread->handle = CreateThread(NULL, 0, PortThread_20, NULL, 0, &thread->id);
	else if(port == 21)	thread->handle = CreateThread(NULL, 0, PortThread_21, NULL, 0, &thread->id);
	else if(port == 22)	thread->handle = CreateThread(NULL, 0, PortThread_22, NULL, 0, &thread->id);
	else if(port == 23)	thread->handle = CreateThread(NULL, 0, PortThread_23, NULL, 0, &thread->id);
	else if(port == 24)	thread->handle = CreateThread(NULL, 0, PortThread_24, NULL, 0, &thread->id);
	else if(port == 25)	thread->handle = CreateThread(NULL, 0, PortThread_25, NULL, 0, &thread->id);
	else if(port == 26)	thread->handle = CreateThread(NULL, 0, PortThread_26, NULL, 0, &thread->id);
	else if(port == 27)	thread->handle = CreateThread(NULL, 0, PortThread_27, NULL, 0, &thread->id);
	else if(port == 28)	thread->handle = CreateThread(NULL, 0, PortThread_28, NULL, 0, &thread->id);
	else if(port == 29)	thread->handle = CreateThread(NULL, 0, PortThread_29, NULL, 0, &thread->id);
	else if(port == 30)	thread->handle = CreateThread(NULL, 0, PortThread_30, NULL, 0, &thread->id);
	else if(port == 31)	thread->handle = CreateThread(NULL, 0, PortThread_31, NULL, 0, &thread->id);
	else if(port == 32)	thread->handle = CreateThread(NULL, 0, PortThread_32, NULL, 0, &thread->id);
	else if(port == 33)	thread->handle = CreateThread(NULL, 0, PortThread_33, NULL, 0, &thread->id);
	else if(port == 34)	thread->handle = CreateThread(NULL, 0, PortThread_34, NULL, 0, &thread->id);
	else if(port == 35)	thread->handle = CreateThread(NULL, 0, PortThread_35, NULL, 0, &thread->id);
	else if(port == 36)	thread->handle = CreateThread(NULL, 0, PortThread_36, NULL, 0, &thread->id);
	else if(port == 37)	thread->handle = CreateThread(NULL, 0, PortThread_37, NULL, 0, &thread->id);
	else if(port == 38)	thread->handle = CreateThread(NULL, 0, PortThread_38, NULL, 0, &thread->id);
	else if(port == 39)	thread->handle = CreateThread(NULL, 0, PortThread_39, NULL, 0, &thread->id);
	else if(port == 40)	thread->handle = CreateThread(NULL, 0, PortThread_40, NULL, 0, &thread->id);
	else if(port == 41)	thread->handle = CreateThread(NULL, 0, PortThread_41, NULL, 0, &thread->id);
	else if(port == 42)	thread->handle = CreateThread(NULL, 0, PortThread_42, NULL, 0, &thread->id);
	else if(port == 43)	thread->handle = CreateThread(NULL, 0, PortThread_43, NULL, 0, &thread->id);
	else if(port == 44)	thread->handle = CreateThread(NULL, 0, PortThread_44, NULL, 0, &thread->id);
	else if(port == 45)	thread->handle = CreateThread(NULL, 0, PortThread_45, NULL, 0, &thread->id);
	else if(port == 46)	thread->handle = CreateThread(NULL, 0, PortThread_46, NULL, 0, &thread->id);
	else if(port == 47)	thread->handle = CreateThread(NULL, 0, PortThread_47, NULL, 0, &thread->id);
	else if(port == 48)	thread->handle = CreateThread(NULL, 0, PortThread_48, NULL, 0, &thread->id);
	else if(port == 49)	thread->handle = CreateThread(NULL, 0, PortThread_49, NULL, 0, &thread->id);

	else;
}

static void Init_050_099(int port, THREAD_PORT_STRUCT *thread)
{
	if(port == 50)		thread->handle = CreateThread(NULL, 0, PortThread_50, NULL, 0, &thread->id);
	else if(port == 51)	thread->handle = CreateThread(NULL, 0, PortThread_51, NULL, 0, &thread->id);
	else if(port == 52)	thread->handle = CreateThread(NULL, 0, PortThread_52, NULL, 0, &thread->id);
	else if(port == 53)	thread->handle = CreateThread(NULL, 0, PortThread_53, NULL, 0, &thread->id);
	else if(port == 54)	thread->handle = CreateThread(NULL, 0, PortThread_54, NULL, 0, &thread->id);
	else if(port == 55)	thread->handle = CreateThread(NULL, 0, PortThread_55, NULL, 0, &thread->id);
	else if(port == 56)	thread->handle = CreateThread(NULL, 0, PortThread_56, NULL, 0, &thread->id);
	else if(port == 57)	thread->handle = CreateThread(NULL, 0, PortThread_57, NULL, 0, &thread->id);
	else if(port == 58)	thread->handle = CreateThread(NULL, 0, PortThread_58, NULL, 0, &thread->id);
	else if(port == 59)	thread->handle = CreateThread(NULL, 0, PortThread_59, NULL, 0, &thread->id);
	else if(port == 60)	thread->handle = CreateThread(NULL, 0, PortThread_60, NULL, 0, &thread->id);
	else if(port == 61)	thread->handle = CreateThread(NULL, 0, PortThread_61, NULL, 0, &thread->id);
	else if(port == 62)	thread->handle = CreateThread(NULL, 0, PortThread_62, NULL, 0, &thread->id);
	else if(port == 63)	thread->handle = CreateThread(NULL, 0, PortThread_63, NULL, 0, &thread->id);
	else if(port == 64)	thread->handle = CreateThread(NULL, 0, PortThread_64, NULL, 0, &thread->id);
	else if(port == 65)	thread->handle = CreateThread(NULL, 0, PortThread_65, NULL, 0, &thread->id);
	else if(port == 66)	thread->handle = CreateThread(NULL, 0, PortThread_66, NULL, 0, &thread->id);
	else if(port == 67)	thread->handle = CreateThread(NULL, 0, PortThread_67, NULL, 0, &thread->id);
	else if(port == 68)	thread->handle = CreateThread(NULL, 0, PortThread_68, NULL, 0, &thread->id);
	else if(port == 69)	thread->handle = CreateThread(NULL, 0, PortThread_69, NULL, 0, &thread->id);
	else if(port == 70)	thread->handle = CreateThread(NULL, 0, PortThread_70, NULL, 0, &thread->id);
	else if(port == 71)	thread->handle = CreateThread(NULL, 0, PortThread_71, NULL, 0, &thread->id);
	else if(port == 72)	thread->handle = CreateThread(NULL, 0, PortThread_72, NULL, 0, &thread->id);
	else if(port == 73)	thread->handle = CreateThread(NULL, 0, PortThread_73, NULL, 0, &thread->id);
	else if(port == 74)	thread->handle = CreateThread(NULL, 0, PortThread_74, NULL, 0, &thread->id);
	else if(port == 75)	thread->handle = CreateThread(NULL, 0, PortThread_75, NULL, 0, &thread->id);
	else if(port == 76)	thread->handle = CreateThread(NULL, 0, PortThread_76, NULL, 0, &thread->id);
	else if(port == 77)	thread->handle = CreateThread(NULL, 0, PortThread_77, NULL, 0, &thread->id);
	else if(port == 78)	thread->handle = CreateThread(NULL, 0, PortThread_78, NULL, 0, &thread->id);
	else if(port == 79)	thread->handle = CreateThread(NULL, 0, PortThread_79, NULL, 0, &thread->id);
	else if(port == 80)	thread->handle = CreateThread(NULL, 0, PortThread_80, NULL, 0, &thread->id);
	else if(port == 81)	thread->handle = CreateThread(NULL, 0, PortThread_81, NULL, 0, &thread->id);
	else if(port == 82)	thread->handle = CreateThread(NULL, 0, PortThread_82, NULL, 0, &thread->id);
	else if(port == 83)	thread->handle = CreateThread(NULL, 0, PortThread_83, NULL, 0, &thread->id);
	else if(port == 84)	thread->handle = CreateThread(NULL, 0, PortThread_84, NULL, 0, &thread->id);
	else if(port == 85)	thread->handle = CreateThread(NULL, 0, PortThread_85, NULL, 0, &thread->id);
	else if(port == 86)	thread->handle = CreateThread(NULL, 0, PortThread_86, NULL, 0, &thread->id);
	else if(port == 87)	thread->handle = CreateThread(NULL, 0, PortThread_87, NULL, 0, &thread->id);
	else if(port == 88)	thread->handle = CreateThread(NULL, 0, PortThread_88, NULL, 0, &thread->id);
	else if(port == 89)	thread->handle = CreateThread(NULL, 0, PortThread_89, NULL, 0, &thread->id);
	else if(port == 90)	thread->handle = CreateThread(NULL, 0, PortThread_90, NULL, 0, &thread->id);
	else if(port == 91)	thread->handle = CreateThread(NULL, 0, PortThread_91, NULL, 0, &thread->id);
	else if(port == 92)	thread->handle = CreateThread(NULL, 0, PortThread_92, NULL, 0, &thread->id);
	else if(port == 93)	thread->handle = CreateThread(NULL, 0, PortThread_93, NULL, 0, &thread->id);
	else if(port == 94)	thread->handle = CreateThread(NULL, 0, PortThread_94, NULL, 0, &thread->id);
	else if(port == 95)	thread->handle = CreateThread(NULL, 0, PortThread_95, NULL, 0, &thread->id);
	else if(port == 96)	thread->handle = CreateThread(NULL, 0, PortThread_96, NULL, 0, &thread->id);
	else if(port == 97)	thread->handle = CreateThread(NULL, 0, PortThread_97, NULL, 0, &thread->id);
	else if(port == 98)	thread->handle = CreateThread(NULL, 0, PortThread_98, NULL, 0, &thread->id);
	else if(port == 99)	thread->handle = CreateThread(NULL, 0, PortThread_99, NULL, 0, &thread->id);

	else;
}

static void Init_100_149(int port, THREAD_PORT_STRUCT *thread)
{
	if(port == 100)			thread->handle = CreateThread(NULL, 0, PortThread_100, NULL, 0, &thread->id);
	else if(port == 101)	thread->handle = CreateThread(NULL, 0, PortThread_101, NULL, 0, &thread->id);
	else if(port == 102)	thread->handle = CreateThread(NULL, 0, PortThread_102, NULL, 0, &thread->id);
	else if(port == 103)	thread->handle = CreateThread(NULL, 0, PortThread_103, NULL, 0, &thread->id);
	else if(port == 104)	thread->handle = CreateThread(NULL, 0, PortThread_104, NULL, 0, &thread->id);
	else if(port == 105)	thread->handle = CreateThread(NULL, 0, PortThread_105, NULL, 0, &thread->id);
	else if(port == 106)	thread->handle = CreateThread(NULL, 0, PortThread_106, NULL, 0, &thread->id);
	else if(port == 107)	thread->handle = CreateThread(NULL, 0, PortThread_107, NULL, 0, &thread->id);
	else if(port == 108)	thread->handle = CreateThread(NULL, 0, PortThread_108, NULL, 0, &thread->id);
	else if(port == 109)	thread->handle = CreateThread(NULL, 0, PortThread_109, NULL, 0, &thread->id);
	else if(port == 110)	thread->handle = CreateThread(NULL, 0, PortThread_110, NULL, 0, &thread->id);
	else if(port == 111)	thread->handle = CreateThread(NULL, 0, PortThread_111, NULL, 0, &thread->id);
	else if(port == 112)	thread->handle = CreateThread(NULL, 0, PortThread_112, NULL, 0, &thread->id);
	else if(port == 113)	thread->handle = CreateThread(NULL, 0, PortThread_113, NULL, 0, &thread->id);
	else if(port == 114)	thread->handle = CreateThread(NULL, 0, PortThread_114, NULL, 0, &thread->id);
	else if(port == 115)	thread->handle = CreateThread(NULL, 0, PortThread_115, NULL, 0, &thread->id);
	else if(port == 116)	thread->handle = CreateThread(NULL, 0, PortThread_116, NULL, 0, &thread->id);
	else if(port == 117)	thread->handle = CreateThread(NULL, 0, PortThread_117, NULL, 0, &thread->id);
	else if(port == 118)	thread->handle = CreateThread(NULL, 0, PortThread_118, NULL, 0, &thread->id);
	else if(port == 119)	thread->handle = CreateThread(NULL, 0, PortThread_119, NULL, 0, &thread->id);
	else if(port == 120)	thread->handle = CreateThread(NULL, 0, PortThread_120, NULL, 0, &thread->id);
	else if(port == 121)	thread->handle = CreateThread(NULL, 0, PortThread_121, NULL, 0, &thread->id);
	else if(port == 122)	thread->handle = CreateThread(NULL, 0, PortThread_122, NULL, 0, &thread->id);
	else if(port == 123)	thread->handle = CreateThread(NULL, 0, PortThread_123, NULL, 0, &thread->id);
	else if(port == 124)	thread->handle = CreateThread(NULL, 0, PortThread_124, NULL, 0, &thread->id);
	else if(port == 125)	thread->handle = CreateThread(NULL, 0, PortThread_125, NULL, 0, &thread->id);
	else if(port == 126)	thread->handle = CreateThread(NULL, 0, PortThread_126, NULL, 0, &thread->id);
	else if(port == 127)	thread->handle = CreateThread(NULL, 0, PortThread_127, NULL, 0, &thread->id);
	else if(port == 128)	thread->handle = CreateThread(NULL, 0, PortThread_128, NULL, 0, &thread->id);
	else if(port == 129)	thread->handle = CreateThread(NULL, 0, PortThread_129, NULL, 0, &thread->id);
	else if(port == 130)	thread->handle = CreateThread(NULL, 0, PortThread_130, NULL, 0, &thread->id);
	else if(port == 131)	thread->handle = CreateThread(NULL, 0, PortThread_131, NULL, 0, &thread->id);
	else if(port == 132)	thread->handle = CreateThread(NULL, 0, PortThread_132, NULL, 0, &thread->id);
	else if(port == 133)	thread->handle = CreateThread(NULL, 0, PortThread_133, NULL, 0, &thread->id);
	else if(port == 134)	thread->handle = CreateThread(NULL, 0, PortThread_134, NULL, 0, &thread->id);
	else if(port == 135)	thread->handle = CreateThread(NULL, 0, PortThread_135, NULL, 0, &thread->id);
	else if(port == 136)	thread->handle = CreateThread(NULL, 0, PortThread_136, NULL, 0, &thread->id);
	else if(port == 137)	thread->handle = CreateThread(NULL, 0, PortThread_137, NULL, 0, &thread->id);
	else if(port == 138)	thread->handle = CreateThread(NULL, 0, PortThread_138, NULL, 0, &thread->id);
	else if(port == 139)	thread->handle = CreateThread(NULL, 0, PortThread_139, NULL, 0, &thread->id);
	else if(port == 140)	thread->handle = CreateThread(NULL, 0, PortThread_140, NULL, 0, &thread->id);
	else if(port == 141)	thread->handle = CreateThread(NULL, 0, PortThread_141, NULL, 0, &thread->id);
	else if(port == 142)	thread->handle = CreateThread(NULL, 0, PortThread_142, NULL, 0, &thread->id);
	else if(port == 143)	thread->handle = CreateThread(NULL, 0, PortThread_143, NULL, 0, &thread->id);
	else if(port == 144)	thread->handle = CreateThread(NULL, 0, PortThread_144, NULL, 0, &thread->id);
	else if(port == 145)	thread->handle = CreateThread(NULL, 0, PortThread_145, NULL, 0, &thread->id);
	else if(port == 146)	thread->handle = CreateThread(NULL, 0, PortThread_146, NULL, 0, &thread->id);
	else if(port == 147)	thread->handle = CreateThread(NULL, 0, PortThread_147, NULL, 0, &thread->id);
	else if(port == 148)	thread->handle = CreateThread(NULL, 0, PortThread_148, NULL, 0, &thread->id);
	else if(port == 149)	thread->handle = CreateThread(NULL, 0, PortThread_149, NULL, 0, &thread->id);

	else;
}

static void Init_150_199(int port, THREAD_PORT_STRUCT *thread)
{
	if(port == 150)			thread->handle = CreateThread(NULL, 0, PortThread_150, NULL, 0, &thread->id);
	else if(port == 151)	thread->handle = CreateThread(NULL, 0, PortThread_151, NULL, 0, &thread->id);
	else if(port == 152)	thread->handle = CreateThread(NULL, 0, PortThread_152, NULL, 0, &thread->id);
	else if(port == 153)	thread->handle = CreateThread(NULL, 0, PortThread_153, NULL, 0, &thread->id);
	else if(port == 154)	thread->handle = CreateThread(NULL, 0, PortThread_154, NULL, 0, &thread->id);
	else if(port == 155)	thread->handle = CreateThread(NULL, 0, PortThread_155, NULL, 0, &thread->id);
	else if(port == 156)	thread->handle = CreateThread(NULL, 0, PortThread_156, NULL, 0, &thread->id);
	else if(port == 157)	thread->handle = CreateThread(NULL, 0, PortThread_157, NULL, 0, &thread->id);
	else if(port == 158)	thread->handle = CreateThread(NULL, 0, PortThread_158, NULL, 0, &thread->id);
	else if(port == 159)	thread->handle = CreateThread(NULL, 0, PortThread_159, NULL, 0, &thread->id);
	else if(port == 160)	thread->handle = CreateThread(NULL, 0, PortThread_160, NULL, 0, &thread->id);
	else if(port == 161)	thread->handle = CreateThread(NULL, 0, PortThread_161, NULL, 0, &thread->id);
	else if(port == 162)	thread->handle = CreateThread(NULL, 0, PortThread_162, NULL, 0, &thread->id);
	else if(port == 163)	thread->handle = CreateThread(NULL, 0, PortThread_163, NULL, 0, &thread->id);
	else if(port == 164)	thread->handle = CreateThread(NULL, 0, PortThread_164, NULL, 0, &thread->id);
	else if(port == 165)	thread->handle = CreateThread(NULL, 0, PortThread_165, NULL, 0, &thread->id);
	else if(port == 166)	thread->handle = CreateThread(NULL, 0, PortThread_166, NULL, 0, &thread->id);
	else if(port == 167)	thread->handle = CreateThread(NULL, 0, PortThread_167, NULL, 0, &thread->id);
	else if(port == 168)	thread->handle = CreateThread(NULL, 0, PortThread_168, NULL, 0, &thread->id);
	else if(port == 169)	thread->handle = CreateThread(NULL, 0, PortThread_169, NULL, 0, &thread->id);
	else if(port == 170)	thread->handle = CreateThread(NULL, 0, PortThread_170, NULL, 0, &thread->id);
	else if(port == 171)	thread->handle = CreateThread(NULL, 0, PortThread_171, NULL, 0, &thread->id);
	else if(port == 172)	thread->handle = CreateThread(NULL, 0, PortThread_172, NULL, 0, &thread->id);
	else if(port == 173)	thread->handle = CreateThread(NULL, 0, PortThread_173, NULL, 0, &thread->id);
	else if(port == 174)	thread->handle = CreateThread(NULL, 0, PortThread_174, NULL, 0, &thread->id);
	else if(port == 175)	thread->handle = CreateThread(NULL, 0, PortThread_175, NULL, 0, &thread->id);
	else if(port == 176)	thread->handle = CreateThread(NULL, 0, PortThread_176, NULL, 0, &thread->id);
	else if(port == 177)	thread->handle = CreateThread(NULL, 0, PortThread_177, NULL, 0, &thread->id);
	else if(port == 178)	thread->handle = CreateThread(NULL, 0, PortThread_178, NULL, 0, &thread->id);
	else if(port == 179)	thread->handle = CreateThread(NULL, 0, PortThread_179, NULL, 0, &thread->id);
	else if(port == 180)	thread->handle = CreateThread(NULL, 0, PortThread_180, NULL, 0, &thread->id);
	else if(port == 181)	thread->handle = CreateThread(NULL, 0, PortThread_181, NULL, 0, &thread->id);
	else if(port == 182)	thread->handle = CreateThread(NULL, 0, PortThread_182, NULL, 0, &thread->id);
	else if(port == 183)	thread->handle = CreateThread(NULL, 0, PortThread_183, NULL, 0, &thread->id);
	else if(port == 184)	thread->handle = CreateThread(NULL, 0, PortThread_184, NULL, 0, &thread->id);
	else if(port == 185)	thread->handle = CreateThread(NULL, 0, PortThread_185, NULL, 0, &thread->id);
	else if(port == 186)	thread->handle = CreateThread(NULL, 0, PortThread_186, NULL, 0, &thread->id);
	else if(port == 187)	thread->handle = CreateThread(NULL, 0, PortThread_187, NULL, 0, &thread->id);
	else if(port == 188)	thread->handle = CreateThread(NULL, 0, PortThread_188, NULL, 0, &thread->id);
	else if(port == 189)	thread->handle = CreateThread(NULL, 0, PortThread_189, NULL, 0, &thread->id);
	else if(port == 190)	thread->handle = CreateThread(NULL, 0, PortThread_190, NULL, 0, &thread->id);
	else if(port == 191)	thread->handle = CreateThread(NULL, 0, PortThread_191, NULL, 0, &thread->id);
	else if(port == 192)	thread->handle = CreateThread(NULL, 0, PortThread_192, NULL, 0, &thread->id);
	else if(port == 193)	thread->handle = CreateThread(NULL, 0, PortThread_193, NULL, 0, &thread->id);
	else if(port == 194)	thread->handle = CreateThread(NULL, 0, PortThread_194, NULL, 0, &thread->id);
	else if(port == 195)	thread->handle = CreateThread(NULL, 0, PortThread_195, NULL, 0, &thread->id);
	else if(port == 196)	thread->handle = CreateThread(NULL, 0, PortThread_196, NULL, 0, &thread->id);
	else if(port == 197)	thread->handle = CreateThread(NULL, 0, PortThread_197, NULL, 0, &thread->id);
	else if(port == 198)	thread->handle = CreateThread(NULL, 0, PortThread_198, NULL, 0, &thread->id);
	else if(port == 199)	thread->handle = CreateThread(NULL, 0, PortThread_199, NULL, 0, &thread->id);

	else;
}

static void Init_200_255(int port, THREAD_PORT_STRUCT *thread)
{
	if(port == 200)			thread->handle = CreateThread(NULL, 0, PortThread_200, NULL, 0, &thread->id);
	else if(port == 201)	thread->handle = CreateThread(NULL, 0, PortThread_201, NULL, 0, &thread->id);
	else if(port == 202)	thread->handle = CreateThread(NULL, 0, PortThread_202, NULL, 0, &thread->id);
	else if(port == 203)	thread->handle = CreateThread(NULL, 0, PortThread_203, NULL, 0, &thread->id);
	else if(port == 204)	thread->handle = CreateThread(NULL, 0, PortThread_204, NULL, 0, &thread->id);
	else if(port == 205)	thread->handle = CreateThread(NULL, 0, PortThread_205, NULL, 0, &thread->id);
	else if(port == 206)	thread->handle = CreateThread(NULL, 0, PortThread_206, NULL, 0, &thread->id);
	else if(port == 207)	thread->handle = CreateThread(NULL, 0, PortThread_207, NULL, 0, &thread->id);
	else if(port == 208)	thread->handle = CreateThread(NULL, 0, PortThread_208, NULL, 0, &thread->id);
	else if(port == 209)	thread->handle = CreateThread(NULL, 0, PortThread_209, NULL, 0, &thread->id);
	else if(port == 210)	thread->handle = CreateThread(NULL, 0, PortThread_210, NULL, 0, &thread->id);
	else if(port == 211)	thread->handle = CreateThread(NULL, 0, PortThread_211, NULL, 0, &thread->id);
	else if(port == 212)	thread->handle = CreateThread(NULL, 0, PortThread_212, NULL, 0, &thread->id);
	else if(port == 213)	thread->handle = CreateThread(NULL, 0, PortThread_213, NULL, 0, &thread->id);
	else if(port == 214)	thread->handle = CreateThread(NULL, 0, PortThread_214, NULL, 0, &thread->id);
	else if(port == 215)	thread->handle = CreateThread(NULL, 0, PortThread_215, NULL, 0, &thread->id);
	else if(port == 216)	thread->handle = CreateThread(NULL, 0, PortThread_216, NULL, 0, &thread->id);
	else if(port == 217)	thread->handle = CreateThread(NULL, 0, PortThread_217, NULL, 0, &thread->id);
	else if(port == 218)	thread->handle = CreateThread(NULL, 0, PortThread_218, NULL, 0, &thread->id);
	else if(port == 219)	thread->handle = CreateThread(NULL, 0, PortThread_219, NULL, 0, &thread->id);
	else if(port == 220)	thread->handle = CreateThread(NULL, 0, PortThread_220, NULL, 0, &thread->id);
	else if(port == 221)	thread->handle = CreateThread(NULL, 0, PortThread_221, NULL, 0, &thread->id);
	else if(port == 222)	thread->handle = CreateThread(NULL, 0, PortThread_222, NULL, 0, &thread->id);
	else if(port == 223)	thread->handle = CreateThread(NULL, 0, PortThread_223, NULL, 0, &thread->id);
	else if(port == 224)	thread->handle = CreateThread(NULL, 0, PortThread_224, NULL, 0, &thread->id);
	else if(port == 225)	thread->handle = CreateThread(NULL, 0, PortThread_225, NULL, 0, &thread->id);
	else if(port == 226)	thread->handle = CreateThread(NULL, 0, PortThread_226, NULL, 0, &thread->id);
	else if(port == 227)	thread->handle = CreateThread(NULL, 0, PortThread_227, NULL, 0, &thread->id);
	else if(port == 228)	thread->handle = CreateThread(NULL, 0, PortThread_228, NULL, 0, &thread->id);
	else if(port == 229)	thread->handle = CreateThread(NULL, 0, PortThread_229, NULL, 0, &thread->id);
	else if(port == 230)	thread->handle = CreateThread(NULL, 0, PortThread_230, NULL, 0, &thread->id);
	else if(port == 231)	thread->handle = CreateThread(NULL, 0, PortThread_231, NULL, 0, &thread->id);
	else if(port == 232)	thread->handle = CreateThread(NULL, 0, PortThread_232, NULL, 0, &thread->id);
	else if(port == 233)	thread->handle = CreateThread(NULL, 0, PortThread_233, NULL, 0, &thread->id);
	else if(port == 234)	thread->handle = CreateThread(NULL, 0, PortThread_234, NULL, 0, &thread->id);
	else if(port == 235)	thread->handle = CreateThread(NULL, 0, PortThread_235, NULL, 0, &thread->id);
	else if(port == 236)	thread->handle = CreateThread(NULL, 0, PortThread_236, NULL, 0, &thread->id);
	else if(port == 237)	thread->handle = CreateThread(NULL, 0, PortThread_237, NULL, 0, &thread->id);
	else if(port == 238)	thread->handle = CreateThread(NULL, 0, PortThread_238, NULL, 0, &thread->id);
	else if(port == 239)	thread->handle = CreateThread(NULL, 0, PortThread_239, NULL, 0, &thread->id);
	else if(port == 240)	thread->handle = CreateThread(NULL, 0, PortThread_240, NULL, 0, &thread->id);
	else if(port == 241)	thread->handle = CreateThread(NULL, 0, PortThread_241, NULL, 0, &thread->id);
	else if(port == 242)	thread->handle = CreateThread(NULL, 0, PortThread_242, NULL, 0, &thread->id);
	else if(port == 243)	thread->handle = CreateThread(NULL, 0, PortThread_243, NULL, 0, &thread->id);
	else if(port == 244)	thread->handle = CreateThread(NULL, 0, PortThread_244, NULL, 0, &thread->id);
	else if(port == 245)	thread->handle = CreateThread(NULL, 0, PortThread_245, NULL, 0, &thread->id);
	else if(port == 246)	thread->handle = CreateThread(NULL, 0, PortThread_246, NULL, 0, &thread->id);
	else if(port == 247)	thread->handle = CreateThread(NULL, 0, PortThread_247, NULL, 0, &thread->id);
	else if(port == 248)	thread->handle = CreateThread(NULL, 0, PortThread_248, NULL, 0, &thread->id);
	else if(port == 249)	thread->handle = CreateThread(NULL, 0, PortThread_249, NULL, 0, &thread->id);
	else if(port == 250)	thread->handle = CreateThread(NULL, 0, PortThread_250, NULL, 0, &thread->id);
	else if(port == 251)	thread->handle = CreateThread(NULL, 0, PortThread_251, NULL, 0, &thread->id);
	else if(port == 252)	thread->handle = CreateThread(NULL, 0, PortThread_252, NULL, 0, &thread->id);
	else if(port == 253)	thread->handle = CreateThread(NULL, 0, PortThread_253, NULL, 0, &thread->id);
	else if(port == 254)	thread->handle = CreateThread(NULL, 0, PortThread_254, NULL, 0, &thread->id);
	else if(port == 255)	thread->handle = CreateThread(NULL, 0, PortThread_255, NULL, 0, &thread->id);

	else;
}

void PortThreadInit(int port)
{
	GLOBAL_PORT_STRUCT *pt = &portBuf[port];

	if(pt->bActiveFlag == 0)		return;	// 2007.9.19 추가
	if(pt->bActiveThread == OFF)	return;

	THREAD_PORT_STRUCT *thread;

	thread = &threadPort[port];

	if(port >= 0 && port <= 49) {
		Init_000_049(port, thread);
	}
	else if(port >= 50 && port <= 99) {
		Init_050_099(port, thread);
	}
	else if(port >= 100 && port <= 149) {
		Init_100_149(port, thread);
	}
	else if(port >= 150 && port <= 199) {
		Init_150_199(port, thread);
	}
	else if(port >= 200 && port <= 255) {
		Init_200_255(port, thread);
	}
}

void PortThreadUnInit(int port)
{
	THREAD_PORT_STRUCT *thread;

	thread = &threadPort[port];

	if(thread->handle == NULL)	return;

	thread->bDo = OFF;

	TimeOutClass timeout;
	while(thread->bEnd == OFF) {
		Sleep(1);
		if(timeout.IsTimeOut(30))	break;
	}

	thread->handle = NULL;
	thread->id = 0;
}*/
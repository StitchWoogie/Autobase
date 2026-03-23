# Keyboard Shortcuts - Complete Reference

> 이 문서는 향후 단축키 설정 창 구현을 위한 참조 문서입니다.
> 현재 단축키는 **Classic** 세트로 분류됩니다.

---

## 1. StudioMain (메인 스튜디오)

| 단축키 | 기능 | 메뉴 항목 | 소스 위치 |
|--------|------|-----------|-----------|
| `Ctrl+N` | 새 Graphic Module 생성 | File > Graphic Module | StudioMain.resx |
| `Ctrl+O` | 파일 열기 | File > Open | StudioMain.resx |
| `Ctrl+F12` | Run LocalMain 실행 | Run > Run LocalMain | StudioMain.resx |
| `F1` | 도움말 | Help > Help | StudioMain.resx |
| `Ctrl+Shift+Alt+F12` | Future Version Go (히든) | (메뉴 없음) | StudioMain.cs:2710 (ProcessCmdKey) |

---

## 2. FormEditGraphic / FormEditGraphicFrame (그래픽 편집기)

### 2.1 파일 (File)

| 단축키 | 기능 | 메뉴 항목 |
|--------|------|-----------|
| `Ctrl+S` | 저장 | File > Save |
| `Ctrl+B` | 비트맵으로 저장 | File > Save As Bitmap |

### 2.2 편집 (Edit)

| 단축키 | 기능 | 메뉴 항목 |
|--------|------|-----------|
| `Ctrl+Z` | 실행 취소 (Undo) | Edit > Undo |
| `Ctrl+Y` | 다시 실행 (Redo) | Edit > Redo |
| `Ctrl+C` | 복사 | Edit > Copy |
| `Ctrl+V` | 붙여넣기 | Edit > Paste |
| `Ctrl+Shift+V` | 자동 붙여넣기 | Edit > Auto Paste |
| `Ctrl+Alt+V` | 클립보드 붙여넣기 | Edit > Clipboard Paste |
| `Del` | 삭제 | Edit > Delete |
| `Ctrl+A` | 전체 선택 | Edit > Select All |
| `Ctrl+H` | 링크된 태그 | Edit > Linked Tag |

### 2.3 객체 순서 (Object Order)

| 단축키 | 기능 | 메뉴 항목 |
|--------|------|-----------|
| `Shift+F1` | 맨 앞으로 이동 | Move to Front |
| `Shift+F2` | 맨 뒤로 이동 | Move to Back |
| `Shift+F7` | 한 단계 앞으로 | Move to Prev Front |
| `Shift+F8` | 한 단계 뒤로 | Move to Next Back |

### 2.4 정렬 (Alignment)

| 단축키 | 기능 | 메뉴 항목 |
|--------|------|-----------|
| `Ctrl+Left` | 왼쪽 정렬 | Align to Left |
| `Ctrl+Right` | 오른쪽 정렬 | Align to Right |
| `Ctrl+Up` | 위쪽 정렬 | Align to Top |
| `Ctrl+Down` | 아래쪽 정렬 | Align to Bottom |

### 2.5 간격/뒤집기 (Spacing / Flip)

| 단축키 | 기능 | 메뉴 항목 |
|--------|------|-----------|
| `Shift+F11` | 수평 간격 균등 | Space Horizontal |
| `Shift+F12` | 수직 간격 균등 | Space Vertical |
| `Shift+F9` | 수평 뒤집기 | Flip Horizontal |
| `Shift+F10` | 수직 뒤집기 | Flip Vertical |

### 2.6 그룹/잠금 (Group / Lock)

| 단축키 | 기능 | 메뉴 항목 |
|--------|------|-----------|
| `Ctrl+G` | 그룹화 | Group |
| `Ctrl+U` | 그룹 해제 | Ungroup |
| `Ctrl+2` | 잠금 | Lock |
| `Ctrl+5` | 전체 잠금 해제 | Unlock All |

### 2.7 라이브러리 (Library)

| 단축키 | 기능 | 메뉴 항목 |
|--------|------|-----------|
| `Ctrl+R` | 라이브러리에 등록 | Register to Library |

### 2.8 줌 (Zoom) - 메뉴

| 단축키 | 기능 | 메뉴 항목 |
|--------|------|-----------|
| `Alt+0` | 50% 줌 | View > 50% |
| `Alt+1` | 100% 줌 | View > 100% |
| `Alt+2` | 200% 줌 | View > 200% |
| `Alt+3` | 300% 줌 | View > 300% |
| `Alt+4` | 400% 줌 | View > 400% |
| `Alt+5` | 500% 줌 | View > 500% |
| `Alt+6` | 600% 줌 | View > 600% |
| `Alt+7` | 700% 줌 | View > 700% |
| `Alt+8` | 800% 줌 | View > 800% |
| `Alt+9` | 900% 줌 | View > 900% |
| `Ctrl++` | 줌 인 | View > Zoom In |
| `Ctrl+-` | 줌 아웃 | View > Zoom Out |

### 2.9 줌 (Zoom) - NumPad (KeyDown 핸들러)

| 단축키 | 기능 | 소스 위치 |
|--------|------|-----------|
| `Alt+NumPad0` | 50% 줌 | FormEditGraphic.cs:3452 |
| `Alt+NumPad1` | 100% 줌 | FormEditGraphic.cs:3463 |
| `Alt+NumPad2` | 200% 줌 | FormEditGraphic.cs:3474 |
| `Alt+NumPad3` | 300% 줌 | FormEditGraphic.cs:3485 |
| `Alt+NumPad4` | 400% 줌 | FormEditGraphic.cs:3496 |
| `Alt+NumPad5` | 500% 줌 | FormEditGraphic.cs:3507 |
| `Alt+NumPad6` | 600% 줌 | FormEditGraphic.cs:3518 |
| `Alt+NumPad7` | 700% 줌 | FormEditGraphic.cs:3529 |
| `Alt+NumPad8` | 800% 줌 | FormEditGraphic.cs:3540 |
| `Alt+NumPad9` | 900% 줌 | FormEditGraphic.cs:3551 |

### 2.10 버튼/디지털 삽입 (Insert Button/Digital) - Ctrl+숫자

| 단축키 | 기능 | 메뉴 항목 |
|--------|------|-----------|
| `Ctrl+0` | 모듈 선택 버튼 삽입 | Insert > Module Selection Button |
| `Ctrl+1` | 모듈 숨기기 버튼 삽입 | Insert > Module Hide Button |
| `Ctrl+3` | 스크립트 버튼 삽입 | Insert > Script Button |
| `Ctrl+4` | 디지털 출력 버튼 삽입 | Insert > Digital Output Button |
| `Ctrl+6` | 디지털 애니메이션 삽입 | Insert > Digital Animation |
| `Ctrl+7` | 디지털 원 삽입 | Insert > Digital Circle |
| `Ctrl+8` | 디지털 사각형 삽입 | Insert > Digital Rectangle |
| `Ctrl+9` | 디지털 문자열 삽입 | Insert > Digital String |

### 2.11 버튼/디지털 삽입 (Insert) - Ctrl+NumPad (KeyDown 핸들러)

| 단축키 | 기능 | 소스 위치 |
|--------|------|-----------|
| `Ctrl+NumPad0` | 모듈 3D 버튼 삽입 | FormEditGraphic.cs:3446 |
| `Ctrl+NumPad1` | 모듈 숨기기 버튼 삽입 | FormEditGraphic.cs:3457 |
| `Ctrl+NumPad2` | 편집 잠금 | FormEditGraphic.cs:3468 |
| `Ctrl+NumPad3` | 프로그램 버튼 삽입 | FormEditGraphic.cs:3479 |
| `Ctrl+NumPad4` | 디지털 출력 버튼 삽입 | FormEditGraphic.cs:3490 |
| `Ctrl+NumPad5` | 편집 잠금 해제 | FormEditGraphic.cs:3501 |
| `Ctrl+NumPad6` | 디지털 애니메이션 삽입 | FormEditGraphic.cs:3512 |
| `Ctrl+NumPad7` | 디지털 원 삽입 | FormEditGraphic.cs:3523 |
| `Ctrl+NumPad8` | 디지털 사각형 삽입 | FormEditGraphic.cs:3534 |
| `Ctrl+NumPad9` | 디지털 문자열 삽입 | FormEditGraphic.cs:3545 |

### 2.12 아날로그 객체 삽입 (Insert Analog)

| 단축키 | 기능 | 메뉴 항목 |
|--------|------|-----------|
| `Ctrl+Shift+A` | 아날로그 사각형 | Insert > Analog Rectangle |
| `Ctrl+Shift+B` | 아날로그 문자열 | Insert > Analog String |
| `Ctrl+Shift+C` | 아날로그 미터 | Insert > Analog Meter |
| `Ctrl+Shift+D` | 아날로그 상태 | Insert > Analog Status |
| `Ctrl+Shift+E` | 아날로그 회전 | Insert > Analog Rotate |

### 2.13 태그/특수 객체 삽입 (Insert Tag/Special)

| 단축키 | 기능 | 메뉴 항목 |
|--------|------|-----------|
| `Ctrl+Shift+F` | 문자열 태그 | Insert > String Tag |
| `Ctrl+Shift+F9` | 태그 애니메이션 | Insert > Tag Animation |
| `Ctrl+Shift+V` | 디스플레이 변경 | Insert > Display Changes |
| `Ctrl+Shift+G` | 그래픽 모듈 | Insert > Graphic Module |
| `Ctrl+Shift+H` | 알람 윈도우 | Insert > Alarm Window |

### 2.14 컨트롤 삽입 (Insert Controls)

| 단축키 | 기능 | 메뉴 항목 |
|--------|------|-----------|
| `Ctrl+Shift+M` | 리스트 박스 | Insert > List Box |
| `Ctrl+Shift+N` | 콤보 박스 | Insert > Combo Box |
| `Ctrl+Shift+O` | 에디트 박스 | Insert > Edit Box |
| `Ctrl+Shift+P` | 라디오 버튼 | Insert > Radio Button |
| `Ctrl+Shift+Q` | 체크 박스 | Insert > Check Box |

### 2.15 그래프/트렌드 삽입 (Insert Graph/Trend)

| 단축키 | 기능 | 메뉴 항목 |
|--------|------|-----------|
| `Ctrl+Shift+I` | 멀티그래프 | Insert > Multi-graph |
| `Ctrl+Shift+J` | 멀티트렌드 | Insert > Multi-trend |
| `Ctrl+Shift+L` | 밀리데이터 윈도우 | Insert > Milli-Data Window |
| `Ctrl+Shift+K` | 디맨드 윈도우 | Insert > Demand Window |
| `Ctrl+Shift+S` | 실시간 테스트 그래프 | Insert > Real-time Test Graph |
| `Ctrl+Shift+T` | XY 그래프 | Insert > XY Graph |
| `Ctrl+Shift+U` | 데이터베이스 트렌드 | Insert > Database Trend |
| `Ctrl+Shift+R` | 데이터베이스 | Insert > Database |

### 2.16 미디어/도형 삽입 (Insert Media/Shapes)

| 단축키 | 기능 | 메뉴 항목 |
|--------|------|-----------|
| `Ctrl+Shift+W` | 비트맵 | Insert > Bitmap |
| `Ctrl+Shift+X` | 애니메이션 | Insert > Animation |
| `Ctrl+Shift+Y` | 단일 라인 텍스트 | Insert > Single Line Text |
| `Ctrl+Shift+F1` | 사각형 | Insert > Rectangle |
| `Ctrl+Shift+F2` | 원 | Insert > Circle |
| `Ctrl+Shift+F3` | 직선 | Insert > Line |
| `Ctrl+Shift+F4` | 둥근 사각형 | Insert > Rounded Rectangle |
| `Ctrl+Shift+F5` | 텍스트 | Insert > Text |
| `Ctrl+Shift+F6` | 시계 | Insert > Clock |
| `Ctrl+Shift+F7` | 날짜 | Insert > Date |
| `Ctrl+Shift+F8` | 객체 라이브러리 | Insert > Object Library |

### 2.17 창 배열 (Window)

| 단축키 | 기능 | 메뉴 항목 |
|--------|------|-----------|
| `Shift+F5` | 계단식 배열 | Window > Cascade |
| `Shift+F4` | 수평 타일 배열 | Window > Tile Horizontally |

### 2.18 도구 선택 (Tool Selection) - 단일 키

| 단축키 | 기능 | 소스 위치 |
|--------|------|-----------|
| `V` | 선택/이동 도구 (Arrow) | FormEditGraphic.cs:3566 |
| `L` | 직선 도구 (Line) | FormEditGraphic.cs:3570 |
| `R` | 사각형 도구 (Rectangle) | FormEditGraphic.cs:3574 |
| `M` | 채움 사각형 도구 (Filled Rectangle) | FormEditGraphic.cs:3578 |
| `S` | 원 도구 (Circle) | FormEditGraphic.cs:3582 |
| `E` | 채움 원 도구 (Filled Circle) | FormEditGraphic.cs:3586 |
| `O` | 다각형 도구 (Polygon) | FormEditGraphic.cs:3590 |
| `Y` | 채움 다각형 도구 (Filled Polygon) | FormEditGraphic.cs:3594 |
| `T` | 텍스트 도구 (Text) | FormEditGraphic.cs:3598 |
| `D` | 둥근 사각형 도구 (Rounded Rectangle) | FormEditGraphic.cs:3602 |
| `C` | 곡선 도구 (Curve) | FormEditGraphic.cs:3606 |
| `N` | 점 도구 (Point) | FormEditGraphic.cs:3610 |
| `I` | 스포이트 도구 (Color Picker) | FormEditGraphic.cs:3614 |

### 2.19 이동/내비게이션 (Move / Navigation)

| 단축키 | 기능 | 소스 위치 |
|--------|------|-----------|
| `Arrow Keys` | 선택 객체 1px 이동 | FormEditGraphic.cs:3402 |
| `Ctrl+Arrow Keys` | 선택 객체 정렬 (Arrange) | FormEditGraphic.cs:3402 |
| `Space` (누르고 있기) | 패닝 모드 (화면 이동) | FormEditGraphic.cs:3619 |
| `Escape` | 속성 시트 닫기 | FormEditGraphic.cs:3559 |

---

## 3. 단축키 충돌 (Conflicts) 주의사항

| 단축키 | 충돌 항목 | 비고 |
|--------|-----------|------|
| `Ctrl+Shift+V` | Auto Paste / Display Changes | 두 기능에 동일 단축키 할당됨 |
| `Ctrl+Left/Right/Up/Down` | 정렬(메뉴) / 이동(KeyDown) | 메뉴 단축키와 KeyDown 핸들러에서 다른 동작 가능 |

---

## 4. 단축키 소스 분류

### 4.1 정의 위치별 분류

| 소스 | 파일 | 설명 |
|------|------|------|
| **Menu ShortcutKeys** | `FormEditGraphicFrame.resx` | 메뉴 항목에 지정된 단축키 (resx 리소스) |
| **KeyDown Handler** | `FormEditGraphic.cs` (`FormEditGraphic_KeyDown`) | KeyDown 이벤트 핸들러에서 처리 |
| **ProcessCmdKey** | `FormEditGraphic.cs` (`ProcessCmdKey`) | ProcessCmdKey 오버라이드에서 처리 |
| **ProcessArrowKey** | `FormEditGraphic.cs` (`ProcessArrowKey`) | 방향키 전용 처리 메서드 |
| **Menu ShortcutKeys** | `StudioMain.resx` | StudioMain 메뉴 단축키 |
| **ProcessCmdKey** | `StudioMain.cs` | StudioMain ProcessCmdKey 히든 단축키 |

### 4.2 통계 요약

| 카테고리 | 개수 |
|----------|------|
| StudioMain 메뉴 단축키 | 4 |
| StudioMain 히든 단축키 | 1 |
| FormEditGraphicFrame 메뉴 단축키 | 82 |
| FormEditGraphic 도구 선택 키 | 13 |
| FormEditGraphic NumPad 단축키 | 20 |
| FormEditGraphic 특수 키 (Space, Esc, Arrows) | 4 |
| **총 단축키 수** | **~124** |

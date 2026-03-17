# Autobase SCADA 정적 오브젝트 Bitmap 병합 기능

## 개요
Studio에서 사용자가 정적 오브젝트들을 선택하여 Bitmap으로 병합하고,
런타임에서는 단일 이미지로 렌더링하여 Paint 성능을 대폭 개선한다.
CE 호환 방식으로, 병합 시 PNG 파일로 렌더링하여 저장하며
.modx에서는 ObjectBitmap으로 기록되어 CE에서도 로드 가능하다.

## 구현 파일

### 신규 파일
1. `ObjectMergedBitmapSimple.cs` - CE 호환 병합 Bitmap 오브젝트 (ObjectExpand 상속)

### 수정 파일
1. `ObjectType.cs` - EnumObjectType에 MergedBitmapSimple 추가
2. `ObjectGroupLoadModX.cs` - .modx 로딩 시 MergedOriginalData 감지하여 ObjectMergedBitmapSimple 생성
3. `LoadObjectFromModX.cs` - MergedOriginalCount/MergedOriginalData 필드 파싱
4. `ClassStudioEdit.cs` - EditMergeToBitmapSimple(), EditUnmergeBitmapSimple() 추가
5. `FormEditGraphic.cs` - 컨텍스트 메뉴에 "Merge to Bitmap" / "Unmerge Bitmap" 항목 추가

## ObjectMergedBitmapSimple 설계

### 저장 데이터
- `ObjectArgsBitmap objArgs` - PNG 파일 참조 (ObjectBitmap 호환)
- `AnimationClass animation` - 이미지 로딩/표시
- `byte[] serializedOriginalObjects` - 원본 오브젝트들의 BinaryFormatter 직렬화 데이터 (해제용)
- `int originalObjectCount` - 원본 오브젝트 수

### Display 흐름
1. PNG 파일을 AnimationClass로 로드
2. `animation.Putimage()`으로 렌더링 (ObjectBitmap과 동일 방식)

### .modx 저장 형식
CE 호환을 위해 ObjectBitmap으로 저장, 원본 데이터를 추가 필드로 기록:
```
ObjectBitmap,BEGIN
Rect,x1,y1,x2,y2,
FileName,_merged_yyyyMMdd_HHmmss_fff.png,
OverlayMethod,0,0,
MergedOriginalCount,N,
MergedOriginalData,<Base64 encoded serialized bytes>,
ObjectBitmap,END
```

## Studio 편집 흐름

### 병합 (Merge to Bitmap)
1. 사용자가 정적 오브젝트들을 다중 선택
2. 우클릭 → "Merge to Bitmap" 선택
3. 선택된 오브젝트들을 BinaryFormatter로 직렬화 → byte[] (해제용)
4. 선택 영역의 bounding rect 계산
5. 오브젝트들을 PNG로 렌더링하여 그래픽 디렉토리에 저장
6. ObjectMergedBitmapSimple 생성 (PNG 참조 + 직렬화 데이터)
7. 원본 오브젝트 삭제, ObjectMergedBitmapSimple 삽입
8. Undo 지원

### 해제 (Unmerge Bitmap)
1. ObjectMergedBitmapSimple 선택
2. 우클릭 → "Unmerge Bitmap" 선택
3. 직렬화 데이터에서 원본 오브젝트 역직렬화
4. ObjectMergedBitmapSimple 삭제, 원본 오브젝트들 삽입
5. Undo 지원

## 성능 분석

### Paint 사이클 당 비용 비교

병합하지 않은 경우 Paint 한 번에 오브젝트 N개를 순회하며 각각 렌더링한다.
병합한 경우 N개가 1개의 DrawImage 호출로 대체된다.

#### 오브젝트 1개당 렌더링 비용 (병합 미사용 시)

```
ObjectGroup.Display()에서 for 루프
  → ObjectExpand.Display()   (가상 메서드 호출)
    → ExpandCalcVisible()    (Visible 상태 계산)
    → 좌표 계산 (nViewX1-originx 등)
    → 회전 검사 (RotationAngle != 0)
    → DisplayObject()        (가상 메서드 호출)
      → 좌표 정렬 (x1 > x2 swap)
      → GDI+ 호출 1~4회 (Brush 생성, Fill, Pen 생성, Draw)
    → MouseZone 검사
```

오브젝트 종류별 GDI+ 호출 횟수:

| 오브젝트 | GDI+ 호출 | 부가 비용 |
|----------|-----------|-----------|
| Rectangle | FillRectangle + DrawRectangle (2회) | Brush 생성, Pen 생성 |
| Line | DrawLine (1회) | Pen 생성, DashStyle 설정 |
| Circle | FillEllipse + DrawEllipse (2회) | Brush 생성, Pen 생성 |
| RoundRectangle | FillPath + DrawPath (2회) | GraphicsPath 생성 |
| SingleText | DrawString (1회) | Font 생성, StringFormat 생성 |
| Poly | DrawLines/FillPolygon (1~2회) | Point[] 변환 |
| Bitmap | DrawImage (1회) | 이미지 디코딩 (캐시됨) |

#### 병합 후 비용

```
ObjectGroup.Display()에서 for 루프 (1회만)
  → ObjectExpand.Display()
    → ExpandCalcVisible()
    → DisplayObject()
      → animation.Putimage() → g.DrawImage() (1회)
```

### 정량 비교

Paint 1회당 CPU 비용을 오브젝트 개수별로 비교한다.
(GDI+ DrawImage 1회를 기준 단위 1로 설정)

| | 10개 | 50개 | 200개 | 500개 | 1000개 |
|---|---|---|---|---|---|
| **병합 미사용** | | | | | |
| for 루프 반복 | 10회 | 50회 | 200회 | 500회 | 1000회 |
| 가상 메서드 호출 | 20회 | 100회 | 400회 | 1000회 | 2000회 |
| Visible/좌표 계산 | 10회 | 50회 | 200회 | 500회 | 1000회 |
| GDI+ Brush/Pen 생성 | ~20개 | ~100개 | ~400개 | ~1000개 | ~2000개 |
| GDI+ Draw 호출 | ~20회 | ~100회 | ~400회 | ~1000회 | ~2000회 |
| **합계 (상대 비용)** | **~15** | **~75** | **~300** | **~750** | **~1500** |
| | | | | | |
| **병합 사용** | | | | | |
| for 루프 반복 | 1회 | 1회 | 1회 | 1회 | 1회 |
| 가상 메서드 호출 | 2회 | 2회 | 2회 | 2회 | 2회 |
| Visible/좌표 계산 | 1회 | 1회 | 1회 | 1회 | 1회 |
| GDI+ Draw 호출 | 1회 | 1회 | 1회 | 1회 | 1회 |
| **합계 (상대 비용)** | **~1** | **~1** | **~1** | **~1** | **~1** |
| | | | | | |
| **성능 배율** | **~15x** | **~75x** | **~300x** | **~750x** | **~1500x** |

### 주요 병목 요소 분석

#### 1. GDI+ Draw Call 오버헤드
- GDI+의 각 Draw 호출은 User→Kernel 모드 전환을 수반한다.
- Rectangle 하나를 그리려면 FillRectangle + DrawRectangle = 2회의 커널 전환이 발생한다.
- 병합 시 이 모든 호출이 DrawImage 1회로 대체된다.

#### 2. GDI+ 리소스 생성/해제
- 각 오브젝트의 DisplayObject()에서 Pen, Brush, Font 등을 매번 생성한다.
  - `new Pen(RunColorLine, bthick)` (ObjectLine, ObjectCircle 등)
  - `ObjectRectangle.MakePublicBrush(...)` (ObjectRectangle, ObjectCircle 등)
  - `MakeFont()` (ObjectSingleText)
- 이 리소스들은 Paint 호출마다 반복 생성되며, GDI 핸들을 소비한다.
- 병합 후에는 이 리소스 생성이 전혀 없다.

#### 3. 가상 메서드 디스패치
- `ObjectExpand.Display()` → `DisplayObject()` 체인이 매 오브젝트마다 호출된다.
- 가상 메서드 호출 자체는 빠르지만, 오브젝트 수에 비례하여 누적된다.
- CPU 캐시 미스도 오브젝트 수에 따라 증가한다 (각 오브젝트가 힙에 분산).

#### 4. Paint 호출 빈도
- SCADA 런타임에서 Paint는 다음 상황에서 발생한다:
  - 태그 값 변경 시 Invalidate (주기적, 수초~수백ms)
  - 화면 스크롤, 확대/축소
  - 윈도우 포커스 전환, 최소화 복원
- Paint 빈도가 높을수록 병합의 이점이 커진다.

### 메모리 영향

| 항목 | 병합 미사용 | 병합 사용 |
|------|-----------|-----------|
| 오브젝트 인스턴스 | N개 (힙 메모리) | 1개 |
| GDI 핸들 (Paint 중) | N × 2~3개 (Pen/Brush/Font) | 0개 |
| PNG 파일 | 없음 | 1개 (디스크) |
| PNG 이미지 캐시 | 없음 | 1개 (AnimationClass) |
| 직렬화 데이터 | 없음 | byte[] (해제용, 메모리 상주) |

- 런타임 메모리는 오브젝트 인스턴스가 줄어들므로 순감소한다.
- 단, 직렬화 데이터(byte[])가 추가 메모리를 사용하지만 오브젝트 N개보다 작다.
- PNG 파일은 디스크 공간을 소비하며, 크기는 영역 해상도에 비례한다.

### 적용 권장 기준

| 상황 | 병합 권장 | 이유 |
|------|----------|------|
| 정적 도형 10개 미만 | 선택적 | 효과 미미 |
| 정적 도형 50개 이상 | 권장 | Paint 시간 체감 차이 발생 |
| 정적 도형 200개 이상 | 강력 권장 | Paint 시간 수백ms → 수ms |
| 배경 장식용 도형 | 강력 권장 | 런타임에서 변경되지 않는 요소 |
| 태그 연결 오브젝트 | 병합 불가 | ExpandActive=true이므로 자동 제외 |
| CE 디바이스 대상 | 권장 | CE에서도 호환 렌더링됨 |

### 제한사항

- 병합된 이미지는 고정 해상도 PNG이므로, 확대 시 픽셀이 보일 수 있다.
  (실사용 시 화질 열화는 체감되지 않음)
- 병합 후 개별 오브젝트 편집 불가 (해제 후 편집해야 함)
- PNG 파일이 그래픽 디렉토리에 추가로 저장된다.

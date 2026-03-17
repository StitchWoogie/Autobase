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

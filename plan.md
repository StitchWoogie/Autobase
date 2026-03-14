# Autobase SCADA 정적 오브젝트 Bitmap 병합 기능

## 개요
Studio에서 사용자가 정적 오브젝트들을 선택하여 Bitmap으로 병합하고,
런타임에서는 단일 이미지로 렌더링하여 Paint 성능을 대폭 개선한다.
확대/축소 시에는 원본 오브젝트 데이터로 재렌더링하여 화질을 유지한다.

## 구현 파일

### 신규 파일
1. `ObjectMergedBitmap.cs` - 병합된 Bitmap 오브젝트 (ObjectExpand 상속)

### 수정 파일
1. `ObjectType.cs` - EnumObjectType에 MergedBitmap 추가
2. `ObjectGroupLoadModX.cs` - .modx 로딩에 ObjectMergedBitmap 파서 추가
3. `ClassStudioEdit.cs` - EditMergeToBitmap(), EditUnmergeBitmap() 추가
4. `FormEditGraphic.cs` - 컨텍스트 메뉴에 병합/해제 항목 추가

## ObjectMergedBitmap 설계

### 저장 데이터
- `byte[] serializedOriginalObjects` - 원본 오브젝트들의 BinaryFormatter 직렬화 데이터
- `int originalObjectCount` - 원본 오브젝트 수
- `Bitmap cachedBitmap` (NonSerialized) - 현재 배율의 캐시 Bitmap
- `int cachedScaleKey` (NonSerialized) - 캐시된 배율 키 (변경 감지용)

### Display 흐름
1. 현재 배율 키 계산 (nScreenSizeX, nScreenSizeY, wOpticRate 조합)
2. 캐시 키와 일치하면 → cachedBitmap을 DrawImage()
3. 일치하지 않으면 → 원본 오브젝트를 역직렬화하여 현재 배율로 렌더링 → 캐시 갱신

### .modx 저장 형식
```
ObjectMergedBitmap,BEGIN
Rect,x1,y1,x2,y2,
OriginalObjectCount,N,
OriginalData,<Base64 encoded serialized bytes>,
ObjectMergedBitmap,END
```

## Studio 편집 흐름

### 병합 (Merge)
1. 사용자가 정적 오브젝트들을 다중 선택
2. 우클릭 → "Bitmap으로 병합" 선택
3. 선택된 오브젝트들을 BinaryFormatter로 직렬화 → byte[]
4. 선택 영역의 bounding rect 계산
5. ObjectMergedBitmap 생성 (직렬화 데이터 포함)
6. 원본 오브젝트 삭제, ObjectMergedBitmap 삽입
7. Undo 지원

### 해제 (Unmerge)
1. ObjectMergedBitmap 선택
2. 우클릭 → "병합 해제" 선택
3. 직렬화 데이터에서 원본 오브젝트 역직렬화
4. ObjectMergedBitmap 삭제, 원본 오브젝트들 삽입
5. Undo 지원

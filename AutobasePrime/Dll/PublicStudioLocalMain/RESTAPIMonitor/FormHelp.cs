using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AutobaseRESTAPIMonitor.FormRestApiMonitor;

namespace AutobaseRESTAPIMonitor
{
    public partial class FormHelp : Form
    {

        private TabControl tabControl;
        public TabControl TabControl => tabControl; // 프로퍼티 추가
        private static FormHelp instance = null;

        public static FormHelp GetInstance(int selectedTabIndex = 0)
        {
            // 인스턴스가 없거나, Disposed되었거나, 보이지 않는 상태일 때 새로 생성
            if (instance == null || instance.IsDisposed || !instance.Visible)
            {
                instance = new FormHelp(selectedTabIndex);
                instance.FormClosed += (s, e) => instance = null;
            }
            instance.TabControl.SelectedIndex = selectedTabIndex;
            return instance;
        }

        public FormHelp(int selectedTabIndex = 0)
        {
            InitializeComponent();

            tabControl = new TabControl();
            InitializeUI();

           tabControl.SelectedIndex = selectedTabIndex;
            
        }

        private void InitializeUI()
        {
            // 폼 기본 설정
            this.Text = GlobalSettings.IsKorean ? "도움말" : "Help";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(400, 300);

            // TabControl 생성
            tabControl.Dock = DockStyle.Fill;
            this.Controls.Add(tabControl);

            // 기본 사용법 탭
            TabPage basicTab = new TabPage(GlobalSettings.IsKorean? "기본 사용법" : "Basic Usage");
            RichTextBox basicText = new RichTextBox();
            basicText.Dock = DockStyle.Fill;
            basicText.ReadOnly = true;
            basicText.Text = GetBasicHelpText();
            basicTab.Controls.Add(basicText);

            // JSON Path 탭
            TabPage jsonPathTab = new TabPage(GlobalSettings.IsKorean? "JSON Path 사용법" : "JSON Path Usage");
            RichTextBox jsonPathText = new RichTextBox();
            jsonPathText.Dock = DockStyle.Fill;
            jsonPathText.ReadOnly = true;
            jsonPathText.Text = GetJsonPathHelpText();
            jsonPathTab.Controls.Add(jsonPathText);

            // 자주 묻는 질문 탭
            TabPage faqTab = new TabPage(GlobalSettings.IsKorean? "자주 묻는 질문" : "FAQ");
            RichTextBox faqText = new RichTextBox();
            faqText.Dock = DockStyle.Fill;
            faqText.ReadOnly = true;
            faqText.Text = GetFAQText();
            faqTab.Controls.Add(faqText);

            // 포맷 사용법 탭
            TabPage formatTab = new TabPage(GlobalSettings.IsKorean ? "URL 포맷 사용법" : "URL Format Usage");
            RichTextBox formatText = new RichTextBox();
            formatText.Dock = DockStyle.Fill;
            formatText.ReadOnly = true;
            formatText.Text = GetFormatHelp();
            formatTab.Controls.Add(formatText);

            // 탭 추가
            tabControl.TabPages.Add(basicTab);
            tabControl.TabPages.Add(jsonPathTab);
           
            tabControl.TabPages.Add(formatTab);
            tabControl.TabPages.Add(faqTab);

            // 닫기 버튼
            Button closeButton = new Button();
            closeButton.Text = GlobalSettings.IsKorean? "닫기" : "Close";
            closeButton.Dock = DockStyle.Bottom;
            closeButton.Height = 30;
            closeButton.Click += (s, e) => this.Dispose();
            this.Controls.Add(closeButton);
        }

        private string GetBasicHelpText()
        {
            if (GlobalSettings.IsKorean)
            {
                return @"Autobase REST API Monitor 프로그램 사용법

Autobase REST API Monitor 는 JSON 응답을 반환하는 REST API를 모니터링하는 프로그램입니다.

1. API 추가하기
- [추가] 버튼을 클릭하여 새로운 API를 등록합니다.
- API의 이름, URL, 갱신 주기를 입력합니다.
- JSON Path와 태그를 설정하여 원하는 데이터를 추출합니다.
- 태그 설정 시 감시프로그램의 메모리태그를 사용합니다.
- 태그가 존재하지 않을 경우, 로그에만 기록됩니다.
- 감시프로그램에서 사용하는 다른 태그와 이름이 중복되지 않게 설정하세요. 

2. API 관리
- 등록된 API는 메인 화면의 목록에서 확인할 수 있습니다.
- 더블 클릭하여 설정을 수정할 수 있습니다.
- [삭제] 버튼으로 불필요한 API를 제거할 수 있습니다.

3. 데이터 모니터링
- 상태 열에서 각 API의 현재 상태를 확인할 수 있습니다.
- 마지막 갱신 시간으로 데이터 최신성을 확인할 수 있습니다.
- 오류 발생 시 상태 열에 오류 내용이 표시됩니다.

4. 데이터 저장
- 길이제한: 숫자데이터는 300자, 문자열데이터는 2048자
- 길이제한을 초과할 경우, 제한된 값으로 태그값에 저장되며 로그에는 전체 값이 저장됩니다.
- 정상적으로 수신된 값은 선택한 메모리태그에서 읽을 수 있습니다.
- 최종 수신 데이터는 Autobase 프로젝트 경로의 APIClient 폴더에 [제목]_log.txt 파일명으로 JSON형태로 저장됩니다.
- Not-Found 문자열을 사용하면 해당 값이 없을 때, 설정한 문자열을 태그에 출력합니다.
- Not-Found 문자열을 사용하지 않으면, 태그 값이 변경되지 않습니다. (이전 값 유지)
";

                
            }
            else
            {
                return @"Autobase REST API Monitor Program Usage Guide

The Autobase REST API Monitor is a program that monitors REST APIs that return JSON responses.

1. Adding an API
- Click the [Add] button to register a new API.
- Enter the API name, URL, and refresh interval.
- Set up JSON Path and Memory tags to extract desired data.
- Use the LocalMain program's Memory tag when setting the tag.
- If the tag does not exist, it will only be logged.
- Make sure that the name does not overlap with other tags used by the LocalMain program.

2. API Management
- View registered APIs in the main screen list.
- Double-click to modify settings.
- Use the [Delete] button to remove unnecessary APIs.

3. Data Monitoring
- Check current status of each API in the Status column.
- Verify data freshness through Last Update time.
- Error details are displayed in the Status column when issues occur.

4. Save data
- Length limit: 300 characters for numeric data and 2048 characters for string data
- If the length limit is exceeded, the tag value is stored at a limited value and the entire value is stored in the log.
- The normally received values can be read from the selected tag.
- The final incoming data is stored in the APIClient folder of the Autobase project path in JSON format as the [Title]_log.txt file name.
- Use the Not-Found string to output the set string to the tag when that value is not present.
- If you do not use the Not-Found string, the tag value will not change. (Keep the previous value)";
            }
        }


        private string GetJsonPathHelpText()
        {
            if (GlobalSettings.IsKorean)
            {
                return @"JSON Path 작성 가이드

1.JSONPath 문법 안내

1-1. 기본 구조
- JSON 경로는 $ 기호로 시작할 수 있으며, $는 생략 가능합니다
- 객체의 속성은 점(.) 으로 접근합니다
- 배열의 요소는 대괄호([]) 안에 인덱스를 지정합니다

1-2. 배열 접근 방법
- [0], [1], [2] : 특정 인덱스 선택 
- [*] : 모든 요소 선택
- [-1] : 마지막 요소 선택
※ 배열 접근 시 대괄호([])는 반드시 필요합니다

1-3. 표현 예시
객체로 시작하는 JSON:
  $.store.book  또는  store.book

배열로 시작하는 JSON:
  $[0].name  또는  [0].name
  $[*].name  또는  [*].name

1-4. 권장사항
- 코드의 명확성을 위해 $ 기호를 포함하는 것을 권장합니다
- 중첩된 구조의 경우 전체 경로를 명시하는 것이 좋습니다

2. 객체({}) 형식일 경우의 기본 문법
$.name                  - 최상위 객체의 'name' 값
$.address.city          - 'address' 객체 안의 'city' 값 
$.phones[0]             - 'phones' 배열의 첫 번째 항목
$.phones[*]             - 'phones' 배열의 모든 항목
$.phones[*].number      - 'phones' 배열의 모든 항목의 'number' 값
$.*.city                - 모든 하위 객체의 'city' 값

3. 배열([]) 형식일 경우의 기본 문법
$[0]                    - 배열의 첫 번째 항목
$[*]                    - 배열의 모든 항목
$[0].name              - 첫 번째 항목의 'name' 값
$[*].name              - 모든 항목의 'name' 값
$[0].items[*].value    - 첫 번째 항목의 'items' 배열의 모든 'value' 값

4. 필터 사용
객체 형식:
$.phones[?(@.type=='mobile')]         - type이 'mobile'인 항목
$.items[?(@.price > 100)]             - price가 100보다 큰 항목
$.phones[?(@.type=='mobile')].number  - type이 'mobile'인 항목의 number 값

배열 형식:
$[?(@.category=='수입')]              - category가 '수입'인 항목
$[?(@.amount > 1000)]                 - amount가 1000보다 큰 항목
$[?(@.category=='지출')].items[*]     - category가 '지출'인 항목의 모든 items

5. 예시 JSON과 경로

객체({}) 형식 예시:
{
   ""name"": ""John"",
   ""address"": {
       ""city"": ""Seoul"",
       ""street"": ""Main St.""
   },
   ""phones"": [
       {""type"": ""home"", ""number"": ""123-456""},
       {""type"": ""mobile"", ""number"": ""789-012""}
   ]
}

배열([]) 형식 예시:
[
   {
       ""category"": ""수입"",
       ""items"": [
           {""name"": ""월급"", ""amount"": 3000000},
           {""name"": ""이자"", ""amount"": 50000}
       ]
   },
   {
       ""category"": ""지출"",
       ""items"": [
           {""name"": ""식비"", ""amount"": 500000},
           {""name"": ""교통비"", ""amount"": 100000}
       ]
   }
]

6. 주의사항
- 대소문자를 정확히 구분해야 합니다.
- 배열 인덱스는 0부터 시작합니다.
- 존재하지 않는 경로는 'Not Found'를 반환합니다.";
            }
            else
            {
                return @"JSON Path Writing Guide

1. JSONPath Syntax Guide

1-1. Basic Structure
- JSON paths can start with $ symbol, which is optional
- Object properties are accessed using dot(.)
- Array elements are accessed using brackets([])

1-2. Array Access Methods
- [0], [1], [2] : Select specific index
- [*] : Select all elements
- [-1] : Select last element
※ Brackets([]) are mandatory for array access

1-3. Expression Examples
For JSON starting with object:
  $.store.book  or  store.book

For JSON starting with array:
  $[0].name  or  [0].name
  $[*].name  or  [*].name

1-4. Recommendations
- Including $ symbol is recommended for code clarity
- For nested structures, specifying the full path is recommended


2. Basic Syntax for Object({}) Format
$.name                  - 'name' value of the root object
$.address.city          - 'city' value inside 'address' object
$.phones[0]             - First item in the 'phones' array
$.phones[*]             - All items in the 'phones' array
$.phones[*].number      - 'number' values of all items in 'phones' array
$.*.city                - 'city' values of all child objects

3. Basic Syntax for Array([]) Format
$[0]                    - First item in the array
$[*]                    - All items in the array
$[0].name              - 'name' value of the first item
$[*].name              - 'name' values of all items
$[0].items[*].value    - All 'value' values in 'items' array of first item

4. Using Filters
Object Format:
$.phones[?(@.type=='mobile')]         - Items where type is 'mobile'
$.items[?(@.price > 100)]             - Items where price is greater than 100
$.phones[?(@.type=='mobile')].number  - Number values where type is 'mobile'

Array Format:
$[?(@.category=='income')]            - Items where category is 'income'
$[?(@.amount > 1000)]                 - Items where amount is greater than 1000
$[?(@.category=='expense')].items[*]  - All items where category is 'expense'

5. Example JSON and Paths

Object({}) Format Example:
{
   ""name"": ""John"",
   ""address"": {
       ""city"": ""Seoul"",
       ""street"": ""Main St.""
   },
   ""phones"": [
       {""type"": ""home"", ""number"": ""123-456""},
       {""type"": ""mobile"", ""number"": ""789-012""}
   ]
}

Array([]) Format Example:
[
   {
       ""category"": ""income"",
       ""items"": [
           {""name"": ""salary"", ""amount"": 3000000},
           {""name"": ""interest"", ""amount"": 50000}
       ]
   },
   {
       ""category"": ""expense"",
       ""items"": [
           {""name"": ""food"", ""amount"": 500000},
           {""name"": ""transport"", ""amount"": 100000}
       ]
   }
]

5. Important Notes
- Case sensitive
- Array indices start at 0
- Non-existent paths return 'Not Found'";
            }
        }

        private string GetFAQText()
        {
            if (GlobalSettings.IsKorean)
            {
                return @"자주 묻는 질문

Q: 태그종류는 무엇으로 설정하나요?
A: 메모리태그로 설정하세요.

Q: 꼭 태그를 이용해야 하나요?
A: 로그에 저장된 JSON파일을 @FileRead 함수로 읽은 후 @StringJson 함수로 파싱하여 사용할 수 있습니다.

Q: API 응답이 오류를 반환하는 경우는?
A: URL이 잘못되었거나, 서버가 응답하지 않을 때, 또는 JSON 형식이 잘못된 경우 발생할 수 있습니다.

Q: 갱신 주기는 어떻게 설정하나요?
A: API 설정에서 초 단위로 입력할 수 있으며, 최소 1초 이상이어야 합니다.

Q: 여러 개의 값을 동시에 가져올 수 있나요?
A: 네, 여러 개의 JSON Path를 설정하여 각각 다른 태그로 매핑할 수 있습니다.

Q: 클라이언트 설정파일을 텍스트파일로 편집할 수 있나요?
    (빠르게 작성하는 방법이 있나요?)
A: 클라이언트 설정 시 Autobase 프로젝트 경로의 RESTAPIMonitor 폴더에 ApiClient.lst 가 생성됩니다. 메모장 등 편집프로그램으로 수정 후 프로그램을 재시작하세요.

Q: 프로그램이 시작될 때 자동으로 창을 숨길 수 있나요?
A: 네, '시작 시 숨기기' 옵션을 체크하면 됩니다.

Q: 데이터가 업데이트되지 않을 때는?
A: 네트워크 연결을 확인하고, API 상태가 '정상'인지 확인해주세요.

Q: 프로그램이 항상 실행되도록 할 수 있나요?
A: 워치독 프로그램을 이용하시면 됩니다.

Q: 원하지 않는 값이 할당됩니다.
A: JSON 경로를 잘못 작성하였거나 태그가 중복할당되었을 경우, 아래쪽에 있는 태그의 JSON 값으로 할당됩니다.";
            }
            else
            {
                return @"Frequently Asked Questions

Q: What kind of tag should I set?
A: Set it as a memory tag.

Q: Do I have to use the tag?
A: You can use the JSON file stored in the log by reading it as a @FileRead function and parsing it as a @StringJson function.

Q: When does the API return an error?
A: This can occur when the URL is incorrect, the server is not responding, or the JSON format is invalid.

Q: How do I set the refresh interval?
A: You can enter it in seconds in the API settings, with a minimum of 1 second.

Q: Can I fetch multiple values simultaneously?
A: Yes, you can set multiple JSON Paths and map them to different tags.

Q: Can I edit the client configuration file to a text file?
    (Is there a quick way to write it?)
A: When setting up the client, ApiClient.lst is created in the RESTAPIMonitor folder in the Autobase project path.
Restart the program after modifying it with an editing program such as Notepad.

Q: Can I hide the window automatically at startup?
A: Yes, check the 'Hide on Start' option.

Q: What if data is not updating?
A: Check your network connection and verify that the API status is 'Normal'.

Q: Can I keep the program running continuously?
A: Yes, you can use a watchdog program.

Q: Unwanted values are assigned.
A: If the JSON path is incorrectly created or the tag is assigned duplicate, it is assigned to the JSON value of the tag below.";
            }

        }

        public string GetFormatHelp()
        {
            if (GlobalSettings.IsKorean)
            {
            return @"URL에 다음과 같은 포맷을 사용하여 비ASCII문자열을 인코딩하거나 현재 시간을 기준으로 날짜 및 시간을 입력할 수 있습니다.

URL 인코딩:
- 날짜/시간 포맷이 아닌 모든 {텍스트}는 자동으로 URL 인코딩 됨
예시: {안녕하세요} -> %EC%95%88%EB%85%95%ED%95%98%EC%84%B8%EC%9A%94

사용 가능한 날짜 / 시간 포맷:
- {yyyy}: 연도 4자리
- {MM}: 월 2자리
- {dd}: 일 2자리
- {HH}: 시간 2자리(24시간)
- {mm}: 분 2자리
- {ss}: 초 2자리
- {yyyyMMdd}: YYYYMMDD 형식
- {HHmmss}: HHMMSS 형식
- {yyyy-MM-dd}: YYYY-MM-DD 형식
- {HH:mm:ss}: HH:MM:SS 형식

UTC 시간 포맷:
- {utc:yyyy}, {utc:MM}, {utc:dd}
- {utc:HH}, {utc:mm}, {utc:ss}

시간 / 날짜 오프셋 사용법: 모든 포맷에 다음과 같은 오프셋 지정 가능
- 시간 단위: -Nh 또는 + Nh(N은 숫자)
- 일 단위: -Nd 또는 + Nd(N은 숫자)

예시:
- {HH-2h}: 2시간 전
- {yyyy-MM-dd-1d}: 1일 전
- {yyyyMMdd-2d}: 2일 전
- {utc:HH+1h}: UTC 기준 1시간 후
- {utc:yyyy-MM-dd-3d}: UTC 기준 3일 전
- {문자열}: URL 인코딩된 문자열";

            }
            else
            {
               return @"You can encode non-ASCII strings in URLs using the following formats, or you can enter a date and time based on the current time.

URL Encoding:
- Any {string} that is not a date/time format will be automatically URL encoded
Example: {Hello World} -> Hello%20World

Available Date / Time Format:
- {yyyy}: 4 digits of the year
- {MM}: 2 digits per month
- {dd}: 2 digits per day
- {HH}: 2 digits of time (24 hours)
- {mm}: 2 minutes
- {ss}: 2 seconds
- {yyyyMMdd}: YYYYMMDD 형식
- {HHmmss}: HHMMSS 형식
- {yyyy-MM-dd}: YYYY-MM-DD 형식
- {HH:mm:ss}: HH:MM:SS 형식

UTC Time Format:
- {utc:yyyy}, {utc:MM}, {utc:dd}
- {utc:HH}, {utc:mm}, {utc:ss}

Use Time/Date Offset: Any format can be offset by
- Hours: -Nh or + Nh (where N is a number)
- Days: -Nd or + Nd (where N is a number)

Example:
- {HH-2h}: 2 hours ago
- {yyyy-MM-dd-1d}: 1 day ago
- {yyyyMMdd-2d}: 2 days ago
- {utc:HH+1h}: 1 hour after UTC
- {utc:yyyyy-MM-dd-3d}: 3 days prior to UTC
- {string}: URL encoded string""";
            }
        }


        public void SelectJsonPathTab()
        {
            tabControl.SelectedIndex = 1;  // JSON Path 탭의 인덱스 (0부터 시작)
        }

    }
}

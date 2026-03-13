using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace AutobaseRESTAPIMonitor
{
    /// <summary>
    /// DataGridView 서브클래스: OnMouseDown 등 내부 메서드에서 발생하는
    /// CurrencyManager IndexOutOfRangeException(-1)이 전역으로 전파되지 않도록 방지.
    ///
    /// 원인: List&lt;T&gt; → BindingSource 바인딩 시 CurrencyManager 위치 동기화 실패로
    /// MakeFirstDisplayedCellCurrentCell → CurrencyManager.get_Item(-1) 예외 발생.
    /// DataError 이벤트로는 잡을 수 없음 (OnMouseDown 내부에서 직접 throw됨).
    /// </summary>
    public class SafeDataGridView : DataGridView
    {
        protected override void OnMouseDown(MouseEventArgs e)
        {
            try
            {
                base.OnMouseDown(e);
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.WriteLine($"[SafeDataGridView] OnMouseDown suppressed: {ex.Message}");
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            try
            {
                base.OnMouseUp(e);
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.WriteLine($"[SafeDataGridView] OnMouseUp suppressed: {ex.Message}");
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            try
            {
                base.OnMouseClick(e);
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.WriteLine($"[SafeDataGridView] OnMouseClick suppressed: {ex.Message}");
            }
        }
    }
}

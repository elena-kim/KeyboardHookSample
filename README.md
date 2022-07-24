# KeyboardHookSample
`user32.dll`을 사용한 Keyboard Hook 샘플입니다.


### Dll Import
```csharp
[DllImport("user32.dll")]
static extern IntPtr SetWindowsHookEx(int idHook, KeyboardProc callback, IntPtr hInstance, int threadId);

[DllImport("user32.dll")]
static extern bool UnhookWindowsHookEx(IntPtr hInstance);

[DllImport("user32.dll")]
static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
private static extern int GetCurrentThreadId();

[DllImport("kernel32.dll")]
static extern IntPtr LoadLibrary(string lpFileName);
```

### SetHook
> [`SetWindowsHookEx` 파라미터 목록](https://docs.microsoft.com/ko-kr/windows/win32/api/winuser/nf-winuser-setwindowshookexa?redirectedfrom=MSDN#parameters)

실행 중인 어플리케이션에서만 Keyboard Hook을 하기 위해 `SetWindowsHookEx` 메서드의 첫번째 파라미터인 idHook에 `WH_KEYBOARD`을 넘겨주었다. 
Windows 전역에서 Keyboard Hook을 하기 위해서는 idHook에 `WH_KEYBOARD_LL`을 파라미터로 줘야한다. `WH_KEYBOARD`와 `WH_KEYBOARD_LL`는 각각 hook procedure가 다르므로 주의해야 한다.

||`WH_KEYBOARD`|`WH_KEYBOARD_LL`|
|:---:|:----:|:-----:|
|Value|2|13|
|Scope|Thread or global|Global only|
|Hook Procedure|[KeyboardProc][KeyboardProc]|[LowLevelKeyboardProc][LowLevelKeyboardProc]|

[KeyboardProc]: https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644984(v=vs.85)
[LowLevelKeyboardProc]: https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644985(v=vs.85)

```csharp
private const int WH_KEYBOARD = 2;

public void SetHook()
{
    IntPtr hInstance = LoadLibrary("User32");
    hHook = SetWindowsHookEx(WH_KEYBOARD, _proc, hInstance, GetCurrentThreadId());
}
```

### Hook Procedure
`lParam`의 [`KF_REPEAT` 플래그](https://docs.microsoft.com/en-us/windows/win32/inputdev/about-keyboard-input?redirectedfrom=MSDN#keystroke-message-flags)를 통해 반복 호출을 피한다.

```csharp
private const int HC_ACTION = 0;
private const int KF_REPEAT = 0x4000;

private IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam)
{
    if (code == HC_ACTION)
    {
        var repeat = HiWord(lParam) & KF_REPEAT;
        
        if (repeat == 0)
        {
            Debug.WriteLine($"{code} / {wParam} / {lParam}");

            switch ((int)wParam)
            {
                case VK_F1:
                    this.txb.Text += "F1 was pressed" + Environment.NewLine; break;
                case VK_F2:
                    this.txb.Text += "F2 was pressed" + Environment.NewLine; break;
                case VK_F3:
                    this.txb.Text += "F3 was pressed" + Environment.NewLine; break;
                case VK_F4:
                    this.txb.Text += "F4 was pressed" + Environment.NewLine; break;
                case VK_F5:
                    this.txb.Text += "F5 was pressed" + Environment.NewLine; break;
                case VK_F6:
                    this.txb.Text += "F6 was pressed" + Environment.NewLine; break;
            }
        }
    }
    return CallNextHookEx(hHook, code, (int)wParam, lParam);
}

private static ulong HiWord(IntPtr ptr)
{
    if (((ulong)ptr & 0x80000000) == 0x80000000)
        return ((ulong)ptr >> 16);
    else
        return ((ulong)ptr >> 16) & 0xffff;
}
```

### Application
F1부터 F6를 차례로 눌렀을 때 텍스트박스에 미리 설정해 둔 텍스트가 입력된다.

<img width="396" alt="image" src="https://user-images.githubusercontent.com/74305823/180645499-f165fe04-3bee-417f-8b0f-ba016d49d3cd.png">

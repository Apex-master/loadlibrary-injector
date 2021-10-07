Public Class Form1
    Public Declare Function WriteProcessMemory Lib "kernel32" (ByVal hProcess As Integer, ByVal lpBaseAddress As Integer, ByVal lpBuffer As Byte(), ByVal nSize As Integer, ByRef lpNumberOfBytesWritten As Integer) As Integer
    Public Declare Function VirtualAllocEx Lib "kernel32" (ByVal hProcess As Integer, ByVal lpAddress As Integer, ByVal dwSize As Integer, ByVal flAllocationType As Integer, ByVal flProtect As Integer) As Integer
    Public Const MEM_COMMIT = 4096, PAGE_EXECUTE_READWRITE = &H40
    Public Declare Function GetProcAddress Lib "kernel32" (ByVal hModule As Integer, ByVal lpProcName As String) As Integer
    Private Declare Function GetModuleHandle Lib "Kernel32" Alias "GetModuleHandleA" (ByVal lpModuleName As String) As Integer
    Public Declare Function CreateRemoteThread Lib "kernel32" (ByVal hProcess As Integer, ByVal lpThreadAttributes As Integer, ByVal dwStackSize As Integer, ByVal lpStartAddress As Integer, ByVal lpParameter As Integer, ByVal dwCreationFlags As Integer, ByRef lpThreadId As Integer) As Integer

    Private Sub MaterialRaisedButton4_Click(sender As Object, e As EventArgs) Handles MaterialRaisedButton4.Click
        Dim DllPath As String = Application.StartupPath + "/FlatSkin.dll"

        If (Process.GetProcessesByName(ComboBox1.Text).Length = 0) Then
            Label1.Text = "Can not find process " + ComboBox1.Text
            Exit Sub
        End If

        Dim TargetHandle As IntPtr = Process.GetProcessesByName(ComboBox1.Text)(0).Handle
        If (TargetHandle.Equals(IntPtr.Zero)) Then
            Label1.Text = "The Process " + ComboBox1.Text + " Has Failed!"
            Exit Sub
        End If

        Dim GetAdrOfLLBA As IntPtr = GetProcAddress(GetModuleHandle("Kernel32"), "LoadLibraryA")
        If (GetAdrOfLLBA.Equals(IntPtr.Zero)) Then
            Label1.Text = "Obtained LoadLibraryA API function base address failed!"
            Exit Sub
        End If

        Dim OperaChar As Byte() = System.Text.Encoding.Default.GetBytes(DllPath)

        Dim DllMemPathAdr = VirtualAllocEx(TargetHandle, 0&, &H64, MEM_COMMIT, PAGE_EXECUTE_READWRITE)
        If (DllMemPathAdr.Equals(IntPtr.Zero)) Then
            Label1.Text = "An error occurred while attempting to inject " + ComboBox1.Text + " Error writing to memory!."
            Exit Sub
        End If

        If (WriteProcessMemory(TargetHandle, DllMemPathAdr, OperaChar, OperaChar.Length, 0) = False) Then
            Label1.Text = "An error occurred while requesting space for process " + ComboBox1.Text + " Error writing to memory!"
            Exit Sub
        End If

        CreateRemoteThread(TargetHandle, 0, 0, GetAdrOfLLBA, DllMemPathAdr, 0, 0)
        Label1.Text = ComboBox1.Text + " Injection completed!"
    End Sub

    Private Sub ComboBoxDropDown(ByVal sender As Object, ByVal e As System.EventArgs) Handles ComboBox1.DropDown
        CType(sender, ComboBox).Items.Clear()
        For Each p As Process In Process.GetProcesses
            CType(sender, ComboBox).Items.Add(p.ProcessName)
        Next
    End Sub

    Private Sub Label8_Click(sender As Object, e As EventArgs) Handles Label8.Click
        Me.Close()
    End Sub
End Class

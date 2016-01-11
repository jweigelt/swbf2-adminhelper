'Memory-access class
Imports System.Runtime.InteropServices
Public Class ProcessMemoryReader

    'Load some kernelapi-funcs:
    <DllImport("kernel32.dll")> _
    Private Shared Function OpenProcess( _
      ByVal dwDesiredAccess As Integer,
      ByVal bInheritHandle As Integer,
      ByVal dwProcessId As Integer) As IntPtr
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)> _
    Private Shared Function CloseHandle(ByVal hObject As IntPtr) As Boolean
    End Function

    <DllImport("kernel32.dll")> _
    Private Shared Function ReadProcessMemory( _
       ByVal hProcess As IntPtr, _
       ByVal lpBaseAddress As IntPtr, _
       <Out()> ByVal lpBuffer As Byte(), _
       ByVal dwSize As Integer, _
       ByRef lpNumberOfBytesRead As Integer) As Boolean
    End Function

    <DllImport("kernel32.dll")> _
    Private Shared Function WriteProcessMemory( _
        ByVal hProcess As IntPtr, _
        ByVal lpBaseAddress As IntPtr, _
        ByVal lpBuffer As Byte(), _
        ByVal nSize As UInt32, _
        ByRef lpNumberOfBytesWritten As UInt32) As Boolean
    End Function

    Public Property ApplicationPath As String = ""

    Private proc As Process
    Private procHwnd As IntPtr
    Private addrBuffer(63, 5) As Int32

    Private Const PROC_RW As Int32 = &H1F0FFF

    Public Function Init() As Boolean
        Dim _pid As Integer = Nothing
        If Not IO.File.Exists(Me.ApplicationPath) Then
            Logger.Log(LogTemplate.PROC_NO_APP, LogLevel.critical, ApplicationPath)
            Return False
        End If

        'Scan for matching procs
        For Each proc As Process In Process.GetProcessesByName("BattlefrontII")
            If proc.MainModule.FileName.ToLower.Contains(Me.ApplicationPath.ToLower) Then 'Startparameter ignorieren
                Logger.Log(LogTemplate.PROC_FOUND, LogLevel.info, proc.Id.ToString(), proc.ProcessName)
                Me.proc = proc
            End If
        Next

        'No process found?
        If Me.proc Is Nothing Then
            Logger.Log(LogTemplate.PROC_NO_PROC, LogLevel.critical, ApplicationPath)
            Return False
        End If

        'Open the process and get our handle
        Logger.Log(LogTemplate.PROC_OPEN, LogLevel.info, ApplicationPath)
        Me.procHwnd = OpenProcess(PROC_RW, False, Me.proc.Id)

        'handle = 0 is a clear sign of failure
        If procHwnd = 0 Then
            Logger.Log(LogTemplate.PROC_OPENPROC_FAIL, LogLevel.critical, ApplicationPath)
            Return False
        End If

        Logger.Log(LogTemplate.PROC_OPENPROC_OK, LogLevel.info, ApplicationPath)

        'Calculate the memory-addresses now and buffer them so we can access them fast later on
        Me.FillAddressBuffer()

        Return True
    End Function

    Public Sub Terminate()
        'Close our process
        If Me.procHwnd <> 0 Then
            Logger.Log(LogTemplate.PROC_CLOSE, LogLevel.info)
            CloseHandle(Me.procHwnd)
        End If
        Me.proc = Nothing
    End Sub

    'Admin-pwd set and read
    Public Function ReadAdminPassword() As String
        Return Me.ReadString(Constants.MEM_1_1_ADMINPASSWORD)
    End Function

    Public Sub SetAdminPassword(ByVal password As String)
        Me.WriteString(Constants.MEM_1_1_ADMINPASSWORD, password)
    End Sub

    Public Sub DisableIngameLogin()
        'Clearing the memory-region setting the pwd to "null" as null can't be
        '"extracted" using split() it becomes impossible to log in
        Me.ClearMem(Constants.MEM_1_1_ADMINPASSWORD, 255)
    End Sub



    Private Sub FillAddressBuffer()

        'Old v1.0 values
        If Constants.V1_0 Then
            addrBuffer(0, 0) = &HC02F68
            addrBuffer(0, 1) = &HC02FF8
            addrBuffer(0, 2) = &HBEAAC0
            addrBuffer(0, 3) = &HBEAAD8
            addrBuffer(0, 4) = &HBEAADC
            addrBuffer(0, 5) = &HC02FD8
            For i = 1 To 63
                addrBuffer(i, 0) = addrBuffer(i - 1, 0) + 612
                addrBuffer(i, 1) = addrBuffer(i - 1, 1) + 612
                addrBuffer(i, 2) = addrBuffer(i - 1, 2) + 92
                addrBuffer(i, 3) = addrBuffer(i - 1, 3) + 92
                addrBuffer(i, 4) = addrBuffer(i - 1, 4) + 92
                addrBuffer(i, 5) = addrBuffer(i - 1, 5) + 612
            Next
        Else
            'The players are stored as specific structures,
            'these usually have a specific baseaddress, the variables
            'are then "fitted behind the base" so we can calulate
            'their position by adding certain offsets
            'they appearently used two memory-regions to store the playerdata

            addrBuffer(0, 0) = Constants.MEM_1_1_CLIENT_BASE             'Name
            addrBuffer(0, 1) = Constants.MEM_1_1_CLIENT_BASE + &H90      'Keyhash
            addrBuffer(0, 2) = Constants.MEM_1_1_PLAYER_BASE             'IP
            addrBuffer(0, 3) = Constants.MEM_1_1_PLAYER_BASE + &H18      'Ping
            addrBuffer(0, 4) = Constants.MEM_1_1_PLAYER_BASE + &H1C      'Stats
            addrBuffer(0, 5) = Constants.MEM_1_1_CLIENT_BASE + &H70      'TeamID

            For i = 1 To 63
                addrBuffer(i, 0) = addrBuffer(i - 1, 0) + 612 + 4
                addrBuffer(i, 1) = addrBuffer(i - 1, 1) + 612 + 4
                addrBuffer(i, 2) = addrBuffer(i - 1, 2) + 92
                addrBuffer(i, 3) = addrBuffer(i - 1, 3) + 92
                addrBuffer(i, 4) = addrBuffer(i - 1, 4) + 92
                addrBuffer(i, 5) = addrBuffer(i - 1, 5) + 612 + 4
            Next
        End If
    End Sub

#Region "Writing and Reading"
    'Just some ReadProcessMemory() and WriteProcessMemory - Wrappers for different datatypes
    Public Function ReadByte(ByVal address As Int32) As Byte
        Dim buf(0) As Byte
        ReadProcessMemory(Me.procHwnd, address, buf, buf.Length, 0)
        Return buf(0)
    End Function
    Public Function ReadInt32(ByVal address As Int32) As Int32
        Dim buf(3) As Byte
        ReadProcessMemory(Me.procHwnd, address, buf, 4, 0)
        Return BitConverter.ToInt32(buf, 0)
    End Function
    Public Function ReadFloat(ByVal address As Int32) As Single
        Dim buf(3) As Byte
        ReadProcessMemory(Me.procHwnd, address, buf, 4, 0)
        Return BitConverter.ToSingle(buf, 0)
    End Function
    Public Function ReadString(ByVal address As Int32) As String
        Dim Buffer(256) As Byte

        ReadProcessMemory(Me.procHwnd, address, Buffer, Buffer.Length, 0)
        Dim b(0) As Byte
        For i = 0 To Buffer.Length - 1
            If Not Buffer(i) = 0 Then
                Array.Resize(b, b.Length + 1)
                b(i) = Buffer(i)
            Else
                Array.Resize(b, b.Length - 1)
                Exit For
            End If
        Next
        Dim ma As String = ArrayFunctions.GetString(b)
        Return ma
    End Function
    Public Function ReadAddress(ByVal base As Int32, Optional ByVal offset As Int32 = 0) As Integer
        Dim address As Int32 = Me.proc.MainModule.BaseAddress + base
        address = Me.ReadInt32(address)
        address += offset
        Return address
    End Function
    Public Function ReadIPAddress(ByVal address As Int32) As Net.IPAddress
        Dim buf(3) As Byte
        ReadProcessMemory(Me.procHwnd, address, buf, buf.Length, 0)
        Return New Net.IPAddress(buf)
    End Function
    Public Function ReadPlayer(ByVal playerId As Byte) As User
        Dim player As New User
        With (player)
            .SlotId = playerId
            playerId -= 1
            .UserName = Me.ReadString(Me.addrBuffer(playerId, 0))
            .KeyHash = Mid(Me.ReadString(Me.addrBuffer(playerId, 1)), 1, 32)
            .IPAddress = Me.ReadIPAddress(Me.addrBuffer(playerId, 2))
            .Ping = Me.ReadByte(Me.addrBuffer(playerId, 3))
            .Points = Me.ToSByte(Me.ReadByte(Me.addrBuffer(playerId, 4)))
            .TeamId = Me.ReadByte(Me.addrBuffer(playerId, 5))
        End With
        Return player
    End Function

    Public Sub ClearMem(ByVal address As Int32, ByVal len As Int32)
        Dim buf(len - 1) As Byte
        WriteProcessMemory(Me.procHwnd, address, buf, buf.Length, 0)
    End Sub
    Public Sub WriteString(ByVal address As Int32, ByVal value As String)
        Dim buf(value.Length - 1) As Byte
        buf = ArrayFunctions.GetBytes(value)
        WriteProcessMemory(Me.procHwnd, address, buf, buf.Length, 0)
    End Sub
    Public Sub WriteSingle(ByVal address As IntPtr, ByVal value As Single)
        Dim buf() As Byte = BitConverter.GetBytes(value)
        WriteProcessMemory(Me.procHwnd, address, buf, buf.Length, 0)
    End Sub
#End Region

    'Converting a C-Style "signed byte" to a .NET SByte
    Private Function ToSByte(ByVal value As Byte) As SByte
        If value < 127 Then
            Return value
        Else
            Return -(256 - value)
        End If
    End Function
End Class
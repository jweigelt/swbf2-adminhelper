'This file is part of SWBF2 SADS-Administation Helper.
'
'SWBF2 SADS-Administation Helper is free software: you can redistribute it and/or modify
'it under the terms of the GNU General Public License as published by
'the Free Software Foundation, either version 3 of the License, or
'(at your option) any later version.

'SWBF2 SADS-Administation Helper is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License for more details.

'You should have received a copy of the GNU General Public License
'along with SWBF2 SADS-Administation Helper.  If not, see <http://www.gnu.org/licenses/>.

'SWBF2-SADS Remote Console Client
Imports System.Security.Cryptography
Imports System.Text
Imports System.Net.Sockets
Public Class RconClient

    Public Property ServerIP As String
    Public Property ServerPort As Integer
    Public Property ServerPwd As String

    Public Property PacketTimeout As Int32 = 2000

    Private listenThread As Threading.Thread
    Private client As TcpClient
    Private stream As NetworkStream

    Private lastResponse As String = String.Empty

    Public Event NewChat(ByVal sender As Object, ByVal user As String, ByVal chat As String)
    Public Event GameEnded(ByVal sender As Object)

    Private Function Login()
        Logger.Log(LogTemplate.RCON_LOGIN, LogLevel.info)
        Dim buffer() As Byte
        'The password is encrypted using plain md5 which is pretty pointless as you can log in by using the hash
        buffer = ArrayFunctions.GetBytes(MD5StringHash(Me.ServerPwd).ToLower)

        '0x64 terminates the login-packet 
        Array.Resize(buffer, buffer.Length + 1)
        buffer(buffer.Length - 1) = 100

        Me.stream.Write(buffer, 0, buffer.Length)

        Dim res(0) As Byte
        Me.stream.Read(res, 0, 1)

        'The Server will return 1 byte, 1->ok, 0->fail
        If res(0) = 1 Then
            Logger.Log(LogTemplate.RCON_LOGIN_OK, LogLevel.info)
            Return True
        End If
        Logger.Log(LogTemplate.RCON_LOGIN_FAIL, LogLevel.critical)
        Return False
    End Function

    Public Function MD5StringHash(ByVal strString As String) As String
        Dim MD5 As New MD5CryptoServiceProvider
        Dim Data As Byte()
        Dim Result As Byte()
        Dim Res As String = String.Empty
        Dim Tmp As String = String.Empty

        Data = ArrayFunctions.GetBytes(strString)
        Result = MD5.ComputeHash(Data)
        For i As Integer = 0 To Result.Length - 1
            Tmp = Hex(Result(i))
            If Len(Tmp) = 1 Then Tmp = "0" & Tmp
            Res += Tmp
        Next
        Return Res
    End Function

    Public Sub SendRaw(ByVal raw As String)
        Dim gpc As New GenericCommandPacket
        gpc.CommandAlias = raw
        Me.SendPacket(gpc)
    End Sub

    Private Sub Send(ByVal command As String)
        Logger.Log(LogTemplate.RCON_SENDING, LogLevel.debug, command)

        'Encode the string
        Dim cmdbytes() As Byte = ArrayFunctions.GetBytes(command)

        'Shift the data
        Dim buffer(cmdbytes.Length + 2) As Byte
        For i = 0 To cmdbytes.Length - 1
            buffer(i + 2) = cmdbytes(i)
        Next

        'We only send commands consisting out of 1 row
        buffer(0) = 1                   'rows
        buffer(1) = command.Length + 1  'chars

        'push it
        Try
            stream.Write(buffer, 0, buffer.Length)
            Logger.Log(LogTemplate.RCON_SENDING_OK, LogLevel.debug, command)
        Catch ex As Exception
            Logger.Log(LogTemplate.RCON_SENDING_FAIL, LogLevel.warning, command)
            'Me.terminate()
        End Try
    End Sub
    Public Sub Terminate()
        If Not IsNothing(listenThread) Then
            If listenThread.IsAlive Then
                listenThread.Abort()
            End If
            listenThread = Nothing
        End If
        If Not IsNothing(Me.stream) Then
            Me.stream.Close()
            Me.stream = Nothing
        End If
        If Not IsNothing(Me.client) Then
            Me.client.Close()
            Me.client = Nothing
        End If
        Logger.Log(LogTemplate.RCON_CLOSE, LogLevel.info)
    End Sub
    Private Sub HandleRconEvent(ByVal str As String)
        If str.Length > 0 Then
            str = Mid(str, 1, str.Length - 2)
        Else
            Return
        End If

        If str(0) = vbTab Then
            str = Replace(str, Chr(0), String.Empty)

            Dim splitStr() As String = Split(str, vbTab)

            If splitStr.Length >= 2 Then
                RaiseEvent NewChat(Me, splitStr(1), splitStr(2))
                Logger.Log(LogTemplate.RCON_CHAT, LogLevel.info, splitStr(1), splitStr(2))
            End If
        Else
            If Not Me.StatusMessage(str) Then Me.lastResponse = str
            Logger.Log(LogTemplate.RCON_ANSWER, LogLevel.debug, str)
        End If
    End Sub

    Private Function StatusMessage(ByVal msg As String) As Boolean
        Select Case msg
            Case "Game has ended"
                RaiseEvent GameEnded(Me)
                Return True
            Case Else
                Return False
        End Select
    End Function


    Private Sub WaitForPackets()
        'vars
        Dim buf(0) As Byte
        Dim str As String = String.Empty

        While True
            Try
                'We read the rowcount-byte first
                'The data gets fragmented as they push the length first
                Array.Resize(buf, 1)
                Me.stream.Read(buf, 0, 1)

                'Save it to a seperate var
                Dim rowCount As Byte = buf(0)

                For i = 0 To rowCount - 1
                    'Wait for the next push, we just read one byte again
                    While Me.stream.Read(buf, 0, 1) = 0
                        Threading.Thread.Sleep(10)
                    End While

                    'Save it to a seperate var
                    Dim rowLen As Byte = buf(0)

                    'The data is fragmented so we need to keep track on how many bytes we read
                    Dim bytesRead As Byte = 0

                    'Allocate memory to fetch the row
                    Array.Resize(buf, rowLen)

                    'And start fetching the data
                    While bytesRead < rowLen
                        bytesRead += Me.stream.Read(buf, bytesRead, rowLen - bytesRead)
                    End While

                    'a row is terminated by a Nullchar (C-Style strings)
                    'so we just trim it off
                    Array.Resize(buf, rowLen - 1)

                    'Attach the row to our response-string
                    str &= GetString(buf) & vbCrLf
                Next
                Me.HandleRconEvent(str)
                str = String.Empty
                Threading.Thread.Sleep(10)
            Catch ex As Exception
                If Not TypeOf (ex) Is System.Threading.ThreadAbortException Then
                    Logger.Log(LogTemplate.RCON_DISCON, LogLevel.critical, ex.ToString)
                End If
            End Try
        End While
    End Sub

    Public Function init() As Boolean
        Me.client = New TcpClient
        Try
            Logger.Log(LogTemplate.RCON_INIT, LogLevel.info, Me._ServerIP, Me._ServerPort.ToString)
            Me.client.Connect(Me._ServerIP, Me._ServerPort)
            Me.stream = Me.client.GetStream
        Catch ex As Exception
            Logger.Log(LogTemplate.RCON_INIT_FAIL, LogLevel.critical, ex.ToString)
            Return False
        End Try
        Logger.Log(LogTemplate.RCON_INIT_OK, LogLevel.info)

        If Me.Login() Then
            Me.listenThread = New Threading.Thread(AddressOf Me.WaitForPackets)
            Me.listenThread.Start()
        Else
            Return False
        End If
        Return True
    End Function

    Public Sub SendPacket(ByRef p As TcpPacket)
        Me.lastResponse = String.Empty
        Me.Send("/" & p.CommandAlias)
        Dim runTime As Int32
        While Me.lastResponse = String.Empty And runTime < Me.PacketTimeout
            runTime += 5
            Threading.Thread.Sleep(5)
        End While
        If runTime < Me.PacketTimeout Then
            p.FetchData(Me.lastResponse)
            Me.lastResponse = String.Empty
        Else
            Logger.Log(LogTemplate.RCON_PACKET_TIMEOUT, LogLevel.debug)
        End If
    End Sub

    Public Sub Say(ByVal str As String)
        SendMessage(str, Nothing)
    End Sub

    Public Sub Warn(ByVal str As String, ByVal user As User)
        SendMessage(str, user)
    End Sub

    Public Sub SendMessage(ByVal str As String, ByVal user As User)
        'Sending more then 100 chars will crash the server
        '-> fragment it
        Dim cmd As String
        If Not user Is Nothing Then
            cmd = "/warn " & user.SlotId.ToString() & " "
        Else
            cmd = "/say "
        End If

        If str.Length <= 100 Then
            Me.Send(cmd & str)
            Exit Sub
        End If
        Dim cache As String = str
        Dim lines As New List(Of String)
        While cache.Length > 0
            If cache.Length >= 100 Then
                lines.Add(Mid(cache, 1, 100))
                cache = Mid(cache, 101)
            Else
                If cache.Length > 0 Then
                    lines.Add(cache)
                    cache = String.Empty
                End If
            End If
        End While
        For Each line As String In lines
            Me.Send(cmd & line)
        Next
    End Sub
End Class
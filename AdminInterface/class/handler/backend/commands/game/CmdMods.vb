Public Class CmdMods
    Inherits Command

    Public Property OnSet As String = "Mod %s will be %e on map restart."
    Public Property OnSyntaxError As String = "Syntax: #mods [<mod name> <on/off>] OR #mods list"
    Public Property OnInvalidMod As String = "%m is not a valid mod"
    Public Property OnModsNotSetup As String = "Mod scripts are not setup on this server."
    Public Property PythonEXE As String = "C:\Users\Administrator\AppData\Local\Programs\Python\Python35-32\python.exe"
    Public Property ModsScriptDir As String = "C:\Users\Administrator\Desktop\SWBF2Server\data\_LVL_PC\"
    Public Property ModsScriptFile As String = "apply_mods.py"

    Sub New()
        Me.CommandAlias = "mods"
        Me.Permission = "mods"
        Me.IsPublic = False
    End Sub

    Private Function SetupProcess(ByVal ModToApply As String, ByVal Revert As Boolean) As Process
        Dim p As New Process()
        Dim info As New ProcessStartInfo()
        info.FileName = PythonEXE
        info.WorkingDirectory = ModsScriptDir
        info.RedirectStandardOutput = True
        info.RedirectStandardError = True
        info.UseShellExecute = False
        info.CreateNoWindow = True
        p.StartInfo = info

        Dim args As String = ModsScriptFile & " "
        If Revert Then
            args &= "--revert "
        End If

        If ModToApply = "" Then
            args &= "--list"
        Else
            args &= ModToApply
        End If

        p.StartInfo.Arguments = args

        Return p
    End Function

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        If ModsScriptFile = "" OrElse ModsScriptDir = "" Then
            Me.Pm(OnModsNotSetup, player)
            Return False
        End If

        Dim params() As String = Split(commandStr, " ")

        If params.Length < 2 OrElse params.Length > 3 Then
            Me.Pm(Me.OnSyntaxError, player)
            Return False
        End If

        Dim p As Process
        If params.Length = 3 Then
            ' We are enabling or disabling a mod
            Dim Revert As Boolean
            If params(2) = "on" Then
                Revert = False
            ElseIf params(2) = "off" Then
                Revert = True
            Else
                Me.Pm(OnSyntaxError, player)
                Return False
            End If
            p = SetupProcess(params(1), Revert)
        ElseIf params(1) = "list" Then
            ' We are listing available mods
            p = SetupProcess("", False)
        Else
            Me.Pm(OnSyntaxError, player)
            Return False
        End If

        Dim response As String = ""

        p.Start()
        While Not p.StandardOutput.EndOfStream
            response &= p.StandardOutput.ReadLine()
        End While

        If response <> "" Then
            If params(1) = "list" Then
                Me.Pm(response, player)
            Else
                Dim ApplyOrRevert As String
                If params(2) = "on" Then
                    ApplyOrRevert = "applied"
                Else
                    ApplyOrRevert = "reverted"
                End If
                Me.Pm(Me.ParseTemplate(Me.OnSet, {response, ApplyOrRevert}, {"s", "e"}), player)
            End If
        End If

        Return True
    End Function

End Class

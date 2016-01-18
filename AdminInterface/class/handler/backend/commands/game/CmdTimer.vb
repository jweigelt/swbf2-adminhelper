Public Class CmdTimer
    Inherits Command

    Public Property OnSyntaxError As String = "Syntax: !timer <minutes> (0 for disable)"
    Public Property OnValueError As String = "Max Timer: 60 Minutes"
    Public Property OnSuccess As String = "Timer set to %s minutes"


    Sub New()
        Me.CommandAlias = "timer"
        Me.Permission = "timer"
        Me.IsPublic = False
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Dim params() As String = Split(commandStr, " ")

        If params.Length <> 2 Then
            Me.Pm(Me.OnSyntaxError, player)
            Return False
        End If

        Dim val As Int32 = Convert.ToInt32(params(1))

        If val > 60 Or val < 0 Then
            Me.Pm(Me.OnValueError, player)
        End If

        Dim mode As String = Me.adminIface.Config.ServerInfo.CurrentMap.Split("_")(1)
        If mode = "eli" Then
            mode = "ctf"
        End If
        Dim command As String = mode & "timelimit " & params(1)

        Me.adminIface.RCClient.SendRaw(command)

        Me.Say(Me.ParseTemplate(Me.OnSuccess,
            {val}, {"s"}
        ))

        Return True
    End Function
End Class

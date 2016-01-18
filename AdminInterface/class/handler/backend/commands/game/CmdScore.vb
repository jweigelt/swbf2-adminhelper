Public Class CmdScore
    Inherits Command

    Public Property StatsStr As String = "Points: %p, Kills: %k, Deaths: %d, K/D: %r" '"You are User %u, Slot %s, User-ID: %i, Group: %g"

    Public Property OnSyntaxError As String = "Syntax: !score <points>"
    Public Property OnMaxScoreError As String = "Max Score Limit: %p Points"
    Public Property OnMinScoreError As String = "Min Score Limit: 1 Point"
    Public Property OnSuccess As String = "Score limit set to %p points"


    Sub New()
        Me.CommandAlias = "score"
        Me.Permission = "score"
        Me.IsPublic = False
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Dim params() As String = Split(commandStr, " ")

        If params.Length <> 2 Then
            Me.Pm(Me.OnSyntaxError, player)
            Return False
        End If

        Dim val As Int32 = Convert.ToInt32(params(1))

        Dim mode As String = Me.adminIface.Config.ServerInfo.CurrentMap.Split("_")(1)
        If mode = "eli" Then
            mode = "ass"
        End If

        Dim maxScore As Integer
        If mode = "ass" Then
            maxScore = 500
        ElseIf mode = "ctf" Then
            maxScore = 15
        ElseIf mode = "hunt" Then
            maxScore = 150
        End If

        If val > maxScore Then
            Me.Pm(Me.ParseTemplate(Me.OnMaxScoreError,
                {val}, {"p"}
            ), player)
        ElseIf val < 1 Then
            Me.Pm(Me.OnMinScoreError, player)
        End If


        Dim command As String = mode & "scorelimit " & params(1)

        Me.adminIface.RCClient.SendRaw(command)

        Me.Say(Me.ParseTemplate(Me.OnSuccess,
            {val}, {"p"}
        ))

        Return True
    End Function
End Class

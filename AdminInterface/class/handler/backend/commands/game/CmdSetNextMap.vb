Public Class CmdSetNextMap
    Inherits Command

    Public Property OnSet As String = "Next map was set to %m."
    Public Property OnSyntaxError As String = "Syntax: !setnextmap <mapname>"

    Sub New()
        Me.CommandAlias = "setnextmap"
        Me.Permission = "setnextmap"
        Me.IsPublic = False
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Dim params() As String = Split(commandStr, " ")
        Dim map As String = Replace(params(1), "/", String.Empty)
        If params.Length >= 2 Then
            Me.adminIface.RCClient.SendRaw("removemap " & map)
            Me.adminIface.RCClient.SendRaw("addmap " & map)
            'Me.adminIface.RCClient.SendRaw("nextmap " & map)
            Me.Say(Me.ParseTemplate(OnSet, {map}, {"m"}))
            Me.adminIface.Config.ServerInfo.NextMap = map
            Return True
        Else
            Me.Say(Me.OnSyntaxError)
            Return False
        End If
    End Function
End Class
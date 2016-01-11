'command baseclass
Public Class Command
    Public Property Permission As String
    Public Property IsPublic As Boolean = False
    Public Property CommandAlias As String

    <Xml.Serialization.XmlIgnore> _
    Public Property adminIface As Core

    Public Overridable Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Return True
    End Function

    Public Sub Say(ByVal str As String)
        adminIface.RCClient.SendRaw("say " & str)
    End Sub

    Public Sub Pm(ByVal str As String, ByVal u As User)
        adminIface.RCClient.SendRaw("warn " & u.SlotId.ToString() & " " & str)
    End Sub

    Friend Function ParseTemplate(ByVal template As String, ByVal replacements() As String, Optional ByVal tags() As String = Nothing) As String
        Return IngameTemplate.Parse(template, replacements, tags)
    End Function

End Class

'Autoannounce Configuration
Imports System.Xml.Serialization
Public Class AnnounceConfig
    Public Property AnnounceInterval As Int32 = 60000
    Public Property EnableVarParser As Boolean = True

    <XmlArray("AnnounceList")>
    <XmlArrayItem("Announce")>
    Public Property AnnounceList As List(Of String)

    Sub New()
        Me.AnnounceList = New List(Of String)
    End Sub
End Class

﻿'Core configuration
Public Class CoreConfig

    Public Property MinLogLevel As Int32 = 1
    Public Property LogToFile As Boolean = True
    Public Property LogFile As String = "log.txt"
    Public Property ExitOnError As Boolean = True

    Public Property RconHostname As String = "localhost"
    Public Property RconPort As UInt16 = 4658
    Public Property RconPassword As String = "123"
    Public Property RconPlayerHandling As Boolean = False

    Public Property LoginAutoFetch As Boolean = False
    Public Property LoginDisable As Boolean = False

    Public Property ApplicationPath As String = "C:\BattlefrontII\BattlefrontII.exe"

    Public Property MySQLHostname As String = "localhost"
    Public Property MySQLPort As UInt16 = 3306
    Public Property MySQLUser As String = "root"
    Public Property MySQLPassword As String = ""
    Public Property MySQLDatabase As String = "adminmod"

    Public Property ListRefreshDelay As Int32 = 5000
    Public Property CommandPrefix As String = "!"

    <Xml.Serialization.XmlIgnore> _
    Public Property ServerInfo As ServerInformation

    <Xml.Serialization.XmlIgnore> _
    Public ReadOnly Property MemReaderRequired As Boolean
        Get
            Return (Not Me.RconPlayerHandling Or Me.LoginAutoFetch Or Me.LoginDisable)
        End Get
    End Property
End Class

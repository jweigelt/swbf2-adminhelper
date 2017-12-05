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
'
'Core configuration
Public Class CoreConfig

    Public Property MinLogLevel As Int32 = 1
    Public Property LogToFile As Boolean = True
    Public Property LogFile As String = "log.txt"
    Public Property ExitOnError As Boolean = True

    Public Property RconHostname As String = "localhost"
    Public Property RconPort As UInt16 = 4658
    Public Property RconPassword As String = "1234"
    Public Property RconPlayerHandling As Boolean = False

    Public Property LoginAutoFetch As Boolean = False
    Public Property LoginDisable As Boolean = False

    Public Property ApplicationPath As String = "C:\BattlefrontII\BattlefrontII.exe"

    Public Property MySQLHostname As String = "localhost"
    Public Property MySQLPort As UInt16 = 3306
    Public Property MySQLUser As String = "root"
    Public Property MySQLPassword As String = ""
    Public Property SQLDatabase As String = "adminmod"
    Public Property UseMySQL As Boolean = False

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

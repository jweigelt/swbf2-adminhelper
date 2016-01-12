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

Public Class IngameTemplate
    Public Property EnableOnRegisteredUserJoin As Boolean = True
    Public Property EnableOnRegisteredPlayerJoin As Boolean = True
    Public Property EnableOnNewPlayerJoin As Boolean = True
    Public Property EnableOnBannedPlayerJoin As Boolean = True

    Public Property OnRegisteredUserJoin As String = "Welcome back [%g] %u!"
    Public Property OnRegisteredPlayerJoin As String = "Welcome back %u"
    Public Property OnNewPlayerJoin As String = "Welcome Player %u #%i on the server!"
    Public Property OnBannedPlayerJoin As String = "Booting Player %u (banned)."

    Public Shared Function Parse(ByVal template As String, ByVal params() As String, Optional ByVal tags() As String = Nothing) As String
        If tags Is Nothing Then
            For Each str As String In params
                If template.Contains("%s") Then
                    template = Replace(template, "%s", str, 1, 1)
                End If
            Next
        Else
            Dim i As Int32 = 0
            For Each tag As String In tags
                If i > params.Length - 1 Then i = 0
                template = Replace(template, "%" & tag, params(i))
                i += 1
            Next
        End If
        Return template
    End Function
End Class

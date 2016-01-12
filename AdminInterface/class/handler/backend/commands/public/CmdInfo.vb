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
Public Class CmdInfo
    Inherits Command

    Sub New()
        Me.CommandAlias = "info"
        Me.Permission = "info"
        Me.IsPublic = True
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Me.adminIface.RCClient.SendRaw("say " & Constants.PRODUCT_NAME & " v" & Constants.PRODUCT_VERSION)

        Me.adminIface.RCClient.SendRaw("say " & Constants.PRODUCT_VENDOR)
        Return True
    End Function
End Class
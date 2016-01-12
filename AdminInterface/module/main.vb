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

'SWBF2 SADS-Administation Helper v3.0
'Star Wars Battlefront II SADS-Administation Helper
'(C) 2012-2015 JW "LeKeks" :: info@janelo.net
'Compile using .NET-FW 4 < / 4 C.P. < (any cpu)
'Requires mysql.data v6.6.5.0, other versions can be used if the reference is updated
'You'll need to use a local copy of the DLL if the GAC's version differs!

Module main

    Sub Main()
        Logger.Log(Constants.PRODUCT_NAME & " " & Constants.PRODUCT_VERSION, LogLevel.always)
        Logger.Log(Constants.PRODUCT_VENDOR, LogLevel.always)
        Dim c As New Core
        c.Run()

        Console.ReadLine()
    End Sub

End Module
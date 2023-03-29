Imports Microsoft.Win32
Imports System.IO
Imports System.Diagnostics
Imports System.Security

Public Class Form1
    Private totalSpaceSaved As Long = 0
    Private tempPath As String = Path.GetTempPath()
    Private prefetchPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch")

    Private Sub btnClean_Click(sender As Object, e As EventArgs) Handles btnClean.Click
        ' Initialize progress bar
        pbarProgress.Minimum = 0
        pbarProgress.Maximum = 6
        pbarProgress.Value = 0

        ' Clean up registry settings
        PerformCleanup(AddressOf CleanRegistry, "Registry cleanup")
        PerformCleanup(AddressOf CleanTempFiles, "Temporary files cleanup")
        PerformCleanup(AddressOf CleanPrefetch, "Prefetch folder cleanup")

        If chkRemovedSoftware.Checked Then
            PerformCleanup(AddressOf CleanRemovedSoftwareEntries, "Prematurely removed software registry entries cleanup")
        End If

        If chkInvalidDlls.Checked Then
            PerformCleanup(AddressOf CleanInvalidDlls, "Invalid shared DLLs cleanup")
        End If

        If chkProgramRegistryEntries.Checked Then
            PerformCleanup(AddressOf CleanProgramRegistryEntries, "Program and application registry entries cleanup")
        End If

        ' Display completion message with total space saved
        MessageBox.Show($"Cleanup completed successfully.{Environment.NewLine}{Environment.NewLine}Total disk space saved: {FormatBytes(totalSpaceSaved)}")
    End Sub

    Private Sub PerformCleanup(cleanupAction As Func(Of Long), operationName As String)
        Try
            totalSpaceSaved += cleanupAction()
            pbarProgress.Value += 1
        Catch ex As Exception
            MessageBox.Show($"An error occurred during {operationName}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function FormatBytes(bytes As Long) As String
        Dim sizes() As String = {"B", "KB", "MB", "GB", "TB"}
        Dim i As Integer = 0
        While bytes >= 1024 AndAlso i < sizes.Length - 1
            bytes /= 1024
            i += 1
        End While
        Return $"{bytes:n0} {sizes(i)}"
    End Function

    Private Function CleanProgramRegistryEntries() As Long
        ' Open the Programs registry key
        Dim programsKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", True)

        ' Delete any keys that don't have an uninstall command
        Dim deletedKeys As New List(Of String)
        For Each subKeyName As String In programsKey.GetSubKeyNames()
            Dim subKey As RegistryKey = programsKey.OpenSubKey(subKeyName)
            If subKey IsNot Nothing Then
                Dim uninstallCommand As String = TryCast(subKey.GetValue("UninstallString"), String)
                If String.IsNullOrEmpty(uninstallCommand) Then
                    programsKey.DeleteSubKeyTree(subKeyName)
                    deletedKeys.Add(subKeyName)
                End If
                subKey.Close()
            End If
        Next

        ' Close the registry key
        programsKey.Close()

        ' Add deleted keys to the ListBox
        For Each deletedKey As String In deletedKeys
            lbxRemoved.Items.Add("Registry key deleted: HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" & deletedKey)
        Next

        ' Calculate total disk space saved
        Dim totalSpaceSaved As Long = deletedKeys.Count

        ' Return total disk space saved
        Return totalSpaceSaved
    End Function

    ' Helper function to determine if a program is installed
    Private Function IsInstalled(programName As String) As Boolean
        Dim uninstallKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", False)
        For Each subKeyName As String In uninstallKey.GetSubKeyNames()
            Dim subKey As RegistryKey = uninstallKey.OpenSubKey(subKeyName)
            If subKey IsNot Nothing Then
                Dim displayName As String = TryCast(subKey.GetValue("DisplayName"), String)
                If Not String.IsNullOrEmpty(displayName) AndAlso displayName.Equals(programName, StringComparison.OrdinalIgnoreCase) Then
                    Return True
                End If
                subKey.Close()
            End If
        Next
        uninstallKey.Close()
        Return False
    End Function

    Private Function CleanInvalidDlls() As Long
        ' Open the SharedDLLs registry key
        Dim sharedDllsKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\Session Manager\SharedDlls", True)

        ' Delete any values that point to non-existent files
        Dim deletedValues As New List(Of String)
        For Each valueName As String In sharedDllsKey.GetValueNames()
            Try
                Dim filePath As String = sharedDllsKey.GetValue(valueName).ToString()
                If Not File.Exists(filePath) Then
                    sharedDllsKey.DeleteValue(valueName, False)
                    deletedValues.Add(valueName)
                End If
            Catch ex As IOException
                ' Ignore files that are in use by another process
            Catch ex As UnauthorizedAccessException
                ' Handle unauthorized access exceptions
            Catch ex As SecurityException
                ' Handle security exceptions
            Catch ex As Exception
                MessageBox.Show("An unexpected error has occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Next

        ' Close the registry key
        sharedDllsKey.Close()

        ' Add deleted values to the ListBox
        For Each deletedValue As String In deletedValues
            lbxRemoved.Items.Add("Registry value deleted: HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\SharedDlls\" & deletedValue)
        Next

        ' Calculate total disk space saved
        Dim totalSpaceSaved As Long = deletedValues.Count

        ' Return total disk space saved
        Return totalSpaceSaved
    End Function

    Private Function CleanRegistry() As Long
        ' Delete unused registry keys and values
        Dim registryKeysToDelete As String() = {"Software\Microsoft\Windows\CurrentVersion\RunOnce", "Software\Microsoft\Windows\CurrentVersion\Explorer\RunMRU"}
        Dim deletedKeys As New List(Of String)
        Dim deletedValues As New List(Of String)
        For Each registryKey As String In registryKeysToDelete
            Dim key As RegistryKey = Registry.CurrentUser.OpenSubKey(registryKey, True)
            If key IsNot Nothing Then
                For Each valueName As String In key.GetValueNames()
                    Try
                        Dim valueData As String = key.GetValue(valueName).ToString()
                        If Not String.IsNullOrEmpty(valueData) AndAlso Not File.Exists(valueData) Then
                            key.DeleteValue(valueName, False)
                            deletedValues.Add(registryKey & "\" & valueName)
                        End If
                    Catch ex As IOException
                        ' Ignore files that are in use by another process
                    Catch ex As UnauthorizedAccessException
                        ' Handle unauthorized access exceptions
                    Catch ex As SecurityException
                        ' Handle security exceptions
                    Catch ex As Exception
                        MessageBox.Show("An unexpected error has occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                Next
                key.Close()
                Try
                    Registry.CurrentUser.DeleteSubKey(registryKey)
                    deletedKeys.Add(registryKey)
                Catch ex As IOException
                    ' Ignore files that are in use by another process
                Catch ex As UnauthorizedAccessException
                    ' Handle unauthorized access exceptions
                Catch ex As SecurityException
                    ' Handle security exceptions
                Catch ex As Exception
                    MessageBox.Show("An unexpected error has occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        Next

        ' Add deleted keys and values to the ListBox
        For Each deletedKey As String In deletedKeys
            lbxRemoved.Items.Add("Registry key deleted: HKEY_CURRENT_USER\" & deletedKey)
        Next
        For Each deletedValue As String In deletedValues
            lbxRemoved.Items.Add("Registry value deleted: HKEY_CURRENT_USER\" & deletedValue)
        Next

        ' Calculate total disk space saved
        Dim totalSpaceSaved As Long = deletedValues.Count

        ' Return total disk space saved
        Return totalSpaceSaved
    End Function

    Private Function CleanTempFiles() As Long
        ' Delete all files in the Temp folder and its subfolders
        Dim tempPath As String = Path.GetTempPath()
        Dim deletedFiles As New List(Of String)
        For Each filePath As String In Directory.GetFiles(tempPath, "*", SearchOption.AllDirectories)
            Dim file As New FileInfo(filePath)
            Try
                file.Delete()
                deletedFiles.Add(file.FullName)
            Catch ex As IOException
                ' Ignore files that are in use by another process
            Catch ex As UnauthorizedAccessException
                ' Handle unauthorized access exceptions
            Catch ex As SecurityException
                ' Handle security exceptions
            Catch ex As Exception
                MessageBox.Show("An unexpected error has occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Next

        ' Add deleted files to the ListBox
        For Each deletedFile As String In deletedFiles
            lbxRemoved.Items.Add("Temporary file deleted: " & deletedFile)
        Next

        ' Calculate total disk space saved
        Dim totalSpaceSaved As Long = deletedFiles.Sum(Function(f) New FileInfo(f).Length)

        ' Return total disk space saved
        Return totalSpaceSaved
    End Function

    Private Function CleanPrefetch() As Long
        ' Delete all files in the Prefetch folder
        Dim prefetchPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch")
        Dim deletedFiles As New List(Of String)
        For Each filePath As String In Directory.GetFiles(prefetchPath)
            Dim file As New FileInfo(filePath)
            Try
                file.Delete()
                deletedFiles.Add(file.Name)
            Catch ex As IOException
                ' Ignore files that are in use by another process
            Catch ex As UnauthorizedAccessException
                ' Handle unauthorized access exceptions
            Catch ex As SecurityException
                ' Handle security exceptions
            Catch ex As Exception
                MessageBox.Show("An unexpected error has occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Next

        ' Add deleted files to the ListBox
        For Each deletedFile As String In deletedFiles
            lbxRemoved.Items.Add("Prefetch file deleted: " & deletedFile)
        Next

        ' Calculate total disk space saved
        Dim totalSpaceSaved As Long = deletedFiles.Sum(Function(f) New FileInfo(Path.Combine(prefetchPath, f)).Length)

        ' Return total disk space saved
        Return totalSpaceSaved
    End Function

    Private Function CleanRemovedSoftwareEntries() As Long
        ' Open the Uninstall registry key
        Dim uninstallKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", True)

        ' Delete any keys that belong to uninstalled programs
        Dim deletedKeys As New List(Of String)
        For Each subKeyName As String In uninstallKey.GetSubKeyNames()
            Dim subKey As RegistryKey = uninstallKey.OpenSubKey(subKeyName)
            If subKey IsNot Nothing Then
                Dim displayName As String = TryCast(subKey.GetValue("DisplayName"), String)
                If String.IsNullOrEmpty(displayName) OrElse Not IsInstalled(displayName) Then
                    uninstallKey.DeleteSubKeyTree(subKeyName)
                    deletedKeys.Add(subKeyName)
                End If
                subKey.Close()
            End If
        Next

        ' Close the registry key
        uninstallKey.Close()

        ' Add deleted keys to the ListBox
        For Each deletedKey As String In deletedKeys
            lbxRemoved.Items.Add("Registry key deleted: HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" & deletedKey)
        Next

        ' Calculate total disk space saved
        Dim totalSpaceSaved As Long = deletedKeys.Count

        ' Return total disk space saved
        Return totalSpaceSaved
    End Function
End Class
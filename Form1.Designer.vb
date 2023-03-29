<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(Form1))
        lbxRemoved = New ListBox()
        btnClean = New Button()
        pbarProgress = New ProgressBar()
        chkRemovedSoftware = New CheckBox()
        chkInvalidDlls = New CheckBox()
        chkProgramRegistryEntries = New CheckBox()
        SuspendLayout()
        ' 
        ' lbxRemoved
        ' 
        lbxRemoved.FormattingEnabled = True
        lbxRemoved.ItemHeight = 48
        lbxRemoved.Location = New Point(12, 12)
        lbxRemoved.Name = "lbxRemoved"
        lbxRemoved.Size = New Size(1220, 436)
        lbxRemoved.TabIndex = 0
        ' 
        ' btnClean
        ' 
        btnClean.Location = New Point(12, 536)
        btnClean.Name = "btnClean"
        btnClean.Size = New Size(225, 69)
        btnClean.TabIndex = 1
        btnClean.Text = "Clean"
        btnClean.UseVisualStyleBackColor = True
        ' 
        ' pbarProgress
        ' 
        pbarProgress.Location = New Point(243, 536)
        pbarProgress.Name = "pbarProgress"
        pbarProgress.Size = New Size(989, 69)
        pbarProgress.TabIndex = 2
        ' 
        ' chkRemovedSoftware
        ' 
        chkRemovedSoftware.AutoSize = True
        chkRemovedSoftware.Location = New Point(12, 478)
        chkRemovedSoftware.Name = "chkRemovedSoftware"
        chkRemovedSoftware.Size = New Size(364, 52)
        chkRemovedSoftware.TabIndex = 3
        chkRemovedSoftware.Text = "Removed Software"
        chkRemovedSoftware.UseVisualStyleBackColor = True
        ' 
        ' chkInvalidDlls
        ' 
        chkInvalidDlls.AutoSize = True
        chkInvalidDlls.Enabled = False
        chkInvalidDlls.Location = New Point(382, 478)
        chkInvalidDlls.Name = "chkInvalidDlls"
        chkInvalidDlls.Size = New Size(262, 52)
        chkInvalidDlls.TabIndex = 4
        chkInvalidDlls.Text = "Invalid DLL's"
        chkInvalidDlls.UseVisualStyleBackColor = True
        ' 
        ' chkProgramRegistryEntries
        ' 
        chkProgramRegistryEntries.AutoSize = True
        chkProgramRegistryEntries.Location = New Point(650, 478)
        chkProgramRegistryEntries.Name = "chkProgramRegistryEntries"
        chkProgramRegistryEntries.Size = New Size(456, 52)
        chkProgramRegistryEntries.TabIndex = 5
        chkProgramRegistryEntries.Text = "Program Registry Entries"
        chkProgramRegistryEntries.UseVisualStyleBackColor = True
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(20F, 48F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1244, 617)
        Controls.Add(chkProgramRegistryEntries)
        Controls.Add(chkInvalidDlls)
        Controls.Add(chkRemovedSoftware)
        Controls.Add(pbarProgress)
        Controls.Add(btnClean)
        Controls.Add(lbxRemoved)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "Form1"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Disk Cleaner - Developed By Cod3xDev"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents lbxRemoved As ListBox
    Friend WithEvents btnClean As Button
    Friend WithEvents pbarProgress As ProgressBar
    Friend WithEvents chkRemovedSoftware As CheckBox
    Friend WithEvents chkInvalidDlls As CheckBox
    Friend WithEvents chkProgramRegistryEntries As CheckBox
End Class

﻿' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Windows.Forms
Imports Microsoft.VisualStudio.Utilities

Namespace Microsoft.VisualStudio.Editors.AddImports
    Friend Class AddImportsDialogService
        Implements IVBAddImportsDialogService

        ' Package Service Provider
        Private ReadOnly _serviceProvider As IServiceProvider

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="packageServiceProvider"></param>
        ''' <remarks></remarks>
        Friend Sub New(packageServiceProvider As IServiceProvider)
            Requires.NotNull(packageServiceProvider, NameOf(packageServiceProvider))
            _serviceProvider = packageServiceProvider
        End Sub

        Public Function ShowDialog([namespace] As String, identifier As String, minimallyQualifiedName As String, dialogType As AddImportsDialogType, helpCallBack As IVBAddImportsDialogHelpCallback) As AddImportsResult Implements IVBAddImportsDialogService.ShowDialog
            Select Case dialogType
                Case AddImportsDialogType.AddImportsCollisionDialog
                    Using (DpiAwareness.EnterDpiScope(DpiAwarenessContext.SystemAware))
                        Using d As New AutoAddImportsCollisionDialog([namespace], identifier, minimallyQualifiedName, helpCallBack, _serviceProvider)
                            Dim result As DialogResult = d.ShowDialog

                            If (result = DialogResult.Cancel) Then
                                Return AddImportsResult.AddImports_Cancel
                            ElseIf (d.ShouldImportAnyways) Then
                                Return AddImportsResult.AddImports_ImportsAnyways
                            Else
                                Return AddImportsResult.AddImports_QualifyCurrentLine
                            End If
                        End Using
                    End Using
                Case AddImportsDialogType.AddImportsExtensionCollisionDialog
                    Using (DpiAwareness.EnterDpiScope(DpiAwarenessContext.SystemAware))
                        Using d As New AutoAddImportsExtensionCollisionDialog([namespace], identifier, minimallyQualifiedName, helpCallBack, _serviceProvider)
                            Dim result As DialogResult = d.ShowDialog

                            If result = DialogResult.Cancel Then
                                Return AddImportsResult.AddImports_Cancel
                            Else
                                Return AddImportsResult.AddImports_QualifyCurrentLine
                            End If
                        End Using
                    End Using
                Case Else
                    Throw New InvalidOperationException("Unexpected Dialog Type")
            End Select
        End Function
    End Class
End Namespace

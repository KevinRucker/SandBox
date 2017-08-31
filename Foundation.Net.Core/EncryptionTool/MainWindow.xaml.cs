// Author: Kevin Rucker
// License: BSD 3-Clause
// Copyright (c) 2014 - 2015, Kevin Rucker
// All rights reserved.

// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice,
//    this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//
// 3. Neither the name of the copyright holder nor the names of its contributors
//    may be used to endorse or promote products derived from this software without
//    specific prior written permission.
//
// Disclaimer:
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using Minimal.Interfaces;
using Minimal.Security.Encryption;
using System;
using System.Windows;

namespace EncryptionTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IEncryptionProvider _crypto = AESEncryptionProvider.Factory();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSource.Text))
            {
                txtResult.Text = _crypto.EncryptString(txtApplicationID.Text, txtSource.Text, null);
            }
        }

        private void btnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSource.Text))
            {
                txtResult.Text = _crypto.DecryptString(txtApplicationID.Text, txtSource.Text, null);
            }
        }

        private void btnMove_Click(object sender, RoutedEventArgs e)
        {
            txtSource.Text = txtResult.Text;
            txtResult.Text = string.Empty;
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtResult.Text))
            {
                Clipboard.SetDataObject(txtResult.Text);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtSource.Text = default(string);
            txtResult.Text = default(string);
        }

        private void btnNewGuid_Click(object sender, RoutedEventArgs e)
        {
            txtApplicationID.Text = Guid.NewGuid().ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_crypto.IsNISTCertifiedAlgorithm) 
            { 
                NISTCert.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                NISTCert.Foreground = System.Windows.Media.Brushes.Red;
            }
            NISTCert.Content = _crypto.IsNISTCertifiedAlgorithm.ToString();
            this.Title = this.Title + " - Version " + Minimal.Utility.Version.ApplicationVersion() + " [Library Version " + Minimal.Utility.Version.LibraryVersion() + "]";
        }

        private void txtApplicationID_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}

// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.IO;
using System.Windows.Input;
using LicenseManagement.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace LicenseManagement.Views.Pages
{
    public partial class DashboardPage : INavigableView<DashboardViewModel>
    {
        public DashboardViewModel ViewModel { get; }

        public DashboardPage(DashboardViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        private void ExportPublicKey(object sender, MouseButtonEventArgs e)
        {
            File.WriteAllText("pkey.key",ViewModel.PublicKey);
            ViewModel.SnackbarService.Show("提示","导出成功!",ControlAppearance.Primary,null,TimeSpan.FromSeconds(3));
        }
    }
}

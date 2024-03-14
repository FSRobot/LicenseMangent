// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.IO;
using LicenseManagement.Models;
using System.Windows.Media;
using LicenseManagement.Helpers;
using LicenseManagement.ViewModels.Common;
using Wpf.Ui.Controls;
using LicenseManagement.SQLite;
using Wpf.Ui;
using MessageBox = System.Windows.MessageBox;
using Wpf.Ui.Extensions;
using System.Diagnostics;

namespace LicenseManagement.ViewModels.Pages
{
    public partial class DataViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        private readonly ISnackbarService SnackbarService;
        private readonly DataContext Context;
        public DataViewModel(ISnackbarService snackbarService, DataContext context)
        {
            SnackbarService = snackbarService;
            Context = context;
        }

        [ObservableProperty]
        private IEnumerable<License> _licenses;
        [ObservableProperty]
        private double _designHeight;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
            LoadViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {

        }
        private void LoadViewModel()
        {
            var licenses = new List<License>();

            foreach (var data in Context.RsaLicense.Take(100))
            {
                licenses.Add(new License()
                {
                    BeginDate = data.BeginDate,
                    Count = data.Count,
                    DisableFunctionList = data.DisableFunctionList,
                    DisableVersionList = data.DisableVersionList,
                    Email =data.Email,
                    EndDate = data.EndDate,
                    Id = data.Id,
                    IsBlock = data.IsBlock,
                    MachineCode = data.MachineCode,
                    Name = data.Name
                });
            }

            Licenses = licenses;

            _isInitialized = true;
        }

        [RelayCommand]
        public void CopyLicenseCode(string id)
        {
            var result = Context.RsaLicense.FirstOrDefault(x => id.Equals(x.Id));
            if (result == null)
            {
                SnackbarService.Show("提示","复制失败,未查询到当前id的激活码!");
            }
            else
            {
                try
                {
                    Clipboard.Clear();
                    Clipboard.SetText(result.Code);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                SnackbarService.Show("提示", "复制成功!");
            }
        }
    }
}

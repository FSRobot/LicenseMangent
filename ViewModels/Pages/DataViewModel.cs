// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using LicenseManagement.SQLite;
using Newtonsoft.Json;
using System.Diagnostics;
using CommonModel;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

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
                    Email = data.Email,
                    EndDate = data.EndDate,
                    Comment = data.Comment,
                    ProductName = data.ProductName,
                    Id = data.Id,
                    IsBlock = data.IsBlock,
                    MachineCode = data.MachineCode,
                    OemId = data.OemId,
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
                SnackbarService.Show("提示", "复制失败,未查询到当前id的激活码!");
            }
            else
            {
                try
                {
                    Clipboard.Clear();
                    Clipboard.SetText($"reg add HKLM\\SOFTWARE\\WOW6432Node\\JKSoft /v {result.ProductName} /t REG_SZ /d {result.Code.ToString()}");
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                SnackbarService.Show("提示", "复制成功!");
            }
        }

        [RelayCommand]
        public void ShowDetails(string id)
        {
            var result = Context.RsaLicense.FirstOrDefault(x => id.Equals(x.Id));
            var content = JsonConvert.SerializeObject(result, Formatting.Indented);
            try
            {
                Clipboard.Clear();
                Clipboard.SetText(content);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            SnackbarService.Show("详情", content);
        }
    }
}

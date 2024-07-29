// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using CommonModel;
using LicenseManagement.SQLite;
using LicenseManagement.SQLite.Models;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace LicenseManagement.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;


        private readonly DataContext Context;
        private readonly ISnackbarService snackbarService;
        private readonly LicenseHelper Helper;
        public SettingsViewModel(DataContext context, ISnackbarService snackbarService, LicenseHelper helper)
        {
            Context = context;
            this.snackbarService = snackbarService;
            this.Helper = helper;
        }

        [ObservableProperty]
        private string _appVersion = String.Empty;

        [ObservableProperty]
        private string _OemName = String.Empty;

        [ObservableProperty]
        private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            CurrentTheme = ApplicationThemeManager.GetAppTheme();
            AppVersion = $"授权管理 - {GetAssemblyVersion()} powerby Baphomet";

            _isInitialized = true;
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? String.Empty;
        }

        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    if (CurrentTheme == ApplicationTheme.Light)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Light);
                    CurrentTheme = ApplicationTheme.Light;

                    break;

                default:
                    if (CurrentTheme == ApplicationTheme.Dark)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                    CurrentTheme = ApplicationTheme.Dark;

                    break;
            }
        }

        [RelayCommand]
        private void CreateOem()
        {
            if (string.IsNullOrWhiteSpace(OemName))
            {
                snackbarService.Show
                (
                    "提示",
                    "名称不允许为空!",
                    ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.ErrorCircle24),
                    TimeSpan.FromSeconds(5)
                );
                return;
            }

            if (Context.Oem.Any(x => x.Name.Equals(OemName.ToUpper())))
            {
                snackbarService.Show
                (
                    "提示",
                    "已存在该厂商!",
                    ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.ErrorCircle24),
                    TimeSpan.FromSeconds(5)
                );
                return;
            }

            var keyPair = Helper.GenerateNewKeyPair();

            Context.Oem.Add(new OemModel()
            {
                CreateTime = DateTime.Now,
                Id = Guid.NewGuid(),
                Name = OemName.ToUpper(),
                OpId = Guid.Empty,
                PublickKey = keyPair.PublicKey,
                SecretKey = keyPair.PrivateKey
            });
            Context.SaveChanges();

            snackbarService.Show
            (
                "提示",
                "新增成功!",
                ControlAppearance.Success,
                new SymbolIcon(SymbolRegular.Add24),
                TimeSpan.FromSeconds(3)
            );
        }
    }
}

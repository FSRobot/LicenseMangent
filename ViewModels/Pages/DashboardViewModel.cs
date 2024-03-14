// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using LicenseManagement.Helpers;
using System.Diagnostics;
using System.IO;
using LicenseManagement.SQLite;
using LicenseManagement.SQLite.Models;
using LicenseManagement.ViewModels.Common;
using Newtonsoft.Json;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace LicenseManagement.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly LicenseHelper Helper;
        public readonly ISnackbarService SnackbarService;
        private readonly DataContext Context;
        public DashboardViewModel(LicenseHelper helper, ISnackbarService snackbarService, DataContext context)
        {
            Helper = helper;
            SnackbarService = snackbarService;
            Context = context;

            LoadData();

            PrivateKey = helper.SecretKey.PrivateKey;
            PublicKey = helper.SecretKey.PublicKey;
        }

        [ObservableProperty]
        private int _counter = 0;

        [ObservableProperty] private string _privateKey = string.Empty;
        [ObservableProperty] private string _publicKey = string.Empty;
        [ObservableProperty] private double _designHeight = 450;
        [ObservableProperty] private double _designWidth = 800;
        [ObservableProperty] private string _name = string.Empty;
        [ObservableProperty] private string _email = string.Empty;
        [ObservableProperty] private string _machineCode = string.Empty;
        [ObservableProperty] private int _totalRun = -1;
        [ObservableProperty] private DateTime _beginDate = DateTime.Now;
        [ObservableProperty] private DateTime _endDate = DateTime.MaxValue;
        [ObservableProperty] private string _disableFunctionList = string.Empty;
        [ObservableProperty] private string _disableVersionList = string.Empty;

        [RelayCommand]
        private void OnSubmit()
        {
            License license = new()
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = Name,
                Email = Email,
                MachineCode = MachineCode,
                BeginDate = BeginDate,
                EndDate = EndDate,
                Count = TotalRun,
                DisableFunctionList = DisableFunctionList,
                DisableVersionList = DisableVersionList,
                IsBlock = false
            };
            var details = Helper.Encrypt(license);
            SnackbarService.Show("授权", "成功!",
                ControlAppearance.Primary, null, TimeSpan.FromSeconds(5));

            Context.RsaLicense.Add(new RsaLicenseModel()
            {
                Id = license.Id,
                Name = license.Name,
                Email = license.Email,
                MachineCode = license.MachineCode,
                BeginDate = license.BeginDate,
                EndDate = license.EndDate,
                Count = license.Count,
                DisableFunctionList = DisableFunctionList,
                DisableVersionList = DisableVersionList,
                IsBlock = false,
                OpName = "",
                CreateDate = DateTime.Now,
                Code = details.ToString()
            });
            Context.SaveChanges();

            try
            {
                Clipboard.Clear();
                Clipboard.SetText(details.ToString());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            Name = string.Empty;
            Email = string.Empty;
            MachineCode = string.Empty;
            BeginDate = DateTime.Now;
            EndDate = DateTime.MaxValue;
            TotalRun = -1;
            DisableFunctionList = string.Empty;
            DisableVersionList = string.Empty;
        }

        private void LoadData()
        {
            var keypair = Context.RsaSecret.FirstOrDefault(x => x.Name.Equals("NECS"));
            if (keypair != null)
            {
                Helper.SecretKey = new RsaSecretKey()
                {
                    PublicKey = keypair.PublicKey,
                    PrivateKey = keypair.PrivateKey
                };
            }
            else
            {
                Helper.GenerateKeyPair(); 
                Context.RsaSecret.Add(new RsaSecretModel()
                {
                    CreateDate = DateTime.Now,
                    Id = Guid.NewGuid(),
                    Name = "NECS",
                    PrivateKey = Helper.SecretKey.PrivateKey,
                    PublicKey = Helper.SecretKey.PublicKey
                });
                Context.SaveChanges();
            }
        }
    }
}

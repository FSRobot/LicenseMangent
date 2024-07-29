// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Collections.ObjectModel;
using CommonModel;
using CommonModel.Encryption;
using LicenseManagement.SQLite;
using LicenseManagement.SQLite.Models;
using System.Diagnostics;
using System.IO;
using LicenseChecker;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace LicenseManagement.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject, INavigationAware
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
            if (string.IsNullOrWhiteSpace(PrivateKey))
            {
                PrivateKey = Helper.SecretKey.PrivateKey;
                PublicKey = Helper.SecretKey.PublicKey;
            }
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
        [ObservableProperty] private string _comment = string.Empty;
        [ObservableProperty] private string _productName = string.Empty;
        [ObservableProperty] private int _totalRun = -1;
        [ObservableProperty] private DateTime _beginDate = DateTime.Now;
        [ObservableProperty] private DateTime _endDate = DateTime.MaxValue;
        [ObservableProperty] private string _disableFunctionList = string.Empty;
        [ObservableProperty] private string _disableVersionList = string.Empty;
        [ObservableProperty] private ObservableCollection<OemModel> _OEMSource = new();
        [ObservableProperty] private Guid _OEMId = Guid.Empty;

        [RelayCommand]
        private void ChangeYearRange(object obj)
        {
            if (obj is string s && int.TryParse(s, out int range))
            {
                EndDate = range < 1 ? DateTime.MaxValue : BeginDate.AddYears(range);
            }
        }
        [RelayCommand]
        private void OnSubmit()
        {
            if (Guid.Empty.Equals(OEMId))
            {
                SnackbarService.Show(
                    "授权",
                    "OEM禁止为空!",
                    ControlAppearance.Primary,
                    new SymbolIcon(SymbolRegular.ErrorCircle24),
                    TimeSpan.FromSeconds(5));
                return;
            }

            if (string.IsNullOrWhiteSpace(ProductName))
            {
                SnackbarService.Show(
                    "授权",
                    "产品名称禁止为空!",
                    ControlAppearance.Primary,
                    new SymbolIcon(SymbolRegular.ErrorCircle24),
                    TimeSpan.FromSeconds(5));
                return;
            }

            License license = new()
            {
                Id = Guid.NewGuid().ToString("N"),
                ProductName = ProductName,
                Name = Name,
                Email = Email,
                MachineCode = MachineCode,
                Comment = Comment,
                BeginDate = BeginDate,
                EndDate = EndDate,
                Count = TotalRun,
                OemId = OEMId,
                DisableFunctionList = DisableFunctionList,
                DisableVersionList = DisableVersionList,
                OemPublicKey = GetOemPublicKey(OEMId),
                IsBlock = false
            };

            // 如果是厂家授权软件删除公钥保留私钥
            if (ProductName.Equals("OEMLicenseGenerator"))
            {
                license.OemPrivateKey = GetOemPrivateKey(OEMId);
                license.OemPublicKey = string.Empty;
            }

            var details = Helper.Encrypt(license);
            SnackbarService.Show("授权", "成功!",
                ControlAppearance.Primary, null, TimeSpan.FromSeconds(5));

            Context.RsaLicense.Add(new RsaLicenseModel()
            {
                Id = license.Id,
                Name = license.Name,
                Email = license.Email,
                MachineCode = license.MachineCode,
                Comment = license.Comment,
                BeginDate = license.BeginDate,
                EndDate = license.EndDate,
                Count = license.Count,
                ProductName = license.ProductName,
                DisableFunctionList = DisableFunctionList,
                DisableVersionList = DisableVersionList,
                IsBlock = false,
                OpId = Guid.Empty,
                OemId = OEMId,
                CreateDate = DateTime.Now,
                Code = details.ToString()
            });
            Context.SaveChanges();

            LicenseCode licenseCode = new()
            {
                Code = details.ToString(),
                ProductName = license.ProductName
            };

            var content = JsonConvert.SerializeObject(licenseCode);
            File.WriteAllText(LicenseCheck.DesktopPath + "\\" + LicenseCheck.LicensePath, content);

            Name = string.Empty;
            Email = string.Empty;
            MachineCode = string.Empty;
            Comment = string.Empty;
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

        private void LoadOEM()
        {
            OEMSource.Clear();
            var keySet = Context.Oem.ToList();
            foreach (var key in keySet)
            {
                OEMSource.Add(key);
            }

            if (Guid.Empty.Equals(OEMId) && OEMSource.Count != 0)
            {
                OEMId = OEMSource.First().Id;
            }
            else
            {
                var id = OEMId;
                OEMId = Guid.Empty;
                OEMId = id;
            }
        }

        private string GetOemPublicKey(Guid id)
        {
            var keySet = OEMSource.First(x => x.Id.Equals(id));
            return keySet == null ? string.Empty : keySet.PublickKey;

        }

        private string GetOemPrivateKey(Guid id)
        {
            var keySet = Context.Oem.FirstOrDefault(x => x.Id.Equals(id));
            return keySet == null ? string.Empty : keySet.SecretKey;
        }

        public void OnNavigatedTo()
        {
            LoadOEM();
        }

        public void OnNavigatedFrom()
        {
        }
    }
}

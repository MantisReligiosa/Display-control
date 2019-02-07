﻿using Microsoft.Deployment.WindowsInstaller;
using Setup.CustomDialogs;
using Setup.Data;
using Setup.Interfaces;
using Setup.Managers;
using System;
using WixSharp;
using WixSharp.Forms;
using static WixSharp.SetupEventArgs;

namespace Setup
{
    public class Setup
    {
        private static readonly ISqlManager _sqlManager = new MsSqlManager();

        private static void Main(string[] args)
        {
            AssemblyManager.GetAssemblyInfo(
                System.IO.Path.Combine(Constants.PublishFolder, Constants.ExecFile),
                out Guid guid, out Version version);

            var managedUI = new ManagedUI();
            managedUI.InstallDialogs
                .Add(Dialogs.Welcome)
                .Add(Dialogs.InstallDir)
                .Add<ConnectionStringDialog>()
                .Add(Dialogs.Progress)
                .Add(Dialogs.Exit);

            managedUI.ModifyDialogs
                .Add(Dialogs.MaintenanceType)
                .Add(Dialogs.Progress)
                .Add(Dialogs.Exit);

            var project = new ManagedProject(Constants.CommonInstallationName,
                new Dir(Constants.InstallationDirectory,
                    new DirFiles(System.IO.Path.Combine(Constants.PublishFolder, "*.*"))),
                new Dir(Constants.ProgramMenuDirectory,
                        new ExeFileShortcut($"Uninstall {Constants.ProductName}", "[System64Folder]msiexec.exe", "/x [ProductCode]"),
                        new ExeFileShortcut(Constants.ProductName, "[INSTALLDIR]Display-control.exe", arguments: "")),
                new Dir(@"%Desktop%",
                        new ExeFileShortcut(Constants.ExecFile, $"[INSTALLDIR]{Constants.ExecFile}", arguments: "")))
            {
                GUID = guid,
                Description = Constants.CommonInstallationName,
                InstallPrivileges = InstallPrivileges.elevated,
                MajorUpgradeStrategy = new MajorUpgradeStrategy
                {
                    UpgradeVersions = VersionRange.OlderThanThis,
                    PreventDowngradingVersions = VersionRange.ThisAndNewer,
                    NewerProductInstalledErrorMessage = Messages.NewerProductInstalledErrorMessage
                },
                ManagedUI = managedUI,
                Version = version,
            };
            project.ControlPanelInfo.Manufacturer = Constants.Manufacturer;
            project.DefaultRefAssemblies.AddRange(
                AssemblyManager.GetAssemblyPathsCollection(System.IO.Path.GetDirectoryName(
                    System.Reflection.Assembly.GetExecutingAssembly().Location)));
            project.AfterInstall += Project_AfterInstall;

            Compiler.BuildMsi(project);
        }

        private static void Project_AfterInstall(SetupEventArgs e)
        {
            if (e.IsUpgrading)
                return;

            if (e.IsUninstalling)
                return;

            var connectionString = e.Data[Properties.ConnectionString.PropertyName];
            try
            {
                _sqlManager.CreateDatabase(connectionString);
                _sqlManager.ApplyMigrations();

            }
            catch (Exception ex)
            {
                e.Session.Log(ex.ToString());
                NotificationManager.ShowErrorMessage(ex.Message,
                    isWizardInstallation: IsWizardInstallationMode(e.Data));

                e.Result = ActionResult.Failure;
                return;
            }
        }

        private static bool IsWizardInstallationMode(AppData data)
        {
            return bool.TryParse(data[Parameters.WizardInstallationParameter], out bool result)
                ? result
                : false;
        }
    }
}

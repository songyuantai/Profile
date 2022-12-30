using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Profile.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Profile
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class SaveAsCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("944d5a99-0d96-4d7f-a8f1-4032fe579a94");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveAsCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private SaveAsCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static SaveAsCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in SaveAsCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new SaveAsCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            
            //string message = $"运行路径：{AppContext.BaseDirectory}";
            //string title = "SaveAsCommand";

            // Show a message box to prove we were here
            //VsShellUtilities.ShowMessageBox(
            //    this.package,
            //    message,
            //    title,
            //    OLEMSGICON.OLEMSGICON_INFO,
            //    OLEMSGBUTTON.OLEMSGBUTTON_OK,
            //    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

            var dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            var solutionName = Path.GetFileName(dte.Solution.FullName);
            //var solutionPath = Path.GetFullPath(dte.Solution.FullName);

            var dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
            var dic = new Dictionary<string, List<string>>();

            foreach (Project project in dte2.Solution.Projects)
            {
                if (!string.IsNullOrEmpty(project.FullName))
                {
                    var rootPath = Path.GetDirectoryName(project.FullName);
                    var fileNames = Directory.GetFiles(rootPath);
                    dic.Add(project.Name, fileNames.ToList());
                }
            }

            var f = new SaveForm(solutionName, dic);
            f.Show();
        }


        SqlSugarClient _client;
        public SqlSugarClient Database
        {
            get
            {
                if(null == _client)
                {
                    ProfilePackage package = this.package as ProfilePackage;
                    var path = package.OptionPath;
                    var fileName = Path.Combine(path, "profile.db");
                    var isInit = !File.Exists(fileName);

                    _client = new SqlSugarClient(new ConnectionConfig()
                    {
                        IsAutoCloseConnection = true,
                        InitKeyType = InitKeyType.Attribute,
                        ConnectionString = $"Data Source={fileName};Cache=Shared",
                        DbType = DbType.Sqlite
                    });

                    if (isInit)
                    {
                        InitTables();
                    }
                }

                return _client;
            }
        }

        public void InitTables()
        {
            Database.CurrentConnectionConfig.InitKeyType = InitKeyType.Attribute;
            Database.DbMaintenance.CreateDatabase();
            Database.CodeFirst.InitTables(
                typeof(ProfileConfig), 
                typeof(ProfileItem));
        }
    }
}

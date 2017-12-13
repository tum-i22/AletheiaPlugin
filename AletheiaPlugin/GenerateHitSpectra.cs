//------------------------------------------------------------------------------
// <copyright file="GenerateHitSpectra.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using EnvDTE80;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace AletheiaPlugin
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class GenerateHitSpectra
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;
        public DTE2 dte;
        private static string vcxProj;
        private static string gtestPath;
        private static string srcDir;
        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("3bd17288-ac74-426b-bd5c-eaafa912f017");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateHitSpectra"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private GenerateHitSpectra(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;
            dte = this.ServiceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;
            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static GenerateHitSpectra Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
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
        public static void Initialize(Package package)
        {
            Instance = new GenerateHitSpectra(package);
        }

        private string getGtestPath(string path)
        {
            string outDir="", outFile="";
            bool outDirFound = false, outFileFound = false;
            string gtestPath = "";
            XmlTextReader reader = new XmlTextReader(path);
            while (reader.Read())
            {
                if(outDir.Length>0 && outFile.Length > 0)
                {
                    gtestPath = outDir + outFile + ".exe";
                    break;
                }
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        if (reader.Name.Equals("OutDir"))
                        {
                            outDirFound = true;
                        }
                            
                        if(reader.Name.Equals("TargetName"))
                        {
                            outFileFound = true;
                            
                        }
                        break;
                    case XmlNodeType.Text: //Display the text in each element.
                        if(outDirFound)
                        {
                            outDir = reader.Value;
                            outDirFound = false;
                        }
                        if (outFileFound)
                        {
                            outFile = reader.Value;
                            outFileFound = false;
                        }
                        //Console.WriteLine(reader.Value);
                        break;
                    case XmlNodeType.EndElement: //Display the end of the element.
                        Console.Write("</" + reader.Name);
                        Console.WriteLine(">");
                        break;
                }
            }
            return gtestPath;

        }
        public static bool ExistsOnPath(string fileName)
        {
            return GetFullPath(fileName) != null;
        }

        public static string GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
                return Path.GetFullPath(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(';'))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }
        public static void LaunchCommandLineApp()
        {

            //string strCmdText;
            //strCmdText = "C:\\Users\\i22\\Desktop\\Repo\\Copy\\Aletheia\\Aletheia\\bin\\x64\\Debug\\Aletheia.exe " + "do= GenerateHitSpectra project_path = " +
            //    vcxProj + " source_directory = " +
           //     srcDir + " gtest_path = " + gtestPath);
           // System.Diagnostics.Process.Start("CMD.exe", strCmdText);


            System.Diagnostics.Process cmd = new System.Diagnostics.Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();



            cmd.StandardInput.WriteLine("Aletheia.exe " + "do=GenerateHitSpectra project_path=" +
                vcxProj + " source_directory=" + 
                srcDir + " degreeofparallelism=15" + " gtest_path=" + gtestPath);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            //cmd.WaitForExit();
            cmd.Close();

       
        }
        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            string title = "GenerateHitSpectra";
            string count = "";
            object arr = dte.ActiveSolutionProjects;
            System.Object []projs = (System.Object [])dte.ActiveSolutionProjects;
            var item = projs.GetEnumerator();
            
            while (item.MoveNext())
            {
                Project p = item.Current as Project;
                vcxProj = p.FileName;
                break;
            }
            
            message += vcxProj;
            gtestPath = getGtestPath(vcxProj);
            srcDir = gtestPath.Substring(0, gtestPath.IndexOf("bin"));
            //aletheia.exe and OpenCppCoverage.exe should be in path
            string AletheiaPath = GetFullPath("Aletheia.exe");
            string OpenCppCoveragePath = GetFullPath("OpenCppCoverage.exe");
            if (AletheiaPath!=null && OpenCppCoveragePath != null)
            {
                //invoke shell=
                LaunchCommandLineApp();
                message = "Default Locaiton of HitSpectra is C:\\Hitspectras";
            }
            else
            {
                if (AletheiaPath == null)
                    message = "Aletheia.exe is not on system path variable; ";
                if (OpenCppCoveragePath == null)
                    message += " OpenCppCoverage.exe is not on system path variable";
            }
            
            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
                this.ServiceProvider,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}

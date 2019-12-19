using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ESFramework;
using System.Xml;
using System.IO;
using ESFramework.Server.UserManagement;
using OAUS.Core;
using System.Configuration;
using ESPlus.Widgets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net;
using ESPlus;

/// <summary>
/// OAUS 自动升级系统，作者：zhuweisky。http://www.cnblogs.com/zhuweisky
/// </summary>
namespace OAUS.Server
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        private static ESPlus.Rapid.IRapidServerEngine RapidServerEngine = ESPlus.Rapid.RapidEngineFactory.CreateServerEngine();
        private static UpdateConfiguration UpgradeConfiguration = null;

        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                  
                GlobalUtil.SetAuthorizedUser("FreeUser", "");
                
                //初始化服务端引擎
                CustomizeHandler customizeHandler = new CustomizeHandler();
                int port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                RapidServerEngine.Initialize(port, customizeHandler);
                RapidServerEngine.UserManager.RelogonMode = RelogonMode.IgnoreNew;
                        
                //显示默认主窗体
                MainServerForm mainForm = new MainServerForm(Program.RapidServerEngine);
                mainForm.Text = ConfigurationManager.AppSettings["Title"];
                mainForm.CustomFunctionActivated += new ESBasic.CbGeneric(mainForm_CustomFunctionActivated);//点击自定义按钮，弹出升级配置信息。

                //动态生成或加载配置信息                               
                if (!File.Exists(UpdateConfiguration.ConfigurationPath))
                {
                    Program.UpgradeConfiguration = new UpdateConfiguration();
                    Program.UpgradeConfiguration.Save();
                }
                else
                {
                    Program.UpgradeConfiguration = (UpdateConfiguration)UpdateConfiguration.Load(UpdateConfiguration.ConfigurationPath);
                }

                customizeHandler.Initialize(RapidServerEngine.FileController, Program.UpgradeConfiguration);

                bool remoting = bool.Parse(ConfigurationManager.AppSettings["RemotingServiceEnabled"]);
                if (remoting)
                {
                    ChannelServices.RegisterChannel(new TcpChannel(port + 2), false);
                    OausService service = new OausService(Program.UpgradeConfiguration);
                    RemotingServices.Marshal(service, "OausService"); 
                }

                Application.Run(mainForm);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message+" - " + ee.StackTrace);
            }
        }

        static void mainForm_CustomFunctionActivated()
        {
            try
            {
                FileVersionForm form = new FileVersionForm(Program.UpgradeConfiguration);
                form.Show();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }
    }
}

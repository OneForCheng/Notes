﻿using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using log4net;

namespace Notes
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App 
    {
        private Mutex _instance;//同步基元，保证不被回收
        public static readonly ILog Log = LogManager.GetLogger("InfoLogger");

        protected override void OnStartup(StartupEventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            base.OnStartup(e);
            Log.Info("程序启动！");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Info("程序关闭！");
            base.OnExit(e);
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            #region Unhandled Exceptions

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            #endregion

            bool createNew;//返回是否赋予了使用线程的互斥体初始所属权
            _instance = new Mutex(true, "WingStudio.ForCheng.Notes", out createNew);//同步基元变量

            if (createNew)//控制程序只启动一次
            {
                var mainWindow = new Windows.MainWindow();
                mainWindow.Show();
            }
            else
            {
                Thread.Sleep(100);
                Extentions.ShowMessageBox("程序已运行!");
                Current.Shutdown(0);
            }
        }

        #region Exception Handling

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception.ToString());
            try
            {
                Extentions.ShowMessageBox("Generic error - unknown");
            }
            catch (Exception)
            {
                //..
            }
            e.Handled = true;

        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (exception == null) return;
            Log.Error(exception.ToString());
            try
            {
                Extentions.ShowMessageBox("Generic error - unhandled");
            }
            catch (Exception)
            {
                //..
            }
        }

        #endregion
    }
}

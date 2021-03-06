﻿using Enumeration;
using FrontFramework.Enums;
using FrontFramework.Help;
using FrontFramework.Language;
using FrontFramework.Plugin;
using FrontFramework.Station;
using FrontFramework.Utils;
using FrontFramework.Utils.Print;
using FrontFramework.Utils.Spring;
using MahApps.Metro.Controls;
using PluginDefinition;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using Translation;

namespace FrontFramework
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : ComponentDynamicTranslate
    {
        private static String userName = "";
        public static String getUserName() 
        {
            return MainWindow.userName;
        }
        private static OperationLevelEnum operationLevel = OperationLevelEnum.OBSERVER;
        public static OperationLevelEnum getOperationLevel()
        {
            return MainWindow.operationLevel;
        }

        // 单例变量
        private static PropertyUtilInterface propUtil = PropUtilFactory.getPropUtil();
        private static ITranslator translator = TranslatorFactory.getTranslator();

        public MainWindow(String userName, OperationLevelEnum operationLevel)
        {
            MainWindow.userName = userName;
            MainWindow.operationLevel = operationLevel;
            try
            {
                InitializeComponent();
                // 窗体所有文字初始化与监听注册
                initializeComponentContents();
                LanguageChangedNotifier.getInstance().addListener(this);
                // 插件加载
                pluginManager.loadPlugins();
                // 报警模块初始化
                AlarmModuleInit(); 
                // “系统”菜单初始化（菜单中的系统选项）
                SysMenuInit(); 
                // 界面窗体大小设置
                this.Width = SystemParameters.WorkArea.Size.Width / 1.25;
                this.Height = SystemParameters.WorkArea.Size.Height / 1.25;
            }
            catch (Exception er) 
            {
                FrontFramework.Log.LogUtil.writeFuncErrorLog("主界面初始化异常", er);
                System.Environment.Exit(0);
            }
        }

        /// <summary>
        /// 所有需要在翻译时刷新的空间都在这里赋值
        /// </summary>
        public void initializeComponentContents() 
        {
            this.Title = translator.getComponentTranslation("MAIN_WINDOW_NAME");
            menuNameLable.Content = translator.getComponentTranslation("Menu");
            menuSystem.Header = translator.getComponentTranslation("System");
            menuUser.Header = translator.getComponentTranslation("User");
            menuLogout.Header = translator.getComponentTranslation("Logout");
            menuLanguage.Header = translator.getComponentTranslation("Language");
            menuChinese.Header = translator.getComponentTranslation("Chinese");
            menuEnglish.Header = translator.getComponentTranslation("English");
            menuTools.Header = translator.getComponentTranslation("Tools");
            menuSnapshot.Header = translator.getComponentTranslation("PrintScr") + " (Ctrl+Shift+C)";
            menuDicManager.Header = translator.getComponentTranslation(new String[] { "Dictionary", "Manage" });
            menuAlarmTest.Header = translator.getComponentTranslation(new String[] { "Alarm", "Test" });
            menuStartAlarm.Header = translator.getComponentTranslation(new String[] { "Start", "Alarm" });
            menuStopAlarm.Header = translator.getComponentTranslation(new String[] { "Stop", "Alarm" });
            menuPlugin.Header = translator.getComponentTranslation("Plugin");
            menuPluginManager.Header = translator.getComponentTranslation(new String[]{"Plugin","Manage"});
            menuView.Header = translator.getComponentTranslation("View");
            normalScreenMenu.Header = translator.getComponentTranslation(new String[]{"Normal","View"});
            //fullScreenMenu.Header = translator.getComponentTranslation("Full Screen");
            //fullScreenMenu.ToolTip = "Esc " + translator.getComponentTranslation("Exit");
            viewSettingMenu.Header = translator.getComponentTranslation(new String[] { "View", "Setting" });
            floatScreenMenu.Header = translator.getComponentTranslation("Float Screen");
            multiPageMenu.Header = translator.getComponentTranslation("MultiPage");
            singlePageMenu.Header = translator.getComponentTranslation(new String[]{"Single","Page"});
            doublePageMenu.Header = translator.getComponentTranslation(new String[] { "Double", "Page" });
            menuHelp.Header = translator.getComponentTranslation("Help");
            menuHelpDoc.Header = translator.getComponentTranslation(new String[]{"Help","Document"});
            menuHelpOnline.Header = translator.getComponentTranslation(new String[]{"Help","Online"});
            menuAbout.Header = translator.getComponentTranslation("About");
            labelWaitingSolved.Content = translator.getComponentTranslation("Unsolved");
            labelWaitingConfirmed.Content = translator.getComponentTranslation("Unconfirmed");
            buttonMute.Content = translator.getComponentTranslation("Mute");
            buttonDetailedAlarmInfo.Content = translator.getComponentTranslation(new String[] { "Detailed", "Info." });
            alarmDataGrid.Columns[0].Header = translator.getComponentTranslation("Type");
            alarmDataGrid.Columns[1].Header = translator.getComponentTranslation("Level");
            alarmDataGrid.Columns[2].Header = translator.getComponentTranslation("Date Time");
            alarmDataGrid.Columns[3].Header = translator.getComponentTranslation("Location");
            alarmDataGrid.Columns[4].Header = translator.getComponentTranslation("System");
            alarmDataGrid.Columns[5].Header = translator.getComponentTranslation("Description");
            alarmDataGrid.Columns[6].Header = translator.getComponentTranslation("State");
            multiMode = PropUtilFactory.getPropUtil().getStrProp("multiMode").info;
            if (multiMode.Equals("multipage"))
            {
                setMultiPageMode();
            }
            else
            {
                setMultiMonitorMode();
            }
            string[] offsets = propUtil.getStrProp("primaryMonitorOffset").info.Split(new char[] { ',' });
            leftOffset = int.Parse(offsets[0]);
            topOffset = int.Parse(offsets[1]);
            PluginInit();
            StationMenuInit();
        }
        public void setTitle(String title) 
        {
            this.Title = title;
        }

        #region 菜单对应动作

            # region 工具菜单
                private void dicManagerOnClick(object sender, RoutedEventArgs e)
                {
                    new FrontFramework.Language.Views.DictionaryEditor(this).ShowDialog();
                }
            #endregion

            # region 插件菜单
            #endregion

                # region 视图菜单
                public void setMultiPageMode()
                {
                    normalScreenMenu.IsEnabled = true;
                    floatScreenMenu.IsEnabled = true;
                    multiPageMenu.IsEnabled = true;
                    multiMonitorMenu.IsEnabled = false;
                }
                public void setMultiMonitorMode()
                {
                    normalScreenMenu.IsEnabled = false;
                    floatScreenMenu.IsEnabled = false;
                    multiPageMenu.IsEnabled = false;
                    multiMonitorMenu.IsEnabled = true;
                }

                private void normalScreenOnClick(object sender, RoutedEventArgs e)
                {
                    setWindowState(screenState, ScreenStateEnum.NORMAL);
                }
                private void fullScreenOnClick(object sender, RoutedEventArgs e)
                {
                    setWindowState(screenState, ScreenStateEnum.FULLSCREEN);
                }
                private void floatScreenOnClick(object sender, RoutedEventArgs e)
                {
                    setWindowState(screenState, ScreenStateEnum.FLOAT);
                }

                private double widthBeforeFullScreen = 0;
                private double heightBeforeFullScreen = 0;
                private double leftBeforeFullScreen = 0;
                private double topBeforeFullScreen = 0;
                private ScreenStateEnum screenState = ScreenStateEnum.NORMAL;
                private void setWindowState(ScreenStateEnum from, ScreenStateEnum to)
                {
                    if (from == to)
                        return;
                    // 操作 View 区域
                    if (to == ScreenStateEnum.NORMAL)
                    {
                        //MenuArea.Width = double.NaN;
                        //MenuArea.Height = double.NaN;
                        ViewSwitchArea.Width = double.NaN;
                        ViewSwitchArea.Height = double.NaN;
                        MainView.Width = double.NaN;
                        MainView.Height = double.NaN;
                        AlarmArea.Width = double.NaN;
                        AlarmArea.Height = double.NaN;
                    }
                    else if (to == ScreenStateEnum.FULLSCREEN)
                    {
                        //MenuArea.Width = 0;
                       // MenuArea.Height = 0;
                        ViewSwitchArea.Width = double.NaN;
                        ViewSwitchArea.Height = double.NaN;
                        MainView.Width = double.NaN;
                        MainView.Height = double.NaN;
                        AlarmArea.Width = 0;
                        AlarmArea.Height = 0;
                    }
                    else if (to == ScreenStateEnum.FLOAT)
                    {
                        //MenuArea.Width = double.NaN;
                        //MenuArea.Height = double.NaN;
                        ViewSwitchArea.Width = 0;
                        ViewSwitchArea.Height = 0;
                        MainView.Width = 0;
                        MainView.Height = 0;
                        AlarmArea.Width = double.NaN;
                        AlarmArea.Height = double.NaN;
                    }
                    // Window title
                    if (to == ScreenStateEnum.FULLSCREEN)
                        this.WindowStyle = WindowStyle.None;
                    else
                        this.WindowStyle = WindowStyle.SingleBorderWindow;
                    // resizable
                    if (to == ScreenStateEnum.FULLSCREEN)
                        this.ResizeMode = ResizeMode.NoResize;
                    else
                        this.ResizeMode = ResizeMode.CanResize;
                    // 全屏
                    if (to == ScreenStateEnum.FULLSCREEN)
                        this.WindowState = WindowState.Maximized;
                    else
                        this.WindowState = WindowState.Normal;
                    // 覆盖开始菜单
                    if (to == ScreenStateEnum.NORMAL)
                        this.Topmost = false;
                    else
                        this.Topmost = true;
                    // Min size
                    if (to == ScreenStateEnum.FLOAT)
                    {
                        this.MinHeight = 0.0;
                        this.MinWidth = 0.0;
                    }
                    else
                    {
                        this.MinHeight = 600.0;
                        this.MinWidth = 800.0;
                    }
                    // 数据存储
                    if (from == ScreenStateEnum.NORMAL)
                    {
                        widthBeforeFullScreen = this.ActualWidth;
                        heightBeforeFullScreen = this.ActualHeight;
                        leftBeforeFullScreen = this.Left;
                        topBeforeFullScreen = this.Top;
                    }
                    // 数据恢复
                    if (to != ScreenStateEnum.NORMAL)
                    {
                        this.Width = widthBeforeFullScreen;
                        this.Height = heightBeforeFullScreen;
                        this.Left = leftBeforeFullScreen;
                        this.Top = topBeforeFullScreen;
                    }
                    if (to == ScreenStateEnum.FLOAT)
                    {
                        this.SizeToContent = SizeToContent.Height;
                    }
                    else
                    {
                        this.SizeToContent = SizeToContent.Manual;
                    }
                    // Max size
                    if (to == ScreenStateEnum.FLOAT)
                    {
                        //fullScreenMenu.IsEnabled = false;
                        this.MaxHeight = this.Height;
                        this.MinHeight = this.Height;
                    }
                    else
                    {
                        //fullScreenMenu.IsEnabled = true;
                        this.MaxHeight = 5000.0;
                    }
                    // menu enabled
                    if (to == ScreenStateEnum.NORMAL)
                    {
                        normalScreenMenu.IsEnabled = false;
                        //fullScreenMenu.IsEnabled = true;
                        floatScreenMenu.IsEnabled = true;
                    }
                    else if (to == ScreenStateEnum.FLOAT)
                    {
                        normalScreenMenu.IsEnabled = true;
                        //fullScreenMenu.IsEnabled = false;
                        floatScreenMenu.IsEnabled = false;
                    }
                    // 状态改变
                    screenState = to;
                }
            #endregion



        #endregion

        /// <summary>
        /// 键盘监听
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        // 截屏一按一起才是一次完整动作
        private void mainScreenKeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.C) 
            {
                snapshotOnClick(null, null);
                e.Handled = true;
            }
            /*
            if (e.Key == Key.Escape && screenState == ScreenStateEnum.FULLSCREEN)
            {
                setWindowState(screenState, ScreenStateEnum.NORMAL);
            }*/
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            LanguageChangedNotifier.getInstance().removeListener(this);
            foreach (PluginInfo pluginInfo in pluginManager.pluginInfos)
            {
                pluginInfo.plugin.closeListeningThread();
            }
        }

        /////////////////////////////////// 多视图功能 //////////////////////////////////////
        private static String multiMode; // multipage or multimonitor
        private void viewSwitchMenuClicked(object sender, EventArgs e)
        {
            if (selectedPageIndex == 0)
            {
                if (!pageDic[((MenuItem)sender).Name].Equals(MainViewFrameA.Content))
                {
                    if (pageDic[((MenuItem)sender).Name].Equals(MainViewFrameB.Content))
                    {
                        MainViewFrameB.Content = null;
                    }
                    MainViewFrameA.Content = null;
                    MainViewFrameA.Content = pageDic[((MenuItem)sender).Name];
                    viewA_Page = pageDic[((MenuItem)sender).Name];
                    selectedPage = viewA_Page;
                    if (pageLocked.Contains(viewA_Page))
                    {
                        viewALockedImage.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        viewALockedImage.Visibility = Visibility.Hidden;
                    }
                }
            }
            else
            {
                if (!pageDic[((MenuItem)sender).Name].Equals(MainViewFrameB.Content))
                {
                    if (pageDic[((MenuItem)sender).Name].Equals(MainViewFrameA.Content))
                    {
                        MainViewFrameA.Content = null;
                    }
                    MainViewFrameB.Content = null;
                    MainViewFrameB.Content = pageDic[((MenuItem)sender).Name];
                    viewB_Page = pageDic[((MenuItem)sender).Name];
                    selectedPage = viewB_Page;
                    if (pageLocked.Contains(viewB_Page))
                    {
                        viewBLockedImage.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        viewBLockedImage.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        private PageModeEnum pageMode = PageModeEnum.SINGLE_PAGE; // 针对一个窗口的视图模式：single double
        private int selectedPageIndex = 0; // 0 是左侧窗口，1 是右侧窗口

        private void setSelectedPageIndex(int selectedPageIndex)
        {
            if (pageMode.Equals(PageModeEnum.DOUBLE_PAGE))
            {
                if (selectedPageIndex == 0)
                {
                    MainViewFrameA_Border.Visibility = Visibility.Visible;
                    MainViewFrameB_Border.Visibility = Visibility.Hidden;
                }
                else
                {
                    MainViewFrameA_Border.Visibility = Visibility.Hidden;
                    MainViewFrameB_Border.Visibility = Visibility.Visible;
                }
            }
            else
            {
                MainViewFrameA_Border.Visibility = Visibility.Hidden;
                MainViewFrameB_Border.Visibility = Visibility.Hidden;
            }
            this.selectedPageIndex = selectedPageIndex;
        }
        private void singlePageOnClick(object sender, RoutedEventArgs e)
        {
            if (pageMode.Equals(PageModeEnum.DOUBLE_PAGE))
            {
                pageMode = PageModeEnum.SINGLE_PAGE;
                // checked in menu
                singlePageCheckedImg.Visibility = Visibility.Visible;
                doublePageCheckedImg.Visibility = Visibility.Hidden;
                MainViewFrameA_Border.Visibility = Visibility.Hidden;
                MainViewFrameB_Border.Visibility = Visibility.Hidden;
                mainViewCol2Def.Width = new System.Windows.GridLength(0);
                if (selectedPageIndex == 0)
                {
                    mainViewCol3Def.Width = new System.Windows.GridLength(0);
                }
                else
                {
                    mainViewCol1Def.Width = new System.Windows.GridLength(0);
                }
            }
        }

        private void doublePageOnClick(object sender, RoutedEventArgs e)
        {
            if (pageMode.Equals(PageModeEnum.SINGLE_PAGE))
            {
                pageMode = PageModeEnum.DOUBLE_PAGE;
                singlePageCheckedImg.Visibility = Visibility.Hidden;
                doublePageCheckedImg.Visibility = Visibility.Visible;
                mainViewCol1Def.Width = new System.Windows.GridLength(1, GridUnitType.Star);
                mainViewCol2Def.Width = new System.Windows.GridLength(1, GridUnitType.Auto);
                mainViewCol3Def.Width = new System.Windows.GridLength(1, GridUnitType.Star);
                if (selectedPageIndex == 0)
                {
                    MainViewFrameA_Border.Visibility = Visibility.Visible;
                }
                else
                {
                    MainViewFrameB_Border.Visibility = Visibility.Visible;
                }
            }
        }

        private Page selectedPage = null;
        private Page viewA_Page = null;
        private Page viewB_Page = null;
        private void MainViewFrameA_OnClick(object sender, MouseButtonEventArgs e)
        {
            setSelectedPageIndex(0);
            selectedPage = viewA_Page;
        }

        private void MainViewFrameB_OnClick(object sender, MouseButtonEventArgs e)
        {
            setSelectedPageIndex(1);
            selectedPage = viewB_Page;
        }

        private void GridSplitter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            mainViewCol1Def.Width = new System.Windows.GridLength(1, GridUnitType.Star);
            mainViewCol3Def.Width = new System.Windows.GridLength(1, GridUnitType.Star);
        }

        ///////////////////////////////////////////// 多屏幕绑定功能 ////////////////////////////////////////////////////
        private static HashSet<Page> pageLocked = new HashSet<Page>();
        private static Dictionary<Page, ViewState> pageStates = new Dictionary<Page, ViewState>();
        /// <summary>
        /// 添加一个新的视图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addWindowMenuOnClick(object sender, RoutedEventArgs e)
        {
            MainWindow appendWindow = new MainWindow(MainWindow.userName, MainWindow.operationLevel);
            appendWindow.Show();
            appendWindow.Top = this.Top + 50;
            appendWindow.Left = this.Left + 50;
        }
        private void bindingOnClick(object sender, RoutedEventArgs e)
        {
            if (selectedPage != null && !pageLocked.Contains(selectedPage))
            {
                pageLocked.Add(selectedPage);
                if (selectedPageIndex == 0)
                {
                    viewALockedImage.Visibility = Visibility.Visible;
                }
                else
                {
                    viewBLockedImage.Visibility = Visibility.Visible;
                }
            }
        }
        private void unbindingOnClick(object sender, RoutedEventArgs e)
        {
            if (pageLocked.Contains(selectedPage))
            {
                pageLocked.Remove(selectedPage);
                if (selectedPageIndex == 0)
                {
                    viewALockedImage.Visibility = Visibility.Hidden;
                }
                else
                {
                    viewBLockedImage.Visibility = Visibility.Hidden;
                }
            }
        }
        private void calMinScrollDis() 
        {

        }
        private void scrollViewerA_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }

        private void scrollViewerB_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }

        private void scrollViewerA_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void scrollViewerB_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void scrollViewerA_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }
        private void scrollViewerB_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void snapshotOnClick(object sender, RoutedEventArgs e)
        {
            PrintScreen();
            Thread.Sleep(10);
            if (System.Windows.Forms.Clipboard.ContainsImage())
            {
                System.Drawing.Image clipImage = System.Windows.Forms.Clipboard.GetImage();
                System.Drawing.Bitmap image = cutPicture(clipImage, this);
                // save
                using (System.Drawing.Graphics oGraphics = System.Drawing.Graphics.FromImage(image))
                {
                    oGraphics.Flush();
                }
                String fileName = "./snapshots/snapshot" + DateTime.Now.ToString("yyyyMMdd HHmmss fff") + ".bmp";
                image.Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);

                /* Window testWindow = new Window();
                Image testImage = new Image();
                System.Windows.Media.Imaging.BitmapSource bi = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    image.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                );
                testImage.Source = bi;
                testWindow.Content = testImage;
                testWindow.Show();
                //return image; */
            }
        }

        private static int topOffset = 0, leftOffset = 0;
        public static void setOffset(int leftOffset, int topOffset)
        {
            MainWindow.leftOffset = leftOffset;
            MainWindow.topOffset = topOffset;
            propUtil.setStrProp("primaryMonitorOffset", leftOffset + ", " + topOffset);
            propUtil.reloadProperty();
        }

        private System.Drawing.Bitmap cutPicture(System.Drawing.Image image, MetroWindow window)
        {
            if (image != null)
            {
                int windowWidth = (int)window.ActualWidth;
                int windowHeight = (int)window.ActualHeight;
                //windowWidth = (window.Left + windowWidth) > image.Width ? (image.Width - (int)window.Left) : windowWidth;
                //windowHeight = (window.Top + windowHeight) > image.Height ? (image.Height - (int)window.Top) : windowHeight;
                //新建一个bmp图片
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(windowWidth, windowHeight);
                //新建一个画板
                System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(bitmap);
                //设置高质量插值法
                graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                //设置高质量，低速度呈现平滑程度
                graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //清空画布并以透明背景色填充
                graphic.Clear(System.Drawing.Color.Transparent);
                //在指定位置并且按指定大小绘制原图片的指定部分
                graphic.DrawImage(
                    image,
                    new System.Drawing.Rectangle(0, 0, windowWidth, windowHeight),
                    new System.Drawing.Rectangle(MainWindow.leftOffset + (int)window.Left, MainWindow.topOffset + (int)window.Top, windowWidth, windowHeight),
                    System.Drawing.GraphicsUnit.Pixel);
                return bitmap;
            }
            else
            {
                return null;
            }
        }

        const int VK_SNAPSHOT = 0x2C;
        private void PrintScreen()
        {
            keybd_event((byte)VK_SNAPSHOT, 0, 0x0, IntPtr.Zero);//down  
            System.Windows.Forms.Application.DoEvents();//强制窗口响应按钮事件
            keybd_event((byte)VK_SNAPSHOT, 0, 0x2, IntPtr.Zero);//up  
            System.Windows.Forms.Application.DoEvents();
        }
        [DllImport("user32.dll")]
        static extern void keybd_event
        (
            byte bVk,// 虚拟键值
            byte bScan,// 硬件扫描码    
            uint dwFlags,// 动作标识    
            IntPtr dwExtraInfo// 与键盘动作关联的辅加信息
        );

        private void viewSettingOnClick(object sender, RoutedEventArgs e)
        {
            new FrontFramework.Utils.ViewSetting.ViewSetting(this).ShowDialog();
        }

        private void pluginMenuOnClick(object sender, RoutedEventArgs e)
        {
            PluginMenuHelper helper = (PluginMenuHelper)((MenuItem)e.Source).Header;
            BasEvent basEvent = new BasEvent (){
                eventSource = "FRAMEWORK",
                isBroadcast = false,
                eventDestination = new List<string>() { helper.pluginID },
                eventName = helper.menuID,
                eventParas = null};
            pluginManager.getPluginInfoByID(helper.pluginID).plugin.trigger(basEvent);
        }
        private void pluginManagerMenuOnClick(object sender, RoutedEventArgs e)
        {
            new FrontFramework.Plugin.Views.PluginManageWindow(this).ShowDialog();
        }

        private void alarmDetailsOnClick(object sender, RoutedEventArgs e)
        {
            new FrontFramework.Alarm.Views.AlarmDetails().Show();
        }
    }

    #region 报警声音操作
    public partial class MainWindow : ComponentDynamicTranslate
    {
        public void AlarmModuleInit()
        {
            double volume = 0.25;
            try
            {
                volume = double.Parse(propUtil.getStrProp("historyVolume").info.Trim());
            }
            catch (Exception er) 
            {
                Console.WriteLine(er.ToString());
            }
            sliderVolume.Value = volume / (VolumnEnum.MAX_VOL - VolumnEnum.MIN_VOL) * 10.0;
            alarmPlayer.Volume = volume;
        }

        private Boolean muteState = false; // true -> mute, false -> not mute
        public void startAlarm()
        {
            alarmPlayer.Stop();
            alarmPlayer.Play();
        }
        public void stopAlarm()
        {
            alarmPlayer.Stop();
        }
        private void muteButtonClick(object sender, RoutedEventArgs e)
        {
            alarmPlayer.IsMuted = !alarmPlayer.IsMuted;
            sliderVolume.IsEnabled = !sliderVolume.IsEnabled;
            muteState = !muteState;
        }

        private void sliderVolumeMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            double volume = ((Slider)sender).Value * (VolumnEnum.MAX_VOL - VolumnEnum.MIN_VOL) / 10.0;
            alarmPlayer.Volume = volume;
            propUtil.setStrProp("historyVolume", volume.ToString());
        }

        private void alarmPlayerMediaEnd(object sender, RoutedEventArgs e)
        {
            (sender as MediaElement).Stop();
            (sender as MediaElement).Play();
        }

        private void testMenuClick(object sender, RoutedEventArgs e)
        {
            switch (((System.Windows.Controls.MenuItem)sender).Name)
            {
                case "menuStartAlarm":
                    startAlarm();
                    break;
                case "menuStopAlarm":
                    stopAlarm();
                    break;
            }
        }
    }
    #endregion

    #region 系统菜单
    public partial class MainWindow : ComponentDynamicTranslate
    {
        public void SysMenuInit() 
        {
            switch (translator.getLanguageTo())
            {
                case LanguageEnum.CHINESE:
                    chCheckedImg.Visibility = Visibility.Visible;
                    break;
                case LanguageEnum.ENGLISH:
                    enCheckedImg.Visibility = Visibility.Visible; 
                    break;
            }
        }
        /// <summary>
        /// 登出操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logoutOnClick(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }

        /// <summary>
        /// 切换中文
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chMenuOnClick(object sender, RoutedEventArgs e)
        {
            chCheckedImg.Visibility = Visibility.Visible;
            enCheckedImg.Visibility = Visibility.Hidden;
            translator.setLanguage(LanguageEnum.ENGLISH, LanguageEnum.CHINESE);
            LanguageChangedNotifier.getInstance().notifyAll();
        }
        /// <summary>
        /// 切换英文
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enMenuOnClick(object sender, RoutedEventArgs e)
        {
            enCheckedImg.Visibility = Visibility.Visible;
            chCheckedImg.Visibility = Visibility.Hidden;
            translator.setLanguage(LanguageEnum.CHINESE, LanguageEnum.ENGLISH);
            LanguageChangedNotifier.getInstance().notifyAll();
        }
    } 
    #endregion

    #region 帮助菜单
    public partial class MainWindow : ComponentDynamicTranslate
    {
        private void docHelpOnClick(object sender, RoutedEventArgs e)
        {
            ((IHelper)SpringUtil.getContext().GetObject("DocHelper")).openHelper();
        }

        private void onlineHelpOnClick(object sender, RoutedEventArgs e)
        {
            ((IHelper)SpringUtil.getContext().GetObject("OnlineHelper")).openHelper();
        }

        private void aboutClick(object sender, RoutedEventArgs e)
        {
            new FrontFramework.Common.Views.AboutBox(this).ShowDialog();
        }
    }
    #endregion

    #region 车站菜单
    public partial class MainWindow : ComponentDynamicTranslate
    {
        private int stationCode;
        public int getStationCode() { return this.stationCode; }
        private void StationMenuInit()
        {
            List<StationInfo> stationInfoList = StationInfoManager.getInstance().getStationInfoList();
            List<StationComboboxModel> newItems = new List<StationComboboxModel>();
            foreach (StationInfo info in stationInfoList) 
            {
                StationComboboxModel temp = new StationComboboxModel();
                temp.stationCode = info.stationCode;
                if (info.isTransfer == true)
                {
                    temp.imgPath = "resources/images/transfer.png";
                    String tip = "| ";
                    foreach (String line in info.transLines)
                    {
                        tip += translator.getComponentTranslation(line) + " | ";
                    }
                    temp.tipText = tip;
                }
                else
                {
                    temp.imgPath = "resources/images/white.png";
                    temp.tipText = null;
                }
                temp.stationName = translator.getComponentTranslation(info.id);
                newItems.Add(temp);
            }
            comboBoxStation.SelectedValuePath = "stationCode";
            comboBoxStation.ItemsSource = null;
            comboBoxStation.ItemsSource = newItems;
            // 从历史中恢复数据
            try
            {
                stationCode = int.Parse(propUtil.getStrProp("historyStationCode").info.Trim());
            }
            catch (Exception er)
            {
                Console.WriteLine(er.ToString());
            }
            foreach (PluginInfo pluginInfo in pluginManager.pluginInfos)
            {
                pluginInfo.plugin.setStationCode(stationCode);
            }
            comboBoxStation.SelectedValue = stationCode;
        }
        private void stationSelected(object sender, RoutedEventArgs e)
        {
            if (comboBoxStation.SelectedValue != null)
            {
                stationCode = (int)comboBoxStation.SelectedValue;
                foreach (PluginInfo pluginInfo in pluginManager.pluginInfos)
                {
                    pluginInfo.plugin.setStationCode(stationCode);
                }
                propUtil.setStrProp("historyStationCode", stationCode.ToString());
            }
        }
    }
    class StationComboboxModel
    {
        public int stationCode { get; set; }
        public String stationName { get; set; }
        public String imgPath { get; set; }
        public String tipText { get; set; }
    }
    #endregion

    #region 插件交互
    public partial class MainWindow : ComponentDynamicTranslate 
    {

        private static PluginManager pluginManager = PluginManager.getPluginManager();
        private Dictionary<String, Page> pageDic = new Dictionary<string, Page>(); // View Switch Menu ID - Page Thread

        /// <summary>
        /// 主要负责初始化插件在界面中的显示与一些点击回调函数
        /// </summary>
        private Brush tabHeaderForegroundBrushTemp;
        private Label selectedLabel = null;
        private void PluginInit()
        {
            int i = 0;
            // 清空原有显示
            for (; i < mainMenu.Items.Count; i++)
            {
                if (((MenuItem)mainMenu.Items.GetItemAt(i)).Name.Equals("fakeSeparator"))
                {
                    i++;
                    break;
                }
            }
            for (; i < mainMenu.Items.Count; i++)
            {
                mainMenu.Items.Remove(mainMenu.Items.GetItemAt(i));
                i--;
            }
            ViewSwitchTabControl.Items.Clear();
            // 添加新显示
            pageDic.Clear();
            foreach (PluginInfo pluginInfo in pluginManager.pluginInfos)
            {
                if (!pluginInfo.isActive) 
                {
                    continue;
                }
                // menu
                MenuItem item = generateMenuFromTree(pluginInfo.plugin.getMenuRoot(), pluginInfo.pluginID);
                if (item != null)
                {
                    mainMenu.Items.Insert(mainMenu.Items.Count, item);
                }
                // view switch menu
                TabItem tabItem = new TabItem();
                Label tabHeaderLable = new Label();
                tabHeaderLable.Content = translator.getComponentTranslation(pluginInfo.plugin.getViewSwitchMenuId().Split(' '));
                tabHeaderLable.FontSize = 16;
                tabHeaderLable.FontFamily = new FontFamily("等线");
                tabItem.Header = tabHeaderLable;
                tabHeaderLable.MouseEnter += (obj, eventArgs) =>
                {
                    tabHeaderForegroundBrushTemp = ((Label)eventArgs.Source).Foreground;
                    ((Label)eventArgs.Source).Foreground = new SolidColorBrush(Colors.LightBlue);
                };
                tabHeaderLable.MouseLeave += (obj, eventArgs) =>
                {
                    ((Label)eventArgs.Source).Foreground = tabHeaderForegroundBrushTemp;
                };
                tabHeaderLable.MouseLeftButtonDown += (obj, eventArgs) =>
                {
                    if (selectedLabel != null)
                        selectedLabel.Foreground = new SolidColorBrush(Colors.Black);
                    selectedLabel = (Label)eventArgs.Source;
                    ((Label)eventArgs.Source).Foreground = new SolidColorBrush(Colors.LightBlue);
                    tabHeaderForegroundBrushTemp = new SolidColorBrush(Colors.LightBlue);
                };
                Menu switchMenu = new Menu();
                switchMenu.FontSize = 3;
                foreach (var view in pluginInfo.plugin.getViewSwitchPages())
                {
                    MenuItem childItem = new MenuItem();
                    childItem.Header = translator.getComponentTranslation(view.Key.Split(' '));
                    childItem.Padding = new Thickness(10, 1, 10, 1);
                    childItem.Name = view.Key.Replace(" ", "");
                    childItem.Click += viewSwitchMenuClicked;
                    switchMenu.Items.Add(childItem);
                }
                tabItem.Content = switchMenu;
                ViewSwitchTabControl.Items.Add(tabItem);
                ViewSwitchTabControl.SelectedIndex = 0;
                // view switch page
                foreach (var id2page in pluginInfo.plugin.getViewSwitchPages())
                {
                    pageDic.Add(id2page.Key.Replace(" ", ""), id2page.Value);
                }
                // event handler
                pluginInfo.plugin.systemEventHandler += systemEventHappend;
                // alarm handler
                pluginInfo.plugin.basAlarmHandler += pluginAlarmHappend;
                pluginInfo.plugin.basAlarmSolvedHandler += pluginAlarmSolvedHappend;
            }
        }
        /// <summary>
        /// 用于生成插件的系统菜单
        /// </summary>
        /// <param name="root"></param>
        /// <param name="pluginID"></param>
        /// <returns></returns>
        private MenuItem generateMenuFromTree(MenuNode root, String pluginID)
        {
            if (root == null)
                return null;
            MenuItem item = new MenuItem();
            PluginMenuHelper helper = new PluginMenuHelper();
            helper.pluginID = pluginID;
            helper.menuID = root.getMenuID();
            helper.displayContent = translator.getComponentTranslation(root.getMenuID().Split(' '));
            item.Header = helper;
            item.Foreground = new SolidColorBrush(Colors.Black);
            item.HorizontalAlignment = HorizontalAlignment.Left;
            if (root.getChildrenMenus().Count == 0) // 叶子节点
            {
                item.Click += pluginMenuOnClick;
            }
            foreach (MenuNode child in root.getChildrenMenus())
            {
                MenuItem childItem = generateMenuFromTree(child, pluginID);
                item.Items.Add(childItem);
            }
            return item;
        }
        private void systemEventHappend(BasEvent e)
        {
            foreach (PluginInfo pluginInfo in pluginManager.pluginInfos)
            {
                if (e.isBroadcast || e.eventDestination.Contains<String>(pluginInfo.plugin.getPluginId()))
                {
                    pluginInfo.plugin.trigger(e);
                }
            }
        }

        /***************************************************************************************************
        ***************************************** 插件报警相关操作 *****************************************
        ****************************************************************************************************/
        private void pluginAlarmHappend(BasAlarm alarm)
        {

        }
        private void pluginAlarmSolvedHappend(BasAlarm alarm)
        {

        }

    }
    #endregion

    class ViewState 
    {
        public double verticalPosition = 0;
        public double horizontalPosition = 0;
        public ViewState(double verticalPosition, double horizontalPosition)
        {
            this.verticalPosition = verticalPosition;
            this.horizontalPosition = horizontalPosition;
        }
    }

    class PluginMenuHelper 
    {
        public String pluginID;
        public String menuID;
        public String displayContent;
        public override string ToString()
        {
            return displayContent;
        }
    }

}

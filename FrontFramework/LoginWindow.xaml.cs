﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FrontFramework.Enums;
using FrontFramework.Language;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FrontFramework.Interfaces;

namespace FrontFramework
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window, ComponentDynamicTranslate
    {
        private static Translator translator = TranslatorBasedOnXML.getTranslator();
        public LoginWindow()
        {
            InitializeComponent();
            translator.recoveryFromHistory();
            initializeComponentContents();
            LanguageChangedNotifier.getInstance().addListener(this);
        }

        ~LoginWindow() 
        {
            LanguageChangedNotifier.getInstance().removeListener(this);
        }

        public void initializeComponentContents() 
        {
            this.Title = translator.getComponentTranslation("Login");
            labelUserName.Content = translator.getComponentTranslation("UserName");
            labelPassword.Content = translator.getComponentTranslation("Password");
            labelLanguage.Content = translator.getComponentTranslation("Language");
            buttonLogin.Content = translator.getComponentTranslation("Login");
            buttonCancel.Content = translator.getComponentTranslation("Cancel");

            languageComboBox.ItemsSource = new LanguageCombo[] { 
                new LanguageCombo(){index=0, show="中文", lang=LanguageEnum.CHINESE}, 
                new LanguageCombo(){index=1, show="English", lang=LanguageEnum.ENGLISH}
            };
            languageComboBox.SelectedValuePath = "lang";
            languageComboBox.DisplayMemberPath = "show";
            languageComboBox.SelectedValue = translator.getLanguageTo();
            statusDisplay.Content = "";
        }

        private void loginClick(object sender, RoutedEventArgs e)
        {
            String userName = userNameTextbox.Text;
            String password = passwordTextbox.Password;
            // loginResult 应该调用后台接口获得
            bool loginResult = true; // <<------------------------------------------------------------------------
            if (userName.Equals("JENNINGLANG") && password.Equals("JENNINGLANG")) 
            {
                loginResult = true;
            }
            if (loginResult)
            {
                FrontFramework.MainWindow mainWindow = new FrontFramework.MainWindow();
                mainWindow.Show();
                this.Close();
            } 
            else 
            {
                statusDisplay.Content = translator.getComponentTranslation(new String[]{"Login","Failure"}) + "!";
            }
        }

        private void cancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void languageComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (languageComboBox.SelectedValue != null)
            {
                if ((LanguageEnum)languageComboBox.SelectedValue == LanguageEnum.ENGLISH)
                {
                    translator.setLanguage(LanguageEnum.CHINESE, LanguageEnum.ENGLISH);
                }
                else
                {
                    translator.setLanguage(LanguageEnum.ENGLISH, LanguageEnum.CHINESE);
                }
                LanguageChangedNotifier.getInstance().notifyAll();
            }
        }
    }

    class LanguageCombo
    {
        public int index { get; set; }
        public String show { get; set; }
        public LanguageEnum lang { get; set; }
    }

}
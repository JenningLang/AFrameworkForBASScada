﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.ComponentModel.Composition;
using PluginDefinition;
using System.Windows.Controls;

namespace PluginExample
{
    [Export(typeof(AbstractPlugin))]
    public class TestPlugin : AbstractPlugin
    {
        // view const
        private static readonly String pluginId = "Test Plugin";
        private static readonly String menuID = "Test Plugin Menu";
        private static readonly String[] menuItemsID = { "Test Plugin Menu Item 1", "Test Plugin Menu Item 2" };
        private static readonly String viewSwitchMenuId = "View Switch Menu";
        private static readonly String[] viewSwitchMenuItemsId = { "Test View Switch Page 1", "Test View Switch Page 2" };
        // event enum
        public enum ReceivableEventsIdEnum
        {

        }
        public enum EmittedEventsIdEnum
        {

        }
        // constructor
        public TestPlugin()
        {
            
        }

        public override void initializeComponentContents()
        {
            Console.WriteLine("Language changed");
        }
        // override
        override public String getPluginId()
        {
            return TestPlugin.pluginId;
        }
        override public MenuNode getMenuRoot() 
        {
            return null;
        }
        override public String getViewSwitchMenuId()
        {
            return TestPlugin.viewSwitchMenuId;
        }

        override public List<String> getViewSwitchMenuItemsId()
        {
            return new List<string>(TestPlugin.viewSwitchMenuItemsId);
        }

        Dictionary<string, Page> id2page = new Dictionary<string, Page>();
        // Dictionary<string, Page> id2thread = new Dictionary<string, Page>();
        override public Dictionary<String, Page> getViewSwitchPages()
        {
            if (id2page.Count == 0)
            {
                id2page.Add(viewSwitchMenuItemsId[0], new TestPluginPage());
                id2page.Add(viewSwitchMenuItemsId[1], new AnotherTestPluginPage());
            }
            return id2page;
        }
        /*override public Dictionary<String, Thread> getViewSwitchThreads() 
        { 
            return null;
        }*/
        override public void confirmAlarm(BasAlarm basAlarm) 
        {
        }
        override protected void eventHandler(BasEvent e) 
        {
        }
        override protected void stationCodeChanged()
        {
            Console.WriteLine("Station code is changed to " + this.stationCode);
        }
    }
}

#region License
/*
Copyright © 2014-2019 European Support Limited

Licensed under the Apache License, Version 2.0 (the "License")
you may not use this file except in compliance with the License.
You may obtain a copy of the License at 

http://www.apache.org/licenses/LICENSE-2.0 

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS, 
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and 
limitations under the License. 
*/
#endregion

using amdocs.ginger.GingerCoreNET;
using Amdocs.Ginger.Common.Enums;
using Ginger.SolutionWindows.TreeViewItems;
using GingerCore;
using GingerWPF.BusinessFlowsLib;
using GingerWPF.UserControlsLib;
using GingerWPF.UserControlsLib.UCTreeView;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Ginger.BusinessFlowWindows
{
    /// <summary>
    /// Interaction logic for BusinessFlowsAutomatePage.xaml
    /// </summary>
    public partial class BusinessFlowsAutomatePage : Page
    {
        SingleItemTreeViewExplorerPage mBusFlowsPage;
        AutomatePage mAutomatePage;
        NewAutomatePage mNewAutomatePage;

        public BusinessFlowsAutomatePage()
        {
            InitializeComponent();

            App.AutomateBusinessFlowEvent += App_AutomateBusinessFlowEvent;
            WorkSpace.UserProfile.PropertyChanged += UserProfile_PropertyChanged;

            Reset();
        }

        private void App_AutomateBusinessFlowEvent(AutomateEventArgs args)
        {
            if (args.EventType == AutomateEventArgs.eEventType.Automate)
            {
                if (WorkSpace.Instance.BetaFeatures.ShowNewautomate)
                {
                    if (mNewAutomatePage == null)
                    {
                        mNewAutomatePage = new NewAutomatePage((BusinessFlow)args.Object);
                    }
                    xContentFrame.Content = mNewAutomatePage;
                }
                else
                {
                    if (mAutomatePage == null)
                    {
                        mAutomatePage = new AutomatePage((BusinessFlow)args.Object);                       
                    }
                    xContentFrame.Content = mAutomatePage;
                }                
            }
            else if (args.EventType == AutomateEventArgs.eEventType.ShowBusinessFlowsList)
            {
                ShiftToBusinessFlowView((BusinessFlow)args.Object);
            }
        }    
         

        private void UserProfile_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UserProfile.Solution))
            {
                Reset();
            }
        }

        private void Reset()
        {
            mBusFlowsPage = null;                      
            ShiftToBusinessFlowView(null);
        }

        private void ShiftToBusinessFlowView(BusinessFlow bf)
        {
            if(mBusFlowsPage == null &&  WorkSpace.UserProfile.Solution != null)
            {
                BusinessFlowsFolderTreeItem busFlowsRootFolder = new BusinessFlowsFolderTreeItem(WorkSpace.Instance.SolutionRepository.GetRepositoryItemRootFolder<BusinessFlow>());
                mBusFlowsPage = new SingleItemTreeViewExplorerPage(GingerCore.GingerDicser.GetTermResValue(GingerCore.eTermResKey.BusinessFlows), eImageType.BusinessFlow, busFlowsRootFolder, busFlowsRootFolder.SaveAllTreeFolderItemsHandler, busFlowsRootFolder.AddItemHandler, treeItemDoubleClickHandler: BusinessFlowsTree_ItemDoubleClick);
            }
            xContentFrame.Content = mBusFlowsPage;
        }

        private void BusinessFlowsTree_ItemDoubleClick(object sender, EventArgs e)
        {
            TreeViewItem i = (TreeViewItem)sender;
            if (i != null)
            {
                ITreeViewItem iv = (ITreeViewItem)i.Tag;

                if (iv.NodeObject() != null && iv.NodeObject() is BusinessFlow)
                {
                    App.OnAutomateBusinessFlowEvent(AutomateEventArgs.eEventType.Automate, (BusinessFlow)iv.NodeObject());
                }
            }
        }

    }
}

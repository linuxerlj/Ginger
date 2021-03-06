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

using Amdocs.Ginger.Common;
using GingerCore.Actions;
using GingerWPF.UserControlsLib.UCTreeView;
using System.Windows.Controls;
using Ginger.Actions;

namespace Ginger.WindowExplorer.Java
{
    public class JavaTableTreeItem : JavaElementTreeItem, ITreeViewItem, IWindowExplorerTreeItem
    {
      ObservableList<Act> mAvailableActions = new ObservableList<Act>();
      ActTableEditPage mActTableEditPage = null;

      StackPanel ITreeViewItem.Header()
      {
          string ImageFileName = "@Grid_16x16.png";
          return TreeViewUtils.CreateItemHeader(Name, ImageFileName);
      }
        ObservableList<Act> IWindowExplorerTreeItem.GetElementActions()
        {
            if (mAvailableActions.Count == 0)
            {
                mAvailableActions.Clear();
                mAvailableActions.Add(new ActTableElement()
                { //TODO:get Row Count
                    Description = "Get " + Name + " Table RowCount",
                    ControlAction = ActTableElement.eTableAction.GetRowCount,
                    ColSelectorValue = ActTableElement.eRunColSelectorValue.ColNum,
                    LocateColTitle = "0",
                    ByRowNum = true,
                    LocateRowValue = "0",
                    LocateRowType = "Row Number"
                });
            }
            return mAvailableActions;
        }

        Page ITreeViewItem.EditPage()
        {
            if (mActTableEditPage == null)
                mActTableEditPage = new ActTableEditPage(base.JavaElementInfo, mAvailableActions);            
            return mActTableEditPage;
        }
    }
}
